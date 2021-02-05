using System;
using System.Collections;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class DataInstance
	{
		private static int nextID = 0;

		private static Hashtable instanceHash = new Hashtable();

		private int id;

		private string name;

		private DataObject dataObject;

		private SortedList children = new SortedList();

		public int ID
		{
			get
			{
				return id;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public DataObject DataObject
		{
			get
			{
				return dataObject;
			}
		}

		public DataInstance(string name, DataObject dataObject)
		{
			id = nextID++;
			instanceHash[id] = this;
			this.name = name;
			this.dataObject = dataObject;
		}

		public static DataInstance GetInstanceFromID(string id)
		{
			return (DataInstance)instanceHash[Convert.ToInt32(id)];
		}

		public override string ToString()
		{
			return name;
		}

		public DataInstance CreateInstance(DataObjectLink objectLink, string instanceName, StepID stepID, DataScope scope, DataChangeType changeType)
		{
			DataInstance child = new DataInstance(instanceName, objectLink.Target);
			return LinkInstance(objectLink, child, stepID, scope, changeType);
		}

		public DataInstance LinkInstance(DataObjectLink objectLink, DataInstance child, StepID stepID, DataScope scope, DataChangeType changeType)
		{
			if (child.DataObject != objectLink.Target)
			{
				throw new Exception("Trying to create link to the wrong type of instance");
			}
			DataInstanceLink dataInstanceLink = new DataInstanceLink(objectLink, this, child, stepID, scope, changeType);
			AddChildLink(objectLink.Name, dataInstanceLink);
			child.AddChildLink(objectLink.Backlink.Name, dataInstanceLink.Backlink);
			return child;
		}

		private void AddChildLink(string objectName, DataInstanceLink link)
		{
			if (!children.Contains(objectName))
			{
				children[objectName] = new ArrayList();
			}
			(children[objectName] as ArrayList).Add(link);
		}

		public DataInstanceSet GetChildren(string objectName, StepID stepID)
		{
			DataInstanceSet result = null;
			if (!dataObject.HasObject(objectName))
			{
				return result;
			}
			result = new DataInstanceSet(dataObject.GetObject(objectName).Target);
			if (!children.Contains(objectName))
			{
				return result;
			}
			foreach (DataInstanceLink item in (ArrayList)children[objectName])
			{
				if (item.IsInScope(stepID))
				{
					item.ApplyToSet(result);
				}
			}
			return result;
		}

		public DataInstanceSet ProcessNode(Node node, StepID stepID, ExecutionInterface executionInterface)
		{
			string text;
			string attribute;
			string[] array;
			string text2;
			if (node.Name == "Object")
			{
				text = (node.HasAttribute("Key1") ? node.GetAttribute("Key1") : "Dump");
				attribute = node.GetAttribute("Key2");
				array = new string[1]
				{
					node.GetAttribute("Key3")
				};
				text2 = node.GetAttribute("Key4");
			}
			else
			{
				if (!(node.Name == "Data"))
				{
					throw new Exception("Called ProcessNode on invalid element");
				}
				text = (node.HasAttribute("Command") ? node.GetAttribute("Command") : "Add");
				attribute = node.GetAttribute("Path");
				text2 = node.GetAttribute("Backlink");
				string expression = (node.HasAttribute("Value") ? "@Value" : ((!node.HasAttribute("Query")) ? "../Result" : node.GetAttribute("Query")));
				array = ExtFunction.Arrayify(ExtFunction.Evaluate(node, expression));
			}
			if (executionInterface.Trace)
			{
				executionInterface.LogText("Processing Node:{0} cmd:{1} lookup:{2} back:{3} stepID:{4}", node.Name, text, attribute, text2, stepID.ToString());
				string[] array2 = array;
				foreach (string text3 in array2)
				{
					executionInterface.LogText("  val: {0}", text3);
				}
			}
			DataInstanceSet dataInstanceSet = new DataInstanceSet(this);
			DataObjectLink dataObjectLink = null;
			if (text == "Dump")
			{
				if (executionInterface.Trace)
				{
					executionInterface.LogText("Dumping with lookup {0} at stepID {1}", attribute, stepID.ToString());
				}
				DataInstanceSet dataInstanceSet2;
				if (attribute.Length > 0)
				{
					DataParser dataParser = new DataParser(attribute, true, node);
					dataInstanceSet2 = dataParser.Evaluate(dataInstanceSet, stepID);
				}
				else
				{
					dataInstanceSet2 = dataInstanceSet;
				}
				if (executionInterface.Trace)
				{
					executionInterface.LogText("  result object {0}", dataInstanceSet2.DataObject);
					{
						foreach (DataInstance item in dataInstanceSet2)
						{
							executionInterface.LogText("    child {0}", item.Name);
						}
						return dataInstanceSet2;
					}
				}
				return dataInstanceSet2;
			}
			if (executionInterface.Trace)
			{
				executionInterface.LogText("Changing with cmd {0}  lookup:{1}  at stepID:{2}", text, attribute, stepID.ToString());
			}
			if (attribute.Length > 0)
			{
				DataParser dataParser = new DataParser(attribute, false, node);
				DataInstanceSet dataInstanceSet3 = dataParser.Evaluate(dataInstanceSet, stepID);
				if (!dataInstanceSet3.DataObject.HasObject(dataInstanceSet3.FinalName))
				{
					if (text2.Length == 0)
					{
						text2 = "..";
					}
					if (text == "Link")
					{
						dataParser = new DataParser(array[0], true, node);
						DataInstanceSet dataInstanceSet4 = dataParser.Evaluate(dataInstanceSet, stepID);
						dataObjectLink = dataInstanceSet3.DataObject.LinkObject(text2, dataInstanceSet3.FinalName, dataInstanceSet4.DataObject);
					}
					else
					{
						dataObjectLink = dataInstanceSet3.DataObject.CreateObject(text2, dataInstanceSet3.FinalName);
					}
				}
				DataScope scope = (node.HasAttribute("Scope") ? ((DataScope)Enum.Parse(typeof(DataScope), node.GetAttribute("Scope"))) : DataScope.Global);
				dataObjectLink = dataInstanceSet3.DataObject.GetObject(dataInstanceSet3.FinalName);
				DataInstanceSet dataInstanceSet2 = new DataInstanceSet(dataObjectLink.Target);
				{
					foreach (DataInstance item2 in dataInstanceSet3)
					{
						string[] array3 = array;
						foreach (string text4 in array3)
						{
							switch (text)
							{
							case "Add":
								dataInstanceSet2.Add(item2.CreateInstance(dataObjectLink, text4, stepID, scope, DataChangeType.Add));
								break;
							case "Replace":
								dataInstanceSet2.Add(item2.CreateInstance(dataObjectLink, text4, stepID, scope, DataChangeType.Replace));
								break;
							case "Delete":
								dataInstanceSet2.Add(item2.CreateInstance(dataObjectLink, text4, stepID, scope, DataChangeType.Delete));
								break;
							case "Link":
							{
								dataParser = new DataParser(text4, true, node);
								DataInstanceSet dataInstanceSet5 = dataParser.Evaluate(dataInstanceSet, stepID);
								foreach (DataInstance item3 in dataInstanceSet5)
								{
									dataInstanceSet2.Add(item2.LinkInstance(dataObjectLink, item3, stepID, scope, DataChangeType.Add));
								}
								break;
							}
							}
						}
					}
					return dataInstanceSet2;
				}
			}
			throw new Exception("Must specify path when changing data");
		}
	}
}
