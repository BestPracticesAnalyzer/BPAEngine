using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	internal class Common
	{
		private static ExecutionInterface execInterface;

		public static ExecutionInterface ExecutionInterface
		{
			get
			{
				return execInterface;
			}
			set
			{
				execInterface = value;
			}
		}

		private Common()
		{
		}

		public static void AddValueElements(Node setting, IList vals, ObjectParentData opd)
		{
			if (IsOptionSet(setting.GetAttribute("Retrieve"), "CountOnly"))
			{
				Node node = setting.OwnerDocument.CreateNode("Value");
				if (vals == null)
				{
					node.Value = "0";
				}
				else
				{
					node.Value = vals.Count.ToString();
				}
				setting.Add(node);
			}
			else if (vals != null)
			{
				string attribute = setting.GetAttribute("Format");
				foreach (object val in vals)
				{
					Node node2 = setting.OwnerDocument.CreateNode("Value");
					ExtFormat.AddValueToNode(val, attribute, node2);
					setting.Add(node2);
				}
			}
			opd.CaptureSubstitution(setting);
		}

		public static bool IsOptionSet(string val, string optToCheck)
		{
			string[] array = val.Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (optToCheck.ToUpper(CultureInfo.InvariantCulture) == text.Trim().ToUpper(CultureInfo.InvariantCulture))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsValidXmlString(string s)
		{
			bool result = true;
			try
			{
				for (int i = 0; i < s.Length; i++)
				{
					ushort num = s[i];
					if ((num >= 32 && num <= 55295) || num == 10 || num == 9 || num == 13)
					{
						continue;
					}
					if (num < 32 || num == 65534 || num == ushort.MaxValue)
					{
						return false;
					}
					if (55296 <= num && num <= 56319)
					{
						num = s[++i];
						if (num < 56320 || num > 57343)
						{
							return false;
						}
					}
				}
				return result;
			}
			catch
			{
				return false;
			}
		}

		private static string AddAttributeCheck(string xpath, Node node, string attrName, ObjectParentData opd)
		{
			string text = "";
			if (!node.HasAttribute(attrName))
			{
				text = "not(@" + attrName + ")";
			}
			else
			{
				string attribute = node.GetAttribute(attrName);
				if (opd != null && opd.HasSubstitution(attribute))
				{
					return xpath;
				}
				ArrayList arrayList = new ArrayList();
				string text2 = attribute;
				while (true)
				{
					int num = text2.IndexOf("'");
					if (num == -1)
					{
						arrayList.Add("'" + text2 + "'");
						break;
					}
					int num2 = text2.IndexOf("\"");
					if (num2 == -1)
					{
						arrayList.Add("\"" + text2 + "\"");
						break;
					}
					if (num2 < num)
					{
						arrayList.Add("'" + text2.Substring(0, num) + "'");
						text2 = text2.Substring(num);
					}
					else
					{
						arrayList.Add("\"" + text2.Substring(0, num2) + "\"");
						text2 = text2.Substring(num2);
					}
				}
				text = "@" + attrName + "=";
				text = ((arrayList.Count != 1) ? (text + "concat(" + string.Join(",", (string[])arrayList.ToArray(typeof(string))) + ")") : (text + arrayList[0]));
			}
			if (!xpath.EndsWith("[]"))
			{
				text = " and " + text;
			}
			xpath = xpath.Substring(0, xpath.Length - 1) + text + "]";
			return xpath;
		}

		public static Node AddOrMatchInstance(Node parent, Node obj, SortedList matchedStrings)
		{
			Node node = null;
			string xpath = "Instance[]";
			xpath = AddAttributeCheck(xpath, obj, "Name", null);
			if (matchedStrings[xpath] == null)
			{
				try
				{
					Node[] nodes = parent.GetNodes(xpath);
					if (nodes.Length == 1)
					{
						node = nodes[0];
					}
					else if (nodes.Length > 1)
					{
						execInterface.LogText(CommonLoc.Warn_MultipleInstances(xpath));
						Node[] array = nodes;
						foreach (Node node2 in array)
						{
							node2.Delete();
						}
					}
					matchedStrings.Add(xpath, true);
				}
				catch (Exception ex)
				{
					execInterface.LogException(ex.Message, ex);
				}
			}
			if (node == null)
			{
				node = parent.OwnerDocument.CreateNode("Instance");
				string[] attributes = obj.Attributes;
				foreach (string attrName in attributes)
				{
					node.SetAttribute(attrName, obj.GetAttribute(attrName));
				}
				parent.Add(node);
			}
			return node;
		}

		public static Node AddOrMatchObject(Node parent, Node obj)
		{
			Node node = null;
			string xpath = "Object[]";
			xpath = AddAttributeCheck(xpath, obj, "Type", null);
			xpath = AddAttributeCheck(xpath, obj, "Name", null);
			xpath = AddAttributeCheck(xpath, obj, "Count", null);
			xpath = AddAttributeCheck(xpath, obj, "Key1", null);
			xpath = AddAttributeCheck(xpath, obj, "Key2", null);
			xpath = AddAttributeCheck(xpath, obj, "Key3", null);
			xpath = AddAttributeCheck(xpath, obj, "Key4", null);
			xpath = AddAttributeCheck(xpath, obj, "Key5", null);
			Node[] nodes = parent.GetNodes(xpath);
			if (nodes.Length > 0)
			{
				node = nodes[0];
			}
			if (nodes.Length > 1)
			{
				execInterface.LogText(CommonLoc.Warn_MultipleObjects(xpath));
			}
			if (node == null)
			{
				node = parent.OwnerDocument.ImportNode(obj, false);
				parent.Add(node);
			}
			return node;
		}

		public static Node FindSetting(Node instance, Node setting, ObjectParentData opd)
		{
			Node result = null;
			string xpath = "Setting[]";
			xpath = AddAttributeCheck(xpath, setting, "Substitution", opd);
			xpath = AddAttributeCheck(xpath, setting, "Format", opd);
			xpath = AddAttributeCheck(xpath, setting, "Key1", opd);
			xpath = AddAttributeCheck(xpath, setting, "Key2", opd);
			xpath = AddAttributeCheck(xpath, setting, "Key3", opd);
			xpath = AddAttributeCheck(xpath, setting, "Key4", opd);
			xpath = AddAttributeCheck(xpath, setting, "Key5", opd);
			Node[] nodes = instance.GetNodes(xpath);
			if (nodes.Length > 0)
			{
				result = nodes[0];
			}
			if (nodes.Length > 1)
			{
				execInterface.LogText(CommonLoc.Warn_MultipleSettings(xpath));
			}
			return result;
		}
	}
}
