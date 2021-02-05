using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public abstract class BPAListReport : BPAReport
	{
		protected BPAListBox listBox;

		protected BPATextBox findText;

		protected BPAComboBox arrangeByEntries;

		protected BPALabel totalItems;

		protected BPAPanel findPanel;

		protected ArrayList itemList;

		protected SortedList itemListInSelectedOrder;

		protected bool allowItemExpansion = true;

		protected bool allowMultipleSortOrders;

		public BPAListReport(MainGUI mainGUI, BPAReportType reportType, string name, bool allowItemExpansion, bool allowMultipleSortOrders)
			: base(mainGUI, reportType, name)
		{
			this.allowItemExpansion = allowItemExpansion;
			this.allowMultipleSortOrders = allowMultipleSortOrders;
			itemListInSelectedOrder = new SortedList();
		}

		public override void Build(Document doc)
		{
		}

		private void PrintPage(object sender, PrintPageEventArgs e)
		{
			PrintPage(e);
		}

		public override void ReloadReport(DataInfo dataInfo)
		{
			base.ReloadReport(dataInfo);
			if (mdataType == DataType.IssueList)
			{
				itemList = dataInfo.Data.IssueList;
			}
			else
			{
				itemList = dataInfo.Data.LogList;
			}
			ShowReport();
		}

		public override void CancelFind()
		{
			findPanel.Visible = false;
			ShowReport();
		}

		public override void FindNext()
		{
		}

		public override void FindPrev()
		{
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
			ShowReport();
		}

		protected abstract void DrawExpandedItem(ListBox listBox, DrawItemEventArgs e, Rectangle bounds, bool toExpand);

		protected abstract void DrawUnexpandedItem(ListBox listBox, DrawItemEventArgs e, Rectangle bounds, bool toExpand);

		protected abstract int MeasureExpandedItem(ListBox listBox, MeasureItemEventArgs e);

		protected abstract int MeasureUnexpandedItem(ListBox listBox, MeasureItemEventArgs e);

		protected abstract void PopulateArrangeByEntries();

		protected override void SaveAsCSV(string fileName)
		{
		}

		protected override void SaveAsHTML(string fileName)
		{
		}

		protected override void PrintPage(PrintPageEventArgs e)
		{
		}

		protected abstract void ShowReport();

		private void ArrangeByChanged(object sender, EventArgs e)
		{
			try
			{
				arrangeByEntries.Text = arrangeByEntries.SelectedItem.ToString();
				ShowReport();
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		protected override void CreateControls()
		{
			mparentControl.Controls.Clear();
			if (allowItemExpansion)
			{
				listBox = new BPAListBox(mainGUI, DrawUnexpandedItem, DrawExpandedItem, MeasureUnexpandedItem, MeasureExpandedItem, false, mparentControl);
			}
			else
			{
				listBox = new BPAListBox(mainGUI, DrawUnexpandedItem, null, MeasureUnexpandedItem, null, true, mparentControl);
			}
			listBox.Dock = DockStyle.Fill;
			listBox.KeyDown += ListBoxKeyPressed;
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
			BPALink bPALink = new BPALink(mainGUI, MainGUI.Actions.None, false, "", mainGUI.ArrowPic, location, 0, findPanel);
			bPALink.Top += 2;
			bPALink.SetOrigRect();
			bPALink.AddClickEvent(StartFindClicked);
			BPALink bPALink2 = new BPALink(mainGUI, MainGUI.Actions.None, false, "", mainGUI.ExitPic, location, 0, findPanel);
			bPALink2.Top += 2;
			Navigate.RightJustify(bPALink2);
			bPALink2.SetOrigRect();
			bPALink2.AddClickEvent(CancelFindClicked);
			BPAPictureBox bPAPictureBox = new BPAPictureBox(Color.White, new Point(0, 0), new Size(mparentControl.Width, 1), mparentControl);
			bPAPictureBox.Dock = DockStyle.Top;
			Panel panel = CreateLinksPanel(mparentControl);
			if (allowMultipleSortOrders)
			{
				location = Navigate.NextTo(panel.Controls[panel.Controls.Count - 1], 3f);
				BPALabel bPALabel2 = new BPALabel(BPALoc.Label_SEArrangeBy, location, 0, panel);
				bPALabel2.Top = (panel.Height - bPALabel2.Height) / 2;
				bPALabel2.SetOrigRect();
				location = Navigate.NextTo(bPALabel2);
				arrangeByEntries = new BPAComboBox(location, 1f, panel);
				arrangeByEntries.SelectedIndexChanged += ArrangeByChanged;
				arrangeByEntries.Top = (panel.Height - arrangeByEntries.Height) / 2;
				arrangeByEntries.SetOrigRect();
			}
			new BPAPictureBox(Color.White, new Point(0, 0), new Size(mparentControl.Width, 1), mparentControl);
			bPAPictureBox.Dock = DockStyle.Top;
			BPAPanel bPAPanel = new BPAPanel(new Point(0, 0), 0, 28, mparentControl);
			bPAPanel.BorderStyle = BorderStyle.None;
			bPAPanel.Dock = DockStyle.Top;
			bPAPanel.AutoScroll = false;
			bPAPanel.BackColor = MainGUI.DarkGray;
			bPAPanel.TabStop = false;
			bPAPanel.SetOrigRect();
			BPALabel bPALabel3 = new BPALabel(mname, new Point(10, 0), 0, bPAPanel);
			bPALabel3.Font = new Font(MainGUI.DefaultFont.FontFamily, 14f, FontStyle.Regular, GraphicsUnit.Point, 0);
			bPALabel3.ForeColor = Color.White;
			bPALabel3.BackColor = MainGUI.DarkGray;
			bPALabel3.Size = bPALabel3.GetWidth();
			bPAPanel.Height = findText.Bottom + MainGUI.DefaultFont.Height / 2;
			if (bPAPanel.Height < bPALabel3.Height + 8)
			{
				bPAPanel.Height = bPALabel3.Height + 8;
			}
			if (bPAPanel.Height < 28)
			{
				bPAPanel.Height = 28;
			}
			bPALabel3.Top = (bPAPanel.Height - bPALabel3.Height) / 2;
			location = Navigate.NextTo(bPALabel3);
			location.Y += bPALabel3.Font.Height - MainGUI.DefaultFont.Height;
			totalItems = new BPALabel(BPALoc.Label_VSTotalItems(itemListInSelectedOrder.Count), location, mparentControl.Width, bPAPanel);
			totalItems.ForeColor = Color.White;
			int num = 200;
			if (arrangeByEntries != null)
			{
				arrangeByEntries.TabIndex = num++;
			}
			bPALabel.TabIndex = num++;
			findText.TabIndex = num++;
			bPALink.SetTabIndex(num++);
			bPALink2.SetTabIndex(num++);
			listBox.TabIndex = num++;
			findPanel.Visible = false;
			if (allowMultipleSortOrders)
			{
				PopulateArrangeByEntries();
			}
		}

		private void StartFindClicked(object sender, EventArgs e)
		{
			try
			{
				StartFind();
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		private void CancelFindClicked(object sender, EventArgs e)
		{
			try
			{
				CancelFind();
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		private void ListBoxKeyPressed(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.Control && (e.KeyCode == Keys.Insert || e.KeyCode == Keys.C) && listBox.SelectedIndex != -1)
				{
					ClipboardCopy.StartCopy(listBox.Items[listBox.SelectedIndex].ToString());
				}
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}
	}
}
