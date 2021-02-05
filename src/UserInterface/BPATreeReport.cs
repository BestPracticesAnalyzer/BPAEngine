using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPATreeReport : BPAReport
	{
		private class NodeInfoExtendedData
		{
			public TreeNode treeViewNode;

			public bool expanded;
		}

		private BPATreeView treeView;

		private BPATextBox findText;

		private BPAPanel findPanel;

		private NodeInfo rootNode;

		private TreeNode lastNodePrinted;

		private Font linkFont;

		public BPATreeReport(MainGUI mainGUI, BPAReportType reportType, string name)
			: base(mainGUI, reportType, name)
		{
			linkFont = new Font(MainGUI.DefaultFont, MainGUI.DefaultFont.Style | FontStyle.Underline);
		}

		public override void Build(Document doc)
		{
		}

		public override void EnableItem(object itemInfo, bool enable, bool all)
		{
		}

		public override void ReloadReport(DataInfo dataInfo)
		{
			base.ReloadReport(dataInfo);
			if (mdataType == DataType.DetailView)
			{
				rootNode = dataInfo.Data.DetailNodes;
			}
			else
			{
				rootNode = dataInfo.Data.SummaryNodes;
			}
			ShowReport();
		}

		public override void ShowMoreInfo(object itemInfo)
		{
		}

		public override void CancelFind()
		{
			findPanel.Visible = false;
		}

		public override void FindNext()
		{
			try
			{
				FindMatch(true);
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		public override void FindPrev()
		{
			try
			{
				FindMatch(false);
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
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
					num = PrintNextNode(num, e);
					if (lastNodePrinted.NextVisibleNode == null)
					{
						e.HasMorePages = false;
						lastNodePrinted = null;
						flag = true;
					}
					if (num >= e.MarginBounds.Bottom || flag)
					{
						Rectangle rect = new Rectangle(e.MarginBounds.X, e.MarginBounds.Bottom - 8, e.MarginBounds.Width, 8);
						e.Graphics.FillRectangle(new SolidBrush(MainGUI.DarkGray), rect);
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

		public override void ShowFind()
		{
			findPanel.Visible = true;
			foreach (Control control in findPanel.Controls)
			{
				control.Visible = true;
			}
		}

		public override void StartFind()
		{
		}

		private void AddLevels(TreeNodeCollection treeNodes, NodeInfo node, int numLevels)
		{
			if (node.Children != null)
			{
				foreach (NodeInfo child in node.Children)
				{
					if (child.ExtendedData == null)
					{
						child.ExtendedData = new NodeInfoExtendedData();
					}
					TreeNode treeNode = treeNodes.Add(child.DisplayName);
					treeNode.ImageIndex = IssueInfo.SeverityIndex(child.Severity);
					treeNode.SelectedImageIndex = IssueInfo.SeverityIndex(child.Severity);
					treeNode.Tag = child;
					if (child.ArticleGuid.Length > 0)
					{
						treeNode.NodeFont = linkFont;
					}
					((NodeInfoExtendedData)child.ExtendedData).treeViewNode = treeNode;
					if (numLevels > 1)
					{
						AddLevels(treeNode.Nodes, child, numLevels - 1);
					}
				}
			}
			mnumberOfEntries++;
			((NodeInfoExtendedData)node.ExtendedData).expanded = true;
		}

		protected override void CreateControls()
		{
			mparentControl.Controls.Clear();
			treeView = new BPATreeView(new Point(0, 0), new Size(0, 0), mparentControl);
			treeView.ShowLines = true;
			treeView.Dock = DockStyle.Fill;
			treeView.BorderStyle = BorderStyle.None;
			ImageList imageList = new ImageList();
			imageList.TransparentColor = Color.Transparent;
			imageList.ImageSize = new Size(16, 16);
			imageList.Images.Add(mainGUI.SelectPic);
			imageList.Images.Add(CommonData.InfoPic);
			imageList.Images.Add(CommonData.NonDefaultPic);
			imageList.Images.Add(CommonData.RecentChangePic);
			imageList.Images.Add(CommonData.BaselinePic);
			imageList.Images.Add(CommonData.BestPracticePic);
			imageList.Images.Add(CommonData.WarningPic);
			imageList.Images.Add(CommonData.ErrorPic);
			treeView.ImageList = imageList;
			treeView.BeforeExpand += ExpandNode;
			treeView.BeforeCollapse += CollapseNode;
			treeView.KeyDown += KeyPressed;
			treeView.MouseDown += MouseDown;
			findPanel = new BPAPanel(new Point(0, 0), 0, 28, mparentControl);
			findPanel.Visible = true;
			findPanel.BorderStyle = BorderStyle.None;
			findPanel.Dock = DockStyle.Top;
			findPanel.AutoScroll = false;
			findPanel.BackColor = SystemColors.Control;
			findPanel.TabStop = false;
			findPanel.SetOrigRect();
			Point location = new Point(10, 4);
			BPALabel bPALabel = new BPALabel(BPALoc.Search_Find, location, 0, findPanel);
			bPALabel.Top += 2;
			bPALabel.SetOrigRect();
			location.X += bPALabel.Width + 10;
			findText = new BPATextBox(location, 1f, findPanel);
			location.X += findText.Width + 10;
			BPALink bPALink = new BPALink(mainGUI, MainGUI.Actions.FindNext, false, "", mainGUI.ShowPic, location, 0, findPanel);
			bPALink.Top += 2;
			bPALink.SetOrigRect();
			location.X += bPALink.Width + 10;
			BPALink bPALink2 = new BPALink(mainGUI, MainGUI.Actions.FindPrev, false, "", mainGUI.HidePic, location, 0, findPanel);
			bPALink2.Top += 2;
			bPALink2.SetOrigRect();
			BPALink bPALink3 = new BPALink(mainGUI, MainGUI.Actions.CancelFind, false, "", mainGUI.ExitPic, location, 0, findPanel);
			bPALink3.Top += 2;
			Navigate.RightJustify(bPALink3);
			bPALink3.SetOrigRect();
			BPAPictureBox bPAPictureBox = new BPAPictureBox(Color.White, new Point(0, 0), new Size(mparentControl.Width, 1), mparentControl);
			bPAPictureBox.Dock = DockStyle.Top;
			CreateLinksPanel(mparentControl);
			new BPAPictureBox(Color.White, new Point(0, 0), new Size(mparentControl.Width, 1), mparentControl);
			bPAPictureBox.Dock = DockStyle.Top;
			BPAPanel bPAPanel = new BPAPanel(new Point(0, 0), 0, 32, mparentControl);
			bPAPanel.BorderStyle = BorderStyle.None;
			bPAPanel.Dock = DockStyle.Top;
			bPAPanel.AutoScroll = false;
			bPAPanel.BackColor = MainGUI.DarkGray;
			bPAPanel.TabStop = false;
			bPAPanel.SetOrigRect();
			location = new Point(0, MainGUI.DefaultFont.Height / 2);
			BPALabel bPALabel2 = new BPALabel(mname, new Point(10, 0), bPAPanel.Width, bPAPanel);
			bPALabel2.Font = new Font(MainGUI.DefaultFont.FontFamily, 14f, FontStyle.Regular, GraphicsUnit.Point, 0);
			bPALabel2.ForeColor = Color.White;
			bPALabel2.BackColor = MainGUI.DarkGray;
			bPALabel2.Size = bPALabel2.GetWidth();
			bPAPanel.Height = findText.Bottom + MainGUI.DefaultFont.Height / 2;
			if (bPAPanel.Height < bPALabel2.Height + 8)
			{
				bPAPanel.Height = bPALabel2.Height + 8;
			}
			if (bPAPanel.Height < 32)
			{
				bPAPanel.Height = 32;
			}
			bPALabel2.Top = (bPAPanel.Height - bPALabel2.Height) / 2;
			int num = 200;
			bPALabel.TabIndex = num++;
			findText.TabIndex = num++;
			bPALink2.SetTabIndex(num++);
			bPALink.SetTabIndex(num++);
			bPALink3.SetTabIndex(num++);
			treeView.TabIndex = num++;
			findPanel.Visible = false;
			mparentControl.Hide();
		}

		private void FindMatch(bool forward)
		{
			Regex regex = new Regex(findText.Text, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
			NodeInfo nodeInfo = rootNode;
			if (treeView.SelectedNode != null && treeView.SelectedNode.Tag != null)
			{
				nodeInfo = (NodeInfo)treeView.SelectedNode.Tag;
				nodeInfo = nodeInfo.Advance(forward);
			}
			else if (!forward)
			{
				while (nodeInfo.Children != null && nodeInfo.Children.Count > 0)
				{
					nodeInfo = (NodeInfo)nodeInfo.Children[nodeInfo.Children.Count - 1];
				}
			}
			while (nodeInfo != null)
			{
				if (regex.Match(nodeInfo.Text).Success)
				{
					ShowNode(nodeInfo);
					break;
				}
				nodeInfo = nodeInfo.Advance(forward);
			}
			if (nodeInfo == null)
			{
				MessageBox.Show(BPALoc.Search_NotFound, mainGUI.Customizations.ShortName);
			}
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
			rectangle = new Rectangle(e.MarginBounds.X, y, e.MarginBounds.Width, MainGUI.TitleFont.Height);
			e.Graphics.DrawString((dataInfo.Label.Length == 0) ? dataInfo.RunTime.ToString() : dataInfo.Label, MainGUI.TitleFont, new SolidBrush(Color.Black), rectangle);
			return y + (rectangle.Height + 8);
		}

		private int PrintNextNode(int nextTop, PrintPageEventArgs e)
		{
			TreeNode treeNode = treeView.Nodes[0];
			if (lastNodePrinted != null)
			{
				treeNode = lastNodePrinted.NextVisibleNode;
			}
			NodeInfo nodeInfo = (NodeInfo)treeNode.Tag;
			int num = 0;
			for (NodeInfo parent = nodeInfo.Parent; parent != null; parent = parent.Parent)
			{
				num++;
			}
			int num2 = e.MarginBounds.X + 3 + CommonData.ErrorPic.Width / 2 + num * (CommonData.ErrorPic.Width + 4);
			Size size = e.Graphics.MeasureString(treeNode.Text, MainGUI.DefaultFont, e.MarginBounds.Width - num2).ToSize();
			if (size.Height > MainGUI.DefaultFont.Height * 4)
			{
				size.Height = MainGUI.DefaultFont.Height * 4;
			}
			if (nextTop + size.Height + 4 > e.MarginBounds.Bottom)
			{
				return e.MarginBounds.Bottom;
			}
			Point pt = new Point(num2 - CommonData.ErrorPic.Width * 3 / 2 - 4, nextTop + CommonData.ErrorPic.Height / 2);
			Point pt2 = new Point(num2, pt.Y);
			if ((nodeInfo.Children == null || nodeInfo.Children.Count == 0 || !treeNode.IsExpanded) && treeNode.ImageIndex != 0)
			{
				e.Graphics.DrawImage(treeView.ImageList.Images[treeNode.ImageIndex], num2 - CommonData.ErrorPic.Width - 4, nextTop);
				pt2.X = num2 - CommonData.ErrorPic.Width - 4;
			}
			e.Graphics.DrawLine(new Pen(Color.DarkGray, 1f), pt, pt2);
			Point pt3 = new Point(pt.X, nextTop);
			Point pt4 = new Point(pt3.X, nextTop + size.Height + 4);
			NodeInfo nodeInfo2 = nodeInfo;
			while (nodeInfo2.Parent != null)
			{
				if (nodeInfo2.Parent.Children.IndexOf(nodeInfo2) < nodeInfo2.Parent.Children.Count - 1)
				{
					e.Graphics.DrawLine(new Pen(Color.DarkGray, 1f), pt3, pt4);
				}
				else if (nodeInfo2 == nodeInfo)
				{
					e.Graphics.DrawLine(new Pen(Color.DarkGray, 1f), pt3, new Point(pt4.X, pt.Y));
				}
				nodeInfo2 = nodeInfo2.Parent;
				pt3.X -= CommonData.ErrorPic.Width + 4;
				pt4.X = pt3.X;
			}
			if (nodeInfo.Children != null && nodeInfo.Children.Count > 0)
			{
				Rectangle rect = new Rectangle(pt.X - 3, pt.Y - 3, 6, 6);
				e.Graphics.FillRectangle(new SolidBrush(Color.White), rect);
				e.Graphics.DrawRectangle(new Pen(Color.DarkGray, 1f), rect);
				e.Graphics.DrawLine(new Pen(Color.DarkGray, 1f), new Point(rect.Left + 1, rect.Y + 3), new Point(rect.Right - 1, rect.Y + 3));
				if (!treeNode.IsExpanded)
				{
					e.Graphics.DrawLine(new Pen(Color.DarkGray, 1f), new Point(rect.X + 3, rect.Top + 1), new Point(rect.X + 3, rect.Bottom - 1));
				}
				else
				{
					e.Graphics.DrawLine(new Pen(Color.DarkGray, 1f), new Point(rect.X + 3 + CommonData.ErrorPic.Width + 4, pt.Y), new Point(rect.X + 3 + CommonData.ErrorPic.Width + 4, nextTop + size.Height + 4));
				}
			}
			RectangleF layoutRectangle = new RectangleF(num2, nextTop, e.MarginBounds.Width - num2, size.Height);
			StringFormat stringFormat = new StringFormat();
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
			e.Graphics.DrawString(treeNode.Text, MainGUI.DefaultFont, new SolidBrush(Color.Black), layoutRectangle, stringFormat);
			nextTop += size.Height + 4;
			lastNodePrinted = treeNode;
			return nextTop;
		}

		protected override void SaveAsCSV(string fileName)
		{
			StreamWriter streamWriter = null;
			try
			{
				streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
				streamWriter.WriteLine("\"Indent Level\",\"Severity\",\"Title\"");
				for (TreeNode treeNode = treeView.Nodes[0]; treeNode != null; treeNode = treeNode.NextVisibleNode)
				{
					NodeInfo nodeInfo = (NodeInfo)treeNode.Tag;
					int num = 0;
					for (NodeInfo parent = nodeInfo.Parent; parent != null; parent = parent.Parent)
					{
						num++;
					}
					streamWriter.WriteLine("{0},\"{1}\",\"{2}\"", num.ToString(), IssueInfo.SeverityString(nodeInfo.Severity), nodeInfo.Text);
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
				for (TreeNode treeNode = treeView.Nodes[0]; treeNode != null; treeNode = treeNode.NextVisibleNode)
				{
					NodeInfo nodeInfo = (NodeInfo)treeNode.Tag;
					int num = 0;
					for (NodeInfo parent = nodeInfo.Parent; parent != null; parent = parent.Parent)
					{
						num++;
					}
					xmlTextWriter.WriteStartElement("table");
					xmlTextWriter.WriteAttributeString("width", "800");
					xmlTextWriter.WriteAttributeString("border", "0");
					xmlTextWriter.WriteStartElement("tr");
					xmlTextWriter.WriteStartElement("td");
					xmlTextWriter.WriteAttributeString("width", (num * 16).ToString());
					xmlTextWriter.WriteEndElement();
					WriteErrorImageToHTML(xmlTextWriter, nodeInfo.Severity);
					xmlTextWriter.WriteStartElement("td");
					xmlTextWriter.WriteAttributeString("width", (800 - (num + 1) * 16).ToString());
					xmlTextWriter.WriteString(nodeInfo.Text);
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndElement();
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

		private void ShowNode(NodeInfo nodeToShow)
		{
			if (nodeToShow.ExtendedData == null)
			{
				nodeToShow.ExtendedData = new NodeInfoExtendedData();
				ArrayList arrayList = new ArrayList();
				arrayList.Add(nodeToShow);
				while (nodeToShow.Parent.ExtendedData == null)
				{
					arrayList.Add(nodeToShow.Parent);
					nodeToShow = nodeToShow.Parent;
				}
				AddLevels(((NodeInfoExtendedData)nodeToShow.Parent.ExtendedData).treeViewNode.Nodes, nodeToShow.Parent, 1);
				for (int num = arrayList.Count - 1; num >= 0; num--)
				{
					AddLevels(((NodeInfoExtendedData)((NodeInfo)arrayList[num]).ExtendedData).treeViewNode.Nodes, (NodeInfo)arrayList[num], 1);
				}
				nodeToShow = (NodeInfo)arrayList[0];
			}
			treeView.SelectedNode = ((NodeInfoExtendedData)nodeToShow.ExtendedData).treeViewNode;
			((NodeInfoExtendedData)nodeToShow.ExtendedData).treeViewNode.EnsureVisible();
		}

		private void ShowReport()
		{
			mnumberOfEntries = 0;
			if (rootNode == null)
			{
				return;
			}
			treeView.Nodes.Clear();
			try
			{
				rootNode.ExtendedData = new NodeInfoExtendedData();
				treeView.SuspendLayout();
				AddLevels(treeView.Nodes, rootNode, 3);
				if (treeView.Nodes.Count > 1)
				{
					treeView.Nodes[0].ExpandAll();
					treeView.Nodes[1].Expand();
					treeView.Nodes[0].EnsureVisible();
				}
				treeView.ResumeLayout();
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		private void CollapseNode(object sender, TreeViewCancelEventArgs e)
		{
			try
			{
				if (e.Node.Tag != null)
				{
					e.Node.ImageIndex = IssueInfo.SeverityIndex(((NodeInfo)e.Node.Tag).Severity);
					e.Node.SelectedImageIndex = IssueInfo.SeverityIndex(((NodeInfo)e.Node.Tag).Severity);
				}
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		private void ExpandNode(object sender, TreeViewCancelEventArgs e)
		{
			try
			{
				if (e.Node.Tag == null)
				{
					return;
				}
				NodeInfo nodeInfo2 = (NodeInfo)e.Node.Tag;
				foreach (TreeNode node in e.Node.Nodes)
				{
					NodeInfo nodeInfo = (NodeInfo)node.Tag;
					if (!((NodeInfoExtendedData)nodeInfo.ExtendedData).expanded)
					{
						AddLevels(node.Nodes, nodeInfo, 1);
					}
				}
				if (e.Node.Nodes.Count > 0)
				{
					e.Node.ImageIndex = 0;
					e.Node.SelectedImageIndex = 0;
				}
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		private void KeyPressed(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.Control && (e.KeyCode == Keys.Insert || e.KeyCode == Keys.C) && treeView.SelectedNode != null)
				{
					ClipboardCopy.StartCopy(treeView.SelectedNode.Text);
				}
				else if (e.Control && e.KeyCode == Keys.E && treeView.SelectedNode != null)
				{
					treeView.SelectedNode.ExpandAll();
				}
				else if (e.KeyCode == Keys.Return && treeView.SelectedNode != null)
				{
					NodeInfo nodeInfo = (NodeInfo)treeView.SelectedNode.Tag;
					if (nodeInfo != null)
					{
						ShowArticle(nodeInfo.ArticleGuid);
					}
				}
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		private void MouseDown(object sender, MouseEventArgs e)
		{
			try
			{
				if (e.Button != MouseButtons.Left)
				{
					return;
				}
				TreeNode nodeAt = treeView.GetNodeAt(e.X, e.Y);
				if (nodeAt != null)
				{
					NodeInfo nodeInfo = (NodeInfo)nodeAt.Tag;
					if (nodeInfo != null)
					{
						ShowArticle(nodeInfo.ArticleGuid);
					}
				}
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}
	}
}
