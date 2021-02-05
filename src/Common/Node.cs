using System.Xml.XPath;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public abstract class Node : IXPathNavigable
	{
		protected Document doc;

		protected object node;

		public virtual string[] Attributes
		{
			get
			{
				return null;
			}
		}

		public virtual Node Parent
		{
			get
			{
				return null;
			}
		}

		public virtual Node[] Children
		{
			get
			{
				return null;
			}
		}

		public virtual string Name
		{
			get
			{
				return null;
			}
		}

		public virtual string Value
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public virtual object UnderlyingNode
		{
			get
			{
				return node;
			}
		}

		public Document OwnerDocument
		{
			get
			{
				return doc;
			}
		}

		public virtual string Comment
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public Node(Document doc, string name)
		{
		}

		internal Node(Document doc, object node)
		{
		}

		public virtual void Delete()
		{
		}

		public virtual void Add(Node child)
		{
		}

		public virtual void InsertBefore(Node child, Node nextNode)
		{
		}

		public virtual void InsertAfter(Node child, Node prevNode)
		{
		}

		public virtual void AddToRoot()
		{
		}

		public virtual Node GetNode(string query)
		{
			return null;
		}

		public virtual Node[] GetNodes(string query)
		{
			return null;
		}

		public virtual bool HasAttribute(string attrName)
		{
			return false;
		}

		public virtual string GetAttribute(string attrName)
		{
			return "";
		}

		public virtual void SetAttribute(string attrName, string val)
		{
		}

		public virtual void DeleteAttribute(string attrName)
		{
		}

		public virtual XPathNavigator CreateNavigator()
		{
			return null;
		}
	}
}
