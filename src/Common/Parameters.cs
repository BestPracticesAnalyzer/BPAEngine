using System.Collections;
using System.Xml;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class Parameters
	{
		private ExecutionInterface execInterface;

		private Hashtable parameterHash;

		public Hashtable Hash
		{
			get
			{
				return parameterHash;
			}
		}

		public Parameters(ExecutionInterface execInterface)
		{
			this.execInterface = execInterface;
			parameterHash = new Hashtable();
		}

		public void ProcessParameters()
		{
			ObjectParentData opd = execInterface.Options.RootOPD();
			parameterHash = new Hashtable();
			Document configuration = execInterface.Options.Configuration;
			Document data = execInterface.Options.Data;
			Node[] nodes = data.GetNodes("//Parameter");
			foreach (Node node in nodes)
			{
				node.Delete();
			}
			ProcessParameters(configuration.GetNode("/*"), data.GetNode("/*"), opd);
		}

		private void ProcessParameters(Node cfgParent, Node dataParent, ObjectParentData opd)
		{
			if (execInterface.Aborting || cfgParent == null || dataParent == null)
			{
				return;
			}
			Node[] nodes = cfgParent.GetNodes("Object");
			Node[] array = nodes;
			foreach (Node node in array)
			{
				Node[] nodes2 = node.GetNodes("Setting");
				Node node2 = Common.AddOrMatchObject(dataParent, node);
				Node[] nodes3 = node2.GetNodes("Instance");
				Node[] array2 = nodes3;
				foreach (Node node3 in array2)
				{
					ObjectParentData objectParentData = opd.Clone();
					Node[] array3 = nodes2;
					foreach (Node node4 in array3)
					{
						Node node5 = Common.FindSetting(node3, node4, objectParentData);
						if (node5 != null)
						{
							objectParentData.CaptureSubstitution(node5);
						}
						Node[] nodes4 = node4.GetNodes("Parameter");
						Node[] array4 = nodes4;
						foreach (Node parameter in array4)
						{
							ProcessParameter(parameter, node5, objectParentData);
						}
					}
					ProcessParameters(node, node3, objectParentData);
				}
			}
		}

		private void ProcessParameter(Node parameter, Node setting, ObjectParentData opd)
		{
			string attribute = parameter.GetAttribute("Name");
			string attribute2 = parameter.GetAttribute("Type");
			if (!parameterHash.Contains(attribute))
			{
				switch (attribute2)
				{
				case "ArrayList":
					parameterHash[attribute] = new ArrayList();
					break;
				case "Hashtable":
					parameterHash[attribute] = new Hashtable();
					break;
				default:
					parameterHash[attribute] = null;
					break;
				}
			}
			if (setting == null)
			{
				return;
			}
			Node node = setting.OwnerDocument.ImportNode(parameter, false);
			Node node2 = opd.ApplySubstitutions(node);
			if (node2 == null)
			{
				return;
			}
			setting.Add(node2);
			switch (attribute2)
			{
			case "String":
			case "":
			{
				Node[] nodes = setting.GetNodes("Value");
				string text = string.Empty;
				Node[] array = nodes;
				foreach (Node node3 in array)
				{
					text += node3.Value;
				}
				parameterHash[attribute] = text;
				break;
			}
			case "ArrayList":
			{
				ArrayList arrayList = (ArrayList)parameterHash[attribute];
				Node[] nodes2 = setting.GetNodes("Value");
				Node[] array2 = nodes2;
				foreach (Node node4 in array2)
				{
					arrayList.Add(node4.Value);
				}
				break;
			}
			case "Hashtable":
			{
				Hashtable hashtable = (Hashtable)parameterHash[attribute];
				string text2 = parameter.GetAttribute("Key");
				if (text2.Length == 0)
				{
					text2 = "../Value";
				}
				string text3 = parameter.GetAttribute("Value");
				if (text3.Length == 0)
				{
					text3 = "../../@Name";
				}
				text2 = "string(" + text2 + ")";
				text3 = "string(" + text3 + ")";
				string text4 = ExtFunction.Evaluate((XmlNode)node2.UnderlyingNode, text2).ToString();
				string text = ExtFunction.Evaluate((XmlNode)node2.UnderlyingNode, text3).ToString();
				if (execInterface.Trace && hashtable.Contains(text4))
				{
					execInterface.LogText("Warning: key {0} already present for parameter {1}", text4, attribute);
				}
				hashtable[text4] = text;
				break;
			}
			default:
				if (execInterface.Trace)
				{
					execInterface.LogText("Unknown type {0} for parameter {1}", attribute2, attribute);
				}
				break;
			}
		}
	}
}
