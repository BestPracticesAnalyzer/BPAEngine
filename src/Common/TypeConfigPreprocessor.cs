using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class TypeConfigPreprocessor : ConfigPreprocessor
	{
		private SortedList docs;

		private Regex matchSubstitutions;

		private Regex matchRuleNames;

		protected Document currentDoc;

		public TypeConfigPreprocessor(ExecutionInterface execInterface, Document config)
			: base(execInterface, config)
		{
			docs = new SortedList();
			matchRuleNames = new Regex("\\$[0-9a-zA-Z_]+", RegexOptions.Compiled);
			matchSubstitutions = new Regex("%[^%]+[%]", RegexOptions.Compiled);
		}

		public override void ProcessConfiguration()
		{
			try
			{
				executionInterface.LogTrace("Starting Type processing.");
				Node[] nodes = config.GetNodes("//Reference[not(ancestor::Type)]");
				ProcessReferences(nodes, new SortedList());
				Node[] nodes2 = config.GetNodes("//Type");
				Node[] array = nodes2;
				foreach (Node node in array)
				{
					node.Delete();
				}
				Node[] nodes3 = config.GetNodes("//*[@SDMName or @SDMMultiplicity or @SDMElement or @SDMDataType or @CustomId]");
				Node[] array2 = nodes3;
				foreach (Node node2 in array2)
				{
					node2.DeleteAttribute("SDMName");
					node2.DeleteAttribute("SDMMultiplicity");
					node2.DeleteAttribute("SDMElement");
					node2.DeleteAttribute("SDMDataType");
					if (!executionInterface.Options.Debug)
					{
						node2.DeleteAttribute("CustomId");
					}
				}
			}
			finally
			{
				if (executionInterface.Options.Debug)
				{
					string fileName = config.FileName;
					config.FileName = executionInterface.GetDefaultOutputFileName("TYPECP.XML");
					config.Save();
					config.FileName = fileName;
				}
			}
		}

		protected virtual void MungeReferences(Node reference, Node typeNode)
		{
			MungeSubstitutions(reference, typeNode);
			MungeRuleNames(reference, typeNode);
			SetDefaultCustomIds(reference, typeNode);
		}

		protected virtual bool CopyMetadata(Document doc, Node metaNode)
		{
			bool result = true;
			switch (metaNode.Name)
			{
			case "ObjectProcessor":
			case "ExtFormat":
			case "ExtFunction":
			case "Template":
			{
				string attribute = metaNode.GetAttribute("Name");
				if (attribute.Length == 0 && metaNode.Name != "Template")
				{
					attribute = metaNode.GetAttribute("ObjectType");
				}
				Node node = config.ConfigurationNode.GetNode(string.Format("{0}[@Name='{1}']", metaNode.Name, attribute));
				if (node == null && metaNode.Name != "Template")
				{
					node = config.ConfigurationNode.GetNode(string.Format("{0}[@ObjectType='{1}']", metaNode.Name, attribute));
				}
				if (node == null)
				{
					config.ConfigurationNode.Add(config.ImportNode(metaNode, true));
					executionInterface.LogTrace("Added {0} node {1} from file {2} to main configuration.", metaNode.Name, attribute, doc.FileName);
				}
				else
				{
					executionInterface.LogTrace("Node {0} with a name of {1} in file {2} already exists in main configuration.", metaNode.Name, attribute, doc.FileName);
				}
				break;
			}
			default:
				result = false;
				break;
			}
			return result;
		}

		private void ProcessReferences(Node[] references, SortedList parentTypes)
		{
			foreach (Node node in references)
			{
				string attribute = node.GetAttribute("Type");
				if (attribute.Length == 0)
				{
					attribute = node.GetAttribute("File");
				}
				if (parentTypes.Contains(attribute))
				{
					executionInterface.LogText(CommonLoc.Error_CircularTypeReference(node.GetAttribute("Name"), attribute));
					continue;
				}
				executionInterface.LogTrace("Processing reference of type {0}.", attribute);
				Node typeNode = GetTypeNode(node);
				if (typeNode == null)
				{
					executionInterface.LogText(CommonLoc.Error_TypeNotFound(attribute));
					continue;
				}
				SetDefaultFiles(node, typeNode);
				Node[] nodes = typeNode.GetNodes(".//Reference");
				SortedList sortedList = (SortedList)parentTypes.Clone();
				sortedList.Add(attribute, true);
				ProcessReferences(nodes, sortedList);
				MungeReferences(node, typeNode);
				Node[] nodes2 = node.GetNodes(".//Customize");
				Node[] array = nodes2;
				foreach (Node customization in array)
				{
					ProcessCustomization(customization, typeNode);
				}
				Node[] nodes3 = typeNode.GetNodes("*");
				Node[] array2 = nodes3;
				foreach (Node child in array2)
				{
					node.Parent.InsertBefore(child, node);
				}
				node.Delete();
			}
		}

		private Node GetTypeNode(Node reference)
		{
			string attribute = reference.GetAttribute("Type");
			string text = reference.GetAttribute("File");
			if (text.Length > 0)
			{
				text = string.Format("{0}\\{1}", Directory.GetParent(executionInterface.Options.Configuration.FileName).ToString(), text);
			}
			currentDoc = null;
			if (text.Length == 0)
			{
				currentDoc = config;
			}
			else if (docs.Contains(text))
			{
				currentDoc = (Document)docs[text];
			}
			else
			{
				currentDoc = new BPADocument();
				currentDoc.FileName = text;
				currentDoc.Load();
				docs.Add(text, currentDoc);
				CopyMetadata(currentDoc);
			}
			string attribute2 = currentDoc.ConfigurationNode.GetAttribute("ReferencedFiles");
			if (attribute2.IndexOf(text) == -1 && currentDoc.FileName != text)
			{
				executionInterface.LogText(CommonLoc.Error_ReferencedFileNotInList(text, attribute, currentDoc.Name));
				return null;
			}
			Node node = null;
			if (attribute.Length == 0)
			{
				node = config.ImportNode(config.CreateNode("Type"), false);
				Node[] nodes = currentDoc.GetNodes("/*/*[(Reference or Object)]");
				Node[] array = nodes;
				foreach (Node srcNode in array)
				{
					node.Add(config.ImportNode(srcNode, true));
				}
			}
			else
			{
				node = currentDoc.GetNode(string.Format("/*/Type[@Name='{0}']", attribute));
				if (node != null)
				{
					node = config.ImportNode(node, true);
				}
			}
			return node;
		}

		private void CopyMetadata(Document doc)
		{
			Node[] nodes = doc.ConfigurationNode.GetNodes("*");
			Node[] array = nodes;
			foreach (Node node in array)
			{
				if (!CopyMetadata(doc, node))
				{
					executionInterface.LogTrace("Ignoring {0} type node in file {1}.", node.Name, doc.FileName);
				}
			}
		}

		private void SetDefaultFiles(Node reference, Node parent)
		{
			string attribute = reference.GetAttribute("File");
			Node[] nodes = parent.GetNodes("*");
			Node[] array = nodes;
			foreach (Node node in array)
			{
				if (node.Name == "Reference" && !node.HasAttribute("File"))
				{
					node.SetAttribute("File", attribute);
				}
				SetDefaultFiles(reference, node);
			}
		}

		private void SetDefaultCustomIds(Node reference, Node parent)
		{
			string attribute = reference.GetAttribute("Name");
			bool flag = false;
			Node[] nodes = parent.GetNodes("*");
			Node[] array = nodes;
			foreach (Node node in array)
			{
				flag = false;
				if (!node.HasAttribute("CustomId"))
				{
					if (node.Name == "Setting")
					{
						node.SetAttribute("CustomId", node.GetAttribute("Key1"));
						flag = true;
					}
					else if (node.HasAttribute("Name"))
					{
						node.SetAttribute("CustomId", node.GetAttribute("Name"));
						flag = true;
					}
				}
				if (attribute.Length > 0 && (node.Name != "Rule" || !flag))
				{
					node.SetAttribute("CustomId", string.Format("{0}_{1}", attribute, node.GetAttribute("CustomId")));
				}
				SetDefaultCustomIds(reference, node);
			}
		}

		private void ProcessCustomization(Node customization, Node typeNode)
		{
			string attribute = customization.GetAttribute("ID");
			string attribute2 = customization.GetAttribute("Type");
			Node[] nodes = customization.GetNodes("*");
			Node node = null;
			if (attribute.Length > 0)
			{
				node = typeNode.GetNode(string.Format(".//*[@CustomId='{0}']", attribute));
			}
			executionInterface.LogTrace("Processing customization {0}.", attribute);
			if (attribute.Length > 0 && node == null)
			{
				executionInterface.LogText(CommonLoc.Error_CustomIdNotFound(typeNode.GetAttribute("Name"), attribute));
				return;
			}
			if (node == null && attribute2 != "Before" && attribute2 != "After")
			{
				executionInterface.LogText(CommonLoc.Error_CustomIdNotFound(typeNode.GetAttribute("Name"), attribute));
				return;
			}
			if (nodes.Length != 1 && attribute2 == "Override")
			{
				executionInterface.LogText(CommonLoc.Error_MultipleOverrideNodes);
				return;
			}
			if (attribute2 != "Before" && attribute2 != "After" && attribute2 != "Under" && attribute2 != "Override" && attribute2 != "Replace")
			{
				executionInterface.LogText(CommonLoc.Error_CustomTypeNotFound(attribute2));
				return;
			}
			if (attribute2 == "Under")
			{
				Node[] array = nodes;
				foreach (Node srcNode in array)
				{
					Node child = config.ImportNode(srcNode, true);
					node.Add(child);
				}
				return;
			}
			if (attribute2 == "Override")
			{
				Node node2 = nodes[0];
				string[] attributes = node2.Attributes;
				foreach (string attrName in attributes)
				{
					node.SetAttribute(attrName, node2.GetAttribute(attrName));
				}
				return;
			}
			Node[] array2 = nodes;
			foreach (Node srcNode2 in array2)
			{
				Node child2 = config.ImportNode(srcNode2, true);
				Node node3 = typeNode;
				if (node != null)
				{
					node3 = node.Parent;
				}
				if ((node == null && attribute2 == "Before") || (node != null && attribute2 == "After"))
				{
					node3.InsertAfter(child2, node);
				}
				else
				{
					node3.InsertBefore(child2, node);
				}
			}
			if (attribute2 == "Replace")
			{
				node.Delete();
			}
		}

		private void MungeSubstitutions(Node reference, Node typeNode)
		{
			SortedList sortedList = new SortedList();
			Node[] nodes = reference.GetNodes("Substitution[@Name]");
			Node[] array = nodes;
			foreach (Node node in array)
			{
				sortedList.Add("%" + node.GetAttribute("Name") + "%", node.Value);
			}
			Node[] nodes2 = typeNode.GetNodes("Setting[@Substitution]");
			Node[] array2 = nodes2;
			foreach (Node node2 in array2)
			{
				string key = "%" + node2.GetAttribute("Substitution") + "%";
				if (sortedList.Contains(key))
				{
					string text = (string)sortedList[key];
					if (text.StartsWith("%") && text.EndsWith("%"))
					{
						node2.SetAttribute("Substitution", text.Substring(1, text.Length - 2));
					}
					else
					{
						node2.DeleteAttribute("Substitution");
					}
				}
			}
			MungeNames(typeNode.GetNodes(".//*"), sortedList, matchSubstitutions);
		}

		private void MungeRuleNames(Node reference, Node typeNode)
		{
			string attribute = reference.GetAttribute("Name");
			if (attribute.Length == 0)
			{
				return;
			}
			SortedList sortedList = new SortedList();
			Node[] nodes = typeNode.GetNodes(".//Rule[@Name]");
			Node[] array = nodes;
			foreach (Node node in array)
			{
				string attribute2 = node.GetAttribute("Name");
				string text = string.Format("${0}_{1}", attribute, attribute2);
				node.SetAttribute("Name", text.Substring(1));
				attribute2 = "$" + attribute2;
				if (!sortedList.Contains(attribute2))
				{
					sortedList.Add(attribute2, text);
				}
			}
			MungeNames(typeNode.GetNodes(".//Rule"), sortedList, matchRuleNames);
		}

		private void MungeNames(Node[] nodes, SortedList newNames, Regex regex)
		{
			foreach (Node node in nodes)
			{
				if (node.Name == "Substitution" && node.HasAttribute("Name"))
				{
					node.Value = MungeName(node.Value, newNames, regex);
				}
				string[] attributes = node.Attributes;
				foreach (string attrName in attributes)
				{
					node.SetAttribute(attrName, MungeName(node.GetAttribute(attrName), newNames, regex));
				}
			}
		}

		private string MungeName(string valueToChange, SortedList newNames, Regex regex)
		{
			string text = valueToChange;
			MatchCollection matchCollection = regex.Matches(valueToChange);
			foreach (Match item in matchCollection)
			{
				string text2 = (string)newNames[item.Value];
				if (text2 != null)
				{
					text = text.Replace(item.Value, text2);
				}
			}
			return text;
		}
	}
}
