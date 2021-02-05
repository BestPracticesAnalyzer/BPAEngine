using System;
using System.Collections;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class ObjectInstanceList
	{
		private SortedList objectInstances;

		private SortedList countSettings;

		private ExecutionInterface execInterface;

		private ObjectInstance objInstIn;

		private string orderBy = "";

		private string ident = "";

		private string countOver = "";

		private bool ascending = true;

		private string keyAttribute = "Key1";

		private int range;

		private int appendValue;

		private bool processCount = true;

		private bool processIdent = true;

		private bool processSort = true;

		private bool processRange = true;

		public int Count
		{
			get
			{
				return objectInstances.Count;
			}
		}

		public ObjectInstance this[int i]
		{
			get
			{
				if (ascending)
				{
					return (ObjectInstance)objectInstances.GetByIndex(i);
				}
				return (ObjectInstance)objectInstances.GetByIndex(objectInstances.Count - 1 - i);
			}
		}

		public bool ProcessCount
		{
			get
			{
				return processCount;
			}
			set
			{
				processCount = value;
			}
		}

		public bool ProcessIdent
		{
			get
			{
				return processIdent;
			}
			set
			{
				processIdent = value;
			}
		}

		public bool ProcessSort
		{
			get
			{
				return processSort;
			}
			set
			{
				processSort = value;
			}
		}

		public bool ProcessRange
		{
			get
			{
				return processRange;
			}
			set
			{
				processRange = value;
			}
		}

		public ObjectInstanceList(ExecutionInterface execInterface, ObjectInstance objInstIn)
		{
			this.execInterface = execInterface;
			this.objInstIn = objInstIn;
			objectInstances = new SortedList();
			countSettings = new SortedList();
			try
			{
				string text = objInstIn.GetObjectAttribute("Sort").Trim();
				string[] array = text.Split(' ');
				foreach (string text2 in array)
				{
					if (text2.Length > 0 && orderBy.Length == 0)
					{
						orderBy = text2;
						continue;
					}
					switch (text2)
					{
					case "ASC":
					case "ASCENDING":
						ascending = true;
						continue;
					case "DESC":
					case "DESCENDING":
						ascending = false;
						continue;
					}
					if (text2.StartsWith("Key"))
					{
						keyAttribute = text2;
					}
				}
				if (objInstIn.GetObjectAttribute("Range").Length > 0)
				{
					range = int.Parse(objInstIn.GetObjectAttribute("Range"));
				}
				if (objInstIn.GetObjectAttribute("Ident").Length > 0)
				{
					ident = objInstIn.GetObjectAttribute("Ident");
				}
				if (objInstIn.GetObjectAttribute("Count").Length > 0)
				{
					countOver = objInstIn.GetObjectAttribute("Count");
				}
			}
			catch (Exception exception)
			{
				execInterface.LogException(exception);
				range = 0;
				orderBy = "";
				ident = "";
				countOver = "";
				keyAttribute = "Key1";
			}
		}

		public void Add(ObjectInstance objInstOut)
		{
			string arg = "";
			string text = "*";
			foreach (Node settingNode in objInstOut.SettingNodes)
			{
				string text2 = "";
				Node node2 = settingNode.GetNode("Value");
				if (node2 != null)
				{
					text2 = node2.Value;
				}
				if (settingNode.GetAttribute(keyAttribute) == orderBy && processSort)
				{
					arg = text2;
				}
				if (settingNode.GetAttribute(keyAttribute) == ident && processIdent)
				{
					objInstOut.SetObjectAttribute("Name", text2);
				}
				if (settingNode.GetAttribute(keyAttribute) == countOver)
				{
					text = text2;
				}
			}
			if (processCount && countOver.Length > 0)
			{
				if (objectInstances.Count == 0)
				{
					objInstOut.SettingNodes.Clear();
					objInstOut.SetObjectAttribute("Name", countOver);
					objectInstances.Add("", objInstOut);
				}
				if (countSettings.Contains(text))
				{
					Node node3 = (Node)countSettings[text];
					Node node4 = node3.Children[0];
					node4.Value = (int.Parse(node4.Value) + 1).ToString();
				}
				else
				{
					Node node5 = objInstOut.ObjectNode.OwnerDocument.CreateNode("Setting");
					node5.SetAttribute("Key1", text);
					Common.AddValueElements(node5, new object[1]
					{
						"1"
					}, objInstOut.OPD);
					((ObjectInstance)objectInstances[""]).SettingNodes.Add(node5);
					countSettings.Add(text, node5);
				}
			}
			else
			{
				appendValue++;
				objectInstances.Add(arg + 'ÿ' + appendValue.ToString("x8"), objInstOut);
			}
			if (range > 0 && range < objectInstances.Count && processRange)
			{
				if (ascending)
				{
					objectInstances.RemoveAt(objectInstances.Count - 1);
				}
				else
				{
					objectInstances.RemoveAt(0);
				}
			}
		}
	}
}
