using System.Collections;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class PostPivot : PostNode
	{
		private string keyValue;

		private string keyRefValue;

		private PostPivot terminal;

		private Hashtable refResults = new Hashtable();

		protected PrePivot prePivot
		{
			get
			{
				return (PrePivot)preNode;
			}
		}

		public string KeyValue
		{
			get
			{
				return keyValue;
			}
		}

		public string KeyRefValue
		{
			get
			{
				return keyRefValue;
			}
		}

		public PostPivot Terminal
		{
			get
			{
				return terminal;
			}
			set
			{
				terminal = value;
			}
		}

		public PostPivot(XmlElement element, PrePivot prePivot, ExecutionInterface executionInterface)
			: base(element, prePivot, executionInterface)
		{
		}

		public void EvaluateAttributes()
		{
			CustomContext context = new CustomContext();
			foreach (RuleReference value in preNode.References.Values)
			{
				if (!EvaluateReference(value, context))
				{
					if (executionInterface.Trace)
					{
						executionInterface.LogText("Reference from {0} to {1} in pivot failed!", prePivot.Name, value.Name);
					}
					return;
				}
			}
			keyValue = ExtFunction.Stringify(EvaluateAttribute("Key", context));
			element.SetAttribute("KeyValue", keyValue);
			if (prePivot.Remote != null)
			{
				keyRefValue = ExtFunction.Stringify(EvaluateAttribute("KeyRef", context));
				element.SetAttribute("KeyRefValue", keyRefValue);
			}
			if (executionInterface.Trace)
			{
				executionInterface.LogText("\t\tKeyValue='{0}' KeyRefValue='{1}'", keyValue, keyRefValue);
			}
			prePivot.KeyHash[keyValue] = this;
			if (prePivot.Remote != null)
			{
				PostPivot postPivot = (PostPivot)prePivot.Remote.KeyHash[keyRefValue];
				if (postPivot != null)
				{
					terminal = postPivot.Terminal;
				}
				else
				{
					terminal = null;
				}
			}
			else
			{
				terminal = this;
			}
		}

		public XPathNodeIterator EvaluateRule(string ruleName)
		{
			if (executionInterface.Trace)
			{
				executionInterface.LogText("\t\t\tGetting value of rule {0}", ruleName, prePivot.Name);
			}
			if (!refResults.Contains(ruleName))
			{
				if (executionInterface.Trace)
				{
					executionInterface.LogText("\t\t\t\tEvaluating using path '{0}'", prePivot.ReferencePaths[ruleName]);
				}
				XPathNavigator xPathNavigator = element.CreateNavigator();
				XPathExpression expr = xPathNavigator.Compile((string)prePivot.ReferencePaths[ruleName]);
				XPathNodeIterator xPathNodeIterator = xPathNavigator.Select(expr);
				refResults[ruleName] = xPathNodeIterator;
				XPathNodeIterator xPathNodeIterator2 = xPathNodeIterator.Clone();
				if (executionInterface.Trace)
				{
					while (xPathNodeIterator2.MoveNext())
					{
						executionInterface.LogText("\t\t\t\t\tname='{0}' value='{1}'", xPathNodeIterator2.Current.Name, xPathNodeIterator2.Current.Value);
					}
				}
			}
			return ((XPathNodeIterator)refResults[ruleName]).Clone();
		}
	}
}
