using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class DataInfo
	{
		public delegate void StatusDelegate(int pctDone);

		private ExecutionInterface execInterface;

		private string label = "";

		private string fileName = "";

		private string restriction = "";

		private DateTime runTime = DateTime.Now;

		private bool completed;

		private int trackedCount;

		private long fileSize;

		private bool analyzed;

		private Version configVersion;

		protected ReportData reportData;

		private object extendedData;

		private bool valid = true;

		public string Label
		{
			get
			{
				return label;
			}
			set
			{
				label = value;
			}
		}

		public string FileName
		{
			get
			{
				return fileName;
			}
		}

		public DateTime RunTime
		{
			get
			{
				return runTime;
			}
		}

		public Version ConfigVersion
		{
			get
			{
				return configVersion;
			}
			set
			{
				configVersion = value;
			}
		}

		public int TrackedCount
		{
			get
			{
				return trackedCount;
			}
		}

		public long FileSize
		{
			get
			{
				return fileSize;
			}
		}

		public bool Analyzed
		{
			get
			{
				return analyzed;
			}
			set
			{
				analyzed = value;
			}
		}

		public bool Completed
		{
			get
			{
				return completed;
			}
			set
			{
				completed = value;
			}
		}

		public ReportData Data
		{
			get
			{
				return reportData;
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

		public bool Valid
		{
			get
			{
				return valid;
			}
		}

		public string Restriction
		{
			get
			{
				return restriction;
			}
		}

		public DataInfo(ExecutionInterface execInterface, string fileName)
		{
			this.execInterface = execInterface;
			this.fileName = fileName;
			ReloadInfo();
		}

		public virtual void ReloadInfo()
		{
			valid = false;
			fileSize = new FileInfo(fileName).Length;
			XmlTextReader xmlTextReader = null;
			try
			{
				xmlTextReader = new XmlTextReader(fileName);
				xmlTextReader.WhitespaceHandling = WhitespaceHandling.None;
				bool flag = false;
				while (xmlTextReader.Read() && !flag)
				{
					XmlNodeType nodeType = xmlTextReader.NodeType;
					if (nodeType == XmlNodeType.Element && xmlTextReader.Name == "Configuration")
					{
						string attributeNonNull = CommonData.GetAttributeNonNull(xmlTextReader, "ConfigVersion");
						if (attributeNonNull.Length == 0)
						{
							configVersion = new Version("0.0.0.0");
						}
						else
						{
							configVersion = new Version(attributeNonNull);
							valid = true;
						}
						restriction = CommonData.GetAttributeNonNull(xmlTextReader, "Restriction");
						string attributeNonNull2 = CommonData.GetAttributeNonNull(xmlTextReader, "StartTime");
						if (attributeNonNull2.Length > 0)
						{
							runTime = DateTime.Parse(attributeNonNull2, CultureInfo.InvariantCulture);
						}
						else
						{
							runTime = File.GetCreationTime(fileName);
						}
						string attributeNonNull3 = CommonData.GetAttributeNonNull(xmlTextReader, "ObjectsTracked");
						if (attributeNonNull3.Length > 0)
						{
							trackedCount = int.Parse(attributeNonNull3) - 1;
						}
						if (trackedCount < 0)
						{
							trackedCount = 0;
						}
						label = CommonData.GetAttributeNonNull(xmlTextReader, "Label");
						completed = CommonData.GetAttributeNonNull(xmlTextReader, "Completed") == "OK";
						string attributeNonNull4 = CommonData.GetAttributeNonNull(xmlTextReader, "Operations");
						analyzed = attributeNonNull4.IndexOf("analyze") != -1;
						flag = true;
					}
				}
			}
			catch
			{
				valid = false;
			}
			finally
			{
				if (xmlTextReader != null)
				{
					xmlTextReader.Close();
				}
			}
		}

		public virtual void GenerateDataLists(ArrayList customReports, int startingPct, MessageSuppression msgSuppress, StatusDelegate Status)
		{
			reportData = new ReportData();
			GenerateReportData(customReports, startingPct, msgSuppress, Status, true);
		}

		public void ClearDataLists()
		{
			reportData = new ReportData();
		}

		public override string ToString()
		{
			return string.Format("{0} {1} {2} {3} {4}", label, runTime.ToString(), fileName, fileSize.ToString(), trackedCount.ToString());
		}

		protected void GenerateReportData(ArrayList customReports, int startingPct, MessageSuppression msgSuppress, StatusDelegate Status, bool isBPADisplay)
		{
			XmlTextReader xmlTextReader = null;
			int num = startingPct;
			try
			{
				xmlTextReader = new XmlTextReader(fileName);
				xmlTextReader.WhitespaceHandling = WhitespaceHandling.None;
				NodeInfo nodeInfo = reportData.DetailNodes;
				NodeInfo nodeInfo2 = reportData.SummaryNodes;
				GroupingClass groupingClass = new GroupingClass(null, "", "", 0);
				bool flag = false;
				bool flag2 = false;
				string text = "";
				string text2 = "";
				string ruleText = "";
				IssueInfo issueInfo = null;
				long num2 = 0L;
				int num3 = 100 - startingPct;
				bool flag3 = true;
				while (flag3)
				{
					num2++;
					num += (int)(num2 * 40 * num3 / fileSize);
					if (num2 % 500 == 0)
					{
						Status(num);
					}
					switch (xmlTextReader.NodeType)
					{
					case XmlNodeType.Element:
						if (nodeInfo.Element == NodeInfo.ElementType.Value)
						{
							nodeInfo.Text += xmlTextReader.ReadOuterXml();
							continue;
						}
						nodeInfo = DetailViewHandleElement(xmlTextReader, nodeInfo, true, isBPADisplay);
						nodeInfo2 = DetailViewHandleElement(xmlTextReader, nodeInfo2, false, isBPADisplay);
						if (xmlTextReader.Name == "Instance")
						{
							if (CommonData.GetAttributeNonNull(xmlTextReader, "GroupingClass") == "True" && !xmlTextReader.IsEmptyElement)
							{
								string attributeNonNull = CommonData.GetAttributeNonNull(xmlTextReader, "Class");
								string attributeNonNull2 = CommonData.GetAttributeNonNull(xmlTextReader, "Name");
								if (attributeNonNull.ToUpper(CultureInfo.InvariantCulture).Trim() != groupingClass.Type.ToUpper(CultureInfo.InvariantCulture) || attributeNonNull2.ToUpper(CultureInfo.InvariantCulture).Trim() != groupingClass.Name.ToUpper(CultureInfo.InvariantCulture))
								{
									groupingClass = new GroupingClass(groupingClass, attributeNonNull, attributeNonNull2, xmlTextReader.Depth);
								}
							}
						}
						else if (xmlTextReader.Name == "Rule")
						{
							CommonData.GetAttributeNonNull(xmlTextReader, "Text");
							try
							{
								ruleText = string.Format(CommonData.GetAttributeNonNull(xmlTextReader, "Text"), "...", "...", "...", "...", "...", "...", "...", "...", "...", "...");
							}
							catch
							{
								ruleText = CommonData.GetAttributeNonNull(xmlTextReader, "Text");
							}
						}
						else if (xmlTextReader.Name == "Message" && CommonData.GetAttributeNonNull(xmlTextReader, "Display") != "Hide")
						{
							if (issueInfo == null)
							{
								issueInfo = new IssueInfo();
							}
							text = CommonData.GetAttributeNonNull(xmlTextReader, "NextStep");
							if (execInterface.Trace)
							{
								text += CommonData.GetAttributeNonNull(xmlTextReader, "ChildStepNodeID");
							}
							string attributeNonNull3 = CommonData.GetAttributeNonNull(xmlTextReader, "Error");
							issueInfo.Severity = IssueInfo.ToSeverity(attributeNonNull3);
							issueInfo.Suppressed = false;
							issueInfo.Groups.Add(groupingClass);
							while (((GroupingClass)issueInfo.Groups[0]).Parent != null)
							{
								issueInfo.Groups.Insert(0, ((GroupingClass)issueInfo.Groups[0]).Parent);
							}
							issueInfo.Title = CommonData.GetAttributeNonNull(xmlTextReader, "Title");
							issueInfo.RuleText = ruleText;
							issueInfo.MsgId = CommonData.GetAttributeNonNull(xmlTextReader, "Name");
							string attributeNonNull4 = CommonData.GetAttributeNonNull(xmlTextReader, "Sev");
							if (attributeNonNull4.Length > 0)
							{
								issueInfo.SevNum = int.Parse(attributeNonNull4);
							}
							issueInfo.ArticleGuid = CommonData.GetAttributeNonNull(xmlTextReader, "GUID");
							if (xmlTextReader.IsEmptyElement)
							{
								reportData.IssueList.Add(issueInfo);
								issueInfo = null;
							}
						}
						else if (xmlTextReader.Name == "Log")
						{
							flag = true;
							text2 = CommonData.GetAttributeNonNull(xmlTextReader, "Time") + ": ";
						}
						else if (xmlTextReader.Name == "Variable")
						{
							if (!flag2)
							{
								nodeInfo.IsDummy = true;
								break;
							}
							string attributeNonNull5 = CommonData.GetAttributeNonNull(xmlTextReader, "Name");
							string attributeNonNull6 = CommonData.GetAttributeNonNull(xmlTextReader, "Value");
							if (nodeInfo.Text.Length == 0)
							{
								nodeInfo.Text = string.Format("Substitution variables: {0} = {1}", attributeNonNull5, attributeNonNull6);
							}
							else
							{
								nodeInfo.Text = string.Format("{0}, {1} = {2}", nodeInfo.Text, attributeNonNull5, attributeNonNull6);
							}
						}
						else if (xmlTextReader.Name == "Substitution" && !xmlTextReader.IsEmptyElement)
						{
							flag2 = true;
						}
						break;
					case XmlNodeType.Text:
						DetailViewHandleText(xmlTextReader, nodeInfo);
						DetailViewHandleText(xmlTextReader, nodeInfo2);
						if (issueInfo != null)
						{
							issueInfo.Description = xmlTextReader.Value;
							if (issueInfo.Title.Length == 0)
							{
								issueInfo.Title = issueInfo.Description;
							}
							if (execInterface.Trace && text != "")
							{
								nodeInfo.Text = string.Format("Next Step: {0} ({1})", nodeInfo.Text, text);
								nodeInfo2.Text = string.Format("Next Step: {0} ({1})", nodeInfo2.Text, text);
							}
						}
						else if (flag)
						{
							text2 += xmlTextReader.Value;
						}
						break;
					case XmlNodeType.EndElement:
						nodeInfo = DetailViewHandleEndElement(xmlTextReader, nodeInfo, true, isBPADisplay);
						nodeInfo2 = DetailViewHandleEndElement(xmlTextReader, nodeInfo2, false, isBPADisplay);
						if (xmlTextReader.Name == "Instance")
						{
							if (groupingClass.Depth == xmlTextReader.Depth)
							{
								GroupingClass groupingClass2 = groupingClass;
								int num4 = 0;
								while (groupingClass2.Parent != null)
								{
									num4++;
									groupingClass2 = groupingClass2.Parent;
								}
								groupingClass.Depth = num4;
								groupingClass = groupingClass.Parent;
							}
						}
						else if (xmlTextReader.Name == "Message")
						{
							if (issueInfo != null)
							{
								issueInfo.Suppressed = msgSuppress.IsSuppressed(issueInfo.MsgId);
								if (!issueInfo.Suppressed)
								{
									issueInfo.Suppressed = msgSuppress.IsSuppressed(issueInfo.MsgIdInstance);
								}
								reportData.IssueList.Add(issueInfo);
								issueInfo = null;
							}
						}
						else if (xmlTextReader.Name == "Log")
						{
							flag = false;
							reportData.LogList.Add(text2);
							text2 = "";
						}
						else if (xmlTextReader.Name == "Substitution")
						{
							flag2 = false;
						}
						break;
					}
					flag3 = xmlTextReader.Read();
				}
			}
			finally
			{
				if (xmlTextReader != null)
				{
					xmlTextReader.Close();
				}
			}
		}

		private NodeInfo DetailViewHandleElement(XmlTextReader dataFile, NodeInfo currentNode, bool detailed, bool isBPADisplay)
		{
			if (dataFile.IsEmptyElement)
			{
				return currentNode;
			}
			NodeInfo nodeInfo = currentNode.Add(string.Empty, dataFile.Name);
			if (nodeInfo == null)
			{
				return currentNode;
			}
			nodeInfo.IsHideAll = currentNode.IsHideAll || CommonData.GetAttributeNonNull(dataFile, "Display") == "HideAll";
			switch (nodeInfo.Element)
			{
			case NodeInfo.ElementType.Substitution:
				nodeInfo.IsDummy = true;
				break;
			case NodeInfo.ElementType.Variable:
			{
				nodeInfo.IsDummy = detailed;
				string attributeNonNull3 = CommonData.GetAttributeNonNull(dataFile, "Name");
				string attributeNonNull9 = CommonData.GetAttributeNonNull(dataFile, "Value");
				if (attributeNonNull3.Length > 0)
				{
					nodeInfo.Text = "Substitution: " + attributeNonNull3 + " = " + attributeNonNull9;
					nodeInfo.IsDummy = false;
				}
				break;
			}
			case NodeInfo.ElementType.Object:
				nodeInfo.IsDummy = true;
				if (!detailed)
				{
					string attributeNonNull3 = CommonData.GetAttributeNonNull(dataFile, "Class");
					if (attributeNonNull3.Length > 0)
					{
						nodeInfo.Text = attributeNonNull3;
						nodeInfo.IsDummy = false;
					}
				}
				break;
			case NodeInfo.ElementType.Instance:
			{
				string attributeNonNull3 = CommonData.GetAttributeNonNull(dataFile, "Name");
				string attributeNonNull = CommonData.GetAttributeNonNull(dataFile, "Display");
				string attributeNonNull10 = CommonData.GetAttributeNonNull(dataFile, "Type");
				string attributeNonNull11 = CommonData.GetAttributeNonNull(dataFile, "Scope");
				if ((detailed && attributeNonNull == "Hide") || (!detailed && CommonData.GetAttributeNonNull(dataFile, "Class").Length == 0) || attributeNonNull3.Length == 0)
				{
					nodeInfo.IsDummy = true;
					break;
				}
				nodeInfo.Text = attributeNonNull3;
				if (attributeNonNull == "Right" || (attributeNonNull10 != "Group" && attributeNonNull11.Length == 0 && attributeNonNull != "Left"))
				{
					nodeInfo.DisplayRight = true;
				}
				break;
			}
			case NodeInfo.ElementType.Setting:
			{
				if (!detailed)
				{
					nodeInfo.IsDummy = true;
					break;
				}
				string attributeNonNull = CommonData.GetAttributeNonNull(dataFile, "Display");
				string attributeNonNull4 = CommonData.GetAttributeNonNull(dataFile, "Key1");
				string attributeNonNull5 = CommonData.GetAttributeNonNull(dataFile, "Key2");
				string attributeNonNull6 = CommonData.GetAttributeNonNull(dataFile, "Key3");
				string attributeNonNull7 = CommonData.GetAttributeNonNull(dataFile, "Key4");
				string attributeNonNull8 = CommonData.GetAttributeNonNull(dataFile, "Key5");
				if (attributeNonNull != "Hide" && (attributeNonNull4.Length > 0 || isBPADisplay))
				{
					nodeInfo.Text = string.Format("{0} {1} {2} {3} {4}", attributeNonNull4.Trim(), attributeNonNull5.Trim(), attributeNonNull6.Trim(), attributeNonNull7.Trim(), attributeNonNull8.Trim());
					nodeInfo.DisplayName = CommonData.GetAttributeNonNull(dataFile, "DisplayName");
					nodeInfo.Format = CommonData.GetAttributeNonNull(dataFile, "Format");
					nodeInfo.DisplayRight = true;
				}
				else
				{
					nodeInfo.IsDummy = true;
				}
				break;
			}
			case NodeInfo.ElementType.Value:
				if (currentNode.Element != NodeInfo.ElementType.Setting || !detailed)
				{
					nodeInfo.IsDummy = true;
					break;
				}
				nodeInfo.DisplayRight = true;
				nodeInfo.Format = currentNode.Format;
				nodeInfo.IsDummy = currentNode.IsDummy;
				currentNode.ValueCount++;
				break;
			case NodeInfo.ElementType.Rule:
				nodeInfo.IsDummy = true;
				break;
			case NodeInfo.ElementType.Message:
			{
				string attributeNonNull = CommonData.GetAttributeNonNull(dataFile, "Display");
				if (attributeNonNull == "Hide")
				{
					nodeInfo.IsDummy = true;
				}
				nodeInfo.DisplayRight = true;
				string attributeNonNull2 = CommonData.GetAttributeNonNull(dataFile, "Error");
				nodeInfo.Severity = IssueInfo.ToSeverity(attributeNonNull2);
				nodeInfo.ArticleGuid = CommonData.GetAttributeNonNull(dataFile, "GUID");
				break;
			}
			}
			if (nodeInfo.IsHideAll)
			{
				nodeInfo.IsDummy = true;
			}
			return nodeInfo;
		}

		private NodeInfo DetailViewHandleEndElement(XmlTextReader dataFile, NodeInfo currentNode, bool detailed, bool isBPADisplay)
		{
			NodeInfo parent = currentNode.Parent;
			switch (NodeInfo.ParseType(dataFile.Name))
			{
			case NodeInfo.ElementType.Instance:
				if (currentNode.DisplayRight)
				{
					currentNode.PromoteLeftChildren();
				}
				break;
			case NodeInfo.ElementType.Setting:
				if (currentNode.ValueCount == 0)
				{
					if (isBPADisplay)
					{
						currentNode.Text = string.Format("{0}={1}", currentNode.Text, CommonLoc.NoValue);
						if (currentNode.DisplayNameExists)
						{
							currentNode.DisplayName = string.Format("{0}={1}", currentNode.DisplayName, CommonLoc.NoValue);
						}
					}
					else
					{
						currentNode.IsDummy = true;
					}
				}
				else if (currentNode.ValueCount == 1 && currentNode.Children.Count > 0)
				{
					currentNode.Text = string.Format("{0}={1}", currentNode.Text, ((NodeInfo)currentNode.Children[0]).Text);
					if (currentNode.DisplayNameExists)
					{
						currentNode.DisplayName = string.Format("{0}={1}", currentNode.DisplayName, ((NodeInfo)currentNode.Children[0]).Text);
					}
					((NodeInfo)currentNode.Children[0]).Remove();
				}
				break;
			default:
				return currentNode;
			case NodeInfo.ElementType.Variable:
			case NodeInfo.ElementType.Substitution:
			case NodeInfo.ElementType.Object:
			case NodeInfo.ElementType.Value:
			case NodeInfo.ElementType.Rule:
			case NodeInfo.ElementType.Message:
				break;
			}
			if (currentNode.IsDummy)
			{
				currentNode.Remove();
			}
			else if (currentNode.Severity == IssueSeverity.Unknown && currentNode.Children != null)
			{
				foreach (NodeInfo child in currentNode.Children)
				{
					if ((short)child.Severity > (short)currentNode.Severity)
					{
						currentNode.Severity = child.Severity;
					}
				}
				return parent;
			}
			return parent;
		}

		private void DetailViewHandleText(XmlTextReader dataFile, NodeInfo node)
		{
			string text = dataFile.Value;
			if (text == null)
			{
				text = string.Empty;
			}
			if (!node.IsDummy)
			{
				if (node.Element == NodeInfo.ElementType.Value)
				{
					ExtFormat.DisplayValue(node, text, node.Format);
				}
				else if (node.Element == NodeInfo.ElementType.Message)
				{
					node.Text = text;
				}
			}
		}
	}
}
