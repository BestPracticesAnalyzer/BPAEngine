using System.Collections;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class NodeInfo : TreeNodeInfo
	{
		public enum ElementType
		{
			Variable,
			Substitution,
			Object,
			Instance,
			Setting,
			Value,
			Rule,
			Message,
			Other
		}

		private string format = "";

		private IssueSeverity severity = IssueSeverity.Unknown;

		private ElementType element = ElementType.Other;

		private int valueCount;

		private string displayName = "";

		private NodeInfo parent;

		private ArrayList children;

		private bool displayRight;

		private object extendedData;

		private bool isDummy;

		private string articleGuid = "";

		private bool isHideAll;

		public override string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}

		public string Format
		{
			get
			{
				return format;
			}
			set
			{
				format = value;
			}
		}

		public IssueSeverity Severity
		{
			get
			{
				return severity;
			}
			set
			{
				severity = value;
			}
		}

		public string DisplayName
		{
			get
			{
				if (displayName.Length != 0)
				{
					return displayName;
				}
				return Text;
			}
			set
			{
				displayName = value;
			}
		}

		public bool DisplayNameExists
		{
			get
			{
				return displayName.Length > 0;
			}
		}

		public ElementType Element
		{
			get
			{
				return element;
			}
			set
			{
				element = value;
			}
		}

		public int ValueCount
		{
			get
			{
				return valueCount;
			}
			set
			{
				valueCount = value;
			}
		}

		public NodeInfo Parent
		{
			get
			{
				return parent;
			}
		}

		public ArrayList Children
		{
			get
			{
				return children;
			}
		}

		public bool DisplayRight
		{
			get
			{
				return displayRight;
			}
			set
			{
				displayRight = value;
			}
		}

		public object ExtendedData
		{
			get
			{
				return extendedData;
			}
			set
			{
				extendedData = value;
			}
		}

		public bool IsDummy
		{
			get
			{
				return isDummy;
			}
			set
			{
				isDummy = value;
			}
		}

		public string ArticleGuid
		{
			get
			{
				return articleGuid;
			}
			set
			{
				articleGuid = value;
			}
		}

		public bool IsHideAll
		{
			get
			{
				return isHideAll;
			}
			set
			{
				isHideAll = value;
			}
		}

		public NodeInfo Add(string val, string typeString)
		{
			ElementType elementType = ParseType(typeString);
			if (elementType == ElementType.Other)
			{
				return null;
			}
			return Add(val, elementType);
		}

		public override TreeNodeInfo Add(string val)
		{
			return Add(val, ElementType.Other);
		}

		public NodeInfo Add(string val, ElementType elementType)
		{
			NodeInfo nodeInfo = new NodeInfo();
			nodeInfo.element = elementType;
			nodeInfo.Text = val;
			if (children == null)
			{
				children = new ArrayList();
			}
			children.Add(nodeInfo);
			nodeInfo.parent = this;
			return nodeInfo;
		}

		public NodeInfo Advance(bool forward)
		{
			NodeInfo nodeInfo = null;
			if (forward)
			{
				if (children != null && children.Count > 0)
				{
					return (NodeInfo)children[0];
				}
				nodeInfo = this;
				while (nodeInfo.parent != null)
				{
					int num = nodeInfo.parent.children.IndexOf(nodeInfo);
					if (num + 1 < nodeInfo.parent.children.Count)
					{
						return (NodeInfo)nodeInfo.parent.children[num + 1];
					}
					nodeInfo = nodeInfo.Parent;
				}
				return null;
			}
			if (parent == null)
			{
				return null;
			}
			int num2 = parent.children.IndexOf(this);
			if (num2 == 0)
			{
				return parent;
			}
			nodeInfo = (NodeInfo)parent.children[num2 - 1];
			while (nodeInfo.children != null && nodeInfo.children.Count > 0)
			{
				nodeInfo = (NodeInfo)nodeInfo.children[nodeInfo.children.Count - 1];
			}
			return nodeInfo;
		}

		public void Delete()
		{
			parent.children.Remove(this);
		}

		public void PromoteLeftChildren()
		{
			if (parent == null || children == null || children.Count <= 0)
			{
				return;
			}
			int num = parent.children.IndexOf(this) + 1;
			for (int num2 = children.Count - 1; num2 >= 0; num2--)
			{
				NodeInfo nodeInfo = (NodeInfo)children[num2];
				if (!nodeInfo.DisplayRight)
				{
					children.Remove(nodeInfo);
					if (num >= parent.children.Count)
					{
						parent.children.Add(nodeInfo);
					}
					else
					{
						parent.children.Insert(num, nodeInfo);
					}
					nodeInfo.parent = parent;
				}
			}
		}

		public NodeInfo Remove()
		{
			if (children != null)
			{
				foreach (NodeInfo child in children)
				{
					parent.children.Add(child);
					child.parent = parent;
				}
				children = null;
			}
			parent.children.Remove(this);
			return parent;
		}

		public static ElementType ParseType(string name)
		{
			switch (name)
			{
			case "Variable":
				return ElementType.Variable;
			case "Substitution":
				return ElementType.Substitution;
			case "Object":
				return ElementType.Object;
			case "Instance":
				return ElementType.Instance;
			case "Setting":
				return ElementType.Setting;
			case "Value":
				return ElementType.Value;
			case "Rule":
				return ElementType.Rule;
			case "Message":
				return ElementType.Message;
			default:
				return ElementType.Other;
			}
		}
	}
}
