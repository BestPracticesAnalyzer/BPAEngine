using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class CommandLineParameters
	{
		protected const string Option_Help = "?,H";

		public const string Option_Config = "CFG";

		protected const string Option_Data = "DAT";

		protected const string Option_Input = "IN";

		protected const string Option_Label = "L";

		private const string Option_Collect = "C";

		private const string Option_Analyze = "A";

		protected const string Option_Export = "E";

		protected const string Option_Report = "REPORT";

		protected const string Option_Restriction = "R";

		internal const string Option_Schedule = "S";

		protected const string Option_Threads = "TH";

		protected const string Option_Timeout = "TO";

		protected const string Option_Users = "U";

		protected const string Option_SaveInterval = "SI";

		protected const string Option_ProcessorOptions = "P";

		public const string Option_NoAutoDownload = "ND";

		public const string Option_UserKey = "UK";

		protected const string Option_GlobalSubstitutions = "GS";

		protected const string dataTimeStampReplace = "%TIMESTAMP%";

		protected const int ENABLE_ECHO_INPUT = 4;

		protected const int STD_INPUT_HANDLE = -10;

		protected string muserKeyName = "";

		protected ExecutionInterface mexecInterface;

		public string UserKeyName
		{
			get
			{
				return muserKeyName;
			}
		}

		public ExecutionInterface ExecInterface
		{
			get
			{
				return mexecInterface;
			}
		}

		public CommandLineParameters(ExecutionInterface execInterface)
		{
			mexecInterface = execInterface;
		}

		public static bool ArgumentSet(string[] args, string optionList)
		{
			ArrayList argumentValues = GetArgumentValues(args, optionList, 0, true);
			return argumentValues != null;
		}

		public static bool HelpSet(string[] args)
		{
			ArrayList argumentValues = GetArgumentValues(args, "?,H", 0, false);
			return argumentValues != null;
		}

		public static ArrayList GetArgumentValues(string[] args, string optionList, int paramCount, bool eraseArgument)
		{
			string[] array = optionList.Split(',');
			ArrayList arrayList = null;
			string arg = "";
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == null)
				{
					continue;
				}
				bool flag = args[i].Length > 0 && (args[i].Substring(0, 1) == "/" || args[i].Substring(0, 1) == "-");
				if (arrayList != null)
				{
					if (flag)
					{
						break;
					}
					arrayList.Add(args[i]);
					if (eraseArgument)
					{
						args[i] = null;
					}
				}
				if (!flag)
				{
					continue;
				}
				string b = args[i].Substring(1).ToUpper(CultureInfo.InvariantCulture);
				string[] array2 = array;
				foreach (string a in array2)
				{
					if (a == b)
					{
						arg = args[i];
						if (eraseArgument)
						{
							args[i] = null;
						}
						arrayList = new ArrayList();
					}
				}
			}
			if (arrayList != null && paramCount != -1 && arrayList.Count != paramCount)
			{
				throw new ArgumentException(CommonLoc.Error_BadParam(arg));
			}
			return arrayList;
		}

		public static string GetArgumentValue(string[] args, string optionList)
		{
			ArrayList argumentValues = GetArgumentValues(args, optionList, 1, true);
			if (argumentValues == null)
			{
				return "";
			}
			return argumentValues[0].ToString();
		}

		public virtual bool ParseArgs(string[] args)
		{
			bool flag = true;
			bool flag2 = true;
			try
			{
				flag = ArgumentSet(args, "?,H");
				mexecInterface.Options.Debug = ArgumentSet(args, "DEBUG,B");
				mexecInterface.Options.Trace = ArgumentSet(args, "TRACE,T");
				mexecInterface.Options.Operations = (OperationsFlags)0;
				if (ArgumentSet(args, "C"))
				{
					mexecInterface.Options.Operations |= OperationsFlags.Collect;
				}
				if (ArgumentSet(args, "A"))
				{
					mexecInterface.Options.Operations |= OperationsFlags.Analyze;
				}
				if (ArgumentSet(args, "REPORT"))
				{
					mexecInterface.Options.Operations |= OperationsFlags.Report;
				}
				if (ArgumentSet(args, "E"))
				{
					mexecInterface.Options.Operations |= OperationsFlags.Export;
				}
				if (mexecInterface.Options.Operations == (OperationsFlags)0)
				{
					mexecInterface.Options.Operations = OperationsFlags.Collect | OperationsFlags.Analyze;
				}
				string argumentValue = GetArgumentValue(args, "CFG");
				if (argumentValue.Length > 0)
				{
					mexecInterface.Options.Configuration.FileName = argumentValue;
				}
				else if (mexecInterface.Options.Configuration.FileName.Length == 0)
				{
					mexecInterface.SetDefaultConfigurationFileName();
				}
				mexecInterface.Options.Configuration.Load();
				mexecInterface.Options.Data.FileName = GetArgumentValue(args, "IN");
				if (mexecInterface.Options.Data.FileName.Length > 0)
				{
					mexecInterface.Options.Data.Load();
				}
				mexecInterface.Options.Label = GetArgumentValue(args, "L");
				mexecInterface.Options.Data.FileName = GetArgumentValue(args, "DAT");
				if (mexecInterface.Options.Data.FileName.Length > 0 && Directory.Exists(mexecInterface.Options.Data.FileName))
				{
					string currentDirectory = Directory.GetCurrentDirectory();
					Directory.SetCurrentDirectory(mexecInterface.Options.Data.FileName);
					mexecInterface.SetDefaultDataFileName();
					Directory.SetCurrentDirectory(currentDirectory);
				}
				string argumentValue2 = GetArgumentValue(args, "TH");
				if (argumentValue2.Length > 0)
				{
					mexecInterface.Options.MaxThreads = uint.Parse(argumentValue2);
				}
				string argumentValue3 = GetArgumentValue(args, "TO");
				if (argumentValue3.Length > 0)
				{
					mexecInterface.Options.Timeout = int.Parse(argumentValue3);
				}
				string argumentValue4 = GetArgumentValue(args, "SI");
				if (argumentValue4.Length > 0)
				{
					mexecInterface.Options.SaveInterval = int.Parse(argumentValue4);
				}
				LoadGlobalSubstitutions(GetArgumentValues(args, "GS", -1, true));
				LoadCredentials(GetArgumentValues(args, "U", -1, true));
				mexecInterface.Options.Restrictions.LoadValid(mexecInterface.Options.Configuration);
				mexecInterface.Options.Restrictions.EnableOptions(GetArgumentValue(args, "R"));
				muserKeyName = GetArgumentValue(args, "UK");
				mexecInterface.Options.SetProcessorOptions(GetArgumentValues(args, "P", -1, true));
			}
			catch (Exception ex)
			{
				mexecInterface.LogException(CommonLoc.Error_Parameters(ex.Message), ex);
				flag2 = false;
				flag = true;
			}
			if (flag2)
			{
				foreach (string text in args)
				{
					if (text != null)
					{
						mexecInterface.LogText(CommonLoc.Error_Parameters(CommonLoc.Error_InvalidParam(text)) + "\n");
						flag = true;
						flag2 = false;
						break;
					}
				}
			}
			if (flag)
			{
				mexecInterface.Options.Log(CommonLoc.Help(Application.ExecutablePath, string.Concat("\"", mexecInterface.Options.Restrictions.FirstOption, "\""), mexecInterface.Options.MaxThreads, mexecInterface.Options.Timeout));
				mexecInterface.Options.Log(CommonLoc.Help_Example(Path.GetFileName(Application.ExecutablePath) + " -dat output.xml -u ADLOGON food\\administrator P@ssw0rd EXLOGON food\\administrator P@ssw0rd -r \"General,Level 3,Server=FOO-01|FOO-02\" -c -a\n"));
				flag2 = false;
				try
				{
					mexecInterface.Options.Log(CommonLoc.Info_ValidRestrictions);
					foreach (string key in mexecInterface.Options.Restrictions.Types.Keys)
					{
						string arg = mexecInterface.Options.Restrictions.CommaList(key, false, false, false);
						mexecInterface.Options.Log(string.Format("   {0}: {1}", key, arg));
					}
					SortedList sortedList = AttributeValues(mexecInterface.Options.Configuration, "SecurityContext");
					string[] array = new string[sortedList.Count];
					sortedList.Keys.CopyTo(array, 0);
					string arg2 = string.Join(",", array);
					mexecInterface.Options.Log(CommonLoc.Info_ValidContexts(arg2));
				}
				catch
				{
				}
				mexecInterface.Options.Log("\n" + CommonLoc.Help_Description);
			}
			return flag2;
		}

		private void LoadGlobalSubstitutions(ArrayList values)
		{
			if (values != null)
			{
				if (values.Count % 2 != 0)
				{
					throw new ArgumentException(CommonLoc.Error_BadUserParam);
				}
				for (int i = 0; i <= values.Count - 2; i += 2)
				{
					string key = values[i].ToString();
					string value = values[i + 1].ToString();
					mexecInterface.Options.Substitutions.Add(key, value);
				}
			}
		}

		private void LoadCredentials(ArrayList values)
		{
			if (values == null)
			{
				return;
			}
			if (values.Count % 3 != 0)
			{
				throw new ArgumentException(CommonLoc.Error_BadUserParam);
			}
			for (int i = 0; i <= values.Count - 3; i += 3)
			{
				string text = values[i].ToString();
				string text2 = values[i + 1].ToString();
				string text3 = values[i + 2].ToString();
				int num = text2.IndexOf("\\");
				string domainName;
				string userName;
				if (num == -1)
				{
					domainName = "";
					userName = text2;
				}
				else
				{
					domainName = text2.Substring(0, num);
					userName = text2.Substring(num + 1);
				}
				SortedList sortedList = AttributeValues(mexecInterface.Options.Configuration, "SecurityContext");
				string b = text.ToUpper(CultureInfo.InvariantCulture);
				bool flag = false;
				foreach (string key in sortedList.Keys)
				{
					if (key.ToUpper(CultureInfo.InvariantCulture) == b)
					{
						flag = true;
						text = key;
						break;
					}
				}
				if (flag)
				{
					if (text3 == "*")
					{
						Console.Write("Password for {0}: ", text);
						IntPtr stdHandle = Kernel32.GetStdHandle(-10);
						int dwMode = 0;
						Kernel32.GetConsoleMode(stdHandle, ref dwMode);
						Kernel32.SetConsoleMode(stdHandle, dwMode & -5);
						text3 = Console.ReadLine();
						Kernel32.SetConsoleMode(stdHandle, dwMode);
						Console.WriteLine("");
					}
					mexecInterface.Options.AddCredentials(text, domainName, userName, text3);
				}
				else if (mexecInterface.Options.Trace)
				{
					mexecInterface.Options.Log(CommonLoc.Error_BadUserParam);
				}
			}
		}

		public static SortedList AttributeValues(Document cfg, string attributeName)
		{
			SortedList sortedList = new SortedList();
			Node[] nodes = cfg.GetNodes("//Object[@" + attributeName + "] | //Setting[@" + attributeName + "]");
			Node[] array = nodes;
			foreach (Node node in array)
			{
				string[] array2 = node.GetAttribute(attributeName).Split(',');
				string[] array3 = array2;
				foreach (string text in array3)
				{
					string text2 = text.Trim();
					if (text2.Length > 0 && !sortedList.Contains(text2))
					{
						sortedList.Add(text2, true);
					}
				}
			}
			return sortedList;
		}
	}
}
