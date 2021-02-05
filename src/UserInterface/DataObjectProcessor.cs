using System.Collections;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class DataObjectProcessor : ObjectProcessor
	{
		public class DataNavigator : XPathNavigator
		{
			private DataObjectLink link;

			private DataInstance parent;

			private DataInstance current;

			private XPathNodeType nodeType;

			private XmlNameTable nameTable;

			private StepID stepID;

			public override XPathNodeType NodeType
			{
				get
				{
					return nodeType;
				}
			}

			public override string LocalName
			{
				get
				{
					string array;
					switch (nodeType)
					{
					case XPathNodeType.Element:
						array = link.Name;
						break;
					case XPathNodeType.Root:
						array = string.Empty;
						break;
					case XPathNodeType.Attribute:
						array = "Name";
						break;
					default:
						array = string.Empty;
						break;
					}
					return nameTable.Add(array);
				}
			}

			public override string Name
			{
				get
				{
					return LocalName;
				}
			}

			public override string NamespaceURI
			{
				get
				{
					return string.Empty;
				}
			}

			public override string Prefix
			{
				get
				{
					return nameTable.Add(string.Empty);
				}
			}

			public override string Value
			{
				get
				{
					switch (nodeType)
					{
					case XPathNodeType.Attribute:
						return current.Name;
					default:
						return string.Empty;
					}
				}
			}

			public override string BaseURI
			{
				get
				{
					return nameTable.Add(string.Empty);
				}
			}

			public override bool IsEmptyElement
			{
				get
				{
					return !HasChildren;
				}
			}

			public override string XmlLang
			{
				get
				{
					return string.Empty;
				}
			}

			public override XmlNameTable NameTable
			{
				get
				{
					return nameTable;
				}
			}

			public override bool HasAttributes
			{
				get
				{
					return true;
				}
			}

			public override bool HasChildren
			{
				get
				{
					foreach (string @object in parent.DataObject.Objects)
					{
						if (@object != ".." && current.GetChildren(@object, stepID).Count > 0)
						{
							return true;
						}
					}
					return false;
				}
			}

			private DataNavigator()
			{
			}

			public DataNavigator(DataInstance current, StepID stepID)
			{
				this.current = current;
				nodeType = XPathNodeType.Element;
				nameTable = new NameTable();
				GetLinkFromParent();
			}

			private void GetLinkFromParent()
			{
				if (current.DataObject.HasObject(".."))
				{
					parent = current.GetChildren("..", stepID)[0];
					link = current.DataObject.GetObject("..").Backlink;
				}
				else
				{
					nodeType = XPathNodeType.Root;
					parent = null;
					link = null;
				}
			}

			public override XPathNavigator Clone()
			{
				DataNavigator dataNavigator = new DataNavigator();
				dataNavigator.current = current;
				dataNavigator.link = link;
				dataNavigator.parent = parent;
				dataNavigator.nodeType = nodeType;
				dataNavigator.nameTable = nameTable;
				dataNavigator.stepID = stepID;
				return dataNavigator;
			}

			public override string GetAttribute(string localName, string namespaceURI)
			{
				if (localName == "Name")
				{
					return current.Name;
				}
				return string.Empty;
			}

			public override bool MoveToAttribute(string localName, string namespaceURI)
			{
				if (nodeType == XPathNodeType.Element && localName == "Name")
				{
					nodeType = XPathNodeType.Attribute;
					return true;
				}
				return false;
			}

			public override bool MoveToFirstAttribute()
			{
				if (nodeType == XPathNodeType.Element)
				{
					nodeType = XPathNodeType.Attribute;
					return true;
				}
				return false;
			}

			public override bool MoveToNextAttribute()
			{
				return false;
			}

			public override string GetNamespace(string name)
			{
				return string.Empty;
			}

			public override bool MoveToNamespace(string name)
			{
				return false;
			}

			public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
			{
				return false;
			}

			public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
			{
				return false;
			}

			public override bool MoveToNext()
			{
				return MoveInDirection(true);
			}

			public override bool MoveToPrevious()
			{
				return MoveInDirection(false);
			}

			private bool MoveInDirection(bool forward)
			{
				if (nodeType != XPathNodeType.Element)
				{
					return false;
				}
				ArrayList objects = parent.DataObject.Objects;
				DataInstanceSet children = parent.GetChildren(link.Name, stepID);
				int num = children.IndexOf(current);
				int num2 = (forward ? 1 : (-1));
				if (num == -1)
				{
					return false;
				}
				if ((forward && num == children.Count - 1) || (!forward && num == 0))
				{
					num = objects.IndexOf(link.Name);
					string text;
					DataInstanceSet children2;
					while (true)
					{
						num += num2;
						if (num >= objects.Count || num < 0)
						{
							return false;
						}
						text = (string)objects[num];
						if (!(text == ".."))
						{
							children2 = parent.GetChildren(text, stepID);
							if (children2.Count != 0)
							{
								break;
							}
						}
					}
					link = parent.DataObject.GetObject(text);
					current = children2[(!forward) ? (children2.Count - 1) : 0];
				}
				else
				{
					current = children[num + num2];
				}
				return true;
			}

			public override bool MoveToFirst()
			{
				if (nodeType != XPathNodeType.Element)
				{
					return false;
				}
				current = parent;
				GetLinkFromParent();
				return MoveToFirstChild();
			}

			public override bool MoveToFirstChild()
			{
				foreach (string @object in current.DataObject.Objects)
				{
					if (!(@object == ".."))
					{
						DataInstanceSet children = current.GetChildren(@object, stepID);
						if (children.Count > 0)
						{
							link = current.DataObject.GetObject(@object);
							parent = current;
							current = children[0];
							return true;
						}
					}
				}
				return false;
			}

			public override bool MoveToParent()
			{
				if (nodeType == XPathNodeType.Element)
				{
					if (current.DataObject.HasObject(".."))
					{
						current = current.GetChildren("..", stepID)[0];
						GetLinkFromParent();
						return true;
					}
					return false;
				}
				if (nodeType == XPathNodeType.Root)
				{
					return false;
				}
				nodeType = XPathNodeType.Element;
				return true;
			}

			public override void MoveToRoot()
			{
				while (MoveToParent())
				{
				}
			}

			public override bool MoveTo(XPathNavigator other)
			{
				DataNavigator dataNavigator = other as DataNavigator;
				if (dataNavigator == null)
				{
					return false;
				}
				current = dataNavigator.current;
				link = dataNavigator.link;
				parent = dataNavigator.parent;
				nodeType = dataNavigator.nodeType;
				nameTable = dataNavigator.nameTable;
				return true;
			}

			public override bool MoveToId(string id)
			{
				return false;
			}

			public override bool IsSamePosition(XPathNavigator other)
			{
				DataNavigator dataNavigator = other as DataNavigator;
				if (dataNavigator != null && current == dataNavigator.current)
				{
					return nodeType == dataNavigator.nodeType;
				}
				return false;
			}
		}

		private static DataObject rootObject = new DataObject();

		private static DataInstance rootInstance = new DataInstance("", rootObject);

		private static StepID stepID = new StepID("1");

		private DataInstance current;

		public DataObjectProcessor(ExecutionInterface executionInterface, ObjectInstance objInstIn)
			: base(executionInterface, objInstIn)
		{
		}

		public static DataInstance GetRootInstance()
		{
			return rootInstance;
		}

		public static void SetStepID(string step)
		{
			stepID = new StepID(step);
		}

		internal static StepID GetStepID()
		{
			return stepID;
		}

		public override void ProcessObject()
		{
			string objectAttribute = objInstIn.GetObjectAttribute("DataID");
			if (objectAttribute.Length == 0)
			{
				current = rootInstance;
			}
			else
			{
				current = DataInstance.GetInstanceFromID(objectAttribute);
			}
			Node objectNode = objInstIn.ObjectNode;
			DataInstanceSet dataInstanceSet = current.ProcessNode(objectNode, stepID, executionInterface);
			foreach (DataInstance item in dataInstanceSet)
			{
				ObjectInstance objectInstance = new ObjectInstance(executionInterface, objInstIn);
				objectInstance.SetObjectAttribute("Name", item.Name);
				foreach (object settingNode in objInstIn.SettingNodes)
				{
					string settingAttribute = objInstIn.GetSettingAttribute(settingNode, "Key1");
					if (settingAttribute.Length == 0)
					{
						objectInstance.AddSettingNode(settingNode, new object[1]
						{
							item
						});
					}
					else if (settingAttribute == "__DATA_ID")
					{
						objectInstance.AddSettingNode(settingNode, new object[1]
						{
							item.ID
						});
					}
					else
					{
						DataParser dataParser = new DataParser(settingAttribute, true, objInstIn.ObjectNode);
						DataInstanceSet dataInstanceSet2 = dataParser.Evaluate(new DataInstanceSet(item), stepID);
						objectInstance.AddSettingNode(settingNode, dataInstanceSet2.ToArray());
					}
				}
				objInstOutList.Add(objectInstance);
			}
		}

		public static string EncodeString(object val)
		{
			string text = val.ToString();
			return string.Format("{{{0}}}{1}", text.Length, text);
		}

		public static object Data(XsltContext context, object[] args, XPathNavigator nav)
		{
			string text = (string)nav.Evaluate("string(ancestor-or-self::*[@DataID][1]/@DataID)");
			DataInstance dataInstance = ((text.Length != 0) ? DataInstance.GetInstanceFromID(text) : rootInstance);
			NodeSetIterator nodeSetIterator = new NodeSetIterator();
			nodeSetIterator.Add(new DataNavigator(dataInstance, GetStepID()));
			return nodeSetIterator;
		}
	}
}
