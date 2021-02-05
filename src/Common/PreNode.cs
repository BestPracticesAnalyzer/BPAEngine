using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public abstract class PreNode : IComparable
	{
		protected string name;

		protected XmlElement element;

		protected ExecutionInterface executionInterface;

		protected Rules allRules;

		protected int[] path;

		protected Hashtable dependencies = new Hashtable();

		protected int dependerCount = 1;

		protected int depLevel;

		protected Hashtable references = new Hashtable();

		protected bool valid = true;

		public string Name
		{
			get
			{
				return name;
			}
		}

		internal bool Valid
		{
			get
			{
				return valid;
			}
			set
			{
				valid = value;
			}
		}

		public Rules AllRules
		{
			get
			{
				return allRules;
			}
		}

		public Hashtable References
		{
			get
			{
				return references;
			}
		}

		public PreNode(XmlElement element, ExecutionInterface executionInterface, Rules rules)
		{
			this.element = element;
			this.executionInterface = executionInterface;
			allRules = rules;
			name = element.GetAttribute("Name");
			FindPathToNode();
		}

		protected void FindPathToNode()
		{
			XPathNavigator xPathNavigator = element.CreateNavigator();
			int num = (int)(double)xPathNavigator.Evaluate("count(ancestor::node())");
			ArrayList arrayList = new ArrayList();
			string a = string.Empty;
			for (int i = 0; i < num; i++)
			{
				if (xPathNavigator.LocalName == "Object" && a != "Rule")
				{
					arrayList.Add(-1);
				}
				arrayList.Add((int)(double)xPathNavigator.Evaluate("count(preceding-sibling::node())"));
				a = xPathNavigator.LocalName;
				xPathNavigator.MoveToParent();
			}
			arrayList.Reverse();
			path = (int[])arrayList.ToArray(typeof(int));
		}

		protected void AddDependency(PreNode node)
		{
			if (node != null && !dependencies.Contains(node))
			{
				dependencies[node] = 1;
				node.dependerCount++;
			}
		}

		public int CompareTo(object rule)
		{
			return ((PreNode)rule).depLevel - depLevel;
		}

		public static int FollowDependencies(ICollection nodeCollection, int depLevel, ExecutionInterface executionInterface)
		{
			int num = 0;
			foreach (PreNode item in nodeCollection)
			{
				if (depLevel > item.depLevel)
				{
					item.depLevel = depLevel;
				}
				item.dependerCount--;
				if (item.dependerCount < 0)
				{
					throw new ExDiagAnalyzerException("Algorithmic failure in dependency processing");
				}
				if (item.dependerCount == 0)
				{
					num += 1 + FollowDependencies(item.dependencies.Keys, item.depLevel + 1, executionInterface);
				}
			}
			return num;
		}

		protected string FindLCAPath(PreNode other)
		{
			int i;
			for (i = 0; i < path.Length && i < other.path.Length - 1 && path[i] == other.path[i]; i++)
			{
			}
			i--;
			int num = path.Length - i - 1;
			int num2 = other.path.Length - i - 2;
			StringBuilder stringBuilder = new StringBuilder();
			for (int j = 0; j < num; j++)
			{
				stringBuilder.Append("../");
			}
			for (int k = 0; k < num2; k++)
			{
				stringBuilder.Append("*/");
			}
			stringBuilder.Append(((other is PreRule) ? "Rule" : "Pivot") + "[@Name='" + other.name + "']");
			return stringBuilder.ToString();
		}

		private string RuleReplacement(Match match)
		{
			if (match.Value.StartsWith("'") || match.Value.StartsWith("\""))
			{
				return match.Value;
			}
			if (match.Value == "$_")
			{
				return "$_";
			}
			if (match.Value == "$.")
			{
				return "($_/../Value | $_/../Instance)";
			}
			if (match.Value.EndsWith("._"))
			{
				return match.Value.Substring(0, match.Value.Length - 1) + Name;
			}
			if (match.Value.EndsWith(".."))
			{
				return match.Value.Substring(0, match.Value.Length - 1) + Name + "/../Value";
			}
			return match.Value + "/Result";
		}

		protected void FindReferences(RuleExpression expression)
		{
			Hashtable hashtable = new Hashtable();
			Regex regex = new Regex("(['\"]).*?\\1|\\$([A-Za-z_\\.][\\w\\.]*)");
			expression.Text = regex.Replace(expression.Text, RuleReplacement);
			Match match = regex.Match(expression.Text);
			while (match.Success)
			{
				if (match.Groups[2].Length > 0)
				{
					string key = match.Groups[2].ToString();
					hashtable[key] = 1;
				}
				match = match.NextMatch();
			}
			expression.Refs = new string[hashtable.Keys.Count];
			hashtable.Keys.CopyTo(expression.Refs, 0);
			string[] refs = expression.Refs;
			foreach (string text in refs)
			{
				if (!references.Contains(text))
				{
					references[text] = ExpandReference(text, expression.IsQuery, expression.IsDependency);
					if (executionInterface.Trace)
					{
						executionInterface.LogText("\t\tThe path to reference {0} is '{1}'", text, ((RuleReference)references[text]).Path);
					}
				}
			}
		}

		protected RuleReference ExpandReference(string ruleName, bool query, bool depend)
		{
			RuleReference ruleReference = new RuleReference(ruleName);
			ruleReference.IsInQuery = query;
			if (ruleName == "_")
			{
				ruleReference.Path = ".";
				return ruleReference;
			}
			ruleReference.IsDependency = depend;
			int num;
			if ((num = ruleName.IndexOf('.')) != -1)
			{
				ruleReference.Name = ruleName.Substring(num + 1, ruleName.Length - num - 1);
				string text = ruleName.Substring(0, num);
				if (!allRules.PivotHash.Contains(text))
				{
					throw new ExDiagAnalyzerException("Pivot name '" + text + "' referenced by rule '" + Name + "' in input file is not defined");
				}
				ruleReference.Pivot = (PrePivot)allRules.PivotHash[text];
				AddDependency(ruleReference.Pivot);
			}
			if (!allRules.RuleHash.Contains(ruleReference.Name))
			{
				throw new ExDiagAnalyzerException("Rule name '" + ruleName + "' referenced by rule '" + Name + "' in input file is not defined");
			}
			PreRule preRule = (PreRule)allRules.RuleHash[ruleReference.Name];
			if (ruleReference.Pivot == null)
			{
				ruleReference.Path = FindLCAPath(preRule);
			}
			else
			{
				ruleReference.Path = FindLCAPath(ruleReference.Pivot);
				ruleReference.Pivot.AddReferencePath(preRule);
			}
			if (query && ruleReference.Name != Name)
			{
				AddDependency(preRule);
			}
			return ruleReference;
		}

		public abstract void ExpandAttributes();
	}
}
