using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VSPowerToys.Updater.Xsd;

namespace Microsoft.VSPowerToys.Updater
{
	public class ManifestManager
	{
		private Uri manifestLocation;

		private string downloadDir;

		private string appStartDir;

		public ManifestManager(Uri manifestLoc, string downloadPath, string appStartupPath)
		{
			manifestLocation = manifestLoc;
			downloadDir = downloadPath;
			appStartDir = appStartupPath;
		}

		public static Manifest GetClientManifest(string clientManifestLoc)
		{
			Microsoft.VSPowerToys.Updater.Xsd.Manifest manifest = (Microsoft.VSPowerToys.Updater.Xsd.Manifest)ValidateAndDeserialize(typeof(Microsoft.VSPowerToys.Updater.Xsd.Manifest), clientManifestLoc, "Microsoft.VSPowerToys.Updater.Xsd.manifest.xsd");
			Manifest manifest2 = new Manifest(manifest);
			foreach (ComponentManifest component in manifest2.Components)
			{
				component.UpdateState = ComponentManifest.State.Installed;
			}
			return manifest2;
		}

		private void ExpandGlobalConstant(FileManifest fm, Manifest manifest)
		{
			for (int i = 0; i < fm.InstallTasks.Count; i++)
			{
				if (fm.InstallTasks[i].GetType() == typeof(FileMoveTask))
				{
					FileMoveTask fileMoveTask = fm.InstallTasks[i] as FileMoveTask;
					if (fileMoveTask.DestinationFile.StartsWith(manifest.AppDirToken))
					{
						fileMoveTask.DestinationFile = fileMoveTask.DestinationFile.Replace(manifest.AppDirToken, appStartDir);
					}
					if (string.IsNullOrEmpty(fileMoveTask.SourceFile))
					{
						fileMoveTask.SourceFile = downloadDir + "\\" + fm.Source;
					}
					else if (fileMoveTask.SourceFile.StartsWith(manifest.AppDirToken))
					{
						fileMoveTask.SourceFile = fileMoveTask.SourceFile.Replace(manifest.AppDirToken, appStartDir);
					}
				}
				else if (fm.InstallTasks[i].GetType() == typeof(FileUnzipTask))
				{
					FileUnzipTask fileUnzipTask = fm.InstallTasks[i] as FileUnzipTask;
					if (fileUnzipTask.DestinationDir.StartsWith(manifest.AppDirToken))
					{
						fileUnzipTask.DestinationDir = fileUnzipTask.DestinationDir.Replace(manifest.AppDirToken, appStartDir);
					}
					if (string.IsNullOrEmpty(fileUnzipTask.SourceFile))
					{
						fileUnzipTask.SourceFile = downloadDir + "\\" + fm.Source;
					}
					else if (fileUnzipTask.SourceFile.StartsWith(manifest.AppDirToken))
					{
						fileUnzipTask.SourceFile = fileUnzipTask.SourceFile.Replace(manifest.AppDirToken, appStartDir);
					}
				}
			}
		}

		public Manifest GetValidManifest(string clientManifestLoc)
		{
			HttpDownloader httpDownloader = new HttpDownloader();
			string text = httpDownloader.DownloadFile(manifestLocation, downloadDir);
			if (text == null)
			{
				return null;
			}
			Microsoft.VSPowerToys.Updater.Xsd.Manifest manifest = (Microsoft.VSPowerToys.Updater.Xsd.Manifest)ValidateAndDeserialize(typeof(Microsoft.VSPowerToys.Updater.Xsd.Manifest), text, "Microsoft.VSPowerToys.Updater.Xsd.manifest.xsd");
			Manifest manifest2 = new Manifest(manifest);
			Manifest clientManifest = GetClientManifest(clientManifestLoc);
			foreach (ComponentManifest component in manifest2.Components)
			{
				if (clientManifest.Components[component.Name] != null)
				{
					continue;
				}
				component.UpdateState = ComponentManifest.State.New;
				foreach (FileManifest file in component.Files)
				{
					ExpandGlobalConstant(file, manifest2);
				}
			}
			foreach (ComponentManifest component2 in clientManifest.Components)
			{
				ComponentManifest componentManifest3 = manifest2.Components[component2.Name];
				foreach (FileManifest file2 in component2.Files)
				{
					FileManifest fileManifest2 = null;
					if (componentManifest3.Files.Contains(file2.Source))
					{
						fileManifest2 = componentManifest3.Files[file2.Source];
						if (string.Compare(fileManifest2.Hash, file2.Hash, false, CultureInfo.InvariantCulture) == 0)
						{
							componentManifest3.Files.Remove(fileManifest2);
							component2.UpdateState = ComponentManifest.State.Installed;
						}
						else
						{
							componentManifest3.UpdateState = ComponentManifest.State.Update;
							fileManifest2.UpdateState = FileManifest.State.Update;
							ExpandGlobalConstant(fileManifest2, manifest2);
						}
					}
				}
				if (componentManifest3.Files.Count <= 0)
				{
					manifest2.Components.Remove(componentManifest3);
				}
			}
			if (manifest2.Components.Count <= 0)
			{
				manifest2.Apply = false;
			}
			else
			{
				manifest2.Apply = true;
			}
			return manifest2;
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public static object ValidateAndDeserialize(Type type, string fileName, string schemaResource)
		{
			object result = null;
			try
			{
				using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream(schemaResource))
				{
					XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
					xmlReaderSettings.Schemas.Add("urn:schemas-microsoft-com:VSPowerToy:manifest", new XmlTextReader(input));
					xmlReaderSettings.ValidationType = ValidationType.Schema;
					XmlReader xmlReader = XmlReader.Create(fileName, xmlReaderSettings);
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(xmlReader);
					xmlReader.Close();
					XmlSerializer xmlSerializer = new XmlSerializer(type);
					XmlReader xmlReader2 = XmlReader.Create(fileName, xmlReaderSettings);
					result = xmlSerializer.Deserialize(xmlReader2);
					xmlReader2.Close();
					return result;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
				return result;
			}
		}

		public static string GetMD5FromFile(string fileName)
		{
			FileStream fileStream = new FileStream(fileName, FileMode.Open);
			MD5 mD = new MD5CryptoServiceProvider();
			byte[] inArray = mD.ComputeHash(fileStream);
			fileStream.Close();
			return Convert.ToBase64String(inArray);
		}
	}
}
