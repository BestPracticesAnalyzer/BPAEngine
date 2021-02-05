using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class BPADocument : Document
	{
		public BPADocument()
			: base(null, new XmlDocument())
		{
		}

		public BPADocument(XmlDocument doc)
			: base(null, doc)
		{
		}

		public BPADocument(ExecutionInterface execInterface)
			: base(execInterface, new XmlDocument())
		{
		}

		public BPADocument(ExecutionInterface execInterface, XmlDocument doc)
			: base(execInterface, doc)
		{
		}

		public override Node[] GetNodes(string query)
		{
			XmlNodeList xmlNodeList = ((XmlDocument)doc).SelectNodes(query);
			Node[] array = new Node[xmlNodeList.Count];
			for (int i = 0; i < xmlNodeList.Count; i++)
			{
				array[i] = new BPANode(this, xmlNodeList.Item(i));
			}
			return array;
		}

		public override Node GetNode(string query)
		{
			XmlNode xmlNode = ((XmlDocument)doc).SelectSingleNode(query);
			if (xmlNode == null)
			{
				return null;
			}
			return new BPANode(this, xmlNode);
		}

		public override Node CreateNode(string name)
		{
			return new BPANode(this, name);
		}

		public override Node ImportNode(Node srcNode, bool deep)
		{
			XmlNode node = ((XmlDocument)doc).ImportNode((XmlNode)srcNode.UnderlyingNode, deep);
			return new BPANode(this, node);
		}

		public override void ClearDocument()
		{
			lock (this)
			{
				((XmlDocument)doc).RemoveAll();
				configNode = null;
			}
		}

		public override void Load()
		{
			lock (this)
			{
				ClearDocument();
				((XmlDocument)doc).Load(fileName);
				configNode = GetNode("/*/Configuration");
			}
		}

		public override void Save()
		{
			lock (this)
			{
				SaveXml((XmlDocument)doc, fileName);
			}
		}

		public override bool IsEmpty()
		{
			lock (this)
			{
				return ((XmlDocument)doc).FirstChild == null;
			}
		}

		internal override void Log(string time, string text)
		{
			lock (this)
			{
				if (runNode != null)
				{
					Node node = new BPANode(this, "Log");
					node.SetAttribute("Time", time);
					node.Value = text;
					runNode.Add(node);
				}
			}
		}

		internal void ReplaceUnderlyingDocument(XmlDocument doc)
		{
			lock (this)
			{
				base.doc = doc;
				configNode = GetNode("/*/Configuration");
				if (configNode != null && runNode != null)
				{
					runNode = configNode.GetNode("Run[last()]");
				}
			}
		}

		internal static void SaveXml(XmlDocument doc, string fileName)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(fileName, Encoding.Default);
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlTextWriter.Indentation = 1;
			xmlTextWriter.IndentChar = '\t';
			doc.Save(xmlTextWriter);
			xmlTextWriter.Close();
		}

		public override XPathNavigator CreateNavigator()
		{
			return ((XmlDocument)doc).CreateNavigator();
		}
	}
}
