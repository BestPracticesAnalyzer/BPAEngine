using System.Collections;
using System.Drawing;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class IssueInfo
	{
		private IssueSeverity severity = IssueSeverity.Unknown;

		private string title = "";

		private string ruleText = "";

		private string description = "";

		private ArrayList groups = new ArrayList();

		private string articleGuid = "";

		private string msgid = "";

		private int sevNum;

		private bool suppressed;

		private object extendedData;

		private static string[] severityString = new string[8]
		{
			CommonLoc.SeverityUnknown,
			CommonLoc.SeverityInfo,
			CommonLoc.SeverityNonDefault,
			CommonLoc.SeverityRecent,
			CommonLoc.SeverityBaseline,
			CommonLoc.SeverityBestPractices,
			CommonLoc.SeverityWarning,
			CommonLoc.SeverityError
		};

		private static string[] severityImageString = new string[8]
		{
			"",
			"info.gif",
			"nondefault.gif",
			"recent.gif",
			"baseline.gif",
			"bestpractice.gif",
			"warning.gif",
			"error.gif"
		};

		private static string[] severityHeaderString = new string[8]
		{
			"",
			CommonLoc.SeverityHeaderInfo,
			CommonLoc.SeverityHeader(CommonLoc.SeverityHeaderNonDefault),
			CommonLoc.SeverityHeader(CommonLoc.SeverityHeaderRecent),
			CommonLoc.SeverityHeader(CommonLoc.SeverityHeaderBaseline),
			CommonLoc.SeverityHeader(CommonLoc.SeverityHeaderBestPractices),
			CommonLoc.SeverityHeader(CommonLoc.SeverityHeaderWarning),
			CommonLoc.SeverityHeader(CommonLoc.SeverityHeaderError)
		};

		private static IssueSeverity[] severityIndexSeverity = new IssueSeverity[8]
		{
			IssueSeverity.Unknown,
			IssueSeverity.Info,
			IssueSeverity.NonDefault,
			IssueSeverity.RecentChange,
			IssueSeverity.Baseline,
			IssueSeverity.BestPractice,
			IssueSeverity.Warning,
			IssueSeverity.Error
		};

		public IssueSeverity Severity
		{
			get
			{
				return severity;
			}
			set
			{
				severity = value;
			}
		}

		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				title = value;
			}
		}

		public string RuleText
		{
			get
			{
				return ruleText;
			}
			set
			{
				ruleText = value;
			}
		}

		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}

		public ArrayList Groups
		{
			get
			{
				return groups;
			}
		}

		public string GroupingName
		{
			get
			{
				if (groups.Count <= 0)
				{
					return string.Empty;
				}
				return ((GroupingClass)groups[groups.Count - 1]).Name;
			}
		}

		public string GroupingType
		{
			get
			{
				if (groups.Count <= 0)
				{
					return string.Empty;
				}
				return ((GroupingClass)groups[groups.Count - 1]).Type;
			}
		}

		public string ArticleGuid
		{
			get
			{
				return articleGuid;
			}
			set
			{
				articleGuid = value;
			}
		}

		public string MsgId
		{
			get
			{
				return msgid;
			}
			set
			{
				msgid = value;
			}
		}

		public int SevNum
		{
			get
			{
				return sevNum;
			}
			set
			{
				sevNum = value;
			}
		}

		public bool Suppressed
		{
			get
			{
				return suppressed;
			}
			set
			{
				suppressed = value;
			}
		}

		public string MsgIdInstance
		{
			get
			{
				return msgid + "\\" + GroupingType + "\\" + GroupingName;
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

		public string SeverityString()
		{
			return SeverityString(severity);
		}

		public Icon SeverityIcon()
		{
			return SeverityIcon(severity);
		}

		public string SeverityImageString()
		{
			return SeverityImageString(severity);
		}

		public string SeverityHeaderString()
		{
			return SeverityHeaderString(severity);
		}

		public static string SeverityString(IssueSeverity severity)
		{
			return severityString[SeverityIndex(severity)];
		}

		public static IssueSeverity SeverityIndexSeverity(int index)
		{
			return severityIndexSeverity[index];
		}

		public static string SeverityIndexSeverityString(int index)
		{
			return SeverityString(SeverityIndexSeverity(index));
		}

		public static string SeverityImageString(IssueSeverity severity)
		{
			return severityImageString[SeverityIndex(severity)];
		}

		public static string SeverityHeaderString(IssueSeverity severity)
		{
			return severityHeaderString[SeverityIndex(severity)];
		}

		public static int SeverityIndex(IssueSeverity severity)
		{
			int result = 0;
			switch (severity)
			{
			case IssueSeverity.Info:
				result = 1;
				break;
			case IssueSeverity.NonDefault:
				result = 2;
				break;
			case IssueSeverity.RecentChange:
				result = 3;
				break;
			case IssueSeverity.Baseline:
				result = 4;
				break;
			case IssueSeverity.BestPractice:
				result = 5;
				break;
			case IssueSeverity.Warning:
				result = 6;
				break;
			case IssueSeverity.Error:
				result = 7;
				break;
			}
			return result;
		}

		public static Icon SeverityIcon(IssueSeverity severity)
		{
			Icon result = null;
			switch (severity)
			{
			case IssueSeverity.Info:
				result = CommonData.InfoIcon;
				break;
			case IssueSeverity.NonDefault:
				result = CommonData.NonDefaultIcon;
				break;
			case IssueSeverity.RecentChange:
				result = CommonData.RecentChangeIcon;
				break;
			case IssueSeverity.Baseline:
				result = CommonData.BaselineIcon;
				break;
			case IssueSeverity.BestPractice:
				result = CommonData.BestPracticeIcon;
				break;
			case IssueSeverity.Warning:
				result = CommonData.WarningIcon;
				break;
			case IssueSeverity.Error:
				result = CommonData.ErrorIcon;
				break;
			}
			return result;
		}

		public override string ToString()
		{
			return string.Format("{0} {1} {2} {3} {4}", SeverityString(), title, GroupingType, GroupingName, description);
		}

		public static IssueSeverity ToSeverity(string val)
		{
			switch (val)
			{
			case "Error":
				return IssueSeverity.Error;
			case "Warning":
				return IssueSeverity.Warning;
			case "BestPractice":
				return IssueSeverity.BestPractice;
			case "Baseline":
				return IssueSeverity.Baseline;
			case "NonDefault":
				return IssueSeverity.NonDefault;
			case "Time":
				return IssueSeverity.RecentChange;
			case "None":
				return IssueSeverity.Info;
			default:
				return IssueSeverity.Unknown;
			}
		}
	}
}
