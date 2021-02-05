using System.Globalization;
using System.Resources;
using System.Threading;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class CommonLoc
	{
		private static CultureInfo m_Culture;

		private static bool loading;

		private static CommonLoc loader;

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

		public static string Error_Configuration
		{
			get
			{
				return GetString(Culture, "Error_Configuration");
			}
		}

		public static string Error_MultipleOverrideNodes
		{
			get
			{
				return GetString(Culture, "Error_MultipleOverrideNodes");
			}
		}

		public static string Info_StepPreprocessing
		{
			get
			{
				return GetString(Culture, "Info_StepPreprocessing");
			}
		}

		public static string Info_StepCollecting
		{
			get
			{
				return GetString(Culture, "Info_StepCollecting");
			}
		}

		public static string Info_StepPostprocessing
		{
			get
			{
				return GetString(Culture, "Info_StepPostprocessing");
			}
		}

		public static string Error_CustNameMismatch
		{
			get
			{
				return GetString(Culture, "Error_CustNameMismatch");
			}
		}

		public static string Error_BadCategoryParam
		{
			get
			{
				return GetString(Culture, "Error_BadCategoryParam");
			}
		}

		public static string Error_BadScopeParam
		{
			get
			{
				return GetString(Culture, "Error_BadScopeParam");
			}
		}

		public static string Error_BadUserParam
		{
			get
			{
				return GetString(Culture, "Error_BadUserParam");
			}
		}

		public static string Help_Description
		{
			get
			{
				return GetString(Culture, "Help_Description");
			}
		}

		public static string Info_ValidRestrictions
		{
			get
			{
				return GetString(Culture, "Info_ValidRestrictions");
			}
		}

		public static string Error_DupToken
		{
			get
			{
				return GetString(Culture, "Error_DupToken");
			}
		}

		public static string SeverityError
		{
			get
			{
				return GetString(Culture, "SeverityError");
			}
		}

		public static string SeverityWarning
		{
			get
			{
				return GetString(Culture, "SeverityWarning");
			}
		}

		public static string SeverityRecent
		{
			get
			{
				return GetString(Culture, "SeverityRecent");
			}
		}

		public static string SeverityNonDefault
		{
			get
			{
				return GetString(Culture, "SeverityNonDefault");
			}
		}

		public static string SeverityInfo
		{
			get
			{
				return GetString(Culture, "SeverityInfo");
			}
		}

		public static string SeverityBaseline
		{
			get
			{
				return GetString(Culture, "SeverityBaseline");
			}
		}

		public static string SeverityUnknown
		{
			get
			{
				return GetString(Culture, "SeverityUnknown");
			}
		}

		public static string SeverityBestPractices
		{
			get
			{
				return GetString(Culture, "SeverityBestPractices");
			}
		}

		public static string SeverityHeaderError
		{
			get
			{
				return GetString(Culture, "SeverityHeaderError");
			}
		}

		public static string SeverityHeaderWarning
		{
			get
			{
				return GetString(Culture, "SeverityHeaderWarning");
			}
		}

		public static string SeverityHeaderRecent
		{
			get
			{
				return GetString(Culture, "SeverityHeaderRecent");
			}
		}

		public static string SeverityHeaderNonDefault
		{
			get
			{
				return GetString(Culture, "SeverityHeaderNonDefault");
			}
		}

		public static string SeverityHeaderInfo
		{
			get
			{
				return GetString(Culture, "SeverityHeaderInfo");
			}
		}

		public static string SeverityHeaderBaseline
		{
			get
			{
				return GetString(Culture, "SeverityHeaderBaseline");
			}
		}

		public static string SeverityHeaderBestPractices
		{
			get
			{
				return GetString(Culture, "SeverityHeaderBestPractices");
			}
		}

		public static string NoValue
		{
			get
			{
				return GetString(Culture, "NoValue");
			}
		}

		public static string Error_Parameters(object arg)
		{
			return GetString(Culture, "Error_Parameters", arg);
		}

		public static string Error_XMLRead(object arg)
		{
			return GetString(Culture, "Error_XMLRead", arg);
		}

		public static string Error_XMLWrite(object arg)
		{
			return GetString(Culture, "Error_XMLWrite", arg);
		}

		public static string Error_AbortTree(object arg)
		{
			return GetString(Culture, "Error_AbortTree", arg);
		}

		public static string Error_LoadingAssembly(object arg, object arg1, object arg2, object arg3, object arg4)
		{
			return GetString(Culture, "Error_LoadingAssembly", arg, arg1, arg2, arg3, arg4);
		}

		public static string Error_Signature(object arg)
		{
			return GetString(Culture, "Error_Signature", arg);
		}

		public static string Error_RuleFormat(object arg)
		{
			return GetString(Culture, "Error_RuleFormat", arg);
		}

		public static string Error_DispatchingObject(object arg, object arg1)
		{
			return GetString(Culture, "Error_DispatchingObject", arg, arg1);
		}

		public static string Error_ProcessingObject(object arg, object arg1)
		{
			return GetString(Culture, "Error_ProcessingObject", arg, arg1);
		}

		public static string Error_ProcessingSetting(object arg, object arg1, object arg2)
		{
			return GetString(Culture, "Error_ProcessingSetting", arg, arg1, arg2);
		}

		public static string Error_MessageDetailLookup(object arg)
		{
			return GetString(Culture, "Error_MessageDetailLookup", arg);
		}

		public static string Info_SkippingDuplicateObject(object arg)
		{
			return GetString(Culture, "Info_SkippingDuplicateObject", arg);
		}

		public static string Error_DataConversion(object arg, object arg1)
		{
			return GetString(Culture, "Error_DataConversion", arg, arg1);
		}

		public static string Error_Executing(object arg)
		{
			return GetString(Culture, "Error_Executing", arg);
		}

		public static string Error_ExecuteStep(object arg, object arg1)
		{
			return GetString(Culture, "Error_ExecuteStep", arg, arg1);
		}

		public static string Error_TypeNotFound(object arg)
		{
			return GetString(Culture, "Error_TypeNotFound", arg);
		}

		public static string Error_CustomIdNotFound(object arg, object arg1)
		{
			return GetString(Culture, "Error_CustomIdNotFound", arg, arg1);
		}

		public static string Error_CustomTypeNotFound(object arg)
		{
			return GetString(Culture, "Error_CustomTypeNotFound", arg);
		}

		public static string Error_CircularTypeReference(object arg, object arg1)
		{
			return GetString(Culture, "Error_CircularTypeReference", arg, arg1);
		}

		public static string Error_ReferencedFileNotInList(object arg, object arg1, object arg2)
		{
			return GetString(Culture, "Error_ReferencedFileNotInList", arg, arg1, arg2);
		}

		public static string Info_StepStart(object arg)
		{
			return GetString(Culture, "Info_StepStart", arg);
		}

		public static string Info_StepEnd(object arg)
		{
			return GetString(Culture, "Info_StepEnd", arg);
		}

		public static string Info_ObjectTimeout(object arg)
		{
			return GetString(Culture, "Info_ObjectTimeout", arg);
		}

		public static string Error_ObjectTimeout(object arg)
		{
			return GetString(Culture, "Error_ObjectTimeout", arg);
		}

		public static string Error_SettingMaxThreads(object arg)
		{
			return GetString(Culture, "Error_SettingMaxThreads", arg);
		}

		public static string Error_InvalidParam(object arg)
		{
			return GetString(Culture, "Error_InvalidParam", arg);
		}

		public static string Error_BadParam(object arg)
		{
			return GetString(Culture, "Error_BadParam", arg);
		}

		public static string Error_BadRestrictionParam(object arg)
		{
			return GetString(Culture, "Error_BadRestrictionParam", arg);
		}

		public static string Error_BadDCParam(object arg, object arg1)
		{
			return GetString(Culture, "Error_BadDCParam", arg, arg1);
		}

		public static string Error_BadCfgParam(object arg)
		{
			return GetString(Culture, "Error_BadCfgParam", arg);
		}

		public static string Error_BadDataParam(object arg, object arg1)
		{
			return GetString(Culture, "Error_BadDataParam", arg, arg1);
		}

		public static string Warn_MultipleObjects(object arg)
		{
			return GetString(Culture, "Warn_MultipleObjects", arg);
		}

		public static string Warn_MultipleSettings(object arg)
		{
			return GetString(Culture, "Warn_MultipleSettings", arg);
		}

		public static string Warn_MultipleInstances(object arg)
		{
			return GetString(Culture, "Warn_MultipleInstances", arg);
		}

		public static string Help(object arg, object arg1, object arg2, object arg3)
		{
			return GetString(Culture, "Help", arg, arg1, arg2, arg3);
		}

		public static string Help_Example(object arg)
		{
			return GetString(Culture, "Help_Example", arg);
		}

		public static string Info_Valid(object arg, object arg1, object arg2, object arg3)
		{
			return GetString(Culture, "Info_Valid", arg, arg1, arg2, arg3);
		}

		public static string Info_ValidContexts(object arg)
		{
			return GetString(Culture, "Info_ValidContexts", arg);
		}

		public static string Error_ImpersonatingUser(object arg, object arg1)
		{
			return GetString(Culture, "Error_ImpersonatingUser", arg, arg1);
		}

		public static string Error_ThresholdExceeded(object arg, object arg1)
		{
			return GetString(Culture, "Error_ThresholdExceeded", arg, arg1);
		}

		public static string Info_Status(object arg, object arg1)
		{
			return GetString(Culture, "Info_Status", arg, arg1);
		}

		public static string Error_BadFormat(object arg, object arg1)
		{
			return GetString(Culture, "Error_BadFormat", arg, arg1);
		}

		public static string SeverityHeader(object arg)
		{
			return GetString(Culture, "SeverityHeader", arg);
		}

		public static string Error_SQMUpload(object arg, object arg1)
		{
			return GetString(Culture, "Error_SQMUpload", arg, arg1);
		}

		public static string Error_ConfigNotFound(object arg)
		{
			return GetString(Culture, "Error_ConfigNotFound", arg);
		}

		public static string Error_ConfigXml(object arg, object arg1)
		{
			return GetString(Culture, "Error_ConfigXml", arg, arg1);
		}

		public static string Error_ConfigMisc(object arg, object arg1)
		{
			return GetString(Culture, "Error_ConfigMisc", arg, arg1);
		}

		public static string Error_ConfigSignature(object arg)
		{
			return GetString(Culture, "Error_ConfigSignature", arg);
		}

		protected CommonLoc()
		{
			resources = new ResourceManager("obj.CommonLoc", GetType().Module.Assembly);
		}

		private static CommonLoc GetLoader()
		{
			if (loader == null && !loading)
			{
				lock (typeof(CommonLoc))
				{
					if (loader == null && !loading)
					{
						loading = true;
						try
						{
							loader = new CommonLoc();
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
			CommonLoc commonLoc = GetLoader();
			if (commonLoc == null)
			{
				return null;
			}
			string @string = commonLoc.resources.GetString(name, culture);
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
			CommonLoc commonLoc = GetLoader();
			if (commonLoc == null)
			{
				return null;
			}
			return commonLoc.resources.GetString(name, culture);
		}
	}
}
