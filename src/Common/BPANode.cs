using System.Xml;
using System.Xml.XPath;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	internal class BPANode : Node
	{
		public override string[] Attributes
		{
			get
			{
				XmlAttributeCollection attributes = ((XmlNode)node).Attributes;
				string[] array = new string[attributes.Count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = attributes[i].Name;
				}
				return array;
			}
		}

		public override Node Parent
		{
			get
			{
				if (((XmlNode)node).ParentNode != null)
				{
					return new BPANode(doc, ((XmlNode)node).ParentNode);
				}
				return null;
			}
		}

		public override Node[] Children
		{
			get
			{
				Node[] array = new Node[((XmlNode)node).ChildNodes.Count];
				for (int i = 0; i < ((XmlNode)node).ChildNodes.Count; i++)
				{
					array[i] = new BPANode(doc, ((XmlNode)node).ChildNodes[i]);
				}
				return array;
			}
		}

		public override string Value
		{
			get
			{
				return ((XmlNode)node).InnerText;
			}
			set
			{
				if (!IsValidXmlString(value))
				{
					((XmlNode)node).InnerText = "";
				}
				else
				{
					((XmlNode)node).InnerText = value;
				}
			}
		}

		public override string Comment
		{
			get
			{
				string result = "";
				XmlNode previousSibling = ((XmlNode)node).PreviousSibling;
				if (previousSibling != null && previousSibling.Name == "#comment")
				{
					result = previousSibling.Value;
				}
				return result;
			}
			set
			{
				XmlNode previousSibling = ((XmlNode)node).PreviousSibling;
				if (previousSibling != null && previousSibling.Name == "#comment")
				{
					if (value.Length > 0)
					{
						previousSibling.Value = value;
					}
					else
					{
						((XmlNode)node).ParentNode.RemoveChild(previousSibling);
					}
				}
				else if (value.Length > 0)
				{
					XmlNode newChild = ((XmlDocument)doc.UnderlyingDocument).CreateComment(value);
					((XmlNode)node).ParentNode.InsertBefore(newChild, (XmlNode)node);
				}
			}
		}

		public override string Name
		{
			get
			{
				return ((XmlNode)node).LocalName;
			}
		}

		public BPANode(Document doc, string name)
			: base(doc, name)
		{
			base.doc = doc;
			node = ((XmlDocument)doc.UnderlyingDocument).CreateElement(name);
		}

		internal BPANode(Document doc, XmlNode node)
			: base(doc, node)
		{
			base.doc = doc;
			base.node = node;
		}

		public override void Delete()
		{
			XmlNode previousSibling = ((XmlNode)node).PreviousSibling;
			if (previousSibling != null && previousSibling.Name == "#comment")
			{
				((XmlNode)node).ParentNode.RemoveChild(previousSibling);
			}
			((XmlNode)node).ParentNode.RemoveChild((XmlNode)node);
		}

		public override void Add(Node child)
		{
			((XmlNode)node).AppendChild((XmlNode)child.UnderlyingNode);
		}

		public override void InsertBefore(Node child, Node nextNode)
		{
			((XmlNode)node).InsertBefore((XmlNode)child.UnderlyingNode, (XmlNode)nextNode.UnderlyingNode);
		}

		public override void InsertAfter(Node child, Node prevNode)
		{
			((XmlNode)node).InsertAfter((XmlNode)child.UnderlyingNode, (XmlNode)prevNode.UnderlyingNode);
		}

		public override void AddToRoot()
		{
			((XmlDocument)doc.UnderlyingDocument).AppendChild((XmlNode)node);
		}

		public override Node[] GetNodes(string query)
		{
			XmlNodeList xmlNodeList = ((XmlNode)node).SelectNodes(query);
			Node[] array = new Node[xmlNodeList.Count];
			for (int i = 0; i < xmlNodeList.Count; i++)
			{
				array[i] = new BPANode(doc, xmlNodeList.Item(i));
			}
			return array;
		}

		public override Node GetNode(string query)
		{
			XmlNode xmlNode = ((XmlNode)node).SelectSingleNode(query);
			if (xmlNode == null)
			{
				return null;
			}
			return new BPANode(doc, xmlNode);
		}

		public override bool HasAttribute(string attrName)
		{
			return ((XmlNode)node).Attributes[attrName] != null;
		}

		public override string GetAttribute(string attrName)
		{
			if (HasAttribute(attrName))
			{
				return ((XmlNode)node).Attributes[attrName].Value;
			}
			return "";
		}

		public override void SetAttribute(string attrName, string val)
		{
			if (val != null && IsValidXmlString(attrName))
			{
				string text = val.ToString();
				if (!IsValidXmlString(text))
				{
					text = string.Empty;
				}
				if (HasAttribute(attrName))
				{
					((XmlNode)node).Attributes[attrName].Value = val;
					return;
				}
				XmlAttribute xmlAttribute = ((XmlNode)node).OwnerDocument.CreateAttribute(attrName);
				xmlAttribute.Value = text;
				((XmlNode)node).Attributes.Append(xmlAttribute);
			}
		}

		public override void DeleteAttribute(string attrName)
		{
			if (HasAttribute(attrName))
			{
				((XmlNode)node).Attributes.RemoveNamedItem(attrName);
			}
		}

		private static bool IsValidXmlString(string s)
		{
			return Common.IsValidXmlString(s);
		}

		public override XPathNavigator CreateNavigator()
		{
			return ((XmlNode)node).CreateNavigator();
		}
	}
}
