using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class MainGUI
	{
		public enum Actions
		{
			None,
			Init,
			CheckVersion,
			StopCheckVersion,
			CheckVersionCompleted,
			ShowVersionUpdate,
			DownloadNewMSI,
			DownloadNewConfig,
			ReloadConfig,
			Exit,
			NewMSIDownloadCompleted,
			NewConfigDownloadCompleted,
			ShowWelcome,
			ShowSelectAnalyzer,
			ShowBrowseDialog,
			ShowScheduleOptions,
			SaveScheduleAndExit,
			ShowCustomScreen,
			StartScan,
			StopScan,
			ScanCompleted,
			SelectScan,
			DeleteScan,
			ShowChangeScanLabel,
			ChangeScanLabel,
			LoadScan,
			LoadScanCompleted,
			ViewScan,
			ShowFind,
			StartFind,
			CancelFind,
			FindNext,
			FindPrev,
			MoreInfo,
			DisableIssueInstance,
			DisableIssueAll,
			EnableIssueInstance,
			EnableIssueAll,
			ExportScan,
			ExportView,
			Import,
			LoadAnalyzers,
			Print,
			Help,
			About,
			WebSite,
			ShowUpdateOption
		}

		public enum ScanStatus
		{
			Pending,
			InProgress,
			CompletedOk,
			CompletedWithWarning,
			CompletedWithError,
			NotStarted,
			Aborted
		}

		private class ControlInfo
		{
			public Control control;

			public Rectangle rect;
		}

		private delegate void EnableLinksCallback(bool enable);

		public const int DefaultComboBoxWidth = 200;

		public const string DefaultFontFamily = "Microsoft Sans Serif";

		public const int DefaultTextBoxWidth = 150;

		public const int DefaultSpacing = 10;

		public const int LinkIndent = 15;

		public const string Button_ReloadConfig = "Reload XML Configuration";

		public const string CopyrightText = "©2006 Microsoft Corporation. All rights reserved.";

		private static AnalyzerCollection analyzerList;

		protected StatusBarPanel statusBarPanelMain;

		protected StatusBarPanel statusBarPanelShort;

		protected StatusBarPanel statusBarPanelRight;

		protected StatusBar mstatusBar;

		protected BPAPictureBox pictureBoxMSLogo;

		protected BPAPictureBox pictureBoxWSSLogo;

		private BPAGradiatedBox gradiatedBoxTopLine;

		private BPALink linkUpdate;

		private BPALink linkWelcome;

		private BPALink linkSelectScan;

		private BPALink linkViewScan;

		private BPALink linkSchedule;

		private BPALink linkHelp;

		private BPALink linkAbout;

		private BPALink linkWebsite;

		private BPALink linkReload;

		private BPALink linkSelectAnalyzer;

		private Icon okIcon;

		private Icon reportIcon;

		private Icon defaultAppIcon;

		private Image exitPic;

		private Image findPic;

		private Image printPic;

		private Image exportPic;

		private Image importPic;

		private Image showPic;

		private Image hidePic;

		private Image arrowPic;

		private Image arrowDisabledPic;

		private Image navBoxPic;

		private Image navBoxDisabledPic;

		private Image bpaLogoPic;

		private Image msLogoPic;

		private Image msLogoDarkPic;

		private Image wssLogoPic;

		private Image selectPic;

		private Image aboutPic;

		private Image linkURLPic;

		private Image linkURLDisabledPic;

		private Image treePic;

		private Image otherPic;

		private Image issuePic;

		private UpdateOption updateOption;

		private CheckForUpdates checkForUpdates;

		private VersionUpdate versionUpdate;

		private DownloadNewVersion downloadNewVersion;

		private Welcome welcome;

		private ScheduleOptions scheduleOptions;

		private SelectAnalyzer selectAnalyzer;

		private StartScan startScan;

		private ScanCompleted scanCompleted;

		private SelectScan selectScan;

		private LoadScan loadScan;

		private ViewScan viewScan;

		private BPALink highlightedLink;

		protected IContainer components;

		protected ResourceManager resources;

		protected Form form;

		protected Panel panelMain;

		protected Panel panelOptions;

		protected Panel panelLeft;

		protected Panel panelUpper;

		protected Panel panelLower;

		protected Panel mcurrentScreen;

		protected Size currentScreenSize;

		protected static Point borderCorner = new Point(15, 20);

		protected static Font mtitleFont = new Font("Microsoft Sans Serif", 18f, FontStyle.Regular, GraphicsUnit.Point, 0);

		protected Size initScreenSize = new Size(0, 0);

		private ExecutionInterface execInterface;

		private DataInfo selectedScan;

		private ConfigurationInfo configInfo;

		private RegistrySettings regSettings;

		private int nextTabIndex;

		private BPACustomizations customizations;

		private bool enableReloadButton;

		private int maxScreenHeight = 600;

		private int maxScreenWidth = 800;

		private string viewScanFile = "";

		private string currentAnalyzer;

		public static AnalyzerCollection Analyzers
		{
			get
			{
				return analyzerList;
			}
		}

		public static Point BorderCornerPoint
		{
			get
			{
				return borderCorner;
			}
		}

		protected CheckForUpdates CheckForUpdatesScreen
		{
			get
			{
				return checkForUpdates;
			}
			set
			{
				checkForUpdates = value;
			}
		}

		public BPACustomizations Customizations
		{
			get
			{
				return customizations;
			}
		}

		public static Font DefaultFont
		{
			get
			{
				return Control.DefaultFont;
			}
		}

		public ExecutionInterface ExecInterface
		{
			get
			{
				return execInterface;
			}
		}

		public Form MainForm
		{
			get
			{
				return form;
			}
		}

		public RegistrySettings RegSettings
		{
			get
			{
				return regSettings;
			}
		}

		public bool ConfigExists
		{
			get
			{
				return configInfo.ConfigVersion.CompareTo(new Version("0.0.0.0")) != 0;
			}
		}

		public ConfigurationInfo ConfigInfo
		{
			get
			{
				return configInfo;
			}
		}

		public static Color DarkGray
		{
			get
			{
				return Color.FromArgb(140, 140, 140);
			}
		}

		protected DownloadNewVersion DownloadNewVersionScreen
		{
			get
			{
				return downloadNewVersion;
			}
			set
			{
				downloadNewVersion = value;
			}
		}

		public virtual int FullHeight
		{
			get
			{
				return maxScreenHeight - panelUpper.Height - panelLower.Height - borderCorner.Y - borderCorner.X;
			}
		}

		public virtual int FullWidth
		{
			get
			{
				return maxScreenWidth - panelOptions.Width - borderCorner.X * 2;
			}
		}

		public static Color Gray
		{
			get
			{
				return Color.FromArgb(204, 204, 204);
			}
		}

		public static Color LightGray
		{
			get
			{
				return Color.FromArgb(241, 241, 241);
			}
		}

		public static Color HiglightBlue
		{
			get
			{
				return Color.FromArgb(131, 166, 244);
			}
		}

		public static Color LightBlue
		{
			get
			{
				return Color.FromArgb(175, 210, 255);
			}
		}

		public static Color DarkBlue
		{
			get
			{
				return Color.FromArgb(0, 70, 213);
			}
		}

		public static Color HeaderColor
		{
			get
			{
				return Color.FromArgb(128, 128, 128);
			}
		}

		public static Color SelectColor
		{
			get
			{
				return Color.FromArgb(255, 245, 206);
			}
		}

		protected internal LoadScan LoadScanScreen
		{
			get
			{
				return loadScan;
			}
			set
			{
				loadScan = value;
			}
		}

		public static Color MediumGray
		{
			get
			{
				return Color.FromArgb(213, 213, 213);
			}
		}

		internal ScanCompleted ScanCompletedScreen
		{
			get
			{
				return scanCompleted;
			}
		}

		internal ScheduleOptions ScheduleOptionsScreen
		{
			get
			{
				return scheduleOptions;
			}
		}

		public DataInfo SelectedScan
		{
			get
			{
				return selectedScan;
			}
			set
			{
				selectedScan = value;
			}
		}

		protected internal SelectScan SelectScanScreen
		{
			get
			{
				return selectScan;
			}
			set
			{
				selectScan = value;
			}
		}

		public int StartingTabIndex
		{
			get
			{
				return nextTabIndex;
			}
		}

		internal StartScan StartScanScreen
		{
			get
			{
				return startScan;
			}
		}

		public static Font TitleFont
		{
			get
			{
				return mtitleFont;
			}
		}

		protected UpdateOption UpdateOptionScreen
		{
			get
			{
				return updateOption;
			}
			set
			{
				updateOption = value;
			}
		}

		protected VersionUpdate VersionUpdateScreen
		{
			get
			{
				return versionUpdate;
			}
			set
			{
				versionUpdate = value;
			}
		}

		protected internal ViewScan ViewScanScreen
		{
			get
			{
				return viewScan;
			}
			set
			{
				viewScan = value;
			}
		}

		internal Welcome WelcomeScreen
		{
			get
			{
				return welcome;
			}
		}

		public Icon OkIcon
		{
			get
			{
				return okIcon;
			}
		}

		public Icon ReportIcon
		{
			get
			{
				return reportIcon;
			}
		}

		public Icon DefaultAppIcon
		{
			get
			{
				return defaultAppIcon;
			}
			set
			{
				defaultAppIcon = value;
			}
		}

		public Image ExitPic
		{
			get
			{
				return exitPic;
			}
		}

		public Image FindPic
		{
			get
			{
				return findPic;
			}
		}

		public Image PrintPic
		{
			get
			{
				return printPic;
			}
		}

		public Image ExportPic
		{
			get
			{
				return exportPic;
			}
		}

		public Image ImportPic
		{
			get
			{
				return importPic;
			}
		}

		public Image ShowPic
		{
			get
			{
				return showPic;
			}
		}

		public Image HidePic
		{
			get
			{
				return hidePic;
			}
		}

		public Image ArrowPic
		{
			get
			{
				return arrowPic;
			}
		}

		public Image ArrowDisabledPic
		{
			get
			{
				return arrowDisabledPic;
			}
		}

		public Image NavBoxPic
		{
			get
			{
				return navBoxPic;
			}
		}

		public Image NavBoxDisabledPic
		{
			get
			{
				return navBoxDisabledPic;
			}
		}

		public Image LinkURLPic
		{
			get
			{
				return linkURLPic;
			}
		}

		public Image LinkURLDisabledPic
		{
			get
			{
				return linkURLDisabledPic;
			}
		}

		public Image BPALogoPic
		{
			get
			{
				return bpaLogoPic;
			}
		}

		public Image MSLogoDarkPic
		{
			get
			{
				return msLogoDarkPic;
			}
		}

		public Image WSSLogoPic
		{
			get
			{
				return wssLogoPic;
			}
		}

		public Image SelectPic
		{
			get
			{
				return selectPic;
			}
		}

		public Image AboutPic
		{
			get
			{
				return aboutPic;
			}
		}

		public Image TreePic
		{
			get
			{
				return treePic;
			}
		}

		public Image OtherPic
		{
			get
			{
				return otherPic;
			}
		}

		public Image IssuePic
		{
			get
			{
				return issuePic;
			}
		}

		public StatusBarPanel StatusBar
		{
			get
			{
				return statusBarPanelRight;
			}
		}

		public Panel CurrentScreen
		{
			get
			{
				return mcurrentScreen;
			}
			set
			{
				mcurrentScreen = value;
			}
		}

		public BPALink LinkSchedule
		{
			get
			{
				return linkSchedule;
			}
			set
			{
				linkSchedule = value;
			}
		}

		public MainGUI(Form form, string[] args, BPACustomizations customizations, ReadOnlyCollection<Analyzer> List)
			: this(form, args, customizations, true, 2, 5)
		{
			analyzerList = new AnalyzerCollection(List);
		}

		public MainGUI(Form form, string[] args, BPACustomizations customizations, bool needInitializeForms)
			: this(form, args, customizations, needInitializeForms, 2, 5)
		{
		}

		public MainGUI(Form form, string[] args, BPACustomizations customizations)
			: this(form, args, customizations, true, 2, 5)
		{
		}

		public MainGUI(Form form, string[] args, BPACustomizations customizations, bool needInitializeForms, int majorVersion, int minorVersion)
		{
			this.form = form;
			this.customizations = customizations;
			Initialize(args, majorVersion, minorVersion, true);
			if (needInitializeForms)
			{
				InitializeForms();
			}
		}

		public virtual void Start()
		{
			TakeAction(Actions.Init, null);
		}

		public void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
		}

		public static void UpdatePluginList()
		{
			PluginsManager<Analyzer> pluginsManager = new PluginsManager<Analyzer>(new string[0]);
			analyzerList = new AnalyzerCollection(pluginsManager.Plugins);
		}

		public virtual void EnableLinks(bool enable)
		{
			if (form.InvokeRequired)
			{
				form.Invoke(new EnableLinksCallback(EnableLinks), enable);
				return;
			}
			linkSelectAnalyzer.Enabled = enable;
			if (linkWelcome != null)
			{
				linkWelcome.Enabled = enable && ConfigExists;
			}
			linkSelectScan.Enabled = enable && ConfigExists;
			linkViewScan.Enabled = enable && selectedScan != null && ConfigExists;
			if (linkSchedule != null)
			{
				linkSchedule.Enabled = enable && ConfigExists;
			}
			if (linkUpdate != null)
			{
				linkUpdate.Enabled = enable;
			}
			foreach (BPAScreen customScreen in customizations.CustomScreens)
			{
				customScreen.EnableLink(enable);
			}
		}

		public void TakeAction(Actions action, object actionInfo)
		{
			TakeAction((int)action, null, actionInfo, "");
		}

		public void TakeAction(Actions action, object actionInfo, string message)
		{
			TakeAction((int)action, null, actionInfo, message);
		}

		public virtual void TakeAction(int action, BPAScreen customScreen, object actionInfo, string message)
		{
			TakeAction((Actions)action, customScreen, actionInfo, message);
		}

		public void TakeAction(Actions action, BPAScreen customScreen, object actionInfo, string message)
		{
			try
			{
				if (message.Length > 0)
				{
					MessageBox.Show(message, customizations.ShortName);
					message = "";
				}
				Output("");
				HighlightLink(action, customScreen);
				BPALink.LinkInfo linkInfo = actionInfo as BPALink.LinkInfo;
				if (linkInfo != null)
				{
					actionInfo = linkInfo.Tag;
				}
				bool flag = false;
				switch (action)
				{
				case Actions.Init:
					foreach (BPAScreen customScreen2 in customizations.CustomScreens)
					{
						customScreen2.Initialize(this);
						customScreen2.Visible = false;
						panelMain.Controls.Add(customScreen2);
					}
					if (linkUpdate != null)
					{
						linkUpdate.Visible = customizations.AllowAutoDownloads && configInfo.DownloadURL.Length > 0;
					}
					if (viewScanFile.Length > 0)
					{
						if (!viewScanFile.StartsWith("\\") && !File.Exists(viewScanFile))
						{
							viewScanFile = string.Format("{0}\\{1}", RegSettings.DataDirectory, viewScanFile);
						}
						if (!File.Exists(viewScanFile))
						{
							TakeAction(Actions.SelectScan, null, BPALoc.Error_FileNotFound);
							break;
						}
						linkInfo = new BPALink.LinkInfo();
						linkInfo.Tag = new DataInfo(execInterface, viewScanFile);
						linkInfo.Key = Keys.None;
						if (!((DataInfo)linkInfo.Tag).Valid)
						{
							TakeAction(Actions.SelectScan, null, BPALoc.Error_FileInvalid);
						}
						else
						{
							TakeAction(Actions.LoadScan, linkInfo);
						}
					}
					else if (selectAnalyzer == null)
					{
						TakeAction(Actions.ShowSelectAnalyzer, null, "");
					}
					else
					{
						if (customScreen == null && customizations.CustomScreens.Count > 0)
						{
							customScreen = (BPAScreen)customizations.CustomScreens[0];
						}
						TakeAction(Actions.ShowCustomScreen, customScreen, null, "");
					}
					break;
				case Actions.ShowUpdateOption:
					EnableLinks(true);
					if (updateOption == null)
					{
						updateOption = new UpdateOption(this);
						updateOption.HelpId = customizations.VersionUpdateHelpId;
						updateOption.Visible = false;
						panelMain.Controls.Add(updateOption);
					}
					if (updateOption.Start())
					{
						ShowNewPanel(updateOption);
					}
					break;
				case Actions.CheckVersion:
					if (configInfo.DownloadURL.Length > 0)
					{
						EnableLinks(false);
						if (checkForUpdates == null)
						{
							checkForUpdates = new CheckForUpdates(this);
							checkForUpdates.HelpId = customizations.CheckForUpdatesHelpId;
							checkForUpdates.Visible = false;
							panelMain.Controls.Add(checkForUpdates);
						}
						if (checkForUpdates.Start())
						{
							ShowNewPanel(checkForUpdates);
						}
					}
					break;
				case Actions.StopCheckVersion:
					if (checkForUpdates != null)
					{
						checkForUpdates.StopCheck();
					}
					break;
				case Actions.CheckVersionCompleted:
					TakeAction(Actions.ShowVersionUpdate, (UpdateInfo)actionInfo);
					break;
				case Actions.ShowVersionUpdate:
					EnableLinks(true);
					if (versionUpdate == null)
					{
						versionUpdate = new VersionUpdate(this);
						versionUpdate.HelpId = customizations.VersionUpdateHelpId;
						versionUpdate.Visible = false;
						panelMain.Controls.Add(versionUpdate);
					}
					if (versionUpdate.Start((UpdateInfo)actionInfo))
					{
						ShowNewPanel(versionUpdate);
					}
					break;
				case Actions.DownloadNewMSI:
				case Actions.DownloadNewConfig:
					EnableLinks(false);
					if (downloadNewVersion == null)
					{
						downloadNewVersion = new DownloadNewVersion(this);
						downloadNewVersion.HelpId = customizations.DownloadNewVersionHelpId;
						downloadNewVersion.Visible = false;
						panelMain.Controls.Add(downloadNewVersion);
					}
					if (downloadNewVersion.Start(action, (UpdateInfo)actionInfo))
					{
						ShowNewPanel(downloadNewVersion);
					}
					break;
				case Actions.NewConfigDownloadCompleted:
					TakeAction(Actions.ShowWelcome, null);
					break;
				case Actions.NewMSIDownloadCompleted:
					downloadNewVersion.StartMSI();
					break;
				case Actions.ReloadConfig:
					ProcessConfiguration(string.Empty);
					break;
				case Actions.Exit:
					Application.Exit();
					break;
				case Actions.ShowWelcome:
					EnableLinks(true);
					if (welcome == null)
					{
						welcome = new Welcome(this);
						welcome.HelpId = customizations.WelcomeHelpId;
						welcome.Visible = false;
						panelMain.Controls.Add(welcome);
					}
					if (welcome.Start())
					{
						ShowNewPanel(welcome);
					}
					break;
				case Actions.ShowSelectAnalyzer:
					if (selectAnalyzer == null)
					{
						selectAnalyzer = new SelectAnalyzer(this);
						selectAnalyzer.HelpId = customizations.WelcomeHelpId;
						selectAnalyzer.Visible = false;
						panelMain.Controls.Add(selectAnalyzer);
					}
					else
					{
						panelOptions.Controls.Clear();
						SetupPanelOptions();
						panelOptions.Parent = panelLeft;
						highlightedLink = linkSelectAnalyzer;
						highlightedLink.Parent = linkSelectAnalyzer.Parent;
						highlightedLink.Highlighted = true;
					}
					EnableLinks(true);
					if (selectAnalyzer.Start())
					{
						ShowNewPanel(selectAnalyzer);
					}
					break;
				case Actions.ShowBrowseDialog:
				{
					EnableLinks(false);
					OpenFileDialog openFileDialog = new OpenFileDialog();
					BPATextBox bPATextBox = (BPATextBox)linkInfo.Tag;
					openFileDialog.FileName = bPATextBox.Text;
					openFileDialog.ShowDialog();
					bPATextBox.Text = openFileDialog.FileName;
					openFileDialog.Dispose();
					EnableLinks(true);
					break;
				}
				case Actions.LoadAnalyzers:
					LoadAnalyzers(currentAnalyzer);
					break;
				case Actions.ShowCustomScreen:
					EnableLinks(true);
					if (customScreen == null)
					{
						TakeAction(Actions.StartScan, null);
					}
					else if (customScreen.Start())
					{
						ShowNewPanel(customScreen);
					}
					break;
				case Actions.ShowScheduleOptions:
					EnableLinks(true);
					if (scheduleOptions == null)
					{
						scheduleOptions = new ScheduleOptions(this);
						scheduleOptions.HelpId = customizations.ScheduleOptionsHelpId;
						scheduleOptions.Visible = false;
						panelMain.Controls.Add(scheduleOptions);
					}
					if (scheduleOptions.Start())
					{
						ShowNewPanel(scheduleOptions);
					}
					break;
				case Actions.SaveScheduleAndExit:
					if (scheduleOptions != null)
					{
						scheduleOptions.SaveAndExit();
					}
					TakeAction(Actions.Exit, null);
					break;
				case Actions.StartScan:
					EnableLinks(false);
					if (startScan == null)
					{
						startScan = new StartScan(this);
						startScan.HelpId = customizations.StartScanHelpId;
						startScan.Visible = false;
						panelMain.Controls.Add(startScan);
					}
					if (startScan.Start())
					{
						ShowNewPanel(startScan);
					}
					break;
				case Actions.StopScan:
					startScan.Stop();
					break;
				case Actions.ScanCompleted:
				{
					EnableLinks(true);
					ScanCompleted.CompletionInfo completionInfo = (ScanCompleted.CompletionInfo)actionInfo;
					if (scanCompleted == null)
					{
						scanCompleted = new ScanCompleted(this);
						scanCompleted.HelpId = customizations.ScanCompletedHelpId;
						scanCompleted.Visible = false;
						panelMain.Controls.Add(scanCompleted);
					}
					if (scanCompleted.Start(completionInfo))
					{
						ShowNewPanel(scanCompleted);
					}
					break;
				}
				case Actions.SelectScan:
					EnableLinks(true);
					if (selectScan == null)
					{
						selectScan = new SelectScan(this);
						selectScan.HelpId = customizations.SelectScanHelpId;
						selectScan.Visible = false;
						panelMain.Controls.Add(selectScan);
					}
					if (selectScan.Start())
					{
						ShowNewPanel(selectScan);
					}
					break;
				case Actions.DeleteScan:
					if ((DataInfo)linkInfo.Tag == selectedScan)
					{
						selectedScan = null;
					}
					EnableLinks(true);
					selectScan.DeleteScan((DataInfo)linkInfo.Tag);
					break;
				case Actions.ShowChangeScanLabel:
					EnableLinks(true);
					selectScan.ShowChangeScanLabel((DataInfo)linkInfo.Tag);
					break;
				case Actions.ChangeScanLabel:
					EnableLinks(true);
					selectScan.ChangeScanLabel((DataInfo)linkInfo.Tag);
					break;
				case Actions.LoadScan:
				{
					EnableLinks(false);
					if (loadScan == null)
					{
						loadScan = new LoadScan(this);
						loadScan.HelpId = customizations.LoadScanHelpId;
						loadScan.Visible = false;
						panelMain.Controls.Add(loadScan);
					}
					Keys keyPressed = Keys.None;
					if (linkInfo != null)
					{
						selectedScan = (DataInfo)linkInfo.Tag;
						keyPressed = linkInfo.Key;
					}
					else if (actionInfo is DataInfo)
					{
						selectedScan = (DataInfo)actionInfo;
					}
					if (loadScan.Start(selectedScan, keyPressed))
					{
						ShowNewPanel(loadScan);
					}
					break;
				}
				case Actions.LoadScanCompleted:
					TakeAction(Actions.ViewScan, null);
					break;
				case Actions.ViewScan:
					EnableLinks(true);
					if (viewScan == null)
					{
						viewScan = new ViewScan(this);
						viewScan.HelpId = customizations.ViewScanHelpId;
						viewScan.Visible = false;
						panelMain.Controls.Add(viewScan);
					}
					if (viewScan.Start())
					{
						ShowNewPanel(viewScan);
					}
					break;
				case Actions.ShowFind:
					EnableLinks(true);
					viewScan.ShowFind();
					break;
				case Actions.StartFind:
					EnableLinks(true);
					viewScan.StartFind();
					break;
				case Actions.CancelFind:
					EnableLinks(true);
					viewScan.CancelFind();
					break;
				case Actions.FindNext:
					EnableLinks(true);
					viewScan.FindNext();
					break;
				case Actions.FindPrev:
					EnableLinks(true);
					viewScan.FindPrev();
					break;
				case Actions.MoreInfo:
					EnableLinks(true);
					viewScan.ShowMoreInfo((IssueInfo)linkInfo.Tag);
					break;
				case Actions.DisableIssueAll:
					EnableLinks(true);
					viewScan.EnableIssue((IssueInfo)linkInfo.Tag, false, true);
					break;
				case Actions.DisableIssueInstance:
					EnableLinks(true);
					viewScan.EnableIssue((IssueInfo)linkInfo.Tag, false, false);
					break;
				case Actions.EnableIssueAll:
					EnableLinks(true);
					viewScan.EnableIssue((IssueInfo)linkInfo.Tag, true, true);
					break;
				case Actions.EnableIssueInstance:
					EnableLinks(true);
					viewScan.EnableIssue((IssueInfo)linkInfo.Tag, true, false);
					break;
				case Actions.ExportScan:
					SelectScan.ExportScan((DataInfo)linkInfo.Tag, regSettings.ImportExportDirectory);
					break;
				case Actions.ExportView:
					viewScan.ExportView();
					break;
				case Actions.Import:
					selectScan.ImportScan();
					break;
				case Actions.Print:
					viewScan.PrintScan();
					break;
				case Actions.Help:
					Help.ShowHelp(form, ConfigInfo.HelpFile, HelpNavigator.Topic, ((BPAScreen)mcurrentScreen).HelpId);
					break;
				case Actions.About:
				{
					AboutBox aboutBox = new AboutBox(this);
					aboutBox.ShowDialog();
					break;
				}
				case Actions.WebSite:
					CommonData.BrowseURL(customizations.AppWebSite);
					break;
				}
			}
			catch (Exception exception)
			{
				TraceError(exception);
			}
		}

		public virtual void TraceError(Exception exception)
		{
			Output(exception.Message);
			TraceWrite(exception.Message);
			TraceWrite(exception.StackTrace);
		}

		public virtual void TraceWrite(string msg)
		{
			using (StreamWriter streamWriter = new StreamWriter(string.Format("{0}\\UIErrors.txt", regSettings.DataDirectory), true))
			{
				streamWriter.WriteLine(string.Format("{0}: {1}", DateTime.Now, msg));
			}
		}

		protected void ShowNewPanel(Panel newPanel)
		{
			if (mcurrentScreen != null)
			{
				mcurrentScreen.Visible = false;
			}
			if (!panelMain.Controls.Contains(newPanel))
			{
				panelMain.Controls.Add(newPanel);
			}
			newPanel.Visible = true;
			ResizePanel(newPanel);
			mcurrentScreen = newPanel;
			SetFocusToFirstControl();
		}

		protected internal static Color Darken(Color color, byte darkenBy)
		{
			byte g = color.G;
			byte r = color.R;
			byte b = color.B;
			byte b2 = darkenBy;
			if (g < b2)
			{
				b2 = g;
			}
			if (r < b2)
			{
				b2 = r;
			}
			if (b < b2)
			{
				b2 = b;
			}
			return Color.FromArgb(r - b2, g - b2, b - b2);
		}

		internal static int GetControlResizeFlags(Control control)
		{
			PropertyInfo property = control.GetType().GetProperty("ResizeFlags");
			if (property != null)
			{
				return (int)property.GetValue(control, null);
			}
			return 0;
		}

		public void Output(string message)
		{
			if (!form.InvokeRequired)
			{
				statusBarPanelMain.Text = message;
			}
		}

		internal void ReloadConfigInfo()
		{
			if (configInfo == null)
			{
				configInfo = new ConfigurationInfo(execInterface, false);
				if (customizations.DownloadURLStart != null)
				{
					configInfo.DownloadStart = customizations.DownloadURLStart;
				}
				if (customizations.ArticleURLStart != null)
				{
					configInfo.ArticleStart = customizations.ArticleURLStart;
				}
				configInfo.AppendToDownloadURL = customizations.AppendToDownloadURL;
			}
			configInfo.ReloadInfo();
			execInterface.Options.Configuration.Load();
			if (customizations.ProcessConfiguration != null)
			{
				customizations.ProcessConfiguration(execInterface.Options.Configuration);
			}
			if (linkUpdate != null)
			{
				linkUpdate.Visible = customizations.AllowAutoDownloads && configInfo.DownloadURL.Length > 0;
			}
		}

		public static void ResizePanel(Control parentControl)
		{
			ControlInfo controlInfo = new ControlInfo();
			controlInfo.control = parentControl;
			controlInfo.rect = new Rectangle(parentControl.Location, parentControl.Size);
			ResizePanel(controlInfo);
			parentControl.Size = controlInfo.rect.Size;
			parentControl.Location = controlInfo.rect.Location;
		}

		private void Initialize(string[] args, int majorVersion, int minorVersion, bool processConfig)
		{
			bool debug = CommandLineParameters.ArgumentSet(args, "DEBUG,B");
			bool trace = CommandLineParameters.ArgumentSet(args, "TRACE,T");
			if (CommandLineParameters.ArgumentSet(args, "NOCCS"))
			{
				customizations.CheckConfigurationSignature = false;
			}
			enableReloadButton = CommandLineParameters.ArgumentSet(args, "RELOAD,R");
			string argumentValue = CommandLineParameters.GetArgumentValue(args, "MAJOR");
			if (argumentValue.Length > 0)
			{
				majorVersion = int.Parse(argumentValue);
			}
			string argumentValue2 = CommandLineParameters.GetArgumentValue(args, "MINOR");
			if (argumentValue2.Length > 0)
			{
				minorVersion = int.Parse(argumentValue2);
			}
			viewScanFile = CommandLineParameters.GetArgumentValue(args, "VIEW");
			execInterface = new ExecutionInterface(customizations.ApplicationName, majorVersion, minorVersion);
			execInterface.Options.Debug = debug;
			execInterface.Options.Trace = trace;
			maxScreenHeight = SystemInformation.PrimaryMonitorMaximizedWindowSize.Height;
			maxScreenWidth = SystemInformation.PrimaryMonitorMaximizedWindowSize.Width;
			execInterface.Options.Operations = OperationsFlags.Collect | OperationsFlags.Analyze;
			execInterface.Options.CheckAssemblySignature = customizations.CheckAssemblySignature;
			execInterface.Options.CheckConfigurationSignature = customizations.CheckConfigurationSignature;
			if (customizations.ConfigurationPublicKey != null)
			{
				execInterface.Options.ConfigurationPublicKey = customizations.ConfigurationPublicKey;
			}
			if (customizations.LogFunction == null)
			{
				execInterface.Options.Log = Output;
			}
			else
			{
				execInterface.Options.Log = customizations.LogFunction;
			}
			regSettings = new RegistrySettings(execInterface, customizations, "");
			if (processConfig)
			{
				ProcessConfiguration(string.Empty);
			}
		}

		private ResourceManager MainResources()
		{
			ResourceManager resourceManager = null;
			try
			{
				resourceManager = new ResourceManager("MainGUI", Assembly.GetExecutingAssembly());
				Image image = (Image)resourceManager.GetObject("mslogo.gif");
				return resourceManager;
			}
			catch
			{
				return new ResourceManager(typeof(MainGUI));
			}
		}

		protected virtual void InitializeForms()
		{
			resources = MainResources();
			components = new Container();
			currentScreenSize = new Size(1024, 768);
			CommonData.LoadImages();
			LoadImages();
			Panel panel = CreateUpperPanel();
			Panel panel2 = CreateLowerPanel();
			Panel panel3 = CreateLeftPanel();
			BPAPictureBox bPAPictureBox = new BPAPictureBox(DarkGray, new Point(panel3.Right, panel3.Top), new Size(1, currentScreenSize.Height - panel.Height - panel2.Height), null);
			bPAPictureBox.Dock = DockStyle.Left;
			bPAPictureBox.TabStop = false;
			panelMain = new Panel();
			panelMain.Dock = DockStyle.Fill;
			panelMain.BackColor = Color.White;
			form.Icon = defaultAppIcon;
			form.BackColor = SystemColors.Control;
			form.Size = regSettings.ScreenRectangle.Size;
			form.Location = regSettings.ScreenRectangle.Location;
			form.WindowState = regSettings.ScreenState;
			form.Text = customizations.LongNameStart + " " + customizations.LongNameEnd;
			form.Resize += MainGUI_Resize;
			form.Move += MainGUI_Move;
			form.Controls.Add(panelMain);
			form.Controls.Add(bPAPictureBox);
			form.Controls.Add(panel3);
			form.Controls.Add(panel);
			form.Controls.Add(panel2);
			ViewScan.LoadReports(this);
		}

		protected virtual Panel CreateLeftPanel()
		{
			panelLeft = new Panel();
			panelLeft.BackColor = LightBlue;
			panelLeft.Location = new Point(0, 0);
			panelLeft.Dock = DockStyle.Left;
			panelLeft.Width = 200;
			panelLeft.TabStop = false;
			panelOptions = new Panel();
			panelOptions.BackColor = Color.White;
			panelOptions.Width = 180;
			panelOptions.TabStop = false;
			panelOptions.BorderStyle = BorderStyle.FixedSingle;
			panelOptions.Location = new Point(5, 15);
			panelOptions.AutoSize = true;
			SetupPanelOptions();
			panelLeft.Controls.Add(panelOptions);
			panelOptions.Parent = panelLeft;
			return panelLeft;
		}

		protected void SetupPanelOptions()
		{
			Point location = new Point(10, 30);
			Label label = new Label();
			label.Dock = DockStyle.Top;
			label.BackColor = DarkBlue;
			label.ForeColor = Color.White;
			label.Text = "Options";
			panelOptions.Controls.Add(label);
			if (linkSelectAnalyzer != null)
			{
				linkSelectAnalyzer.Dispose();
				linkSelectAnalyzer = null;
			}
			linkSelectAnalyzer = new BPALink(this, Actions.ShowSelectAnalyzer, true, BPALoc.Button_SelectAnalyzer, navBoxPic, location, panelOptions.Width - 20, panelOptions);
			linkSelectAnalyzer.SetTabIndex(nextTabIndex++);
			location = Navigate.Below(linkSelectAnalyzer, 0.2f);
			foreach (BPAScreen customScreen in customizations.CustomScreens)
			{
				if (customScreen.LinkName != null)
				{
					customScreen.Link = new BPALink(this, Actions.ShowCustomScreen, true, customScreen.LinkName, navBoxPic, location, panelOptions.Width - 20, panelOptions);
					customScreen.Link.CustomScreen = customScreen;
					customScreen.Link.SetTabIndex(nextTabIndex++);
					location = Navigate.Below(customScreen.Link, 0.2f);
				}
			}
			linkSelectScan = new BPALink(this, Actions.SelectScan, true, customizations.SelectScanLink, navBoxPic, location, panelOptions.Width - 20, panelOptions);
			linkSelectScan.SetTabIndex(nextTabIndex++);
			location = Navigate.Below(linkSelectScan, 0.2f);
			linkViewScan = new BPALink(this, Actions.ViewScan, true, BPALoc.Button_ViewScan, navBoxPic, location, panelOptions.Width - 20, panelOptions);
			linkViewScan.SetTabIndex(nextTabIndex++);
			location = Navigate.Below(linkViewScan, 0.2f);
			if (enableReloadButton)
			{
				linkReload = new BPALink(this, Actions.ReloadConfig, true, "Reload XML Configuration", navBoxPic, location, panelOptions.Width - 20, panelOptions);
				linkReload.SetTabIndex(nextTabIndex++);
				location = Navigate.Below(linkReload, 0.2f);
			}
			BPALabel bPALabel = new BPALabel(BPALoc.Label_SeeAlso, location, panelOptions.Width - 20, panelOptions);
			bPALabel.TabIndex = nextTabIndex++;
			bPALabel.Font = new Font(DefaultFont, FontStyle.Bold);
			location = Navigate.Below(bPALabel, 0.6f);
			bPALabel.Visible = false;
			linkHelp = new BPALink(this, Actions.Help, true, BPALoc.Button_Help(customizations.ShortName), navBoxPic, location, panelOptions.Width - 20, panelOptions);
			linkHelp.SetTabIndex(nextTabIndex++);
			linkHelp.Visible = false;
			location = Navigate.Below(linkHelp, 0.2f);
			linkAbout = new BPALink(this, Actions.About, true, BPALoc.Button_About(customizations.ShortName), navBoxPic, location, panelOptions.Width - 20, panelOptions);
			linkAbout.SetTabIndex(nextTabIndex++);
			location = Navigate.Below(linkAbout, 0.2f);
			linkAbout.Visible = false;
			if (customizations.Button_AppWebSite != null)
			{
				linkWebsite = new BPALink(this, Actions.WebSite, true, customizations.Button_AppWebSite, navBoxPic, location, panelOptions.Width - 20, panelOptions);
				linkWebsite.SetTabIndex(nextTabIndex++);
				linkWebsite.Visible = false;
				location = Navigate.Below(linkWebsite, 0.2f);
			}
			linkUpdate = new BPALink(this, Actions.ShowUpdateOption, true, BPALoc.Label_NVTitle(customizations.ShortName), navBoxPic, location, panelOptions.Width - 20, panelOptions);
			linkUpdate.SetTabIndex(nextTabIndex++);
			location = Navigate.Below(linkUpdate, 0.2f);
			linkUpdate.Visible = false;
		}

		protected virtual Panel CreateLowerPanel()
		{
			panelLower = new Panel();
			panelLower.BackColor = DarkBlue;
			panelLower.Dock = DockStyle.Bottom;
			panelLower.TabStop = false;
			statusBarPanelRight = new StatusBarPanel();
			statusBarPanelRight.Width = 50;
			statusBarPanelShort = new StatusBarPanel();
			statusBarPanelShort.Width = 100;
			statusBarPanelMain = new StatusBarPanel();
			statusBarPanelMain.Width = currentScreenSize.Width - statusBarPanelShort.Width - statusBarPanelRight.Width;
			mstatusBar = new StatusBar();
			mstatusBar.Dock = DockStyle.Top;
			mstatusBar.Left = 0;
			mstatusBar.Panels.AddRange(new StatusBarPanel[3]
			{
				statusBarPanelMain,
				statusBarPanelShort,
				statusBarPanelRight
			});
			mstatusBar.ShowPanels = true;
			mstatusBar.Size = new Size(currentScreenSize.Width, 22);
			mstatusBar.SizingGrip = false;
			mstatusBar.TabStop = false;
			panelLower.Controls.Add(mstatusBar);
			pictureBoxMSLogo = new BPAPictureBox(DarkBlue, new Point(0, 0), new Size(currentScreenSize.Width, msLogoPic.Height + 2), panelLower);
			pictureBoxMSLogo.Dock = DockStyle.Top;
			pictureBoxMSLogo.Paint += CopyRightLogoPaint;
			BPAPictureBox bPAPictureBox = new BPAPictureBox(DarkBlue, new Point(0, 0), new Size(currentScreenSize.Width, 1), panelLower);
			bPAPictureBox.Dock = DockStyle.Top;
			panelLower.Size = new Size(currentScreenSize.Width, mstatusBar.Height + pictureBoxMSLogo.Height + bPAPictureBox.Height);
			return panelLower;
		}

		protected virtual Panel CreateUpperPanel()
		{
			panelUpper = new Panel();
			panelUpper.Dock = DockStyle.Top;
			panelUpper.Location = new Point(0, 0);
			panelUpper.TabStop = false;
			panelUpper.BackColor = DarkBlue;
			panelUpper.BorderStyle = BorderStyle.FixedSingle;
			BPAGradiatedBox bPAGradiatedBox = new BPAGradiatedBox(LightBlue, DarkBlue, new Point(0, 0), new Size(currentScreenSize.Width, 16), panelUpper);
			bPAGradiatedBox.Dock = DockStyle.Top;
			pictureBoxWSSLogo = new BPAPictureBox(LightBlue, new Point(0, 0), new Size(currentScreenSize.Width, 40), panelUpper);
			pictureBoxWSSLogo.Dock = DockStyle.Top;
			pictureBoxWSSLogo.Paint += BPALogoPaint;
			gradiatedBoxTopLine = new BPAGradiatedBox(LightBlue, DarkBlue, new Point(0, 0), new Size(currentScreenSize.Width, 16), panelUpper);
			gradiatedBoxTopLine.Dock = DockStyle.Top;
			panelUpper.Size = new Size(currentScreenSize.Width, gradiatedBoxTopLine.Height + pictureBoxWSSLogo.Height + bPAGradiatedBox.Height);
			return panelUpper;
		}

		protected virtual void ProcessConfiguration(string fileName)
		{
			try
			{
				if (string.IsNullOrEmpty(fileName))
				{
					execInterface.SetDefaultConfigurationFileName();
				}
				else
				{
					execInterface.Options.Configuration.FileName = fileName;
				}
				ValidateConfigurations validateConfigurations = new ValidateConfigurations(execInterface);
				if (!validateConfigurations.Validate(execInterface.Options.Configuration.FileName))
				{
					throw new ArgumentException(validateConfigurations.ValidationError);
				}
				ReloadConfigInfo();
			}
			catch (ArgumentException ex)
			{
				TraceWrite(ex.Message);
				MessageBox.Show(ex.Message, customizations.ShortName);
				throw;
			}
			catch (Exception ex2)
			{
				TraceWrite(ex2.Message);
				MessageBox.Show(BPALoc.Label_NVConfigMiscError(ex2.Message), customizations.ShortName);
				throw;
			}
		}

		private void HighlightLink(Actions action, BPAScreen customScreen)
		{
			if (highlightedLink != null)
			{
				highlightedLink.Highlighted = false;
			}
			switch (action)
			{
			case Actions.Init:
			case Actions.CheckVersion:
			case Actions.CheckVersionCompleted:
			case Actions.ShowVersionUpdate:
			case Actions.DownloadNewMSI:
			case Actions.DownloadNewConfig:
			case Actions.NewMSIDownloadCompleted:
			case Actions.NewConfigDownloadCompleted:
			case Actions.ShowWelcome:
				highlightedLink = linkWelcome;
				break;
			case Actions.ShowSelectAnalyzer:
				highlightedLink = linkSelectAnalyzer;
				break;
			case Actions.ShowScheduleOptions:
				highlightedLink = linkSchedule;
				break;
			case Actions.SelectScan:
			case Actions.DeleteScan:
			case Actions.ShowChangeScanLabel:
			case Actions.ChangeScanLabel:
				highlightedLink = linkSelectScan;
				break;
			case Actions.StartScan:
			case Actions.StopScan:
			case Actions.ScanCompleted:
			case Actions.LoadScan:
			case Actions.LoadScanCompleted:
			case Actions.ViewScan:
			case Actions.ShowFind:
			case Actions.StartFind:
			case Actions.CancelFind:
			case Actions.FindNext:
			case Actions.FindPrev:
			case Actions.MoreInfo:
			case Actions.DisableIssueInstance:
			case Actions.DisableIssueAll:
			case Actions.EnableIssueInstance:
			case Actions.EnableIssueAll:
				highlightedLink = linkViewScan;
				break;
			default:
			{
				int num = customizations.CustomScreens.IndexOf(customScreen);
				while (num >= 0 && ((BPAScreen)customizations.CustomScreens[num]).Link == null)
				{
					num--;
				}
				if (num == -1)
				{
					highlightedLink = linkWelcome;
				}
				else
				{
					highlightedLink = ((BPAScreen)customizations.CustomScreens[num]).Link;
				}
				break;
			}
			}
			if (highlightedLink != null)
			{
				highlightedLink.Highlighted = true;
			}
		}

		public void RunAnalyzer(object sender, RunAnalyzerEventArgs e)
		{
			currentAnalyzer = e.AnalyzerName;
			TakeAction(Actions.LoadAnalyzers, null, null, "");
		}

		private void LoadAnalyzers(string analyzerName)
		{
			try
			{
				Analyzer analyzer = analyzerList[analyzerName];
				if (analyzer.Customizations == null)
				{
					analyzer.Initialize();
					analyzer.InitializeAnalyzerCustomizations();
					analyzer.CommonGUI = this;
				}
				form.Controls.Clear();
				execInterface.Dispose();
				execInterface = null;
				if (welcome != null)
				{
					welcome.Dispose();
					welcome = null;
				}
				if (viewScan != null)
				{
					viewScan.Dispose();
					viewScan = null;
				}
				if (loadScan != null)
				{
					loadScan.Dispose();
					loadScan = null;
				}
				customizations = analyzer.Customizations;
				Initialize(analyzer.Args, 2, 5, false);
				InitializeForms();
				DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins"));
				FileInfo[] files = directoryInfo.GetFiles(analyzer.ConfigurationFileName, SearchOption.TopDirectoryOnly);
				if (files.Length > 0 && files[0].Exists)
				{
					configInfo = null;
					ProcessConfiguration(files[0].FullName);
				}
				TakeAction(Actions.Init, null);
			}
			catch (Exception ex)
			{
				TraceWrite(ex.Message);
			}
		}

		protected virtual void LoadImages()
		{
			Size size = new Size(16, 16);
			ResourceManager resourceManager = MainResources();
			okIcon = new Icon((Icon)resourceManager.GetObject("ok.ico"), size);
			reportIcon = new Icon((Icon)resourceManager.GetObject("report.ico"), size);
			if (defaultAppIcon == null)
			{
				defaultAppIcon = (Icon)resourceManager.GetObject("defaultapp.ico");
			}
			exitPic = new Icon((Icon)resourceManager.GetObject("exit.ico"), size).ToBitmap();
			linkURLPic = new Icon((Icon)resourceManager.GetObject("linkurl.ico"), size).ToBitmap();
			linkURLDisabledPic = new Icon((Icon)resourceManager.GetObject("linkurl_disabled.ico"), size).ToBitmap();
			treePic = new Icon((Icon)resourceManager.GetObject("treeview.ico"), size).ToBitmap();
			otherPic = new Icon((Icon)resourceManager.GetObject("logging.ico"), size).ToBitmap();
			issuePic = new Icon((Icon)resourceManager.GetObject("RenderTasks.ico"), size).ToBitmap();
			printPic = (Image)resourceManager.GetObject("print.gif");
			exportPic = (Image)resourceManager.GetObject("export.gif");
			importPic = (Image)resourceManager.GetObject("import.gif");
			findPic = (Image)resourceManager.GetObject("find.gif");
			showPic = (Image)resourceManager.GetObject("show.gif");
			hidePic = (Image)resourceManager.GetObject("hide.gif");
			arrowPic = (Image)resourceManager.GetObject("arrow.gif");
			arrowDisabledPic = (Image)resourceManager.GetObject("arrow_disabled.gif");
			navBoxPic = (Image)resourceManager.GetObject("navbox.gif");
			navBoxDisabledPic = (Image)resourceManager.GetObject("navbox_disabled.gif");
			msLogoPic = (Image)resourceManager.GetObject("mslogo.gif");
			bpaLogoPic = (Image)resourceManager.GetObject("BPALogo.gif");
			msLogoDarkPic = (Image)resourceManager.GetObject("mslogodark.gif");
			wssLogoPic = (Image)resourceManager.GetObject("wsslogo.gif");
			selectPic = (Image)resourceManager.GetObject("select.gif");
			aboutPic = (Image)resourceManager.GetObject("about.png");
		}

		private static void ResizePanel(ControlInfo parentControlInfo)
		{
			int num = BorderCornerPoint.X;
			if (((uint)GetControlResizeFlags(parentControlInfo.control) & 0x10u) != 0)
			{
				num = 0;
			}
			ArrayList arrayList = new ArrayList();
			foreach (Control control in parentControlInfo.control.Controls)
			{
				ControlInfo controlInfo = new ControlInfo();
				controlInfo.control = control;
				PropertyInfo property = control.GetType().GetProperty("OrigRect");
				if (property != null)
				{
					controlInfo.rect = (Rectangle)property.GetValue(control, null);
				}
				else if (control.Tag != null)
				{
					controlInfo.rect = (Rectangle)control.Tag;
				}
				else
				{
					controlInfo.rect = new Rectangle(control.Location, control.Size);
				}
				arrayList.Add(controlInfo);
			}
			SortedList sortedList = new SortedList();
			for (int i = 0; i < arrayList.Count; i++)
			{
				ControlInfo controlInfo2 = (ControlInfo)arrayList[i];
				sortedList.Add(controlInfo2.rect.Top.ToString("0000") + i.ToString("00"), controlInfo2);
			}
			ArrayList arrayList2 = new ArrayList();
			SortedList sortedList2 = null;
			int num2 = 0;
			for (int j = 0; j < sortedList.Count; j++)
			{
				ControlInfo controlInfo3 = (ControlInfo)sortedList.GetByIndex(j);
				if (controlInfo3.rect.Top >= num2)
				{
					if (sortedList2 != null)
					{
						arrayList2.Add(sortedList2);
					}
					sortedList2 = new SortedList();
				}
				sortedList2.Add(controlInfo3.rect.Left.ToString("0000") + j.ToString("00"), controlInfo3);
				num2 = controlInfo3.rect.Bottom;
			}
			if (sortedList2 != null)
			{
				arrayList2.Add(sortedList2);
			}
			int num3 = 0;
			int num4 = 0;
			foreach (SortedList item in arrayList2)
			{
				Rectangle rect = ((ControlInfo)item.GetByIndex(0)).rect;
				Rectangle rect2 = ((ControlInfo)item.GetByIndex(item.Count - 1)).rect;
				int num5 = rect2.Right - (parentControlInfo.rect.Width - num);
				int num6 = rect.Left - num;
				if (num6 < 0)
				{
					num6 = 0;
				}
				int num7 = 0;
				int num8 = 0;
				if (num5 <= 0)
				{
					num5 = 0;
				}
				if (num6 >= num5)
				{
					num8 = num5;
					num5 = 0;
				}
				else
				{
					num8 = num6;
					num7 = (num5 - num6) / item.Count;
				}
				int num9 = 9999;
				int num10 = 0;
				int num11 = 0;
				foreach (ControlInfo value in item.Values)
				{
					int controlResizeFlags = GetControlResizeFlags(value.control);
					value.rect.X -= num8;
					value.rect.Y += num3;
					if (value.rect.Top < num9)
					{
						num9 = value.rect.Top;
					}
					if (value.rect.Bottom > num10)
					{
						num10 = value.rect.Bottom;
					}
					if (num7 > 0 && (controlResizeFlags & 2) == 0)
					{
						int width = value.rect.Width;
						value.rect.Width -= num7;
						if (value.rect.Width < 30 && width >= 30)
						{
							value.rect.Width = 30;
						}
						num8 += width - value.rect.Width;
					}
					MethodInfo method = value.control.GetType().GetMethod("GetSizeToFitSize");
					if (method != null)
					{
						object[] parameters = new object[1]
						{
							value.rect.Size
						};
						value.rect.Height = ((Size)method.Invoke(value.control, parameters)).Height;
					}
					else if ((value.control.GetType().IsSubclassOf(typeof(Panel)) || value.control.GetType() == typeof(BPAGroupBox)) && (controlResizeFlags & 8) == 0)
					{
						ResizePanel(value);
					}
					if (((uint)controlResizeFlags & 4u) != 0)
					{
						value.rect.Height = parentControlInfo.rect.Height - value.rect.Top - num;
					}
					if (value.rect.Bottom > num11)
					{
						num11 = value.rect.Bottom;
					}
				}
				foreach (ControlInfo value2 in item.Values)
				{
					value2.rect.Y = num9 + (num11 - num9 - value2.rect.Height) / 2;
				}
				num3 += num11 - num10;
				num4 = num11;
			}
			int controlResizeFlags2 = GetControlResizeFlags(parentControlInfo.control);
			if ((controlResizeFlags2 & 1) == 0)
			{
				parentControlInfo.rect.Height = num4 + num;
			}
			bool flag = true;
			if (parentControlInfo.control.GetType().IsSubclassOf(typeof(Panel)))
			{
				Panel panel = (Panel)parentControlInfo.control;
				if (panel.AutoScroll && panel.Height < num4)
				{
					panel.AutoScroll = false;
					parentControlInfo.rect.Width -= SystemInformation.VerticalScrollBarWidth;
					ResizePanel(parentControlInfo);
					parentControlInfo.rect.Width += SystemInformation.VerticalScrollBarWidth;
					panel.AutoScroll = true;
					flag = false;
				}
			}
			if (!flag)
			{
				return;
			}
			parentControlInfo.control.SuspendLayout();
			Point autoScrollPosition = new Point(0, 0);
			if (parentControlInfo.control.GetType().IsSubclassOf(typeof(Panel)))
			{
				autoScrollPosition = ((Panel)parentControlInfo.control).AutoScrollPosition;
				((Panel)parentControlInfo.control).AutoScrollPosition = new Point(0, 0);
			}
			foreach (ControlInfo item2 in arrayList)
			{
				item2.control.SuspendLayout();
				item2.control.Size = item2.rect.Size;
				item2.control.Location = item2.rect.Location;
				item2.control.ResumeLayout();
			}
			if (parentControlInfo.control.GetType().IsSubclassOf(typeof(Panel)))
			{
				((Panel)parentControlInfo.control).AutoScrollPosition = autoScrollPosition;
			}
			parentControlInfo.control.ResumeLayout();
		}

		public void SetFocusToFirstControl()
		{
			Control control = null;
			foreach (Control control2 in mcurrentScreen.Controls)
			{
				BPALink bPALink = control2 as BPALink;
				if (control2.Enabled && (bPALink != null || (control2.CanFocus && control2.CanSelect)) && (control == null || control.TabIndex > control2.TabIndex))
				{
					control = control2;
				}
			}
			if (control != null)
			{
				BPALink bPALink2 = control as BPALink;
				if (bPALink2 != null)
				{
					bPALink2.SetFocus();
					return;
				}
				control.Focus();
				control.Select();
			}
		}

		private void MainGUI_Move(object sender, EventArgs e)
		{
			MainGUI_Move();
		}

		protected void MainGUI_Move()
		{
			try
			{
				if (form.WindowState == FormWindowState.Normal)
				{
					regSettings.ScreenRectangle = new Rectangle(form.Location, form.Size);
				}
				if (form.WindowState != FormWindowState.Minimized)
				{
					regSettings.ScreenState = form.WindowState;
				}
			}
			catch (Exception exception)
			{
				TraceError(exception);
			}
		}

		private void MainGUI_Resize(object sender, EventArgs e)
		{
			MainGUI_Resize();
		}

		protected void MainGUI_Resize()
		{
			try
			{
				if (form.WindowState == FormWindowState.Normal)
				{
					regSettings.ScreenRectangle = new Rectangle(form.Location, form.Size);
				}
				if (form.WindowState != FormWindowState.Minimized)
				{
					regSettings.ScreenState = form.WindowState;
				}
				int num = form.Width - currentScreenSize.Width;
				currentScreenSize = form.Size;
				if (initScreenSize.Height == 0)
				{
					initScreenSize = panelMain.Size;
				}
				mstatusBar.Width += num;
				if (statusBarPanelMain.Width + num < statusBarPanelMain.MinWidth)
				{
					statusBarPanelMain.Width = statusBarPanelMain.MinWidth;
				}
				else
				{
					statusBarPanelMain.Width += num;
				}
				pictureBoxWSSLogo.Invalidate();
				pictureBoxMSLogo.Invalidate();
				gradiatedBoxTopLine.Invalidate();
				ResizePanel(mcurrentScreen);
			}
			catch (Exception exception)
			{
				TraceError(exception);
			}
		}

		private void CopyRightLogoPaint(object sender, PaintEventArgs e)
		{
			try
			{
				CopyRightLogoPaint((Control)sender, e);
			}
			catch (Exception exception)
			{
				TraceError(exception);
			}
		}

		protected void CopyRightLogoPaint(Control control, PaintEventArgs e)
		{
			try
			{
				Rectangle rect = new Rectangle(0, 0, control.Width, control.Height);
				e.Graphics.FillRectangle(new LinearGradientBrush(rect, LightBlue, DarkBlue, 0f, false), rect);
				int num = (int)e.Graphics.MeasureString("©2006 Microsoft Corporation. All rights reserved.", DefaultFont).Width;
				e.Graphics.DrawString("©2006 Microsoft Corporation. All rights reserved.", DefaultFont, new SolidBrush(Color.White), 5f, 6f);
				int num2 = rect.Width - bpaLogoPic.Width - 5;
				if (num2 < num + 5)
				{
					num2 = num + 5;
				}
			}
			catch (Exception exception)
			{
				TraceError(exception);
			}
		}

		private void BPALogoPaint(object sender, PaintEventArgs e)
		{
			try
			{
				BPALogoPaint((Control)sender, e);
			}
			catch (Exception exception)
			{
				TraceError(exception);
			}
		}

		protected void BPALogoPaint(Control control, PaintEventArgs e)
		{
			try
			{
				Rectangle rect = new Rectangle(0, 0, control.Width, control.Height);
				e.Graphics.FillRectangle(new LinearGradientBrush(rect, DarkBlue, LightBlue, 0f, false), rect);
				int x = 0;
				e.Graphics.DrawImage(bpaLogoPic, x, (control.Height - bpaLogoPic.Height) / 2);
				string longNameStart = customizations.LongNameStart;
				int num = (int)e.Graphics.MeasureString(longNameStart, mtitleFont).Width;
				e.Graphics.DrawString(longNameStart, mtitleFont, new SolidBrush(Color.White), 10 + bpaLogoPic.Width, (control.Height - mtitleFont.Height) / 2);
				string longNameEnd = customizations.LongNameEnd;
				float width = e.Graphics.MeasureString(longNameEnd, mtitleFont).Width;
				e.Graphics.DrawString(longNameEnd, mtitleFont, new SolidBrush(Color.White), 10 + num + bpaLogoPic.Width, (control.Height - mtitleFont.Height) / 2);
			}
			catch (Exception exception)
			{
				TraceError(exception);
			}
		}
	}
}
