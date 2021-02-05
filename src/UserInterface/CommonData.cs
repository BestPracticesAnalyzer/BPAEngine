using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class CommonData
	{
		public delegate DataInfo ConstructDataInfoDelegate(ExecutionInterface execInterface, string fileName);

		public const string VersionNone = "0.0.0.0";

		private static Icon infoIcon = null;

		private static Icon nonDefaultIcon = null;

		private static Icon recentChangeIcon = null;

		private static Icon baselineIcon = null;

		private static Icon bestPracticeIcon = null;

		private static Icon warningIcon = null;

		private static Icon errorIcon = null;

		private static Image infoPic = null;

		private static Image nonDefaultPic = null;

		private static Image recentChangePic = null;

		private static Image baselinePic = null;

		private static Image bestPracticePic = null;

		private static Image warningPic = null;

		private static Image errorPic = null;

		private static Image defaultObjectPic = null;

		protected static ArrayList mdataInfoList = new ArrayList();

		private static WebProxy proxy = null;

		private static bool canAccessWeb = false;

		public static Icon InfoIcon
		{
			get
			{
				return infoIcon;
			}
		}

		public static Icon NonDefaultIcon
		{
			get
			{
				return nonDefaultIcon;
			}
		}

		public static Icon RecentChangeIcon
		{
			get
			{
				return recentChangeIcon;
			}
		}

		public static Icon BaselineIcon
		{
			get
			{
				return baselineIcon;
			}
		}

		public static Icon BestPracticeIcon
		{
			get
			{
				return bestPracticeIcon;
			}
		}

		public static Icon WarningIcon
		{
			get
			{
				return warningIcon;
			}
		}

		public static Icon ErrorIcon
		{
			get
			{
				return errorIcon;
			}
		}

		public static Image InfoPic
		{
			get
			{
				return infoPic;
			}
		}

		public static Image NonDefaultPic
		{
			get
			{
				return nonDefaultPic;
			}
		}

		public static Image RecentChangePic
		{
			get
			{
				return recentChangePic;
			}
		}

		public static Image BaselinePic
		{
			get
			{
				return baselinePic;
			}
		}

		public static Image BestPracticePic
		{
			get
			{
				return bestPracticePic;
			}
		}

		public static Image WarningPic
		{
			get
			{
				return warningPic;
			}
		}

		public static Image ErrorPic
		{
			get
			{
				return errorPic;
			}
		}

		public static Image DefaultObjectPic
		{
			get
			{
				return defaultObjectPic;
			}
		}

		public static ArrayList DataInfoList
		{
			get
			{
				return mdataInfoList;
			}
		}

		public static bool CanAccessWeb
		{
			get
			{
				return canAccessWeb;
			}
			set
			{
				canAccessWeb = value;
			}
		}

		public static DataInfo ConstructDataInfo(ExecutionInterface execInterface, string fileName)
		{
			return new DataInfo(execInterface, fileName);
		}

		public static void InitializeSecurity()
		{
			Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
			Ole32.CoInitializeSecurity(IntPtr.Zero, -1, IntPtr.Zero, IntPtr.Zero, 2, 3, IntPtr.Zero, 32, IntPtr.Zero);
		}

		public static void LoadImages()
		{
			ResourceManager resourceManager = null;
			try
			{
				resourceManager = new ResourceManager("CommonFunctions", Assembly.GetExecutingAssembly());
				Image image = (Image)resourceManager.GetObject("error.gif");
			}
			catch
			{
				resourceManager = new ResourceManager(typeof(CommonData));
			}
			Size size = new Size(16, 16);
			warningIcon = new Icon((Icon)resourceManager.GetObject("warning.ico"), size);
			errorIcon = new Icon((Icon)resourceManager.GetObject("error.ico"), size);
			infoIcon = new Icon((Icon)resourceManager.GetObject("info.ico"), size);
			nonDefaultIcon = new Icon((Icon)resourceManager.GetObject("nondefault.ico"), size);
			recentChangeIcon = new Icon((Icon)resourceManager.GetObject("recent.ico"), size);
			baselineIcon = new Icon((Icon)resourceManager.GetObject("baseline.ico"), size);
			bestPracticeIcon = new Icon((Icon)resourceManager.GetObject("bestpractice.ico"), size);
			warningPic = (Image)resourceManager.GetObject("warning.gif");
			errorPic = (Image)resourceManager.GetObject("error.gif");
			infoPic = (Image)resourceManager.GetObject("info.gif");
			nonDefaultPic = (Image)resourceManager.GetObject("nondefault.gif");
			recentChangePic = (Image)resourceManager.GetObject("recent.gif");
			baselinePic = (Image)resourceManager.GetObject("baseline.gif");
			bestPracticePic = (Image)resourceManager.GetObject("bestpractice.gif");
			defaultObjectPic = new Icon((Icon)resourceManager.GetObject("server.ico"), size).ToBitmap();
		}

		public static void LoadDataInfoList(string dataDirectory, ExecutionInterface execInterface, ConstructDataInfoDelegate constructDataInfo)
		{
			mdataInfoList.Clear();
			string[] files = Directory.GetFiles(dataDirectory, execInterface.DataFileNameSearchPattern);
			string[] files2 = Directory.GetFiles(dataDirectory, "output.*.xml");
			ArrayList arrayList = new ArrayList();
			string[] array = files;
			foreach (string value in array)
			{
				arrayList.Add(value);
			}
			string[] array2 = files2;
			foreach (string value2 in array2)
			{
				arrayList.Add(value2);
			}
			foreach (string item in arrayList)
			{
				try
				{
					DataInfo value3 = constructDataInfo(execInterface, item);
					mdataInfoList.Add(value3);
				}
				catch (Exception exception)
				{
					execInterface.LogException(exception);
				}
			}
		}

		public static void LoadDataInfoList(string dataDirectory, ExecutionInterface execInterface)
		{
			LoadDataInfoList(dataDirectory, execInterface, ConstructDataInfo);
		}

		public static WebProxy GetWebProxy(ExecutionInterface execInterface, string url)
		{
			if (proxy == null)
			{
				proxy = DetectProxy.GetProxy(execInterface, execInterface.ApplicationName, url);
			}
			return proxy;
		}

		public static void BrowseURL(string url)
		{
			Process.Start("rundll32", "url.dll,FileProtocolHandler " + url);
		}

		public static string GetRootDirectory(string fileName)
		{
			return Directory.GetParent(Directory.GetParent(fileName).FullName).FullName;
		}

		public static string GetAttributeNonNull(XmlTextReader xmlFile, string attrName)
		{
			string text = xmlFile.GetAttribute(attrName);
			if (text == null)
			{
				text = "";
			}
			return text;
		}

		public static string EncodeStringArray(ICollection vals)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object val in vals)
			{
				string text = val.ToString();
				stringBuilder.Append(string.Format("{{{0}}}{1}", text.Length, text));
			}
			return stringBuilder.ToString();
		}

		public static ArrayList DecodeStringArray(string val)
		{
			ArrayList arrayList = new ArrayList();
			int num = 0;
			while (num < val.Length)
			{
				int num2 = val.IndexOf('}', num);
				if (num2 == -1 || val[num] != '{')
				{
					break;
				}
				int num3 = int.Parse(val.Substring(num + 1, num2 - num - 1));
				arrayList.Add(val.Substring(num2 + 1, num3));
				num = num2 + 1 + num3;
			}
			if (num != val.Length)
			{
				throw new FormatException();
			}
			return arrayList;
		}
	}
}
