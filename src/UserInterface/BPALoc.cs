using System.Globalization;
using System.Resources;
using System.Threading;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPALoc
	{
		private static CultureInfo m_Culture;

		private static bool loading;

		private static BPALoc loader;

		private ResourceManager resources;

		public static CultureInfo Culture
		{
			get
			{
				if (m_Culture == null)
				{
					return Thread.CurrentThread.CurrentUICulture;
				}
				return m_Culture;
			}
			set
			{
				m_Culture = value;
			}
		}

		public static string Tool_LongNameStart
		{
			get
			{
				return GetString(Culture, "Tool_LongNameStart");
			}
		}

		public static string Tool_LongNameEnd
		{
			get
			{
				return GetString(Culture, "Tool_LongNameEnd");
			}
		}

		public static string Tool_FullName
		{
			get
			{
				return GetString(Culture, "Tool_FullName");
			}
		}

		public static string Label_SeeAlso
		{
			get
			{
				return GetString(Culture, "Label_SeeAlso");
			}
		}

		public static string Label_SelectAnalyzer
		{
			get
			{
				return GetString(Culture, "Label_SelectAnalyzer");
			}
		}

		public static string Label_LoadAnalyzer
		{
			get
			{
				return GetString(Culture, "Label_LoadAnalyzer");
			}
		}

		public static string Button_Welcome
		{
			get
			{
				return GetString(Culture, "Button_Welcome");
			}
		}

		public static string Button_StartScan
		{
			get
			{
				return GetString(Culture, "Button_StartScan");
			}
		}

		public static string Button_SelectScan
		{
			get
			{
				return GetString(Culture, "Button_SelectScan");
			}
		}

		public static string Button_ViewScan
		{
			get
			{
				return GetString(Culture, "Button_ViewScan");
			}
		}

		public static string Button_ConnectToAD
		{
			get
			{
				return GetString(Culture, "Button_ConnectToAD");
			}
		}

		public static string Button_Schedule
		{
			get
			{
				return GetString(Culture, "Button_Schedule");
			}
		}

		public static string Button_Compare
		{
			get
			{
				return GetString(Culture, "Button_Compare");
			}
		}

		public static string Button_SelectAnalyzer
		{
			get
			{
				return GetString(Culture, "Button_SelectAnalyzer");
			}
		}

		public static string Label_UOTitle
		{
			get
			{
				return GetString(Culture, "Label_UOTitle");
			}
		}

		public static string RadioButton_UOEnableUpdates
		{
			get
			{
				return GetString(Culture, "RadioButton_UOEnableUpdates");
			}
		}

		public static string RadioButton_UODisableUpdates
		{
			get
			{
				return GetString(Culture, "RadioButton_UODisableUpdates");
			}
		}

		public static string Label_UOSQMDescription
		{
			get
			{
				return GetString(Culture, "Label_UOSQMDescription");
			}
		}

		public static string RadioButton_UOEnableSQM
		{
			get
			{
				return GetString(Culture, "RadioButton_UOEnableSQM");
			}
		}

		public static string RadioButton_UODisableSQM
		{
			get
			{
				return GetString(Culture, "RadioButton_UODisableSQM");
			}
		}

		public static string LinkLabel_UOSQMTellMeMore
		{
			get
			{
				return GetString(Culture, "LinkLabel_UOSQMTellMeMore");
			}
		}

		public static string Label_UOCheckVersion
		{
			get
			{
				return GetString(Culture, "Label_UOCheckVersion");
			}
		}

		public static string Label_AConfigVersion
		{
			get
			{
				return GetString(Culture, "Label_AConfigVersion");
			}
		}

		public static string Label_AAppVersion
		{
			get
			{
				return GetString(Culture, "Label_AAppVersion");
			}
		}

		public static string Label_CTitle
		{
			get
			{
				return GetString(Culture, "Label_CTitle");
			}
		}

		public static string Label_CPleaseWait
		{
			get
			{
				return GetString(Culture, "Label_CPleaseWait");
			}
		}

		public static string Label_CEstimatedTime
		{
			get
			{
				return GetString(Culture, "Label_CEstimatedTime");
			}
		}

		public static string LinkLabel_CStopCheck
		{
			get
			{
				return GetString(Culture, "LinkLabel_CStopCheck");
			}
		}

		public static string Label_NVDescNoneFound
		{
			get
			{
				return GetString(Culture, "Label_NVDescNoneFound");
			}
		}

		public static string Label_NVDescCanceled
		{
			get
			{
				return GetString(Culture, "Label_NVDescCanceled");
			}
		}

		public static string Label_NVDescError
		{
			get
			{
				return GetString(Culture, "Label_NVDescError");
			}
		}

		public static string LinkLabel_NVDownloadConfig
		{
			get
			{
				return GetString(Culture, "LinkLabel_NVDownloadConfig");
			}
		}

		public static string LinkLabel_NVUpdateVersion
		{
			get
			{
				return GetString(Culture, "LinkLabel_NVUpdateVersion");
			}
		}

		public static string LinkLabel_NVContinue
		{
			get
			{
				return GetString(Culture, "LinkLabel_NVContinue");
			}
		}

		public static string LinkLabel_NVWelcome
		{
			get
			{
				return GetString(Culture, "LinkLabel_NVWelcome");
			}
		}

		public static string Label_NVNoConfig
		{
			get
			{
				return GetString(Culture, "Label_NVNoConfig");
			}
		}

		public static string Label_DTitle
		{
			get
			{
				return GetString(Culture, "Label_DTitle");
			}
		}

		public static string Label_DPleaseWait
		{
			get
			{
				return GetString(Culture, "Label_DPleaseWait");
			}
		}

		public static string Label_DEstimatedTime
		{
			get
			{
				return GetString(Culture, "Label_DEstimatedTime");
			}
		}

		public static string Label_DVTitle
		{
			get
			{
				return GetString(Culture, "Label_DVTitle");
			}
		}

		public static string Label_DVPleaseWait
		{
			get
			{
				return GetString(Culture, "Label_DVPleaseWait");
			}
		}

		public static string Label_DVEstimatedTime
		{
			get
			{
				return GetString(Culture, "Label_DVEstimatedTime");
			}
		}

		public static string Label_WDesc
		{
			get
			{
				return GetString(Culture, "Label_WDesc");
			}
		}

		public static string LinkLabel_WSelectOptions
		{
			get
			{
				return GetString(Culture, "LinkLabel_WSelectOptions");
			}
		}

		public static string LinkLabel_WSelectScan
		{
			get
			{
				return GetString(Culture, "LinkLabel_WSelectScan");
			}
		}

		public static string Label_PCTitle
		{
			get
			{
				return GetString(Culture, "Label_PCTitle");
			}
		}

		public static string LinkLabel_PCShowAdvancedShow
		{
			get
			{
				return GetString(Culture, "LinkLabel_PCShowAdvancedShow");
			}
		}

		public static string LinkLabel_PCShowAdvancedHide
		{
			get
			{
				return GetString(Culture, "LinkLabel_PCShowAdvancedHide");
			}
		}

		public static string LinkLabel_PCCheck
		{
			get
			{
				return GetString(Culture, "LinkLabel_PCCheck");
			}
		}

		public static string Label_PCUser
		{
			get
			{
				return GetString(Culture, "Label_PCUser");
			}
		}

		public static string Label_PCDomain
		{
			get
			{
				return GetString(Culture, "Label_PCDomain");
			}
		}

		public static string Label_PCPassword
		{
			get
			{
				return GetString(Culture, "Label_PCPassword");
			}
		}

		public static string Label_PTitle
		{
			get
			{
				return GetString(Culture, "Label_PTitle");
			}
		}

		public static string Label_PPleaseWait
		{
			get
			{
				return GetString(Culture, "Label_PPleaseWait");
			}
		}

		public static string Label_PEstimatedTime
		{
			get
			{
				return GetString(Culture, "Label_PEstimatedTime");
			}
		}

		public static string Label_PExCredError
		{
			get
			{
				return GetString(Culture, "Label_PExCredError");
			}
		}

		public static string Label_PADCredError
		{
			get
			{
				return GetString(Culture, "Label_PADCredError");
			}
		}

		public static string Label_PADConnectError
		{
			get
			{
				return GetString(Culture, "Label_PADConnectError");
			}
		}

		public static string Label_PNoOrgFoundError
		{
			get
			{
				return GetString(Culture, "Label_PNoOrgFoundError");
			}
		}

		public static string Label_PNoAGsFoundError
		{
			get
			{
				return GetString(Culture, "Label_PNoAGsFoundError");
			}
		}

		public static string Label_SSTitle
		{
			get
			{
				return GetString(Culture, "Label_SSTitle");
			}
		}

		public static string Label_SSGetScope
		{
			get
			{
				return GetString(Culture, "Label_SSGetScope");
			}
		}

		public static string GroupBox_SSScope
		{
			get
			{
				return GetString(Culture, "GroupBox_SSScope");
			}
		}

		public static string Label_SSSelectType
		{
			get
			{
				return GetString(Culture, "Label_SSSelectType");
			}
		}

		public static string Label_SSSelectTypeOptionsNone
		{
			get
			{
				return GetString(Culture, "Label_SSSelectTypeOptionsNone");
			}
		}

		public static string Label_SSBaseline
		{
			get
			{
				return GetString(Culture, "Label_SSBaseline");
			}
		}

		public static string Label_SSBaselineDesc
		{
			get
			{
				return GetString(Culture, "Label_SSBaselineDesc");
			}
		}

		public static string Label_SSBaselineControls
		{
			get
			{
				return GetString(Culture, "Label_SSBaselineControls");
			}
		}

		public static string Label_SSBaselineTargets
		{
			get
			{
				return GetString(Culture, "Label_SSBaselineTargets");
			}
		}

		public static string Label_SSSpeed
		{
			get
			{
				return GetString(Culture, "Label_SSSpeed");
			}
		}

		public static string Label_SSLabel
		{
			get
			{
				return GetString(Culture, "Label_SSLabel");
			}
		}

		public static string ComboBox_SSSpeedFastLAN
		{
			get
			{
				return GetString(Culture, "ComboBox_SSSpeedFastLAN");
			}
		}

		public static string ComboBox_SSSpeedLAN
		{
			get
			{
				return GetString(Culture, "ComboBox_SSSpeedLAN");
			}
		}

		public static string ComboBox_SSSpeedFastWAN
		{
			get
			{
				return GetString(Culture, "ComboBox_SSSpeedFastWAN");
			}
		}

		public static string ComboBox_SSSpeedWAN
		{
			get
			{
				return GetString(Culture, "ComboBox_SSSpeedWAN");
			}
		}

		public static string LinkLabel_SSStartScan
		{
			get
			{
				return GetString(Culture, "LinkLabel_SSStartScan");
			}
		}

		public static string Label_SSStartError
		{
			get
			{
				return GetString(Culture, "Label_SSStartError");
			}
		}

		public static string Label_IPTitle
		{
			get
			{
				return GetString(Culture, "Label_IPTitle");
			}
		}

		public static string Label_IPEstimatedTime
		{
			get
			{
				return GetString(Culture, "Label_IPEstimatedTime");
			}
		}

		public static string LinkLabel_IPStopScan
		{
			get
			{
				return GetString(Culture, "LinkLabel_IPStopScan");
			}
		}

		public static string Label_IPDetails
		{
			get
			{
				return GetString(Culture, "Label_IPDetails");
			}
		}

		public static string Label_IPTimeUndetermined
		{
			get
			{
				return GetString(Culture, "Label_IPTimeUndetermined");
			}
		}

		public static string Label_IPServerStatusPending
		{
			get
			{
				return GetString(Culture, "Label_IPServerStatusPending");
			}
		}

		public static string Label_IPServerStatusCompleted
		{
			get
			{
				return GetString(Culture, "Label_IPServerStatusCompleted");
			}
		}

		public static string Label_IPServerStatusAborted
		{
			get
			{
				return GetString(Culture, "Label_IPServerStatusAborted");
			}
		}

		public static string Label_IPServerStatusNotStarted
		{
			get
			{
				return GetString(Culture, "Label_IPServerStatusNotStarted");
			}
		}

		public static string Label_IPStartError
		{
			get
			{
				return GetString(Culture, "Label_IPStartError");
			}
		}

		public static string Label_IPServerStatusCompletedWithWarning
		{
			get
			{
				return GetString(Culture, "Label_IPServerStatusCompletedWithWarning");
			}
		}

		public static string Label_IPServerStatusCompletedWithError
		{
			get
			{
				return GetString(Culture, "Label_IPServerStatusCompletedWithError");
			}
		}

		public static string Label_SCTitleCompleted
		{
			get
			{
				return GetString(Culture, "Label_SCTitleCompleted");
			}
		}

		public static string Label_SCTitleAborted
		{
			get
			{
				return GetString(Culture, "Label_SCTitleAborted");
			}
		}

		public static string Label_SCStatusSuccess
		{
			get
			{
				return GetString(Culture, "Label_SCStatusSuccess");
			}
		}

		public static string Label_SCStatusAborted
		{
			get
			{
				return GetString(Culture, "Label_SCStatusAborted");
			}
		}

		public static string LinkLabel_SCViewScan
		{
			get
			{
				return GetString(Culture, "LinkLabel_SCViewScan");
			}
		}

		public static string Label_SCDetails
		{
			get
			{
				return GetString(Culture, "Label_SCDetails");
			}
		}

		public static string Label_SETitle
		{
			get
			{
				return GetString(Culture, "Label_SETitle");
			}
		}

		public static string Label_SEImport
		{
			get
			{
				return GetString(Culture, "Label_SEImport");
			}
		}

		public static string Label_SEArrangeBy
		{
			get
			{
				return GetString(Culture, "Label_SEArrangeBy");
			}
		}

		public static string ComboBox_SEArrangeTask
		{
			get
			{
				return GetString(Culture, "ComboBox_SEArrangeTask");
			}
		}

		public static string ComboBox_SEArrangeScope
		{
			get
			{
				return GetString(Culture, "ComboBox_SEArrangeScope");
			}
		}

		public static string ComboBox_SEArrangeTime
		{
			get
			{
				return GetString(Culture, "ComboBox_SEArrangeTime");
			}
		}

		public static string Label_SEScanEntryFullScope
		{
			get
			{
				return GetString(Culture, "Label_SEScanEntryFullScope");
			}
		}

		public static string LinkLabel_SEScanEntryViewScan
		{
			get
			{
				return GetString(Culture, "LinkLabel_SEScanEntryViewScan");
			}
		}

		public static string LinkLabel_SEScanEntryDeleteScan
		{
			get
			{
				return GetString(Culture, "LinkLabel_SEScanEntryDeleteScan");
			}
		}

		public static string Label_SEScanDeletePrompt
		{
			get
			{
				return GetString(Culture, "Label_SEScanDeletePrompt");
			}
		}

		public static string LinkLabel_SEScanEntryExportScan
		{
			get
			{
				return GetString(Culture, "LinkLabel_SEScanEntryExportScan");
			}
		}

		public static string Label_SEExportTypeXML
		{
			get
			{
				return GetString(Culture, "Label_SEExportTypeXML");
			}
		}

		public static string Label_SEExportTypeSQL
		{
			get
			{
				return GetString(Culture, "Label_SEExportTypeSQL");
			}
		}

		public static string Label_SEExportTypeCSV
		{
			get
			{
				return GetString(Culture, "Label_SEExportTypeCSV");
			}
		}

		public static string Label_SEExportTypeHTM
		{
			get
			{
				return GetString(Culture, "Label_SEExportTypeHTM");
			}
		}

		public static string Label_VSTitle
		{
			get
			{
				return GetString(Culture, "Label_VSTitle");
			}
		}

		public static string Label_VSReport
		{
			get
			{
				return GetString(Culture, "Label_VSReport");
			}
		}

		public static string ComboBox_VSReportCritical
		{
			get
			{
				return GetString(Culture, "ComboBox_VSReportCritical");
			}
		}

		public static string ComboBox_VSReportFull
		{
			get
			{
				return GetString(Culture, "ComboBox_VSReportFull");
			}
		}

		public static string ComboBox_VSReportDetail
		{
			get
			{
				return GetString(Culture, "ComboBox_VSReportDetail");
			}
		}

		public static string ComboBox_VSReportHTML
		{
			get
			{
				return GetString(Culture, "ComboBox_VSReportHTML");
			}
		}

		public static string ComboBox_VSReportBaseline
		{
			get
			{
				return GetString(Culture, "ComboBox_VSReportBaseline");
			}
		}

		public static string ComboBox_VSReportDisabled
		{
			get
			{
				return GetString(Culture, "ComboBox_VSReportDisabled");
			}
		}

		public static string ComboBox_VSReportItemsOfInterest
		{
			get
			{
				return GetString(Culture, "ComboBox_VSReportItemsOfInterest");
			}
		}

		public static string ComboBox_VSReportSummary
		{
			get
			{
				return GetString(Culture, "ComboBox_VSReportSummary");
			}
		}

		public static string ComboBox_VSReportLog
		{
			get
			{
				return GetString(Culture, "ComboBox_VSReportLog");
			}
		}

		public static string ComboBox_VSReportRecent
		{
			get
			{
				return GetString(Culture, "ComboBox_VSReportRecent");
			}
		}

		public static string ComboBox_VSReportNonDefault
		{
			get
			{
				return GetString(Culture, "ComboBox_VSReportNonDefault");
			}
		}

		public static string ComboBox_VSReportDifferences
		{
			get
			{
				return GetString(Culture, "ComboBox_VSReportDifferences");
			}
		}

		public static string ComboBox_VSReportBestPractices
		{
			get
			{
				return GetString(Culture, "ComboBox_VSReportBestPractices");
			}
		}

		public static string Label_VSExport
		{
			get
			{
				return GetString(Culture, "Label_VSExport");
			}
		}

		public static string Label_VSPrint
		{
			get
			{
				return GetString(Culture, "Label_VSPrint");
			}
		}

		public static string Label_VSArrange
		{
			get
			{
				return GetString(Culture, "Label_VSArrange");
			}
		}

		public static string ComboBox_VSArrangeServer
		{
			get
			{
				return GetString(Culture, "ComboBox_VSArrangeServer");
			}
		}

		public static string ComboBox_VSArrangeSeverity
		{
			get
			{
				return GetString(Culture, "ComboBox_VSArrangeSeverity");
			}
		}

		public static string ComboBox_VSArrangeTask
		{
			get
			{
				return GetString(Culture, "ComboBox_VSArrangeTask");
			}
		}

		public static string ComboBox_VSArrangeIssue
		{
			get
			{
				return GetString(Culture, "ComboBox_VSArrangeIssue");
			}
		}

		public static string ComboBox_VSArrangeClass
		{
			get
			{
				return GetString(Culture, "ComboBox_VSArrangeClass");
			}
		}

		public static string Label_VSUnclassifiedHeader
		{
			get
			{
				return GetString(Culture, "Label_VSUnclassifiedHeader");
			}
		}

		public static string Label_VSOrg
		{
			get
			{
				return GetString(Culture, "Label_VSOrg");
			}
		}

		public static string LinkLabel_VSMore
		{
			get
			{
				return GetString(Culture, "LinkLabel_VSMore");
			}
		}

		public static string LinkLabel_VSMoreNonError
		{
			get
			{
				return GetString(Culture, "LinkLabel_VSMoreNonError");
			}
		}

		public static string LinkLabel_VSDisableInstance
		{
			get
			{
				return GetString(Culture, "LinkLabel_VSDisableInstance");
			}
		}

		public static string LinkLabel_VSDisableAll
		{
			get
			{
				return GetString(Culture, "LinkLabel_VSDisableAll");
			}
		}

		public static string LinkLabel_VSEnableInstance
		{
			get
			{
				return GetString(Culture, "LinkLabel_VSEnableInstance");
			}
		}

		public static string LinkLabel_VSEnableAll
		{
			get
			{
				return GetString(Culture, "LinkLabel_VSEnableAll");
			}
		}

		public static string Sample_AG
		{
			get
			{
				return GetString(Culture, "Sample_AG");
			}
		}

		public static string Sample_Server
		{
			get
			{
				return GetString(Culture, "Sample_Server");
			}
		}

		public static string Sample_Description
		{
			get
			{
				return GetString(Culture, "Sample_Description");
			}
		}

		public static string Sample_Title
		{
			get
			{
				return GetString(Culture, "Sample_Title");
			}
		}

		public static string Search_Find
		{
			get
			{
				return GetString(Culture, "Search_Find");
			}
		}

		public static string Search_FindNext
		{
			get
			{
				return GetString(Culture, "Search_FindNext");
			}
		}

		public static string Search_NotFound
		{
			get
			{
				return GetString(Culture, "Search_NotFound");
			}
		}

		public static string Label_GVTitle
		{
			get
			{
				return GetString(Culture, "Label_GVTitle");
			}
		}

		public static string Lavel_GVDesc
		{
			get
			{
				return GetString(Culture, "Lavel_GVDesc");
			}
		}

		public static string Label_SHTitle
		{
			get
			{
				return GetString(Culture, "Label_SHTitle");
			}
		}

		public static string Label_SHDesc
		{
			get
			{
				return GetString(Culture, "Label_SHDesc");
			}
		}

		public static string CheckBox_SHEnable
		{
			get
			{
				return GetString(Culture, "CheckBox_SHEnable");
			}
		}

		public static string Label_SHStartTime
		{
			get
			{
				return GetString(Culture, "Label_SHStartTime");
			}
		}

		public static string Label_SHRunFrequency
		{
			get
			{
				return GetString(Culture, "Label_SHRunFrequency");
			}
		}

		public static string Label_SHRunFrequencyOnce
		{
			get
			{
				return GetString(Culture, "Label_SHRunFrequencyOnce");
			}
		}

		public static string Label_SHRunFrequencyDaily
		{
			get
			{
				return GetString(Culture, "Label_SHRunFrequencyDaily");
			}
		}

		public static string Label_SHRunFrequencyWeekly
		{
			get
			{
				return GetString(Culture, "Label_SHRunFrequencyWeekly");
			}
		}

		public static string Label_SHRunFrequencyMonthly
		{
			get
			{
				return GetString(Culture, "Label_SHRunFrequencyMonthly");
			}
		}

		public static string LinkLabel_SHExit
		{
			get
			{
				return GetString(Culture, "LinkLabel_SHExit");
			}
		}

		public static string LinkLabel_SHSaveAndExit
		{
			get
			{
				return GetString(Culture, "LinkLabel_SHSaveAndExit");
			}
		}

		public static string Label_SHNotAllowedNotLocalAdmin
		{
			get
			{
				return GetString(Culture, "Label_SHNotAllowedNotLocalAdmin");
			}
		}

		public static string Label_SHNotAllowedNoContext
		{
			get
			{
				return GetString(Culture, "Label_SHNotAllowedNoContext");
			}
		}

		public static string Label_PSHDeleteError
		{
			get
			{
				return GetString(Culture, "Label_PSHDeleteError");
			}
		}

		public static string Label_PSHViewError
		{
			get
			{
				return GetString(Culture, "Label_PSHViewError");
			}
		}

		public static string Label_PSHSubmitError
		{
			get
			{
				return GetString(Culture, "Label_PSHSubmitError");
			}
		}

		public static string Label_CSTitle
		{
			get
			{
				return GetString(Culture, "Label_CSTitle");
			}
		}

		public static string Label_CSDesc
		{
			get
			{
				return GetString(Culture, "Label_CSDesc");
			}
		}

		public static string Label_CSLabel
		{
			get
			{
				return GetString(Culture, "Label_CSLabel");
			}
		}

		public static string Label_CSSource
		{
			get
			{
				return GetString(Culture, "Label_CSSource");
			}
		}

		public static string Label_CSSourceObject
		{
			get
			{
				return GetString(Culture, "Label_CSSourceObject");
			}
		}

		public static string Label_CSSourceValues
		{
			get
			{
				return GetString(Culture, "Label_CSSourceValues");
			}
		}

		public static string Label_CSValue
		{
			get
			{
				return GetString(Culture, "Label_CSValue");
			}
		}

		public static string Label_CSTestFile
		{
			get
			{
				return GetString(Culture, "Label_CSTestFile");
			}
		}

		public static string Label_CSTestObjects
		{
			get
			{
				return GetString(Culture, "Label_CSTestObjects");
			}
		}

		public static string LinkLabel_CSViewResults
		{
			get
			{
				return GetString(Culture, "LinkLabel_CSViewResults");
			}
		}

		public static string Button_OK
		{
			get
			{
				return GetString(Culture, "Button_OK");
			}
		}

		public static string Label_SEImportTypeXML
		{
			get
			{
				return GetString(Culture, "Label_SEImportTypeXML");
			}
		}

		public static string Label_SEImportTypeAll
		{
			get
			{
				return GetString(Culture, "Label_SEImportTypeAll");
			}
		}

		public static string Label_VSRegex
		{
			get
			{
				return GetString(Culture, "Label_VSRegex");
			}
		}

		public static string Label_SEDeleteAllScans
		{
			get
			{
				return GetString(Culture, "Label_SEDeleteAllScans");
			}
		}

		public static string LinkLabel_SEViewScan
		{
			get
			{
				return GetString(Culture, "LinkLabel_SEViewScan");
			}
		}

		public static string Label_SSSelectAll
		{
			get
			{
				return GetString(Culture, "Label_SSSelectAll");
			}
		}

		public static string Label_SSUnselectAll
		{
			get
			{
				return GetString(Culture, "Label_SSUnselectAll");
			}
		}

		public static string Error_FileNotFound
		{
			get
			{
				return GetString(Culture, "Error_FileNotFound");
			}
		}

		public static string Error_FileInvalid
		{
			get
			{
				return GetString(Culture, "Error_FileInvalid");
			}
		}

		public static string Error_FileAlreadyExists
		{
			get
			{
				return GetString(Culture, "Error_FileAlreadyExists");
			}
		}

		public static string Label_VSSelectReportType
		{
			get
			{
				return GetString(Culture, "Label_VSSelectReportType");
			}
		}

		public static string Label_VSTreeReports
		{
			get
			{
				return GetString(Culture, "Label_VSTreeReports");
			}
		}

		public static string Label_VSIssueReports
		{
			get
			{
				return GetString(Culture, "Label_VSIssueReports");
			}
		}

		public static string Label_VSOtherReports
		{
			get
			{
				return GetString(Culture, "Label_VSOtherReports");
			}
		}

		public static string ContextMenu_Copy
		{
			get
			{
				return GetString(Culture, "ContextMenu_Copy");
			}
		}

		public static string ContextMenu_Save
		{
			get
			{
				return GetString(Culture, "ContextMenu_Save");
			}
		}

		public static string ContextMenu_SelectAll
		{
			get
			{
				return GetString(Culture, "ContextMenu_SelectAll");
			}
		}

		public static string ContextMenu_Print
		{
			get
			{
				return GetString(Culture, "ContextMenu_Print");
			}
		}

		public static string File_RTFFiles
		{
			get
			{
				return GetString(Culture, "File_RTFFiles");
			}
		}

		public static string File_TextFiles
		{
			get
			{
				return GetString(Culture, "File_TextFiles");
			}
		}

		public static string Title_Main(object arg)
		{
			return GetString(Culture, "Title_Main", arg);
		}

		public static string Button_Help(object arg)
		{
			return GetString(Culture, "Button_Help", arg);
		}

		public static string Button_About(object arg)
		{
			return GetString(Culture, "Button_About", arg);
		}

		public static string Button_Exchange(object arg)
		{
			return GetString(Culture, "Button_Exchange", arg);
		}

		public static string Label_UODescription(object arg)
		{
			return GetString(Culture, "Label_UODescription", arg);
		}

		public static string Label_CDesc(object arg, object arg1)
		{
			return GetString(Culture, "Label_CDesc", arg, arg1);
		}

		public static string Label_NVTitle(object arg)
		{
			return GetString(Culture, "Label_NVTitle", arg);
		}

		public static string Label_NVDescNewVersion(object arg)
		{
			return GetString(Culture, "Label_NVDescNewVersion", arg);
		}

		public static string Label_NVDescNewConfig(object arg)
		{
			return GetString(Culture, "Label_NVDescNewConfig", arg);
		}

		public static string LinkLabel_NVExit(object arg)
		{
			return GetString(Culture, "LinkLabel_NVExit", arg);
		}

		public static string Label_NVConfigMiscError(object arg)
		{
			return GetString(Culture, "Label_NVConfigMiscError", arg);
		}

		public static string Label_DDesc(object arg, object arg1)
		{
			return GetString(Culture, "Label_DDesc", arg, arg1);
		}

		public static string Label_DVDesc(object arg, object arg1)
		{
			return GetString(Culture, "Label_DVDesc", arg, arg1);
		}

		public static string Label_DVDownloadError(object arg)
		{
			return GetString(Culture, "Label_DVDownloadError", arg);
		}

		public static string Label_WTitle(object arg)
		{
			return GetString(Culture, "Label_WTitle", arg);
		}

		public static string Label_PCDesc(object arg)
		{
			return GetString(Culture, "Label_PCDesc", arg);
		}

		public static string Label_PCDC(object arg)
		{
			return GetString(Culture, "Label_PCDC", arg);
		}

		public static string Label_PCCreds(object arg)
		{
			return GetString(Culture, "Label_PCCreds", arg);
		}

		public static string CheckBox_PCADLogon(object arg)
		{
			return GetString(Culture, "CheckBox_PCADLogon", arg);
		}

		public static string CheckBox_PCExLogon(object arg)
		{
			return GetString(Culture, "CheckBox_PCExLogon", arg);
		}

		public static string CheckBox_PCExAGLogon(object arg)
		{
			return GetString(Culture, "CheckBox_PCExAGLogon", arg);
		}

		public static string Label_PDesc(object arg)
		{
			return GetString(Culture, "Label_PDesc", arg);
		}

		public static string Label_SSScopeSummaryOrg(object arg)
		{
			return GetString(Culture, "Label_SSScopeSummaryOrg", arg);
		}

		public static string Label_SSScopeSummaryAG(object arg, object arg1)
		{
			return GetString(Culture, "Label_SSScopeSummaryAG", arg, arg1);
		}

		public static string Label_SSScopeSummaryServer(object arg)
		{
			return GetString(Culture, "Label_SSScopeSummaryServer", arg);
		}

		public static string Label_SSScopeWithOrg(object arg, object arg1)
		{
			return GetString(Culture, "Label_SSScopeWithOrg", arg, arg1);
		}

		public static string Label_SSScopeWithoutOrg(object arg, object arg1)
		{
			return GetString(Culture, "Label_SSScopeWithoutOrg", arg, arg1);
		}

		public static string Label_SSSelectTypeOptions(object arg)
		{
			return GetString(Culture, "Label_SSSelectTypeOptions", arg);
		}

		public static string Label_SSEstimatedTime(object arg)
		{
			return GetString(Culture, "Label_SSEstimatedTime", arg);
		}

		public static string Label_IPPleaseWait(object arg)
		{
			return GetString(Culture, "Label_IPPleaseWait", arg);
		}

		public static string Label_IPTotalServers(object arg)
		{
			return GetString(Culture, "Label_IPTotalServers", arg);
		}

		public static string Label_IPTotalServersPending(object arg)
		{
			return GetString(Culture, "Label_IPTotalServersPending", arg);
		}

		public static string Label_IPTotalServersInProgress(object arg)
		{
			return GetString(Culture, "Label_IPTotalServersInProgress", arg);
		}

		public static string Label_IPTotalServersCompleted(object arg)
		{
			return GetString(Culture, "Label_IPTotalServersCompleted", arg);
		}

		public static string Label_IPTimeInSeconds(object arg)
		{
			return GetString(Culture, "Label_IPTimeInSeconds", arg);
		}

		public static string Label_IPTimeInMinutes(object arg, object arg1)
		{
			return GetString(Culture, "Label_IPTimeInMinutes", arg, arg1);
		}

		public static string Label_IPTimeInHours(object arg, object arg1, object arg2)
		{
			return GetString(Culture, "Label_IPTimeInHours", arg, arg1, arg2);
		}

		public static string Label_IPServerStatusInProgress(object arg)
		{
			return GetString(Culture, "Label_IPServerStatusInProgress", arg);
		}

		public static string Label_SEListTitle(object arg)
		{
			return GetString(Culture, "Label_SEListTitle", arg);
		}

		public static string Label_SETotal(object arg)
		{
			return GetString(Culture, "Label_SETotal", arg);
		}

		public static string Label_SEScanEntryRunTime(object arg)
		{
			return GetString(Culture, "Label_SEScanEntryRunTime", arg);
		}

		public static string Label_SEScanEntryServers(object arg)
		{
			return GetString(Culture, "Label_SEScanEntryServers", arg);
		}

		public static string Label_SEScanEntryFileName(object arg)
		{
			return GetString(Culture, "Label_SEScanEntryFileName", arg);
		}

		public static string Label_SEScanEntryFileSize(object arg)
		{
			return GetString(Culture, "Label_SEScanEntryFileSize", arg);
		}

		public static string Label_SEScanEntryTasks(object arg)
		{
			return GetString(Culture, "Label_SEScanEntryTasks", arg);
		}

		public static string Label_SEScanEntryVersion(object arg)
		{
			return GetString(Culture, "Label_SEScanEntryVersion", arg);
		}

		public static string Label_SEHeaderScope(object arg)
		{
			return GetString(Culture, "Label_SEHeaderScope", arg);
		}

		public static string Label_SEHeaderTask(object arg)
		{
			return GetString(Culture, "Label_SEHeaderTask", arg);
		}

		public static string Label_VSTotalItems(object arg)
		{
			return GetString(Culture, "Label_VSTotalItems", arg);
		}

		public static string Label_VSServer(object arg)
		{
			return GetString(Culture, "Label_VSServer", arg);
		}

		public static string Label_VSAdminGroup(object arg)
		{
			return GetString(Culture, "Label_VSAdminGroup", arg);
		}

		public static string Label_VSADServer(object arg)
		{
			return GetString(Culture, "Label_VSADServer", arg);
		}

		public static string Label_VSRoutingGroup(object arg)
		{
			return GetString(Culture, "Label_VSRoutingGroup", arg);
		}

		public static string Label_VSTasks(object arg)
		{
			return GetString(Culture, "Label_VSTasks", arg);
		}

		public static string Label_VSHeaderTask(object arg)
		{
			return GetString(Culture, "Label_VSHeaderTask", arg);
		}

		public static string Label_VSHeaderServer(object arg)
		{
			return GetString(Culture, "Label_VSHeaderServer", arg);
		}

		public static string Sample_FileName(object arg)
		{
			return GetString(Culture, "Sample_FileName", arg);
		}

		public static string Label_GVPleaseWait(object arg)
		{
			return GetString(Culture, "Label_GVPleaseWait", arg);
		}

		public static string Label_Page(object arg)
		{
			return GetString(Culture, "Label_Page", arg);
		}

		public static string Label_CSSourceCustomer(object arg)
		{
			return GetString(Culture, "Label_CSSourceCustomer", arg);
		}

		public static string Label_IPTimeInMinutesNoSeconds(object arg)
		{
			return GetString(Culture, "Label_IPTimeInMinutesNoSeconds", arg);
		}

		public static string Label_IPTimeInHoursNoSeconds(object arg, object arg1)
		{
			return GetString(Culture, "Label_IPTimeInHoursNoSeconds", arg, arg1);
		}

		public static string Label_SEScanEntryType(object arg)
		{
			return GetString(Culture, "Label_SEScanEntryType", arg);
		}

		public static string Label_SEScanEntryDate(object arg)
		{
			return GetString(Culture, "Label_SEScanEntryDate", arg);
		}

		public static string Label_VSExportError(object arg)
		{
			return GetString(Culture, "Label_VSExportError", arg);
		}

		public static string Label_ConfigFileOld(object arg)
		{
			return GetString(Culture, "Label_ConfigFileOld", arg);
		}

		public static string Label_ConfigFileInvalid(object arg)
		{
			return GetString(Culture, "Label_ConfigFileInvalid", arg);
		}

		protected BPALoc()
		{
			resources = new ResourceManager("obj.BPALoc", GetType().Module.Assembly);
		}

		private static BPALoc GetLoader()
		{
			if (loader == null && !loading)
			{
				lock (typeof(BPALoc))
				{
					if (loader == null && !loading)
					{
						loading = true;
						try
						{
							loader = new BPALoc();
						}
						finally
						{
							loading = false;
						}
					}
				}
			}
			return loader;
		}

		public static string GetString(string name, params object[] args)
		{
			return GetString(Culture, name, args);
		}

		public static string GetString(CultureInfo culture, string name, params object[] args)
		{
			BPALoc bPALoc = GetLoader();
			if (bPALoc == null)
			{
				return null;
			}
			string @string = bPALoc.resources.GetString(name, culture);
			if (args != null && args.Length > 0)
			{
				return string.Format(culture, @string, args);
			}
			return @string;
		}

		public static string GetString(string name)
		{
			return GetString(Culture, name);
		}

		public static string GetString(CultureInfo culture, string name)
		{
			BPALoc bPALoc = GetLoader();
			if (bPALoc == null)
			{
				return null;
			}
			return bPALoc.resources.GetString(name, culture);
		}
	}
}
