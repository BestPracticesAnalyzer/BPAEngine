using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public abstract class BPAReport
	{
		protected enum DataType
		{
			IssueList,
			LogList,
			DetailView,
			SummaryView,
			Other
		}

		public const int INDEXNODEFAULT = 999;

		protected MainGUI mainGUI;

		protected DataInfo dataInfo;

		protected BPALink linkPrint;

		protected BPALink linkExport;

		protected BPALink linkFind;

		protected string mname = "";

		protected Panel mparentControl;

		protected DataType mdataType;

		protected int mprintPage;

		private bool controlsCreated;

		private int defaultIndex = 999;

		private BPATabPage reportPage;

		protected int mnumberOfEntries;

		protected BPAReportType mreportType = BPAReportType.CustomReport;

		public string Name
		{
			get
			{
				return mname;
			}
		}

		public Control ParentControl
		{
			get
			{
				return mparentControl;
			}
		}

		public int DefaultIndex
		{
			get
			{
				return defaultIndex;
			}
			set
			{
				defaultIndex = value;
			}
		}

		public int NumberOfEntries
		{
			get
			{
				return mnumberOfEntries;
			}
		}

		public BPAReportType ReportType
		{
			get
			{
				return mreportType;
			}
		}

		public BPATabPage ReportPage
		{
			get
			{
				return reportPage;
			}
			set
			{
				reportPage = value;
			}
		}

		public BPAReport(MainGUI mainGUI, BPAReportType reportType, string name)
		{
			mreportType = reportType;
			switch (reportType)
			{
			case BPAReportType.DetailedReport:
				mdataType = DataType.DetailView;
				break;
			case BPAReportType.SummaryReport:
				mdataType = DataType.SummaryView;
				break;
			case BPAReportType.LogReport:
				mdataType = DataType.LogList;
				break;
			case BPAReportType.CustomReport:
				mdataType = DataType.Other;
				break;
			default:
				mdataType = DataType.IssueList;
				break;
			}
			this.mainGUI = mainGUI;
			mname = name;
			mparentControl = new BPAPanel();
			mparentControl.BorderStyle = BorderStyle.None;
			mparentControl.Dock = DockStyle.Fill;
		}

		public abstract void Build(Document doc);

		public abstract void CancelFind();

		public abstract void EnableItem(object itemInfo, bool enable, bool all);

		public abstract void FindNext();

		public abstract void FindPrev();

		public abstract void ShowFind();

		public abstract void ShowMoreInfo(object itemInfo);

		public abstract void StartFind();

		public virtual void ExportView()
		{
			string fileName = "";
			if (dataInfo != null)
			{
				fileName = dataInfo.FileName.Substring(0, dataInfo.FileName.Length - 4);
			}
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Title = BPALoc.Label_VSExport;
			if (dataInfo == null)
			{
				saveFileDialog.Filter = string.Format("{0}|*.htm|{1}|*.csv", BPALoc.Label_SEExportTypeHTM, BPALoc.Label_SEExportTypeCSV);
			}
			else
			{
				saveFileDialog.Filter = string.Format("{0}|*.htm|{1}|*.csv|{2}|*.xml", BPALoc.Label_SEExportTypeHTM, BPALoc.Label_SEExportTypeCSV, BPALoc.Label_SEExportTypeXML);
			}
			saveFileDialog.InitialDirectory = mainGUI.RegSettings.ImportExportDirectory;
			saveFileDialog.AddExtension = false;
			saveFileDialog.OverwritePrompt = true;
			saveFileDialog.FileName = fileName;
			if (saveFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			if (saveFileDialog.FilterIndex == 3)
			{
				if (!saveFileDialog.FileName.ToUpper().EndsWith(".XML"))
				{
					saveFileDialog.FileName += ".xml";
				}
				if (dataInfo.FileName != saveFileDialog.FileName)
				{
					try
					{
						File.Copy(dataInfo.FileName, saveFileDialog.FileName, true);
					}
					catch (Exception arg)
					{
						mainGUI.TraceWrite(BPALoc.Label_VSExportError(arg));
					}
				}
			}
			else if (saveFileDialog.FilterIndex == 1)
			{
				if (!saveFileDialog.FileName.ToUpper().EndsWith(".HTM"))
				{
					saveFileDialog.FileName += ".htm";
				}
				SaveAsHTML(saveFileDialog.FileName);
			}
			else
			{
				if (!saveFileDialog.FileName.ToUpper().EndsWith(".CSV"))
				{
					saveFileDialog.FileName += ".csv";
				}
				SaveAsCSV(saveFileDialog.FileName);
			}
		}

		public virtual void PrintScan()
		{
			PrintDocument printDocument = new PrintDocument();
			mprintPage = 1;
			printDocument.PrintPage += PrintPage;
			PrintDialog printDialog = new PrintDialog();
			printDialog.AllowSelection = false;
			printDialog.AllowPrintToFile = false;
			printDialog.AllowSomePages = false;
			printDialog.ShowHelp = true;
			printDialog.Document = printDocument;
			if (printDialog.ShowDialog() == DialogResult.OK)
			{
				printDocument.Print();
			}
		}

		private void PrintPage(object sender, PrintPageEventArgs e)
		{
			PrintPage(e);
		}

		public virtual void ReloadReport(DataInfo dataInfo)
		{
			this.dataInfo = dataInfo;
			if (linkExport != null)
			{
				linkExport.Tag = dataInfo;
			}
			if (!controlsCreated)
			{
				CreateControls();
				controlsCreated = true;
			}
		}

		public virtual void LoadCustomData(Document doc)
		{
		}

		public override string ToString()
		{
			return mname;
		}

		protected abstract void SaveAsCSV(string fileName);

		protected abstract void SaveAsHTML(string fileName);

		protected abstract void PrintPage(PrintPageEventArgs e);

		protected abstract void CreateControls();

		protected void ShowArticle(string articleGuid)
		{
			if (articleGuid.Length != 0)
			{
				if (CommonData.CanAccessWeb)
				{
					string url = string.Format(mainGUI.ConfigInfo.ArticleURL, mainGUI.ExecInterface.Culture, mainGUI.ExecInterface.EngineVersion.Major, articleGuid);
					CommonData.BrowseURL(url);
				}
				else if (mainGUI.ConfigInfo.HelpFile.Length > 0)
				{
					string parameter = string.Format("html\\{0}.htm", articleGuid);
					Help.ShowHelp(mainGUI.MainForm, mainGUI.ConfigInfo.HelpFile, HelpNavigator.Topic, parameter);
				}
			}
		}

		protected void WriteErrorImageToHTML(XmlTextWriter htmlReport, IssueSeverity severity)
		{
			htmlReport.WriteStartElement("td");
			htmlReport.WriteAttributeString("width", "16");
			htmlReport.WriteAttributeString("height", "16");
			string value = mainGUI.ConfigInfo.DownloadURL + "/" + IssueInfo.SeverityImageString(severity);
			string text = IssueInfo.SeverityString(severity);
			if (text != CommonLoc.SeverityUnknown)
			{
				htmlReport.WriteStartElement("img");
				htmlReport.WriteAttributeString("src", value);
				htmlReport.WriteAttributeString("alt", text);
				htmlReport.WriteEndElement();
			}
			htmlReport.WriteEndElement();
		}

		protected Panel CreateLinksPanel(Control parentControl)
		{
			BPAPanel bPAPanel = new BPAPanel(new Point(0, 0), parentControl.Width, 28, parentControl);
			bPAPanel.Dock = DockStyle.Top;
			bPAPanel.BorderStyle = BorderStyle.None;
			bPAPanel.BackColor = SystemColors.Control;
			bPAPanel.SetOrigRect();
			bPAPanel.TabStop = false;
			int num = 100;
			linkPrint = new BPALink(location: new Point(10, 0), mainGUI: mainGUI, action: MainGUI.Actions.None, showBorder: false, text: BPALoc.Label_VSPrint, image: mainGUI.PrintPic, width: 0, parent: bPAPanel);
			linkPrint.SetTabIndex(num++);
			linkPrint.Top = (bPAPanel.Height - linkPrint.Height) / 2;
			linkPrint.SetOrigRect();
			linkPrint.LinkColor = Color.Black;
			linkPrint.AddClickEvent(PrintClicked);
			Point location2 = Navigate.NextTo(linkPrint);
			linkExport = new BPALink(mainGUI, MainGUI.Actions.None, false, BPALoc.Label_VSExport, mainGUI.ExportPic, location2, 0, bPAPanel);
			linkExport.SetTabIndex(num++);
			linkExport.Tag = dataInfo;
			linkExport.Top = (bPAPanel.Height - linkExport.Height) / 2;
			linkExport.SetOrigRect();
			linkExport.LinkColor = Color.Black;
			linkExport.AddClickEvent(ExportClicked);
			location2 = Navigate.NextTo(linkExport);
			linkFind = new BPALink(mainGUI, MainGUI.Actions.None, false, BPALoc.Search_FindNext, mainGUI.FindPic, location2, 0, bPAPanel);
			linkFind.SetTabIndex(num++);
			linkFind.Top = (bPAPanel.Height - linkFind.Height) / 2;
			linkFind.SetOrigRect();
			linkFind.LinkColor = Color.Black;
			linkFind.AddClickEvent(FindClicked);
			location2 = Navigate.NextTo(linkFind);
			return bPAPanel;
		}

		private void ExportClicked(object sender, EventArgs e)
		{
			try
			{
				ExportView();
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		private void PrintClicked(object sender, EventArgs e)
		{
			try
			{
				PrintScan();
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		private void FindClicked(object sender, EventArgs e)
		{
			try
			{
				ShowFind();
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}
	}
}
