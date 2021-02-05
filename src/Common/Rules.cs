using System;
using System.Collections;
using System.Xml;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class Rules
	{
		private int nameCounter;

		private Hashtable ruleHash = new Hashtable();

		private Hashtable pivotHash = new Hashtable();

		private ArrayList ruleList = new ArrayList();

		private ExecutionInterface executionInterface;

		private ArrayList issueProcessors = new ArrayList();

		public Hashtable RuleHash
		{
			get
			{
				return ruleHash;
			}
		}

		public Hashtable PivotHash
		{
			get
			{
				return pivotHash;
			}
		}

		public ArrayList IssueProcessors
		{
			get
			{
				return issueProcessors;
			}
		}

		public Rules(ExecutionInterface executionInterface, XmlNodeList ruleElements, XmlNodeList pivotElements)
		{
			this.executionInterface = executionInterface;
			foreach (XmlElement pivotElement in pivotElements)
			{
				PrePivot prePivot = new PrePivot(pivotElement, executionInterface, this);
				pivotHash.Add(prePivot.Name, prePivot);
				ruleList.Add(prePivot);
			}
			foreach (XmlElement ruleElement in ruleElements)
			{
				PreRule preRule = new PreRule(ruleElement, executionInterface, this);
				ruleHash.Add(preRule.Name, preRule);
				ruleList.Add(preRule);
				if (executionInterface.Trace)
				{
					executionInterface.LogText("\tRead rule '{0}' from config", preRule.Name);
				}
			}
			if (executionInterface.Trace)
			{
				executionInterface.LogText("Loading processors");
			}
			foreach (LoadedProcessor value2 in executionInterface.CodeLibraries.LoadedIssueProcessors.Values)
			{
				if (value2.ProcessorOptions == null || !value2.ProcessorOptions.Contains("Enable"))
				{
					continue;
				}
				Type processorType = value2.ProcessorType;
				object[] parms = new object[1]
				{
					executionInterface
				};
				try
				{
					IssueProcessor value = (IssueProcessor)value2.CreateInstance(parms);
					issueProcessors.Add(value);
					if (executionInterface.Trace)
					{
						executionInterface.LogText("\tLoaded issue processor {0}", value2.Name);
					}
				}
				catch (Exception ex)
				{
					if (executionInterface.Trace)
					{
						executionInterface.LogException(string.Format("Failed to load issue processor {0}: {1}", value2.Name, ex.Message), ex);
					}
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return ruleList.GetEnumerator();
		}

		public void SortList()
		{
			int num = PreNode.FollowDependencies(ruleList, 0, executionInterface);
			if (num != ruleList.Count)
			{
				throw new ExDiagRuleFormatException("Cyclic dependency in rules - cannot evaluate " + (ruleList.Count - num));
			}
			ruleList.Sort();
		}

		public string UniqueName()
		{
			string text;
			do
			{
				text = "__rule_" + nameCounter;
				nameCounter++;
			}
			while (RuleHash.Contains(text));
			return text;
		}

		public void ProcessRules()
		{
			ObjectParentData opd = executionInterface.Options.RootOPD();
			Document configuration = executionInterface.Options.Configuration;
			Document data = executionInterface.Options.Data;
			ProcessRules(configuration.GetNode("/*"), data.GetNode("/*"), opd);
		}

		private void ProcessRules(Node cfgParent, Node dataParent, ObjectParentData opd)
		{
			if (executionInterface.Aborting)
			{
				return;
			}
			Node[] nodes = cfgParent.GetNodes("Object");
			Node[] array = nodes;
			foreach (Node node in array)
			{
				Node[] nodes2 = node.GetNodes("Setting");
				Node[] nodes3 = node.GetNodes("Pivot");
				Node node2 = Common.AddOrMatchObject(dataParent, node);
				Node[] nodes4 = node.GetNodes("Rule");
				Node[] array2 = nodes4;
				foreach (Node node3 in array2)
				{
					InsertNode(node3, node2, opd);
				}
				Node[] nodes5 = node2.GetNodes("Instance");
				Node[] array3 = nodes5;
				foreach (Node node4 in array3)
				{
					ObjectParentData objectParentData = opd.Clone();
					Node[] array4 = nodes3;
					foreach (Node node5 in array4)
					{
						InsertNode(node5, node4, objectParentData);
					}
					Node[] array5 = nodes2;
					foreach (Node node6 in array5)
					{
						Node node7 = Common.FindSetting(node4, node6, objectParentData);
						if (node7 != null)
						{
							objectParentData.CaptureSubstitution(node7);
							nodes4 = node6.GetNodes("Rule");
							Node[] array6 = nodes4;
							foreach (Node node8 in array6)
							{
								InsertNode(node8, node7, objectParentData);
							}
						}
					}
					ProcessRules(node, node4, objectParentData);
				}
			}
		}

		private void InsertNode(Node node, Node parent, ObjectParentData opd)
		{
			Node node2 = parent.OwnerDocument.ImportNode(node, false);
			Node node3 = opd.ApplySubstitutions(node2);
			if (node3 != null)
			{
				parent.Add(node3);
				Node[] nodes = node.GetNodes("*");
				Node[] array = nodes;
				foreach (Node node4 in array)
				{
					InsertNode(node4, node3, opd);
				}
			}
		}
	}
}
