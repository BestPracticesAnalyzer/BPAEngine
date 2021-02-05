using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class ViewScan : BPAScreen
	{
		private BPATitle title;

		private BPARadioButtonWithPicture selectIssueReports;

		private BPARadioButtonWithPicture selectTreeReports;

		private BPARadioButtonWithPicture selectOtherReports;

		private BPALabel subtitle;

		private BPATabControl reportGroup;

		private DataInfo dataInfo;

		private int nextTabIndex;

		private static ArrayList reportList;

		private BPAReport currentIssueReport;

		private BPAReport currentTreeReport;

		private BPAReport currentOtherReport;

		private BPAReport currentReport;

		private bool disableReportChange;

		public DataInfo CurrentScan
		{
			get
			{
				return dataInfo;
			}
		}

		public ViewScan(MainGUI mainGUI, string titleText)
			: base(mainGUI)
		{
			Point borderCornerPoint = MainGUI.BorderCornerPoint;
			Dock = DockStyle.Fill;
			AutoScroll = true;
			BackColor = Color.White;
			base.mainGUI = mainGUI;
			nextTabIndex = mainGUI.StartingTabIndex;
			title = new BPATitle(titleText, borderCornerPoint, mainGUI.FullWidth, this);
			title.TabIndex = nextTabIndex++;
			borderCornerPoint = Navigate.Below(title);
			subtitle = new BPALabel("******", borderCornerPoint, mainGUI.FullWidth, this);
			subtitle.Top -= 8;
			subtitle.SetOrigRect();
			subtitle.ForeColor = title.ForeColor;
			subtitle.TabIndex = nextTabIndex++;
			borderCornerPoint = Navigate.Below(subtitle);
			BPALabel control = new BPALabel(BPALoc.Label_VSSelectReportType, borderCornerPoint, 0, this)
			{
				TabIndex = nextTabIndex++
			};
			borderCornerPoint = Navigate.NextTo(control);
			selectIssueReports = new BPARadioButtonWithPicture(BPALoc.Label_VSIssueReports, mainGUI.IssuePic, borderCornerPoint, 0, this);
			selectIssueReports.AddClickEvent(SelectReports);
			selectIssueReports.TabIndex = nextTabIndex++;
			borderCornerPoint = Navigate.NextTo(selectIssueReports, 2f);
			selectTreeReports = new BPARadioButtonWithPicture(BPALoc.Label_VSTreeReports, mainGUI.TreePic, borderCornerPoint, 0, this);
			selectTreeReports.AddClickEvent(SelectReports);
			selectTreeReports.TabIndex = nextTabIndex++;
			borderCornerPoint = Navigate.NextTo(selectTreeReports, 2f);
			selectOtherReports = new BPARadioButtonWithPicture(BPALoc.Label_VSOtherReports, mainGUI.OtherPic, borderCornerPoint, 0, this);
			selectOtherReports.AddClickEvent(SelectReports);
			selectOtherReports.TabIndex = nextTabIndex++;
			borderCornerPoint = Navigate.Below(control);
			reportGroup = new BPATabControl(borderCornerPoint, new Size(mainGUI.FullWidth, mainGUI.FullHeight - (borderCornerPoint.Y - MainGUI.BorderCornerPoint.Y)), this);
			reportGroup.SelectedIndexChanged += ReportChanged;
			reportGroup.TabStop = false;
			reportGroup.ResizeFlags = 4;
		}

		public ViewScan(MainGUI mainGUI)
			: this(mainGUI, mainGUI.Customizations.ViewScanTitle)
		{
		}

		public static void LoadReports(MainGUI mainGUI)
		{
			reportList = new ArrayList();
			if (mainGUI.Customizations.AllowReport[1])
			{
				reportList.Add(new BPAIssueListReport(mainGUI, BPAReportType.CriticalIssuesReport, BPALoc.ComboBox_VSReportCritical, true, false, 128));
			}
			if (mainGUI.Customizations.AllowReport[3])
			{
				reportList.Add(new BPAIssueListReport(mainGUI, BPAReportType.FullIssuesReport, BPALoc.ComboBox_VSReportFull, true, false, 224));
			}
			if (mainGUI.Customizations.AllowReport[5])
			{
				reportList.Add(new BPAIssueListReport(mainGUI, BPAReportType.NonDefaultReport, BPALoc.ComboBox_VSReportNonDefault, true, false, 4));
			}
			if (mainGUI.Customizations.AllowReport[6])
			{
				reportList.Add(new BPAIssueListReport(mainGUI, BPAReportType.RecentChangesReport, BPALoc.ComboBox_VSReportRecent, true, false, 8));
			}
			if (mainGUI.Customizations.AllowReport[0])
			{
				reportList.Add(new BPAIssueListReport(mainGUI, BPAReportType.BaselineReport, BPALoc.ComboBox_VSReportBaseline, true, false, 16));
			}
			if (mainGUI.Customizations.AllowReport[7])
			{
				reportList.Add(new BPAIssueListReport(mainGUI, BPAReportType.ItemsOfInterestReport, BPALoc.ComboBox_VSReportItemsOfInterest, true, false, 2));
			}
			if (mainGUI.Customizations.AllowReport[8])
			{
				reportList.Add(new BPATreeReport(mainGUI, BPAReportType.DetailedReport, BPALoc.ComboBox_VSReportDetail));
			}
			if (mainGUI.Customizations.AllowReport[9])
			{
				reportList.Add(new BPATreeReport(mainGUI, BPAReportType.SummaryReport, BPALoc.ComboBox_VSReportSummary));
			}
			if (mainGUI.Customizations.AllowReport[2])
			{
				reportList.Add(new BPAIssueListReport(mainGUI, BPAReportType.DisabledIssuesReport, BPALoc.ComboBox_VSReportDisabled, true, true, 65535));
			}
			if (mainGUI.Customizations.AllowReport[10])
			{
				reportList.Add(new BPALogListReport(mainGUI, BPAReportType.LogReport, BPALoc.ComboBox_VSReportLog));
			}
			foreach (BPAReport customReport in mainGUI.Customizations.CustomReports)
			{
				reportList.Add(customReport);
			}
		}

		public void CancelFind()
		{
			currentReport.CancelFind();
		}

		public void EnableIssue(IssueInfo issueInfo, bool enable, bool all)
		{
			currentReport.EnableItem(issueInfo, enable, all);
			foreach (BPAReport report in reportList)
			{
				if (report.ReportType != BPAReportType.SummaryReport && report.ReportType != BPAReportType.DetailedReport)
				{
					report.ReloadReport(dataInfo);
				}
			}
		}

		public void ExportView()
		{
			currentReport.ExportView();
		}

		public void FindNext()
		{
			currentReport.FindNext();
		}

		public void FindPrev()
		{
			currentReport.FindPrev();
		}

		public void PrintScan()
		{
			currentReport.PrintScan();
		}

		public void ShowFind()
		{
			currentReport.ShowFind();
		}

		public void StartFind()
		{
			currentReport.StartFind();
		}

		public void ShowMoreInfo(IssueInfo issueInfo)
		{
			currentReport.ShowMoreInfo(issueInfo);
		}

		public override bool Start()
		{
			if (dataInfo != mainGUI.SelectedScan)
			{
				dataInfo = mainGUI.SelectedScan;
				currentReport = null;
				reportGroup.Controls.Clear();
				foreach (BPAReport report in reportList)
				{
					BPATabPage bPATabPage = new BPATabPage(report.Name, new Point(0, 0), reportGroup.Size, reportGroup);
					bPATabPage.BackColor = Color.White;
					bPATabPage.Controls.Add(report.ParentControl);
					bPATabPage.Tag = report;
					report.ReportPage = bPATabPage;
					report.ReloadReport(dataInfo);
				}
				if (reportList.Count > 0)
				{
					currentIssueReport = (BPAReport)reportList[0];
				}
				foreach (BPAReport report2 in reportList)
				{
					if (report2.NumberOfEntries > 0)
					{
						currentIssueReport = report2;
						break;
					}
					if (!(report2 is BPAIssueListReport))
					{
						break;
					}
				}
				currentReport = currentIssueReport;
				SelectReports(selectIssueReports, null);
				if (dataInfo.Label.Length > 0)
				{
					subtitle.Text = dataInfo.Label;
				}
				else
				{
					subtitle.Text = dataInfo.RunTime.ToString();
				}
			}
			return true;
		}

		private void ReportChanged(object sender, EventArgs e)
		{
			try
			{
				if (reportGroup.SelectedTab != null && !disableReportChange)
				{
					currentReport = (BPAReport)reportGroup.SelectedTab.Tag;
					currentReport.ParentControl.Visible = true;
					if (currentReport is BPAIssueListReport && currentReport.ReportType != BPAReportType.DisabledIssuesReport)
					{
						currentIssueReport = currentReport;
					}
					else if (currentReport is BPATreeReport)
					{
						currentTreeReport = currentReport;
					}
					else
					{
						currentOtherReport = currentReport;
					}
				}
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		private void SelectReports(object sender, EventArgs e)
		{
			try
			{
				selectIssueReports.Checked = selectIssueReports.ContainsControl(sender);
				selectTreeReports.Checked = selectTreeReports.ContainsControl(sender);
				selectOtherReports.Checked = selectOtherReports.ContainsControl(sender);
				reportGroup.SuspendLayout();
				disableReportChange = true;
				reportGroup.TabPages.Clear();
				disableReportChange = false;
				foreach (BPAReport report in reportList)
				{
					if ((report is BPAIssueListReport && report.ReportType != BPAReportType.DisabledIssuesReport && selectIssueReports.Checked) || (report is BPATreeReport && selectTreeReports.Checked) || ((report is BPALogListReport || report.ReportType == BPAReportType.DisabledIssuesReport) && selectOtherReports.Checked))
					{
						reportGroup.TabPages.Add(report.ReportPage);
						if (currentTreeReport == null && selectTreeReports.Checked)
						{
							currentTreeReport = report;
						}
						else if (currentOtherReport == null && selectOtherReports.Checked)
						{
							currentOtherReport = report;
						}
						if (report == currentIssueReport || report == currentTreeReport || report == currentOtherReport)
						{
							reportGroup.SelectedTab = report.ReportPage;
							report.ParentControl.Visible = true;
							currentReport = report;
						}
					}
				}
				reportGroup.ResumeLayout();
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}
	}
}
