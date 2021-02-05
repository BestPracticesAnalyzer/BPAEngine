using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Xml;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class UpdateInfo
	{
		public delegate void StatusDelegate(int pctDone);

		public const string NEXTVERSIONPOSTFIX = "nextversion.xml";

		public const string HELPPOSTFIX = "chm";

		public const string MSIPOSTFIX = "msi";

		private const int BLOCK = 20000;

		private string urlLocale = "";

		private string fileLocale = "";

		private ExecutionInterface execInterface;

		private SortedList configVersionInfoList = new SortedList();

		private VersionInfo masterConfigVersionInfo = new VersionInfo();

		private bool masterNewerThanLocal;

		private VersionInfo binaryVersionInfo = new VersionInfo();

		private object extendedData;

		public VersionInfo ConfigVersionInfo
		{
			get
			{
				return masterConfigVersionInfo;
			}
		}

		public VersionInfo BinaryVersionInfo
		{
			get
			{
				return binaryVersionInfo;
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

		public UpdateInfo(ExecutionInterface execInterface, ConfigurationInfo configInfo)
		{
			this.execInterface = execInterface;
			masterConfigVersionInfo.ConfigInfo = configInfo;
			configVersionInfoList.Add(configInfo.FileName, masterConfigVersionInfo);
			if (configInfo.AppendToDownloadURL)
			{
				urlLocale = "/" + execInterface.Culture;
				fileLocale = "\\" + execInterface.Culture;
			}
		}

		public void CheckForUpdates()
		{
			if (masterConfigVersionInfo.ConfigInfo.DownloadURL.Length == 0)
			{
				binaryVersionInfo.Found = true;
				binaryVersionInfo.NewerThanLocal = false;
				masterConfigVersionInfo.Found = true;
				masterConfigVersionInfo.NewerThanLocal = false;
				masterNewerThanLocal = false;
				configVersionInfoList.Clear();
				configVersionInfoList.Add(masterConfigVersionInfo.ConfigInfo.FileName, masterConfigVersionInfo);
				return;
			}
			if (GetBinaryVersion())
			{
				GetConfigVersion(masterConfigVersionInfo);
			}
			masterNewerThanLocal = masterConfigVersionInfo.NewerThanLocal;
			foreach (VersionInfo value in configVersionInfoList.Values)
			{
				if (value.NewerThanLocal)
				{
					masterConfigVersionInfo.NewerThanLocal = true;
					break;
				}
			}
		}

		public void DownloadConfig(StatusDelegate Status, bool downloadHelp)
		{
			foreach (VersionInfo value in configVersionInfoList.Values)
			{
				if ((value.NewerThanLocal && !value.ConfigInfo.Master) || (masterNewerThanLocal && value.ConfigInfo.Master))
				{
					DownloadFile(string.Format("{0}{1}/{2}", value.ConfigInfo.DownloadURL, urlLocale, value.ConfigInfo.Name), value.ConfigInfo.FileName, string.Format("{0}.{1}", value.ConfigInfo.FileName, value.ConfigInfo.ConfigVersion.ToString()), Status);
				}
			}
			if (downloadHelp)
			{
				DownloadFile(string.Format("{0}{1}/{2}.{3}", masterConfigVersionInfo.ConfigInfo.DownloadURL, urlLocale, execInterface.ApplicationName, "chm"), string.Format("{0}{1}\\{2}.{3}", CommonData.GetRootDirectory(execInterface.Options.Configuration.FileName), fileLocale, execInterface.ApplicationName, "chm"), string.Format("{0}{1}\\{2}.{3}.{4}", CommonData.GetRootDirectory(execInterface.Options.Configuration.FileName), fileLocale, execInterface.ApplicationName, "chm", masterConfigVersionInfo.ConfigInfo.ConfigVersion.ToString()), Status);
			}
		}

		public void DownloadBinaries(StatusDelegate Status)
		{
			DownloadFile(string.Format("{0}", binaryVersionInfo.AutoUrl), string.Format("{0}{1}\\{2}.{3}", CommonData.GetRootDirectory(execInterface.Options.Configuration.FileName), fileLocale, execInterface.ApplicationName, "msi"), string.Format("{0}{1}\\{2}.{3}.{4}", CommonData.GetRootDirectory(execInterface.Options.Configuration.FileName), fileLocale, execInterface.ApplicationName, "msi", execInterface.EngineVersion.ToString()), Status);
		}

		private bool GetBinaryVersion()
		{
			string text = string.Format("{0}\\nextversion.xml", execInterface.ExecutionDirectory);
			XmlDocument xmlDocument = null;
			try
			{
				DownloadFile(string.Format("{0}{1}/{2}.{3}", masterConfigVersionInfo.ConfigInfo.DownloadURL, urlLocale, execInterface.ApplicationName, "nextversion.xml"), text, string.Format("{0}.sav", text), null);
			}
			catch (Exception exception)
			{
				execInterface.LogException(exception);
				return false;
			}
			xmlDocument = new XmlDocument();
			xmlDocument.PreserveWhitespace = true;
			xmlDocument.Load(text);
			ValidateConfigurations validateConfigurations = new ValidateConfigurations(execInterface);
			if (!validateConfigurations.ValidateOneFile(xmlDocument, text))
			{
				execInterface.LogText(validateConfigurations.ValidationError);
				return false;
			}
			try
			{
				XmlNode xmlNode = xmlDocument.SelectSingleNode("NextVersion");
				binaryVersionInfo.WebVersion = new Version(xmlNode.Attributes["AppVersion"].Value);
				binaryVersionInfo.Found = true;
				binaryVersionInfo.NewerThanLocal = binaryVersionInfo.WebVersion.CompareTo(execInterface.EngineVersion) > 0;
				XmlNode xmlNode2 = xmlDocument.SelectSingleNode("/*/Description");
				binaryVersionInfo.Description = xmlNode2.InnerText;
				XmlNode xmlNode3 = xmlDocument.SelectSingleNode("/*/AutoMSI");
				binaryVersionInfo.AutoUrl = xmlNode3.InnerText;
				XmlNode xmlNode4 = xmlDocument.SelectSingleNode("/*/ManualMSI");
				binaryVersionInfo.ManualUrl = xmlNode4.InnerText;
			}
			catch (Exception exception2)
			{
				execInterface.LogException(exception2);
				return false;
			}
			return true;
		}

		private bool GetConfigVersion(VersionInfo versionInfo)
		{
			XmlTextReader xmlTextReader = null;
			bool flag = true;
			int num = 0;
			HttpWebResponse httpWebResponse = null;
			while (flag)
			{
				try
				{
					flag = false;
					string text = string.Format("{0}{1}/{2}", versionInfo.ConfigInfo.DownloadURL, urlLocale, versionInfo.ConfigInfo.Name);
					HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(text);
					httpWebRequest.PreAuthenticate = true;
					httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
					WebProxy webProxy = CommonData.GetWebProxy(execInterface, text);
					if (webProxy != null)
					{
						httpWebRequest.Proxy = webProxy;
					}
					httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
					if (httpWebResponse.ResponseUri != httpWebRequest.RequestUri)
					{
						break;
					}
					xmlTextReader = new XmlTextReader(httpWebResponse.GetResponseStream());
					xmlTextReader.WhitespaceHandling = WhitespaceHandling.None;
					bool flag2 = false;
					while (xmlTextReader.Read() && !flag2)
					{
						XmlNodeType nodeType = xmlTextReader.NodeType;
						if (nodeType == XmlNodeType.Element && xmlTextReader.Name == "Configuration")
						{
							versionInfo.WebVersion = new Version(CommonData.GetAttributeNonNull(xmlTextReader, "ConfigVersion"));
							versionInfo.Description = CommonData.GetAttributeNonNull(xmlTextReader, "ChangeDescription");
							versionInfo.ManualUrl = CommonData.GetAttributeNonNull(xmlTextReader, "ManualUrl");
							string attributeNonNull = CommonData.GetAttributeNonNull(xmlTextReader, "ReferencedFiles");
							if (attributeNonNull.Length > 0)
							{
								versionInfo.ReferencedFiles = attributeNonNull.Split(',');
							}
							flag2 = true;
						}
					}
					ProcessReferencedFiles(versionInfo.ReferencedFiles);
					ProcessReferencedFiles(versionInfo.ConfigInfo.ReferencedFiles);
					continue;
				}
				catch (Exception exception)
				{
					flag = true;
					num++;
					execInterface.LogException(exception);
					if (num >= 2)
					{
						break;
					}
					continue;
				}
				finally
				{
					if (xmlTextReader != null)
					{
						xmlTextReader.Close();
					}
					xmlTextReader = null;
					if (httpWebResponse != null)
					{
						httpWebResponse.Close();
					}
					httpWebResponse = null;
					CommonData.CanAccessWeb = !flag;
					versionInfo.Found = !flag;
					versionInfo.NewerThanLocal = versionInfo.WebVersion.CompareTo(versionInfo.ConfigInfo.ConfigVersion) > 0;
				}
			}
			return !flag;
		}

		private void ProcessReferencedFiles(string[] referencedFiles)
		{
			if (referencedFiles == null)
			{
				return;
			}
			foreach (string arg in referencedFiles)
			{
				string text = string.Format("{0}\\{1}", Directory.GetParent(execInterface.Options.Configuration.FileName).ToString(), arg);
				if (!configVersionInfoList.Contains(text))
				{
					ConfigurationInfo configInfo = new ConfigurationInfo(execInterface, text, masterConfigVersionInfo.ConfigInfo);
					VersionInfo versionInfo = new VersionInfo();
					versionInfo.ConfigInfo = configInfo;
					configVersionInfoList.Add(text, versionInfo);
					GetConfigVersion(versionInfo);
				}
			}
		}

		private void DownloadFile(string url, string fileName, string saveFileName, StatusDelegate Status)
		{
			string tempFileName = Path.GetTempFileName();
			BinaryReader binaryReader = null;
			FileStream fileStream = null;
			BinaryWriter binaryWriter = null;
			bool flag = true;
			int num = 0;
			HttpWebResponse httpWebResponse = null;
			while (flag)
			{
				try
				{
					flag = false;
					HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
					httpWebRequest.PreAuthenticate = true;
					httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
					WebProxy webProxy = CommonData.GetWebProxy(execInterface, url);
					if (webProxy != null)
					{
						httpWebRequest.Proxy = webProxy;
					}
					httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
					if (httpWebResponse.ResponseUri != httpWebRequest.RequestUri)
					{
						num = 3;
						throw new FileNotFoundException();
					}
					long contentLength = httpWebResponse.ContentLength;
					long num2 = 0L;
					binaryReader = new BinaryReader(httpWebResponse.GetResponseStream());
					fileStream = new FileStream(tempFileName, FileMode.Create);
					binaryWriter = new BinaryWriter(fileStream);
					byte[] buffer = new byte[20000];
					int num3 = 0;
					while ((num3 = binaryReader.Read(buffer, 0, 20000)) > 0)
					{
						num2 += num3;
						if (Status != null)
						{
							Status((int)(num2 * 100 / contentLength));
						}
						binaryWriter.Write(buffer, 0, num3);
					}
				}
				catch (Exception)
				{
					flag = true;
					num++;
					if (num >= 3)
					{
						throw;
					}
				}
				finally
				{
					if (binaryWriter != null)
					{
						binaryWriter.Close();
					}
					binaryWriter = null;
					if (binaryReader != null)
					{
						binaryReader.Close();
					}
					binaryReader = null;
					if (fileStream != null)
					{
						fileStream.Close();
					}
					fileStream = null;
					if (httpWebResponse != null)
					{
						httpWebResponse.Close();
					}
					httpWebResponse = null;
				}
			}
			try
			{
				File.Move(fileName, saveFileName);
			}
			catch
			{
				try
				{
					File.Delete(fileName);
				}
				catch
				{
				}
			}
			if (!Directory.Exists(Directory.GetParent(fileName).FullName))
			{
				Directory.CreateDirectory(Directory.GetParent(fileName).FullName);
			}
			File.Move(tempFileName, fileName);
		}
	}
}
