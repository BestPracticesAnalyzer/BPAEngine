using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;
using Microsoft.Win32;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class RegistrySettings
	{
		private Rectangle screenRectangle = new Rectangle(0, 0, 1024, 768);

		private FormWindowState screenState = FormWindowState.Maximized;

		private bool versionCheckAlways = true;

		private bool showedUpdateOption;

		private bool alwaysUseCHM;

		private string dataDirectory = "";

		private RegistryKey bpaKey;

		private string suppressionData = "";

		private MessageSuppression msgSuppress;

		private string importExportDirectory = "";

		private bool sqmEnabled = true;

		public RegistryKey BPAKey
		{
			get
			{
				return bpaKey;
			}
		}

		public Rectangle ScreenRectangle
		{
			get
			{
				return screenRectangle;
			}
			set
			{
				if (bpaKey != null)
				{
					screenRectangle = value;
					bpaKey.SetValue("ScreenRectangle.X", screenRectangle.X);
					bpaKey.SetValue("ScreenRectangle.Y", screenRectangle.Y);
					bpaKey.SetValue("ScreenRectangle.Width", screenRectangle.Width);
					bpaKey.SetValue("ScreenRectangle.Height", screenRectangle.Height);
				}
			}
		}

		public FormWindowState ScreenState
		{
			get
			{
				return screenState;
			}
			set
			{
				screenState = value;
				if (bpaKey != null)
				{
					bpaKey.SetValue("ScreenState", (int)screenState);
				}
			}
		}

		public bool AlwaysUseCHM
		{
			get
			{
				return alwaysUseCHM;
			}
		}

		public bool VersionCheckAlways
		{
			get
			{
				return versionCheckAlways;
			}
			set
			{
				versionCheckAlways = value;
				if (bpaKey != null)
				{
					bpaKey.SetValue("VersionCheckAlways", versionCheckAlways ? 1 : 0);
				}
			}
		}

		public bool ShowedUpdateOption
		{
			get
			{
				return showedUpdateOption;
			}
			set
			{
				showedUpdateOption = value;
				if (bpaKey != null)
				{
					bpaKey.SetValue("ShowedUpdateOption", showedUpdateOption ? 1 : 0);
				}
			}
		}

		public bool SQMEnabled
		{
			get
			{
				return sqmEnabled;
			}
			set
			{
				sqmEnabled = value;
				if (bpaKey != null)
				{
					bpaKey.SetValue("SQMEnabled", sqmEnabled ? 1 : 0);
				}
			}
		}

		public bool SQMEnabledSet
		{
			get
			{
				if (bpaKey != null)
				{
					return bpaKey.GetValue("SQMEnabled", null) != null;
				}
				return false;
			}
		}

		public string DataDirectory
		{
			get
			{
				return dataDirectory;
			}
		}

		public string ImportExportDirectory
		{
			get
			{
				return importExportDirectory;
			}
		}

		public MessageSuppression MsgSuppress
		{
			get
			{
				return msgSuppress;
			}
		}

		public RegistrySettings(ExecutionInterface execInterface, BPACustomizations customizations, string userKeyName)
		{
			RegistryKey registryKey = null;
			try
			{
				registryKey = Registry.CurrentUser;
				if (userKeyName.Length > 0)
				{
					string sidFromName = NTSD.GetSidFromName(execInterface, null, userKeyName);
					registryKey = Registry.Users.OpenSubKey(sidFromName);
				}
				string[] array = customizations.RegistryKey.Split('\\');
				string text = array[0];
				for (int i = 1; i <= array.Length; i++)
				{
					bpaKey = registryKey.OpenSubKey(text, true);
					if (bpaKey == null)
					{
						bpaKey = registryKey.CreateSubKey(text);
					}
					if (i < array.Length)
					{
						bpaKey.Close();
						text = text + "\\" + array[i];
					}
				}
				if (customizations.RegistrySettings != null)
				{
					customizations.RegistrySettings.Initialize(bpaKey);
				}
				screenState = (FormWindowState)(int)bpaKey.GetValue("ScreenState", (SystemInformation.PrimaryMonitorMaximizedWindowSize.Width <= 1024) ? FormWindowState.Maximized : FormWindowState.Normal);
				versionCheckAlways = (int)bpaKey.GetValue("VersionCheckAlways", versionCheckAlways ? 1 : 0) == 1;
				showedUpdateOption = (int)bpaKey.GetValue("ShowedUpdateOption", showedUpdateOption ? 1 : 0) == 1;
				sqmEnabled = (int)bpaKey.GetValue("SQMEnabled", sqmEnabled ? 1 : 0) == 1;
				alwaysUseCHM = (int)bpaKey.GetValue("AlwaysUseCHM", alwaysUseCHM ? 1 : 0) == 1;
				suppressionData = (string)bpaKey.GetValue("SuppressionData", suppressionData);
				msgSuppress = new MessageSuppression();
				string[] array2 = suppressionData.Split(',');
				string[] array3 = array2;
				foreach (string text2 in array3)
				{
					msgSuppress.Suppress(text2.Trim());
				}
				dataDirectory = customizations.DefaultDataDirectory;
				dataDirectory = (string)bpaKey.GetValue("DataDirectory", dataDirectory);
				if (!Directory.Exists(dataDirectory))
				{
					Directory.CreateDirectory(dataDirectory);
				}
				importExportDirectory = (string)bpaKey.GetValue("ImportExportDirectory", dataDirectory);
				screenRectangle.X = (int)bpaKey.GetValue("ScreenRectangle.X", screenRectangle.X);
				screenRectangle.Y = (int)bpaKey.GetValue("ScreenRectangle.Y", screenRectangle.Y);
				screenRectangle.Width = (int)bpaKey.GetValue("ScreenRectangle.Width", screenRectangle.Width);
				screenRectangle.Height = (int)bpaKey.GetValue("ScreenRectangle.Height", screenRectangle.Height);
			}
			catch
			{
				bpaKey = null;
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Close();
				}
			}
		}

		public void SaveSuppressionData()
		{
			if (bpaKey != null)
			{
				bpaKey.SetValue("SuppressionData", msgSuppress.ToString());
			}
		}
	}
}
