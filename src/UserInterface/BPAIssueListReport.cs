using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPAIssueListReport : BPAListReport
	{
		private class IssueInfoExtendedData
		{
			private Point location = new Point(0, 0);

			public Point Location
			{
				get
				{
					return location;
				}
				set
				{
					location = value;
				}
			}
		}

		private bool showDisabledIssues;

		private int severityMask;

		private int lastIssuePrinted = -1;

		private IssueInfo expandedEntry;

		private bool inMoveLogic;

		private BPALink moreLink;

		private BPALink disableAllLink;

		private BPALink disableInstanceLink;

		private BPAPanel issueText;

		public BPAIssueListReport(MainGUI mainGUI, BPAReportType reportType, string name, bool allowMultipleSortOrders, bool showDisabledIssues, int severityMask)
			: base(mainGUI, reportType, name, true, allowMultipleSortOrders)
		{
			this.showDisabledIssues = showDisabledIssues;
			this.severityMask = severityMask;
		}

		public override void EnableItem(object itemInfo, bool enable, bool all)
		{
			IssueInfo issueInfo = (IssueInfo)itemInfo;
			listBox.SelectedIndex = -1;
			listBox.SuspendLayout();
			if (all)
			{
				if (enable)
				{
					mainGUI.RegSettings.MsgSuppress.Unsuppress(issueInfo.MsgId);
				}
				else
				{
					mainGUI.RegSettings.MsgSuppress.Suppress(issueInfo.MsgId);
				}
				for (int i = 0; i < itemList.Count; i++)
				{
					IssueInfo issueInfo2 = (IssueInfo)itemList[i];
					if (issueInfo2.MsgId == issueInfo.MsgId && issueInfo2.Suppressed != !enable)
					{
						issueInfo2.Suppressed = !enable;
						RemoveIssue(issueInfo2, i);
						if (issueInfo2.GroupingName.Length > 0)
						{
							mainGUI.RegSettings.MsgSuppress.Unsuppress(issueInfo2.MsgIdInstance);
						}
					}
				}
			}
			else
			{
				if (enable)
				{
					mainGUI.RegSettings.MsgSuppress.Unsuppress(issueInfo.MsgIdInstance);
				}
				else
				{
					mainGUI.RegSettings.MsgSuppress.Suppress(issueInfo.MsgIdInstance);
				}
				issueInfo.Suppressed = !enable;
				RemoveIssue(issueInfo, itemList.IndexOf(issueInfo));
			}
			ResetLinks();
			if (listBox.SelectedIndex >= listBox.Items.Count)
			{
				listBox.SelectedIndex = listBox.Items.Count - 1;
			}
			totalItems.Text = BPALoc.Label_VSTotalItems(itemListInSelectedOrder.Count);
			listBox.ResumeLayout();
			mainGUI.RegSettings.SaveSuppressionData();
			listBox.Focus();
		}

		public override void ShowMoreInfo(object itemInfo)
		{
			ShowArticle(((IssueInfo)itemInfo).ArticleGuid);
		}

		private void DrawLastLine(DrawItemEventArgs e, Rectangle bounds)
		{
			if (e.Index + 1 >= listBox.Items.Count || !(listBox.Items[e.Index + 1] is HeaderInfo))
			{
				Rectangle rect = new Rectangle(bounds.X + 2, bounds.Y + bounds.Height - 1, bounds.Width - 2, 1);
				e.Graphics.FillRectangle(new LinearGradientBrush(rect, MainGUI.DarkGray, MainGUI.LightGray, 0f, false), rect);
			}
		}

		private int GetIndent(ListBox listBox, int index)
		{
			int num = index - 1;
			while (num >= 0 && !(listBox.Items[num] is HeaderInfo))
			{
				num--;
			}
			if (num != -1)
			{
				return 20 * ((HeaderInfo)listBox.Items[num]).Depth;
			}
			return 0;
		}

		private string GetIssueKey(IssueInfo issueInfo, int unique)
		{
			string text = ((int)(100 - issueInfo.Severity)).ToString("0000") + issueInfo.SevNum.ToString("0000") + issueInfo.Title + unique;
			if (allowMultipleSortOrders && arrangeByEntries.SelectedItem.ToString() == BPALoc.ComboBox_VSArrangeIssue)
			{
				text = ((issueInfo.Title == issueInfo.Description) ? issueInfo.RuleText : issueInfo.Title) + text;
			}
			else if (!allowMultipleSortOrders || arrangeByEntries.SelectedItem.ToString() != BPALoc.ComboBox_VSArrangeSeverity)
			{
				ArrayList arrayList = issueInfo.Groups;
				if (allowMultipleSortOrders && arrangeByEntries.SelectedItem.ToString() != BPALoc.ComboBox_VSArrangeClass)
				{
					arrayList = ArrangeGroupsByClass(arrangeByEntries.SelectedItem.ToString(), issueInfo.Groups);
				}
				string str = "";
				foreach (GroupingClass item in arrayList)
				{
					str = str + item.Type.PadRight(100, ' ') + item.Name.PadRight(100, ' ');
				}
				text = str + text;
			}
			return text;
		}

		private ArrayList ArrangeGroupsByClass(string topClassName, ArrayList groups)
		{
			ArrayList arrayList = new ArrayList();
			foreach (GroupingClass group in groups)
			{
				if (group.Type.Length != 0)
				{
					if (group.Type == topClassName)
					{
						arrayList.Insert(0, new GroupingClass(group.Parent, group.Type, group.Name, 0));
					}
					else
					{
						arrayList.Add(group);
					}
				}
			}
			if (arrayList.Count == 0 || ((GroupingClass)arrayList[0]).Type != topClassName)
			{
				arrayList.Insert(0, new GroupingClass(null, BPALoc.Label_VSUnclassifiedHeader, "", 0));
			}
			return arrayList;
		}

		protected override void PopulateArrangeByEntries()
		{
			arrangeByEntries.Items.Clear();
			arrangeByEntries.Items.Add(BPALoc.ComboBox_VSArrangeClass);
			arrangeByEntries.Items.Add(BPALoc.ComboBox_VSArrangeSeverity);
			arrangeByEntries.Items.Add(BPALoc.ComboBox_VSArrangeIssue);
			arrangeByEntries.SelectedItem = BPALoc.ComboBox_VSArrangeClass;
			arrangeByEntries.Text = BPALoc.ComboBox_VSArrangeClass;
		}

		private int PrintHeader(PrintPageEventArgs e)
		{
			int y = e.MarginBounds.Y;
			Size size = e.Graphics.MeasureString(mname, MainGUI.TitleFont, e.MarginBounds.Width - 10).ToSize();
			Rectangle rectangle = new Rectangle(e.MarginBounds.X, y, e.MarginBounds.Width, size.Height + 20);
			e.Graphics.FillRectangle(new SolidBrush(MainGUI.DarkGray), rectangle);
			rectangle.X += 10;
			rectangle.Y += 10;
			rectangle.Width -= 10;
			rectangle.Height -= 10;
			e.Graphics.DrawString(mname, MainGUI.TitleFont, new SolidBrush(Color.White), rectangle);
			string text = BPALoc.Label_Page(mprintPage);
			Size size2 = e.Graphics.MeasureString(text, MainGUI.DefaultFont, 0).ToSize();
			rectangle.X += e.MarginBounds.Width - size2.Width - 20;
			rectangle.Size = size2;
			rectangle.Width += 20;
			e.Graphics.DrawString(text, MainGUI.DefaultFont, new SolidBrush(Color.White), rectangle);
			y += size.Height + 20 + 4;
			if (dataInfo != null)
			{
				rectangle = new Rectangle(e.MarginBounds.X, y, e.MarginBounds.Width, MainGUI.TitleFont.Height);
				e.Graphics.DrawString((dataInfo.Label.Length == 0) ? dataInfo.RunTime.ToString() : dataInfo.Label, MainGUI.TitleFont, new SolidBrush(Color.Black), rectangle);
				y += rectangle.Height + 8;
			}
			rectangle = new Rectangle(e.MarginBounds.X, y, e.MarginBounds.Width, MainGUI.DefaultFont.Height);
			e.Graphics.DrawString(BPALoc.Label_VSTotalItems(itemListInSelectedOrder.Count), MainGUI.DefaultFont, new SolidBrush(Color.Black), rectangle);
			return y + (rectangle.Height + 8);
		}

		private int PrintNextIssue(int nextTop, PrintPageEventArgs e)
		{
			if (listBox.Items[lastIssuePrinted + 1] is HeaderInfo)
			{
				e.Graphics.DrawString(listBox.Items[lastIssuePrinted + 1].ToString(), new Font(MainGUI.DefaultFont.FontFamily, MainGUI.DefaultFont.Size, FontStyle.Bold), new SolidBrush(MainGUI.DarkGray), e.MarginBounds.X, nextTop + MainGUI.DefaultFont.Height);
				nextTop += MainGUI.DefaultFont.Height * 2 + 4;
			}
			else
			{
				IssueInfo issueInfo = (IssueInfo)listBox.Items[lastIssuePrinted + 1];
				Size size = e.Graphics.MeasureString(issueInfo.Description, MainGUI.DefaultFont, e.MarginBounds.Width - CommonData.ErrorIcon.Width - 8).ToSize();
				if (nextTop + size.Height + MainGUI.DefaultFont.Height + 10 > e.MarginBounds.Bottom)
				{
					return e.MarginBounds.Bottom;
				}
				Rectangle rectangle = new Rectangle(e.MarginBounds.X, nextTop, e.MarginBounds.Width, 1);
				e.Graphics.FillRectangle(new LinearGradientBrush(rectangle, MainGUI.DarkGray, MainGUI.LightGray, 0f, false), rectangle);
				nextTop += rectangle.Height;
				Icon icon = issueInfo.SeverityIcon();
				if (icon != null)
				{
					e.Graphics.DrawIcon(icon, e.MarginBounds.X + 4, nextTop + 4);
				}
				e.Graphics.DrawString(issueInfo.Title, MainGUI.DefaultFont, new SolidBrush(Color.Black), e.MarginBounds.X + CommonData.ErrorIcon.Width + 8, nextTop + 4);
				if (issueInfo.GroupingName.Length > 0)
				{
					e.Graphics.DrawString(issueInfo.GroupingType + ": " + issueInfo.GroupingName, MainGUI.DefaultFont, new SolidBrush(Color.Black), e.MarginBounds.X + e.MarginBounds.Width * 3 / 4, nextTop + 4);
				}
				nextTop += MainGUI.DefaultFont.Height + 8;
				rectangle = new Rectangle(e.MarginBounds.X + CommonData.ErrorIcon.Width + 8, nextTop, e.MarginBounds.Width - CommonData.ErrorPic.Width - 8, size.Height);
				e.Graphics.DrawString(issueInfo.Description, MainGUI.DefaultFont, new SolidBrush(Color.Black), rectangle);
				nextTop += size.Height;
			}
			lastIssuePrinted++;
			return nextTop;
		}

		protected override void PrintPage(PrintPageEventArgs e)
		{
			try
			{
				int num = PrintHeader(e);
				bool flag = false;
				e.HasMorePages = true;
				while (!flag)
				{
					num = PrintNextIssue(num, e);
					if (lastIssuePrinted + 1 >= listBox.Items.Count || listBox.Items[lastIssuePrinted + 1].GetType() == typeof(string))
					{
						Rectangle rect = new Rectangle(e.MarginBounds.X, num, e.MarginBounds.Width, 1);
						e.Graphics.FillRectangle(new LinearGradientBrush(rect, MainGUI.DarkGray, MainGUI.LightGray, 0f, false), rect);
						num += rect.Height;
					}
					if (lastIssuePrinted + 1 >= listBox.Items.Count)
					{
						e.HasMorePages = false;
						lastIssuePrinted = -1;
						flag = true;
					}
					if (num >= e.MarginBounds.Bottom || flag)
					{
						Rectangle rect2 = new Rectangle(e.MarginBounds.X, e.MarginBounds.Bottom - 8, e.MarginBounds.Width, 8);
						e.Graphics.FillRectangle(new SolidBrush(MainGUI.DarkGray), rect2);
						flag = true;
					}
				}
				mprintPage++;
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		private void RemoveIssue(IssueInfo issueInfo, int i)
		{
			itemListInSelectedOrder.Remove(GetIssueKey(issueInfo, i));
			int num = listBox.Items.IndexOf(issueInfo);
			listBox.Items.RemoveAt(num);
			if (num > 0 && listBox.Items[num - 1].GetType() == typeof(string) && (num == listBox.Items.Count || listBox.Items[num].GetType() == typeof(string)))
			{
				listBox.Items.RemoveAt(num - 1);
			}
		}

		private void ResetLinks()
		{
			if (issueText != null)
			{
				listBox.Controls.Remove(issueText);
				issueText.LocationChanged -= ExpandedIssueMoved;
			}
			if (moreLink != null)
			{
				listBox.Controls.Remove(moreLink);
			}
			if (disableInstanceLink != null)
			{
				listBox.Controls.Remove(disableInstanceLink);
			}
			if (disableAllLink != null)
			{
				listBox.Controls.Remove(disableAllLink);
			}
			issueText = null;
			moreLink = null;
			disableInstanceLink = null;
			disableAllLink = null;
		}

		protected override void ShowReport()
		{
			mnumberOfEntries = 0;
			if (itemList == null)
			{
				return;
			}
			Regex regex = new Regex(findText.Text, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
			itemListInSelectedOrder.Clear();
			for (int i = 0; i < itemList.Count; i++)
			{
				IssueInfo issueInfo = (IssueInfo)itemList[i];
				if (issueInfo.Suppressed == showDisabledIssues && ((uint)issueInfo.Severity & (uint)severityMask) != 0 && (!findPanel.Visible || regex.Match(issueInfo.Title).Success || regex.Match(issueInfo.Description).Success))
				{
					itemListInSelectedOrder.Add(GetIssueKey(issueInfo, i), issueInfo);
				}
			}
			expandedEntry = null;
			ResetLinks();
			listBox.ClearExpandedIndex();
			listBox.SelectedIndex = -1;
			listBox.Items.Clear();
			ArrayList arrayList = new ArrayList();
			arrayList.Add("");
			foreach (IssueInfo value in itemListInSelectedOrder.Values)
			{
				ArrayList arrayList2 = new ArrayList();
				if (allowMultipleSortOrders && arrangeByEntries.SelectedItem.ToString() == BPALoc.ComboBox_VSArrangeSeverity)
				{
					arrayList2.Add(value.SeverityHeaderString());
				}
				else if (allowMultipleSortOrders && arrangeByEntries.SelectedItem.ToString() == BPALoc.ComboBox_VSArrangeIssue)
				{
					arrayList2.Add((value.Title == value.Description) ? value.RuleText : value.Title);
				}
				else
				{
					ArrayList arrayList3 = value.Groups;
					if (allowMultipleSortOrders && arrangeByEntries.SelectedItem.ToString() != BPALoc.ComboBox_VSArrangeClass)
					{
						arrayList3 = ArrangeGroupsByClass(arrangeByEntries.SelectedItem.ToString(), value.Groups);
					}
					foreach (GroupingClass item in arrayList3)
					{
						arrayList2.Add(item.Type + ": " + item.Name);
					}
				}
				bool flag = false;
				for (int j = 0; j < arrayList2.Count; j++)
				{
					if (((arrayList.Count <= j || arrayList2[j].ToString() != arrayList[j].ToString()) && arrayList2[j].ToString().Length > 2) || flag)
					{
						flag = true;
						listBox.Items.Add(new HeaderInfo(listBox, (string)arrayList2[j], j));
					}
				}
				arrayList = arrayList2;
				listBox.Items.Add(value);
			}
			mnumberOfEntries = itemListInSelectedOrder.Count;
			totalItems.Text = BPALoc.Label_VSTotalItems(itemListInSelectedOrder.Count);
		}

		protected override void SaveAsCSV(string fileName)
		{
			StreamWriter streamWriter = null;
			try
			{
				streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
				streamWriter.WriteLine("\"Header\",\"Severity\",\"Title\",\"Group Type\",\"Group Name\",\"Description\",\"More Info Link\"");
				string text = "";
				for (int i = 0; i < listBox.Items.Count; i++)
				{
					HeaderInfo headerInfo = listBox.Items[i] as HeaderInfo;
					if (headerInfo != null)
					{
						text = headerInfo.Header;
						continue;
					}
					IssueInfo issueInfo = (IssueInfo)listBox.Items[i];
					streamWriter.WriteLine("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"", text, issueInfo.SeverityString(), issueInfo.Title, issueInfo.GroupingType, issueInfo.GroupingName, issueInfo.Description, string.Format(mainGUI.ConfigInfo.ArticleURL, mainGUI.ExecInterface.Culture, mainGUI.ExecInterface.EngineVersion.Major, issueInfo.ArticleGuid));
				}
			}
			catch (Exception arg)
			{
				mainGUI.TraceWrite(BPALoc.Label_VSExportError(arg));
			}
			finally
			{
				if (streamWriter != null)
				{
					streamWriter.Close();
				}
			}
		}

		protected override void SaveAsHTML(string fileName)
		{
			XmlTextWriter xmlTextWriter = null;
			try
			{
				xmlTextWriter = new XmlTextWriter(fileName, Encoding.UTF8);
				xmlTextWriter.Formatting = Formatting.Indented;
				xmlTextWriter.WriteStartElement("html");
				xmlTextWriter.WriteStartElement("head");
				xmlTextWriter.WriteElementString("title", mname);
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.WriteStartElement("body");
				xmlTextWriter.WriteStartElement("table");
				xmlTextWriter.WriteAttributeString("width", "800");
				xmlTextWriter.WriteStartElement("tr");
				xmlTextWriter.WriteStartElement("td");
				xmlTextWriter.WriteAttributeString("bgcolor", "Gray");
				xmlTextWriter.WriteStartElement("font");
				xmlTextWriter.WriteAttributeString("size", "7");
				xmlTextWriter.WriteAttributeString("color", "white");
				xmlTextWriter.WriteString(mname);
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.WriteStartElement("table");
				xmlTextWriter.WriteAttributeString("width", "800");
				xmlTextWriter.WriteAttributeString("border", "0");
				bool flag = false;
				for (int i = 0; i < listBox.Items.Count; i++)
				{
					if (listBox.Items[i] is HeaderInfo)
					{
						xmlTextWriter.WriteStartElement("table");
						xmlTextWriter.WriteAttributeString("width", "800");
						xmlTextWriter.WriteAttributeString("border", "0");
						xmlTextWriter.WriteStartElement("tr");
						xmlTextWriter.WriteStartElement("td");
						xmlTextWriter.WriteAttributeString("width", "800");
						xmlTextWriter.WriteAttributeString("height", "30");
						xmlTextWriter.WriteAttributeString("valign", "bottom");
						xmlTextWriter.WriteStartElement("font");
						xmlTextWriter.WriteAttributeString("size", "4");
						xmlTextWriter.WriteAttributeString("color", "gray");
						xmlTextWriter.WriteString(listBox.Items[i].ToString());
						xmlTextWriter.WriteEndElement();
						xmlTextWriter.WriteEndElement();
						xmlTextWriter.WriteEndElement();
						xmlTextWriter.WriteStartElement("tr");
						xmlTextWriter.WriteStartElement("td");
						xmlTextWriter.WriteAttributeString("width", "800");
						xmlTextWriter.WriteAttributeString("height", "1");
						xmlTextWriter.WriteAttributeString("bgcolor", "black");
						xmlTextWriter.WriteEndElement();
						xmlTextWriter.WriteEndElement();
						xmlTextWriter.WriteStartElement("table");
						xmlTextWriter.WriteAttributeString("width", "800");
						xmlTextWriter.WriteAttributeString("border", "0");
						flag = true;
						continue;
					}
					IssueInfo issueInfo = (IssueInfo)listBox.Items[i];
					xmlTextWriter.WriteStartElement("table");
					xmlTextWriter.WriteAttributeString("width", "800");
					xmlTextWriter.WriteAttributeString("border", "0");
					xmlTextWriter.WriteStartElement("tr");
					WriteErrorImageToHTML(xmlTextWriter, issueInfo.Severity);
					xmlTextWriter.WriteStartElement("td");
					xmlTextWriter.WriteAttributeString("width", "450");
					xmlTextWriter.WriteString(issueInfo.Title);
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteStartElement("td");
					xmlTextWriter.WriteAttributeString("width", "334");
					xmlTextWriter.WriteString(issueInfo.GroupingType + ": " + issueInfo.GroupingName);
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteStartElement("table");
					xmlTextWriter.WriteAttributeString("width", "800");
					xmlTextWriter.WriteAttributeString("border", "0");
					xmlTextWriter.WriteStartElement("tr");
					xmlTextWriter.WriteStartElement("td");
					xmlTextWriter.WriteAttributeString("width", "16");
					xmlTextWriter.WriteAttributeString("height", "16");
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteStartElement("td");
					xmlTextWriter.WriteAttributeString("width", "784");
					xmlTextWriter.WriteString(issueInfo.Description);
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndElement();
					if (issueInfo.ArticleGuid.Length > 0)
					{
						xmlTextWriter.WriteStartElement("table");
						xmlTextWriter.WriteAttributeString("width", "800");
						xmlTextWriter.WriteStartElement("tr");
						xmlTextWriter.WriteStartElement("td");
						xmlTextWriter.WriteAttributeString("width", "32");
						xmlTextWriter.WriteEndElement();
						xmlTextWriter.WriteStartElement("td");
						xmlTextWriter.WriteAttributeString("width", "500");
						xmlTextWriter.WriteStartElement("a");
						xmlTextWriter.WriteAttributeString("href", string.Format(mainGUI.ConfigInfo.ArticleURL, mainGUI.ExecInterface.Culture, mainGUI.ExecInterface.EngineVersion.Major, issueInfo.ArticleGuid));
						xmlTextWriter.WriteAttributeString("target", "&quot;_blank&quot;");
						xmlTextWriter.WriteStartElement("img");
						xmlTextWriter.WriteAttributeString("src", mainGUI.ConfigInfo.DownloadURL + "/arrow.gif");
						xmlTextWriter.WriteAttributeString("alt", "arrow");
						xmlTextWriter.WriteAttributeString("border", "0");
						xmlTextWriter.WriteAttributeString("align", "absbottom");
						xmlTextWriter.WriteEndElement();
						string str = BPALoc.LinkLabel_VSMore;
						if (issueInfo.Severity != IssueSeverity.Error && issueInfo.Severity != IssueSeverity.Warning)
						{
							str = BPALoc.LinkLabel_VSMoreNonError;
						}
						xmlTextWriter.WriteString(" " + str);
						xmlTextWriter.WriteEndElement();
						xmlTextWriter.WriteEndElement();
						xmlTextWriter.WriteEndElement();
						xmlTextWriter.WriteEndElement();
					}
					xmlTextWriter.WriteStartElement("table");
					xmlTextWriter.WriteAttributeString("width", "800");
					xmlTextWriter.WriteAttributeString("border", "0");
					xmlTextWriter.WriteStartElement("tr");
					xmlTextWriter.WriteStartElement("td");
					if (i + 1 == listBox.Items.Count)
					{
						xmlTextWriter.WriteAttributeString("height", "16");
						xmlTextWriter.WriteAttributeString("bgcolor", "Gray");
					}
					else
					{
						xmlTextWriter.WriteAttributeString("height", "1");
						xmlTextWriter.WriteAttributeString("bgcolor", "black");
					}
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndElement();
					if (flag && (i + 1 == listBox.Items.Count || listBox.Items[i + 1].GetType() == typeof(string)))
					{
						xmlTextWriter.WriteEndElement();
						xmlTextWriter.WriteEndElement();
						flag = false;
					}
				}
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.WriteEndElement();
			}
			catch (Exception arg)
			{
				mainGUI.TraceWrite(BPALoc.Label_VSExportError(arg));
			}
			finally
			{
				if (xmlTextWriter != null)
				{
					xmlTextWriter.Close();
				}
			}
		}

		protected override void DrawExpandedItem(ListBox listBox, DrawItemEventArgs e, Rectangle bounds, bool toExpand)
		{
			if (bounds.Height <= MainGUI.DefaultFont.Height * 2)
			{
				return;
			}
			try
			{
				IssueInfo issueInfo = (IssueInfo)listBox.Items[e.Index];
				DrawUnexpandedItem(listBox, e, bounds, toExpand);
				e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
				int indent = GetIndent(listBox, e.Index);
				bounds.X += indent;
				bounds.Width -= indent;
				int num = (int)e.Graphics.MeasureString(issueInfo.Description, MainGUI.DefaultFont, bounds.Width - 24).Height + 8;
				int num2 = bounds.Height - (MainGUI.DefaultFont.Height + 12 + 4);
				BPALink bPALink = new BPALink(mainGUI, MainGUI.Actions.None, false, "temp", mainGUI.ArrowPic, new Point(0, 0), 0, null);
				if (issueInfo.ArticleGuid.Length > 0 && mainGUI.Customizations.AllowDetailedArticleLinks)
				{
					num2 -= bPALink.Height + 8;
				}
				if (mainGUI.Customizations.AllowReport[2])
				{
					if (issueInfo.Suppressed)
					{
						if (issueInfo.GroupingName.Length > 0 && mainGUI.RegSettings.MsgSuppress.IsSuppressed(issueInfo.MsgIdInstance))
						{
							num2 -= bPALink.Height + 8;
						}
						if (mainGUI.RegSettings.MsgSuppress.IsSuppressed(issueInfo.MsgId))
						{
							num2 -= bPALink.Height + 8;
						}
					}
					else
					{
						num2 -= bPALink.Height + 8;
						if (issueInfo.GroupingName.Length > 0)
						{
							num2 -= bPALink.Height + 8;
						}
					}
				}
				if (expandedEntry != issueInfo)
				{
					expandedEntry = issueInfo;
					ResetLinks();
					Point location = new Point(bounds.X + 24, bounds.Y + MainGUI.DefaultFont.Height + 12);
					if (issueInfo.ExtendedData == null)
					{
						issueInfo.ExtendedData = new IssueInfoExtendedData();
					}
					((IssueInfoExtendedData)issueInfo.ExtendedData).Location = new Point(39, MainGUI.DefaultFont.Height + 12);
					if (num2 < num)
					{
						num = num2;
					}
					issueText = new BPAPanel(location, bounds.Width - 24, num, listBox);
					issueText.BackColor = MainGUI.SelectColor;
					issueText.BorderStyle = BorderStyle.None;
					issueText.AutoScroll = true;
					BPALabel bPALabel = new BPALabel(issueInfo.Description, new Point(0, 0), issueText.Width - SystemInformation.VerticalScrollBarWidth, issueText);
					bPALabel.BackColor = MainGUI.SelectColor;
					location = new Point(location.X + 15, location.Y + num + 4);
					issueText.LocationChanged += ExpandedIssueMoved;
					if (issueInfo.ArticleGuid.Length > 0 && mainGUI.Customizations.AllowDetailedArticleLinks)
					{
						string text = BPALoc.LinkLabel_VSMore;
						if (issueInfo.Severity != IssueSeverity.Error && issueInfo.Severity != IssueSeverity.Warning)
						{
							text = BPALoc.LinkLabel_VSMoreNonError;
						}
						moreLink = new BPALink(mainGUI, MainGUI.Actions.None, false, text, mainGUI.ArrowPic, location, 0, listBox);
						moreLink.Tag = issueInfo;
						moreLink.BackColor = MainGUI.SelectColor;
						moreLink.AddClickEvent(MoreInfoClicked);
						location = Navigate.Below(moreLink, 0.8f);
					}
					if (mainGUI.Customizations.AllowReport[2])
					{
						if (issueInfo.Suppressed)
						{
							if (issueInfo.GroupingName.Length > 0 && mainGUI.RegSettings.MsgSuppress.IsSuppressed(issueInfo.MsgIdInstance))
							{
								disableInstanceLink = new BPALink(mainGUI, MainGUI.Actions.EnableIssueInstance, false, BPALoc.LinkLabel_VSEnableInstance, mainGUI.ArrowPic, location, 0, listBox);
								disableInstanceLink.Tag = issueInfo;
								disableInstanceLink.BackColor = MainGUI.SelectColor;
								location = Navigate.Below(disableInstanceLink, 0.8f);
							}
							if (mainGUI.RegSettings.MsgSuppress.IsSuppressed(issueInfo.MsgId))
							{
								disableAllLink = new BPALink(mainGUI, MainGUI.Actions.EnableIssueAll, false, BPALoc.LinkLabel_VSEnableAll, mainGUI.ArrowPic, location, 0, listBox);
								disableAllLink.Tag = issueInfo;
								disableAllLink.BackColor = MainGUI.SelectColor;
								location = Navigate.Below(disableAllLink, 0.8f);
							}
						}
						else
						{
							if (issueInfo.GroupingName.Length > 0)
							{
								disableInstanceLink = new BPALink(mainGUI, MainGUI.Actions.DisableIssueInstance, false, BPALoc.LinkLabel_VSDisableInstance, mainGUI.ArrowPic, location, 0, listBox);
								disableInstanceLink.Tag = issueInfo;
								disableInstanceLink.BackColor = MainGUI.SelectColor;
								location = Navigate.Below(disableInstanceLink, 0.8f);
							}
							disableAllLink = new BPALink(mainGUI, MainGUI.Actions.DisableIssueAll, false, BPALoc.LinkLabel_VSDisableAll, mainGUI.ArrowPic, location, 0, listBox);
							disableAllLink.Tag = issueInfo;
							disableAllLink.BackColor = MainGUI.SelectColor;
							location = Navigate.Below(disableAllLink, 0.8f);
						}
					}
					listBox.Invalidate();
				}
				DrawLastLine(e, bounds);
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		private void MoreInfoClicked(object sender, EventArgs e)
		{
			try
			{
				IssueInfo issueInfo = (IssueInfo)((Control)sender).Tag;
				ShowArticle(issueInfo.ArticleGuid);
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		protected override void DrawUnexpandedItem(ListBox listBox, DrawItemEventArgs e, Rectangle bounds, bool toExpand)
		{
			try
			{
				IssueInfo issueInfo = (IssueInfo)listBox.Items[e.Index];
				int indent = GetIndent(listBox, e.Index);
				bounds.X += indent;
				bounds.Width -= indent;
				Rectangle rect = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
				if (listBox.SelectedIndex == e.Index)
				{
					e.Graphics.FillRectangle(new SolidBrush(MainGUI.SelectColor), rect);
				}
				Icon icon = issueInfo.SeverityIcon();
				if (icon != null)
				{
					e.Graphics.DrawIcon(icon, bounds.X + 4, bounds.Y + 6);
				}
				rect = new Rectangle(bounds.X + CommonData.ErrorIcon.Width + 8, bounds.Y + 8, bounds.Width * 2 / 3, MainGUI.DefaultFont.Height + 2);
				StringFormat stringFormat = new StringFormat();
				stringFormat.Trimming = StringTrimming.EllipsisCharacter;
				e.Graphics.DrawString(issueInfo.Title, MainGUI.DefaultFont, new SolidBrush(SystemColors.ControlText), rect, stringFormat);
				if (issueInfo.GroupingName.Length > 0)
				{
					rect = new Rectangle(rect.Right, bounds.Y + 8, bounds.Width - rect.Right, MainGUI.DefaultFont.Height + 2);
					e.Graphics.DrawString(issueInfo.GroupingType + ": " + issueInfo.GroupingName, MainGUI.DefaultFont, new SolidBrush(SystemColors.ControlText), rect, stringFormat);
				}
				if (!toExpand)
				{
					DrawLastLine(e, bounds);
					if (expandedEntry == issueInfo)
					{
						expandedEntry = null;
						ResetLinks();
					}
				}
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		private void ExpandedIssueMoved(object sender, EventArgs e)
		{
			try
			{
				if (inMoveLogic)
				{
					return;
				}
				inMoveLogic = true;
				if (listBox.SelectedIndex == -1)
				{
					ResetLinks();
				}
				else
				{
					Rectangle itemRectangle = listBox.GetItemRectangle(listBox.SelectedIndex);
					Point location = new Point(((IssueInfoExtendedData)expandedEntry.ExtendedData).Location.X + itemRectangle.X + GetIndent(listBox, listBox.SelectedIndex), ((IssueInfoExtendedData)expandedEntry.ExtendedData).Location.Y + itemRectangle.Y);
					if (issueText != null)
					{
						issueText.Location = location;
						location = new Point(location.X + 15, location.Y + issueText.Height + 4);
						issueText.Invalidate(true);
					}
					if ((expandedEntry.Severity == IssueSeverity.Error || expandedEntry.Severity == IssueSeverity.Warning) && moreLink != null)
					{
						moreLink.Location = location;
						location = Navigate.Below(moreLink, 0.8f);
						moreLink.Invalidate();
					}
					if (disableInstanceLink != null)
					{
						disableInstanceLink.Location = location;
						location = Navigate.Below(disableInstanceLink, 0.8f);
						disableInstanceLink.Invalidate();
					}
					if (disableAllLink != null)
					{
						disableAllLink.Location = location;
						location = Navigate.Below(disableAllLink, 0.8f);
						disableAllLink.Invalidate();
					}
				}
				listBox.Invalidate(true);
				inMoveLogic = false;
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		protected override int MeasureExpandedItem(ListBox listBox, MeasureItemEventArgs e)
		{
			int num = 0;
			try
			{
				IssueInfo issueInfo = (IssueInfo)listBox.Items[e.Index];
				num = MeasureUnexpandedItem(listBox, e);
				e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
				BPALink bPALink = new BPALink(mainGUI, MainGUI.Actions.None, false, "temp", mainGUI.ArrowPic, new Point(0, 0), 0, null);
				num += (int)e.Graphics.MeasureString(issueInfo.Description, MainGUI.DefaultFont, listBox.Width - 51 - 20 - SystemInformation.VerticalScrollBarWidth).Height + 8 + 4;
				if (issueInfo.ArticleGuid.Length > 0 && mainGUI.Customizations.AllowDetailedArticleLinks)
				{
					num += bPALink.Height + 8;
				}
				if (mainGUI.Customizations.AllowReport[2])
				{
					if (issueInfo.Suppressed)
					{
						if (issueInfo.GroupingName.Length > 0 && mainGUI.RegSettings.MsgSuppress.IsSuppressed(issueInfo.MsgIdInstance))
						{
							num += bPALink.Height + 8;
						}
						if (mainGUI.RegSettings.MsgSuppress.IsSuppressed(issueInfo.MsgId))
						{
							num += bPALink.Height + 8;
							return num;
						}
						return num;
					}
					num += bPALink.Height + 8;
					if (issueInfo.GroupingName.Length > 0)
					{
						num += bPALink.Height + 8;
						return num;
					}
					return num;
				}
				return num;
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
				return num;
			}
		}

		protected override int MeasureUnexpandedItem(ListBox listBox, MeasureItemEventArgs e)
		{
			return MainGUI.DefaultFont.Height + 12;
		}
	}
}
