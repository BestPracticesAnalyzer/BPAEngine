using System.Collections;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class PostRule : PostNode
	{
		private bool isTrue;

		protected PreRule preRule
		{
			get
			{
				return (PreRule)preNode;
			}
		}

		public bool IsTrue
		{
			get
			{
				return isTrue;
			}
			set
			{
				isTrue = value;
			}
		}

		public XmlElement Element
		{
			get
			{
				return element;
			}
		}

		public PreRule PreRule
		{
			get
			{
				return preRule;
			}
		}

		public PostRule(XmlElement element, PreRule preRule, ExecutionInterface executionInterface)
			: base(element, preRule, executionInterface)
		{
		}

		private object EvaluateParam(string paramName, CustomContext context)
		{
			object obj = EvaluateAttribute(paramName, context);
			if (obj is double)
			{
				return obj;
			}
			return ExtFunction.Stringify(obj);
		}

		public bool EvaluateQuery()
		{
			ArrayList arrayList = new ArrayList();
			CustomContext customContext = new CustomContext();
			customContext.AddNamespace("exbpa", "http://exbpa");
			foreach (RuleReference value in preRule.References.Values)
			{
				if (value.IsInQuery && !EvaluateReference(value, customContext))
				{
					if (executionInterface.Trace)
					{
						executionInterface.LogText("Reference from {0} to {1} failed; exiting rule", preRule.Name, value.Name);
					}
					return false;
				}
			}
			object obj = EvaluateAttribute("Query", customContext);
			if (preRule.CheckCondition)
			{
				isTrue = ExtFunction.Boolify(obj);
				element.SetAttribute("Pass", isTrue ? "True" : "False");
			}
			else
			{
				element.SetAttribute("Pass", "Unknown");
			}
			if (obj is XPathNodeIterator)
			{
				XPathNodeIterator xPathNodeIterator = (XPathNodeIterator)obj;
				if (executionInterface.Trace)
				{
					executionInterface.LogText("\t\t\tnodeset:");
				}
				while (xPathNodeIterator.MoveNext())
				{
					XPathNavigator current = xPathNodeIterator.Current;
					XmlNode xmlNode;
					if (current.NodeType == XPathNodeType.Element)
					{
						if (current is IHasXmlNode)
						{
							xmlNode = ((IHasXmlNode)current).GetNode().CloneNode(true);
						}
						else
						{
							xmlNode = element.OwnerDocument.CreateElement(current.Name);
							if (current.MoveToFirstAttribute())
							{
								do
								{
									((XmlElement)xmlNode).SetAttribute(current.Name, current.Value);
								}
								while (current.MoveToNextAttribute());
							}
						}
					}
					else
					{
						xmlNode = element.OwnerDocument.CreateTextNode(current.Value);
					}
					arrayList.Add(xmlNode);
					if (executionInterface.Trace)
					{
						executionInterface.LogText("\t\t\t\tnode '{0}' of type '{1}' InnerText '{2}'", xPathNodeIterator.Current.ToString(), xmlNode.GetType(), xmlNode.InnerText);
					}
				}
			}
			else if (obj is bool)
			{
				if ((bool)obj)
				{
					arrayList.Add(element.OwnerDocument.CreateTextNode("1"));
				}
				if (executionInterface.Trace)
				{
					executionInterface.LogText("\t\t\tboolean: {0}", obj.ToString());
				}
			}
			else if (obj is double)
			{
				if (executionInterface.Trace)
				{
					executionInterface.LogText("\t\t\tnumber: {0}", ExtFunction.Stringify(obj));
				}
				arrayList.Add(element.OwnerDocument.CreateTextNode(ExtFunction.Stringify(obj)));
			}
			else
			{
				if (executionInterface.Trace)
				{
					executionInterface.LogText("\t\t\tother: {0}", obj.ToString());
				}
				arrayList.Add(element.OwnerDocument.CreateTextNode(obj.ToString()));
			}
			foreach (XmlNode item in arrayList)
			{
				XmlElement xmlElement = element.OwnerDocument.CreateElement("Result");
				xmlElement.AppendChild(item);
				element.AppendChild(xmlElement);
			}
			return true;
		}

		public void CreateMessage()
		{
			CustomContext customContext = new CustomContext();
			customContext.AddNamespace("exbpa", "http://exbpa");
			if (isTrue != preRule.ConditionToTrigger)
			{
				return;
			}
			if (executionInterface.Trace)
			{
				executionInterface.LogText("\tCreating Message for {0}", preRule.Name);
			}
			foreach (RuleReference value in preRule.References.Values)
			{
				EvaluateReference(value, customContext);
			}
			XmlElement xmlElement = element.OwnerDocument.CreateElement("Message");
			object[] array = new object[preRule.Param.Length];
			foreach (XmlAttribute attribute in element.Attributes)
			{
				xmlElement.SetAttribute(attribute.Name, attribute.Value);
			}
			for (int i = 0; i < preRule.Param.Length; i++)
			{
				if (preRule.Param[i] != null)
				{
					array[i] = EvaluateParam("P" + i, customContext);
					xmlElement.SetAttribute("P" + i, ExtFunction.Stringify(array[i]));
				}
				else if (element.HasAttribute("S" + i))
				{
					array[i] = element.GetAttribute("S" + i);
				}
			}
			string text = string.Format(element.GetAttribute("Text"), array);
			if (Common.IsValidXmlString(text))
			{
				xmlElement.InnerText = text;
			}
			if (element.HasAttribute("Title"))
			{
				string text2 = string.Format(element.GetAttribute("Title"), array);
				if (Common.IsValidXmlString(text2))
				{
					xmlElement.SetAttribute("Title", text2);
				}
			}
			element.ParentNode.InsertAfter(xmlElement, element);
			Node messageNode = new BPANode(executionInterface.Options.Data, xmlElement);
			Node ruleNode = new BPANode(executionInterface.Options.Data, element);
			foreach (IssueProcessor issueProcessor in preRule.AllRules.IssueProcessors)
			{
				issueProcessor.ProcessIssue(messageNode, ruleNode, array);
			}
		}
	}
}
