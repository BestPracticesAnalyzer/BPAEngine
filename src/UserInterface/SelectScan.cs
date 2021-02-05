using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class SelectScan : BPAScreen
	{
		public class DataInfoExtendedData
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

		private BPAPanel statusPanel;

		private BPALabel totalServers;

		private BPAPanel fixedPanel;

		private BPALink viewLink;

		private BPALink deleteLink;

		private BPALink changeLabelLink;

		private BPALink changeLabelOkLink;

		private BPATextBox changeLabel;

		private BPALink exportLink;

		private BPAListBox scanListBox;

		private SortedList scanListInSelectedOrder;

		private DataInfo expandedEntry;

		private bool inMoveLogic;

		private int nextTabIndex;

		private int startingTabIndex;

		private string scanTitle = string.Empty;

		private string deletePrompt = string.Empty;

		private string importTitle = string.Empty;

		private string deleteScan = string.Empty;

		private static string scanEntryExport = string.Empty;

		private string scanEntryView = string.Empty;

		private string scanEntryListTitle = string.Empty;

		private string scanEntryTotal = string.Empty;

		private bool isBPADisplay = true;

		private CommonData.ConstructDataInfoDelegate constructDataInfo;

		public SelectScan(MainGUI mainGUI, CommonData.ConstructDataInfoDelegate constructDataInfoDel, string scanTitle, string deletePrompt, string deleteScan, string importTitle, string scanEntryExport, string scanEntryView, string scanEntryListTitle, string scanEntryTotal, bool isBPADisplay)
			: base(mainGUI)
		{
			Point borderCornerPoint = MainGUI.BorderCornerPoint;
			Dock = DockStyle.Fill;
			AutoScroll = true;
			BackColor = Color.White;
			base.mainGUI = mainGUI;
			scanListInSelectedOrder = new SortedList();
			nextTabIndex = mainGUI.StartingTabIndex;
			constructDataInfo = constructDataInfoDel;
			this.scanTitle = scanTitle;
			this.deletePrompt = deletePrompt;
			this.deleteScan = deleteScan;
			this.importTitle = importTitle;
			SelectScan.scanEntryExport = scanEntryExport;
			this.scanEntryView = scanEntryView;
			this.scanEntryListTitle = scanEntryListTitle;
			this.scanEntryTotal = scanEntryTotal;
			this.isBPADisplay = isBPADisplay;
			BPATitle control = new BPATitle(scanTitle, borderCornerPoint, mainGUI.FullWidth, this)
			{
				TabIndex = nextTabIndex++
			};
			borderCornerPoint = Navigate.Below(control);
			BPALink bPALink = new BPALink(mainGUI, MainGUI.Actions.Import, false, importTitle, mainGUI.ImportPic, borderCornerPoint, 0, this);
			bPALink.SetTabIndex(nextTabIndex++);
			bPALink.Left = mainGUI.FullWidth - bPALink.Width;
			bPALink.SetOrigRect();
			borderCornerPoint = Navigate.Below(control, 4f);
			statusPanel = new BPAPanel(borderCornerPoint, mainGUI.FullWidth, mainGUI.FullHeight - (borderCornerPoint.Y - MainGUI.BorderCornerPoint.Y), this);
			statusPanel.TabStop = false;
			statusPanel.ResizeFlags = 4;
			scanListBox = new BPAListBox(mainGUI, DrawUnexpandedScan, DrawExpandedScan, MeasureUnexpandedScan, MeasureExpandedScan, false, statusPanel);
			scanListBox.TabIndex = nextTabIndex++;
			scanListBox.Dock = DockStyle.Fill;
			BPAPictureBox bPAPictureBox = new BPAPictureBox(MainGUI.DarkGray, new Point(0, 0), new Size(0, 1), statusPanel)
			{
				Dock = DockStyle.Top
			};
			BPAPanel bPAPanel = new BPAPanel(new Point(0, 0), 0, MainGUI.DefaultFont.Height * 2, statusPanel);
			bPAPanel.Dock = DockStyle.Top;
			bPAPanel.BorderStyle = BorderStyle.None;
			bPAPanel.TabStop = false;
			bPAPanel.SetOrigRect();
			totalServers = new BPALabel(location: new Point(MainGUI.DefaultFont.Height / 2, MainGUI.DefaultFont.Height / 2), text: " " + string.Format(scanEntryTotal, CommonData.DataInfoList.Count), width: statusPanel.Width, parent: bPAPanel);
			totalServers.TabIndex = nextTabIndex++;
			fixedPanel = new BPAPanel(new Point(0, 0), 0, 0, statusPanel);
			fixedPanel.BorderStyle = BorderStyle.None;
			fixedPanel.Dock = DockStyle.Top;
			fixedPanel.SetOrigRect();
			fixedPanel.BackColor = MainGUI.DarkGray;
			fixedPanel.TabStop = false;
			startingTabIndex = nextTabIndex;
		}

		public SelectScan(MainGUI mainGUI)
			: this(mainGUI, CommonData.ConstructDataInfo, BPALoc.Label_SETitle, BPALoc.Label_SEScanDeletePrompt, BPALoc.LinkLabel_SEScanEntryDeleteScan, BPALoc.Label_SEImport, BPALoc.LinkLabel_SEScanEntryExportScan, BPALoc.LinkLabel_SEScanEntryViewScan, BPALoc.GetString("Label_SEListTitle"), BPALoc.GetString("Label_SETotal"), true)
		{
		}

		public void DeleteScan(DataInfo dataInfo)
		{
			DialogResult dialogResult = DialogResult.No;
			try
			{
				dialogResult = MessageBox.Show(deletePrompt, mainGUI.Customizations.ShortName, MessageBoxButtons.YesNo);
				if (dialogResult == DialogResult.Yes)
				{
					File.Delete(dataInfo.FileName);
					File.Delete(dataInfo.FileName.Substring(0, dataInfo.FileName.Length - "data.xml".Length) + "log");
					File.Delete(dataInfo.FileName.Substring(0, dataInfo.FileName.Length - "data.xml".Length) + "report.htm");
				}
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
			if (dialogResult == DialogResult.Yes)
			{
				CommonData.DataInfoList.Remove(dataInfo);
				ShowScanList();
			}
		}

		public void ChangeScanLabel(DataInfo dataInfo)
		{
			BPADocument bPADocument = new BPADocument();
			bPADocument.FileName = dataInfo.FileName;
			bPADocument.Load();
			bPADocument.ConfigurationNode.GetAttribute("Label");
			string text = changeLabel.Text;
			bPADocument.ConfigurationNode.SetAttribute("Label", text);
			bPADocument.Save();
			dataInfo.Label = text;
			expandedEntry = null;
			int selectedIndex = scanListBox.SelectedIndex;
			scanListBox.SelectedIndex = -1;
			scanListBox.SelectedIndex = selectedIndex;
		}

		public void ShowChangeScanLabel(DataInfo dataInfo)
		{
			if (changeLabel == null)
			{
				Point location = Navigate.NextTo(changeLabelLink);
				Control parent = changeLabelLink.Parent;
				int num = changeLabelLink.TabIndex++;
				changeLabel = new BPATextBox(location, 2f, parent);
				changeLabel.Text = dataInfo.Label;
				changeLabel.TabIndex = num++;
				location = Navigate.NextTo(changeLabel, 0.8f);
				changeLabelOkLink = new BPALink(mainGUI, MainGUI.Actions.ChangeScanLabel, false, BPALoc.Button_OK, mainGUI.ArrowPic, location, 0, parent);
				changeLabelOkLink.Tag = dataInfo;
				changeLabelOkLink.BackColor = MainGUI.MediumGray;
				changeLabelOkLink.SetTabIndex(num++);
			}
		}

		public static void ExportScan(DataInfo dataInfo, string initialDirectory)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Title = scanEntryExport;
			saveFileDialog.Filter = string.Format("{0}|*.xml", BPALoc.Label_SEExportTypeXML);
			FileInfo fileInfo = new FileInfo(dataInfo.FileName);
			saveFileDialog.InitialDirectory = initialDirectory;
			saveFileDialog.FileName = fileInfo.Name;
			if (saveFileDialog.ShowDialog() == DialogResult.OK && dataInfo.FileName != saveFileDialog.FileName)
			{
				File.Copy(dataInfo.FileName, saveFileDialog.FileName, true);
			}
		}

		public void ImportScan()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = importTitle;
			openFileDialog.Filter = string.Format("{0}(*.xml)|*.xml|{1}(*.*)|*.*", BPALoc.Label_SEImportTypeXML, BPALoc.Label_SEImportTypeAll);
			openFileDialog.CheckFileExists = true;
			openFileDialog.InitialDirectory = mainGUI.RegSettings.ImportExportDirectory;
			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			string empty = string.Empty;
			try
			{
				FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
				string text = fileInfo.Name;
				if (!text.StartsWith("output."))
				{
					text = "output." + text;
				}
				if (!text.EndsWith(".xml"))
				{
					text += ".xml";
				}
				empty = string.Format("{0}\\{1}", mainGUI.RegSettings.DataDirectory, text);
				if (File.Exists(empty))
				{
					MessageBox.Show(BPALoc.Error_FileAlreadyExists, mainGUI.Customizations.ShortName, MessageBoxButtons.OK);
					return;
				}
				File.Copy(openFileDialog.FileName, empty);
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
				return;
			}
			DataInfo dataInfo = null;
			try
			{
				dataInfo = constructDataInfo(mainGUI.ExecInterface, empty);
				if (dataInfo.Valid)
				{
					CommonData.DataInfoList.Add(dataInfo);
					ShowScanList();
				}
				else
				{
					MessageBox.Show(BPALoc.Error_FileInvalid, mainGUI.Customizations.ShortName, MessageBoxButtons.OK);
				}
			}
			catch (Exception exception2)
			{
				mainGUI.TraceError(exception2);
				try
				{
					File.Delete(empty);
				}
				catch (Exception exception3)
				{
					mainGUI.TraceError(exception3);
				}
				return;
			}
			if (dataInfo != null && dataInfo.Valid)
			{
				BPALink.LinkInfo linkInfo = new BPALink.LinkInfo();
				linkInfo.Key = Keys.None;
				linkInfo.Tag = dataInfo;
				mainGUI.TakeAction(25, null, linkInfo, string.Empty);
			}
		}

		public override bool Start()
		{
			CommonData.LoadDataInfoList(mainGUI.RegSettings.DataDirectory, mainGUI.ExecInterface, constructDataInfo);
			nextTabIndex = startingTabIndex;
			ShowScanList();
			statusPanel.SetOrigRect();
			fixedPanel.SetOrigRect();
			scanListBox.SetOrigRect();
			return true;
		}

		private void DrawLastLine(DrawItemEventArgs e, Rectangle bounds)
		{
			if (e.Index + 1 >= scanListBox.Items.Count || scanListBox.Items[e.Index + 1].GetType() == typeof(string))
			{
				Rectangle rect = new Rectangle(bounds.X + 2, bounds.Y + bounds.Height - 1, bounds.Width - 2, 1);
				e.Graphics.FillRectangle(new LinearGradientBrush(rect, MainGUI.DarkGray, MainGUI.LightGray, 0f, false), rect);
			}
		}

		private void ResetLinks()
		{
			if (viewLink != null)
			{
				scanListBox.Controls.Remove(viewLink);
				viewLink.LocationChanged -= ExpandedScanMoved;
			}
			if (deleteLink != null)
			{
				scanListBox.Controls.Remove(deleteLink);
			}
			if (exportLink != null)
			{
				scanListBox.Controls.Remove(exportLink);
			}
			if (changeLabelLink != null)
			{
				scanListBox.Controls.Remove(changeLabelLink);
			}
			if (changeLabel != null)
			{
				scanListBox.Controls.Remove(changeLabel);
			}
			if (changeLabelOkLink != null)
			{
				scanListBox.Controls.Remove(changeLabelOkLink);
			}
			viewLink = null;
			deleteLink = null;
			exportLink = null;
			changeLabelLink = null;
			changeLabel = null;
			changeLabelOkLink = null;
		}

		private void ShowScanList()
		{
			scanListInSelectedOrder.Clear();
			for (int i = 0; i < CommonData.DataInfoList.Count; i++)
			{
				DataInfo dataInfo = (DataInfo)CommonData.DataInfoList[i];
				string key = DateTime.Now.Subtract(dataInfo.RunTime).TotalSeconds.ToString("0000000000000000") + dataInfo.FileName + i;
				scanListInSelectedOrder.Add(key, dataInfo);
			}
			expandedEntry = null;
			ResetLinks();
			scanListBox.ClearExpandedIndex();
			scanListBox.SelectedIndex = -1;
			scanListBox.Items.Clear();
			foreach (DataInfo value in scanListInSelectedOrder.Values)
			{
				if (value.Valid)
				{
					scanListBox.Items.Add(value);
				}
			}
			ShowScanListHeader();
		}

		private void ShowScanListHeader()
		{
			fixedPanel.Controls.Clear();
			new Point(0, MainGUI.DefaultFont.Height / 2);
			BPALabel bPALabel = new BPALabel(string.Format(scanEntryListTitle, mainGUI.Customizations.ShortName), new Point(5, 0), fixedPanel.Width, fixedPanel);
			bPALabel.Font = new Font("Microsoft Sans Serif", 14f, FontStyle.Regular, GraphicsUnit.Point, 0);
			bPALabel.Size = bPALabel.GetSizeToFit();
			bPALabel.Height += 4;
			bPALabel.SetOrigRect();
			bPALabel.ForeColor = Color.White;
			bPALabel.BackColor = MainGUI.DarkGray;
			bPALabel.TabIndex = nextTabIndex++;
			fixedPanel.Height = bPALabel.Height + MainGUI.DefaultFont.Height / 2;
			if (fixedPanel.Height < 28)
			{
				fixedPanel.Height = 28;
			}
			bPALabel.Top = (fixedPanel.Height - bPALabel.Height) / 2;
			totalServers.Text = " " + string.Format(scanEntryTotal, CommonData.DataInfoList.Count);
			scanListBox.TabIndex = nextTabIndex++;
		}

		private void DrawExpandedScan(ListBox listBox, DrawItemEventArgs e, Rectangle bounds, bool toExpand)
		{
			DataInfo dataInfo = (DataInfo)listBox.Items[e.Index];
			DrawUnexpandedScan(listBox, e, bounds, toExpand);
			int num = bounds.Y + MainGUI.DefaultFont.Height + 14;
			if (dataInfo.Label.Length > 0)
			{
				e.Graphics.DrawString(BPALoc.Label_SEScanEntryRunTime(dataInfo.RunTime.ToString()), MainGUI.DefaultFont, new SolidBrush(SystemColors.ControlText), bounds.X + 24, num);
				num += MainGUI.DefaultFont.Height + 8;
			}
			if (isBPADisplay)
			{
				e.Graphics.DrawString(BPALoc.Label_SEScanEntryServers(dataInfo.TrackedCount.ToString()), MainGUI.DefaultFont, new SolidBrush(SystemColors.ControlText), bounds.X + 24, num);
				num += MainGUI.DefaultFont.Height + 8;
			}
			e.Graphics.DrawString(BPALoc.Label_SEScanEntryVersion(dataInfo.ConfigVersion.ToString()), MainGUI.DefaultFont, new SolidBrush(SystemColors.ControlText), bounds.X + 24, num);
			num += MainGUI.DefaultFont.Height + 8;
			FileInfo fileInfo = new FileInfo(dataInfo.FileName);
			e.Graphics.DrawString(BPALoc.Label_SEScanEntryFileName(fileInfo.Name), MainGUI.DefaultFont, new SolidBrush(SystemColors.ControlText), bounds.X + 24, num);
			num += MainGUI.DefaultFont.Height + 8;
			e.Graphics.DrawString(BPALoc.Label_SEScanEntryFileSize(dataInfo.FileSize.ToString()), MainGUI.DefaultFont, new SolidBrush(SystemColors.ControlText), bounds.X + 24, num);
			num += MainGUI.DefaultFont.Height + 8;
			if (expandedEntry != dataInfo)
			{
				expandedEntry = dataInfo;
				ResetLinks();
				int num2 = scanListBox.TabIndex + 1;
				Point location = new Point(bounds.X + 24 + 15, num);
				if (dataInfo.ExtendedData == null)
				{
					dataInfo.ExtendedData = new DataInfoExtendedData();
				}
				((DataInfoExtendedData)dataInfo.ExtendedData).Location = new Point(54, num - bounds.Y);
				viewLink = new BPALink(mainGUI, MainGUI.Actions.LoadScan, false, scanEntryView, mainGUI.ArrowPic, location, 0, listBox);
				viewLink.Tag = dataInfo;
				viewLink.BackColor = MainGUI.SelectColor;
				viewLink.LocationChanged += ExpandedScanMoved;
				viewLink.SetTabIndex(num2++);
				location = Navigate.Below(viewLink, 0.8f);
				exportLink = new BPALink(mainGUI, MainGUI.Actions.ExportScan, false, scanEntryExport, mainGUI.ArrowPic, location, 0, listBox);
				exportLink.Tag = dataInfo;
				exportLink.BackColor = MainGUI.SelectColor;
				exportLink.SetTabIndex(num2++);
				location = Navigate.Below(exportLink, 0.8f);
				deleteLink = new BPALink(mainGUI, MainGUI.Actions.DeleteScan, false, deleteScan, mainGUI.ArrowPic, location, 0, listBox);
				deleteLink.Tag = dataInfo;
				deleteLink.BackColor = MainGUI.SelectColor;
				deleteLink.SetTabIndex(num2++);
				location = Navigate.Below(deleteLink, 0.8f);
				changeLabelLink = new BPALink(mainGUI, MainGUI.Actions.ShowChangeScanLabel, false, BPALoc.Label_SSLabel, mainGUI.ArrowPic, location, 0, listBox);
				changeLabelLink.Tag = dataInfo;
				changeLabelLink.BackColor = MainGUI.SelectColor;
				changeLabelLink.SetTabIndex(num2++);
				location = Navigate.NextTo(changeLabelLink);
				listBox.Invalidate();
			}
			DrawLastLine(e, bounds);
		}

		private void DrawUnexpandedScan(ListBox listBox, DrawItemEventArgs e, Rectangle bounds, bool toExpand)
		{
			try
			{
				DataInfo dataInfo = (DataInfo)listBox.Items[e.Index];
				e.Graphics.PageUnit = GraphicsUnit.Pixel;
				Rectangle rectangle = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
				if (listBox.SelectedIndex == e.Index)
				{
					e.Graphics.FillRectangle(new SolidBrush(MainGUI.SelectColor), rectangle);
				}
				rectangle = new Rectangle(bounds.X + 2, bounds.Y, bounds.Width - 2, bounds.Height);
				if (e.Index != 0)
				{
					rectangle.Height = 1;
					e.Graphics.FillRectangle(new LinearGradientBrush(rectangle, MainGUI.DarkGray, MainGUI.LightGray, 0f, false), rectangle);
				}
				mainGUI.ReportIcon.ToBitmap();
				rectangle = new Rectangle(bounds.X + 4, bounds.Y + 6, 16, 16);
				e.Graphics.DrawIcon(mainGUI.ReportIcon, rectangle);
				string s = dataInfo.RunTime.ToString();
				if (dataInfo.Label.Length > 0)
				{
					s = dataInfo.Label;
				}
				StringFormat stringFormat = new StringFormat(StringFormatFlags.FitBlackBox | StringFormatFlags.NoWrap | StringFormatFlags.NoClip);
				stringFormat.Trimming = StringTrimming.EllipsisCharacter;
				RectangleF layoutRectangle = new RectangleF(bounds.X + 24, bounds.Y + 8, bounds.Width / 2 - 24, listBox.Font.Height);
				e.Graphics.DrawString(s, MainGUI.DefaultFont, new SolidBrush(SystemColors.ControlText), layoutRectangle, stringFormat);
				if (!toExpand)
				{
					DrawLastLine(e, bounds);
					if (expandedEntry == dataInfo)
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

		private void ExpandedScanMoved(object sender, EventArgs e)
		{
			try
			{
				if (inMoveLogic)
				{
					return;
				}
				inMoveLogic = true;
				if (scanListBox.SelectedIndex == -1)
				{
					ResetLinks();
				}
				else
				{
					Rectangle itemRectangle = scanListBox.GetItemRectangle(scanListBox.SelectedIndex);
					viewLink.Location = new Point(((DataInfoExtendedData)expandedEntry.ExtendedData).Location.X + itemRectangle.X, ((DataInfoExtendedData)expandedEntry.ExtendedData).Location.Y + itemRectangle.Y);
					Point location = Navigate.Below(viewLink, 0.8f);
					viewLink.Invalidate();
					exportLink.Location = location;
					location = Navigate.Below(exportLink, 0.8f);
					exportLink.Invalidate();
					deleteLink.Location = location;
					location = Navigate.Below(deleteLink, 0.8f);
					deleteLink.Invalidate();
					changeLabelLink.Location = location;
					location = Navigate.NextTo(changeLabelLink);
					changeLabelLink.Invalidate();
					if (changeLabel != null)
					{
						changeLabel.Location = location;
						location = Navigate.NextTo(changeLabel, 0.8f);
						changeLabel.Invalidate();
						changeLabelOkLink.Location = location;
						location = Navigate.Below(changeLabelLink, 0.8f);
						changeLabelOkLink.Invalidate();
					}
				}
				scanListBox.Invalidate();
				inMoveLogic = false;
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		private int MeasureExpandedScan(ListBox listBox, MeasureItemEventArgs e)
		{
			int num = 0;
			try
			{
				DataInfo dataInfo = (DataInfo)listBox.Items[e.Index];
				num = MeasureUnexpandedScan(listBox, e);
				BPALink bPALink = new BPALink(mainGUI, MainGUI.Actions.None, false, "temp", mainGUI.ArrowPic, new Point(0, 0), 0, null);
				num += (bPALink.Height + 8) * 4 + (MainGUI.DefaultFont.Height + 8) * 4;
				if (dataInfo.Label.Length > 0)
				{
					num += MainGUI.DefaultFont.Height + 8;
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

		private int MeasureUnexpandedScan(ListBox listBox, MeasureItemEventArgs e)
		{
			return MainGUI.DefaultFont.Height + 12;
		}
	}
}
