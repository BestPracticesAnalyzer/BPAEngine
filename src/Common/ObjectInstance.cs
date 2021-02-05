using System;
using System.Collections;
using System.Data;
using System.Text;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class ObjectInstance
	{
		private ExecutionInterface executionInterface;

		private Node objectNode;

		private ArrayList settingNodes;

		private ObjectParentData opd;

		private Document localDoc;

		private int lastActivity;

		public Node ObjectNode
		{
			get
			{
				return objectNode;
			}
			set
			{
				UpdateLastActivity();
				objectNode = value;
			}
		}

		public ArrayList SettingNodes
		{
			get
			{
				return settingNodes;
			}
			set
			{
				UpdateLastActivity();
				settingNodes = value;
			}
		}

		internal ObjectParentData OPD
		{
			get
			{
				return opd;
			}
			set
			{
				opd = value;
			}
		}

		public ObjectInstance(ExecutionInterface executionInterface, ObjectInstance objInstIn)
		{
			this.executionInterface = executionInterface;
			settingNodes = new ArrayList();
			if (objInstIn != null)
			{
				localDoc = objInstIn.localDoc;
				objectNode = localDoc.ImportNode(objInstIn.objectNode, false);
				opd = objInstIn.opd.Clone();
			}
			else
			{
				localDoc = new BPADocument(executionInterface);
				objectNode = null;
				opd = new ObjectParentData();
				Node node = localDoc.CreateNode("LocalRoot");
				node.AddToRoot();
			}
			UpdateLastActivity();
		}

		public void AddSettingNode(object settingNode, IList propVals)
		{
			UpdateLastActivity();
			Node srcNode = (Node)settingNode;
			if (propVals != null)
			{
				Node node = localDoc.ImportNode(srcNode, false);
				settingNodes.Add(node);
				Common.AddValueElements(node, propVals, opd);
			}
		}

		[Obsolete("This method is obsolete.  Use AddMultipleSettingNodes instead.")]
		public void AddCountSettingNodes(SortedList settingNames)
		{
			AddMultipleSettingNodes(settingNames);
		}

		public void AddMultipleSettingNodes(SortedList settingNames)
		{
			UpdateLastActivity();
			foreach (string key in settingNames.Keys)
			{
				Node node = localDoc.CreateNode("Setting");
				node.SetAttribute("Key1", key);
				settingNodes.Add(node);
				IList vals = settingNames[key] as IList;
				Common.AddValueElements(node, vals, opd);
			}
		}

		[Obsolete("This method is obsolete.  Count processing is now handled by the engine itself.")]
		public static void CollectCountPropTotals(SortedList countPropTotals, string val)
		{
			if (!countPropTotals.ContainsKey(val))
			{
				countPropTotals.Add(val, 1);
			}
			else
			{
				countPropTotals[val] = (int)countPropTotals[val] + 1;
			}
		}

		internal bool CheckTimeout()
		{
			int num = ProcessingInfo.GetTimeout(objectNode) * 1000;
			return Environment.TickCount - lastActivity > num;
		}

		public string GetObjectAttribute(string attrName)
		{
			return objectNode.GetAttribute(attrName);
		}

		public string GetSettingAttribute(object setting, string name)
		{
			return ((Node)setting).GetAttribute(name);
		}

		public void SetObjectAttribute(string attrName, string attrValue)
		{
			UpdateLastActivity();
			objectNode.SetAttribute(attrName, attrValue);
		}

		public void UpdateLastActivity()
		{
			lastActivity = Environment.TickCount;
		}

		public void SetInheritedProperty(string propName, object propValue)
		{
			opd.SetInheritedProperty(propName, propValue);
		}

		public object InheritedProperty(string propName)
		{
			return opd.InheritedProperty(propName);
		}

		private static Type InferType(string format, Type defaultType)
		{
			switch (format)
			{
			case "System.DateTime":
				return typeof(DateTime);
			case "System.String":
				return typeof(string);
			case "System.Int32":
				return typeof(int);
			case "System.Double":
				return typeof(double);
			default:
				return defaultType;
			}
		}

		public void AddColumn(DataRow dataRow, Type defaultType, string format, string property, IList propVals)
		{
			UpdateLastActivity();
			if (propVals == null || propVals.Count == 0)
			{
				return;
			}
			Type type = InferType(format, defaultType);
			if (!dataRow.Table.Columns.Contains(property))
			{
				dataRow.Table.Columns.Add(new DataColumn(property, type));
			}
			if (propVals.Count == 1 && propVals[0].GetType() == type)
			{
				dataRow[property] = propVals[0];
			}
			else
			{
				if (type != typeof(string))
				{
					return;
				}
				StringBuilder stringBuilder = new StringBuilder();
				foreach (object propVal in propVals)
				{
					stringBuilder.Append(propVal.ToString());
					stringBuilder.Append(";");
				}
				dataRow[property] = stringBuilder.ToString().TrimEnd(';');
			}
		}
	}
}
