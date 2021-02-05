using System.Xml;
using System.Xml.XPath;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public abstract class PostNode
	{
		protected XmlElement element;

		protected ExecutionInterface executionInterface;

		protected PreNode preNode;

		public string Name
		{
			get
			{
				return preNode.Name;
			}
		}

		public PostNode(XmlElement element, PreNode preNode, ExecutionInterface executionInterface)
		{
			this.element = element;
			this.preNode = preNode;
			this.executionInterface = executionInterface;
		}

		protected object EvaluateAttribute(string attributeName, CustomContext context)
		{
			XPathNavigator xPathNavigator = element.CreateNavigator();
			XPathExpression xPathExpression = xPathNavigator.Compile(element.GetAttribute(attributeName));
			xPathExpression.SetContext(context);
			object obj = xPathNavigator.Evaluate(xPathExpression);
			if (executionInterface.Trace)
			{
				executionInterface.LogText("\t\tAttribute {0}=\"{1}\" expected type={2} actual type={3} result={4}", attributeName, element.GetAttribute(attributeName), xPathExpression.ReturnType, obj.GetType(), obj);
			}
			return obj;
		}

		protected bool EvaluateReference(RuleReference reference, CustomContext context)
		{
			XPathNavigator xPathNavigator = element.CreateNavigator();
			XPathExpression expr = xPathNavigator.Compile(reference.Path);
			XPathNodeIterator xPathNodeIterator = xPathNavigator.Select(expr);
			if (reference.Pivot != null)
			{
				if (!xPathNodeIterator.MoveNext())
				{
					executionInterface.LogText("Couldn't find pivot {0} in XML", reference.Pivot.Name);
					return false;
				}
				xPathNodeIterator.Current.MoveToAttribute("KeyValue", "");
				string value = xPathNodeIterator.Current.Value;
				if (executionInterface.Trace)
				{
					executionInterface.LogText("\t\tEvaluating pivot reference to {0} with key '{1}'", reference.Pivot.Name, value);
				}
				PostPivot postPivot = (PostPivot)reference.Pivot.KeyHash[value];
				if (postPivot == null)
				{
					return false;
				}
				PostPivot terminal = postPivot.Terminal;
				if (terminal == null)
				{
					return false;
				}
				xPathNodeIterator = terminal.EvaluateRule(reference.Name);
			}
			else if (reference.IsDependency)
			{
				bool flag = false;
				while (xPathNodeIterator.MoveNext())
				{
					if (xPathNodeIterator.Current.MoveToAttribute("Pass", ""))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
				xPathNodeIterator = xPathNavigator.Select(expr);
			}
			context.AddParam(((reference.Pivot == null) ? "" : (reference.Pivot.Name + ".")) + reference.Name, xPathNodeIterator);
			return true;
		}
	}
}
