using System.Collections;
using System.Xml;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class PrePivot : PreNode
	{
		private RuleExpression key;

		private PrePivot remote;

		private RuleExpression keyRef;

		private PrePivot terminal;

		private Hashtable referencePaths = new Hashtable();

		private Hashtable keyHash = new Hashtable();

		public RuleExpression Key
		{
			get
			{
				return key;
			}
		}

		public PrePivot Remote
		{
			get
			{
				return remote;
			}
		}

		public RuleExpression KeyRef
		{
			get
			{
				return keyRef;
			}
		}

		public Hashtable ReferencePaths
		{
			get
			{
				return referencePaths;
			}
			set
			{
				referencePaths = value;
			}
		}

		public Hashtable KeyHash
		{
			get
			{
				return keyHash;
			}
			set
			{
				keyHash = value;
			}
		}

		public PrePivot(XmlElement element, ExecutionInterface executionInterface, Rules rules)
			: base(element, executionInterface, rules)
		{
		}

		public override void ExpandAttributes()
		{
			if (executionInterface.Trace)
			{
				executionInterface.LogText("\tPreprocessing pivot '{0}'", name);
			}
			string attribute = element.GetAttribute("Remote");
			if (attribute.Length > 0)
			{
				if (!allRules.PivotHash.Contains(attribute))
				{
					throw new ExDiagAnalyzerException("Can't find remote pivot" + attribute + " on pivot " + name);
				}
				remote = (PrePivot)allRules.PivotHash[attribute];
			}
			FindTerminalReference();
			key = new RuleExpression(element.GetAttribute("Key"), true, true);
			FindReferences(key);
			element.SetAttribute("Key", key.Text);
			keyRef = new RuleExpression(element.GetAttribute("KeyRef"), true, true);
			FindReferences(keyRef);
			element.SetAttribute("KeyRef", keyRef.Text);
		}

		public void FindTerminalReference()
		{
			if (terminal != null)
			{
				return;
			}
			if (remote == null)
			{
				terminal = this;
			}
			else
			{
				AddDependency(remote);
				if (remote.terminal == null)
				{
					remote.FindTerminalReference();
				}
				terminal = remote.terminal;
			}
			if (executionInterface.Trace)
			{
				executionInterface.LogText("\t\tThe terminal pivot of {0} is {1}.", name, terminal.name);
			}
		}

		public void AddReferencePath(PreRule rule)
		{
			if (terminal != null)
			{
				terminal.ReferencePaths[rule.Name] = terminal.FindLCAPath(rule);
				if (executionInterface.Trace)
				{
					executionInterface.LogText("\t\tThe path from pivot {0} to rule {1} is {2}", terminal.Name, rule.Name, terminal.ReferencePaths[rule.Name]);
				}
			}
		}
	}
}
