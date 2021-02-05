using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VSPowerToys.Updater.Xsd;

namespace Microsoft.VSPowerToys.Updater
{
	[Serializable]
	public class Manifest : ISerializable
	{
		private Microsoft.VSPowerToys.Updater.Xsd.Manifest updateManifest;

		private Guid updaterManifestId;

		private ComponentsCollection componentsToUpdate;

		private bool apply;

		private string appDirTokenString = "$(AppDir)";

		public Guid ManifestId
		{
			get
			{
				if (updaterManifestId == Guid.Empty)
				{
					updaterManifestId = new Guid(updateManifest.ManifestId);
				}
				return updaterManifestId;
			}
		}

		public string Description
		{
			get
			{
				return updateManifest.Description;
			}
		}

		public bool Apply
		{
			get
			{
				return apply;
			}
			set
			{
				apply = value;
			}
		}

		public string Application
		{
			get
			{
				return updateManifest.Application.AppDir + "\\" + updateManifest.Application.MainExe.file;
			}
		}

		public string AppDirToken
		{
			get
			{
				return appDirTokenString;
			}
		}

		public string AppDir
		{
			get
			{
				return updateManifest.Application.AppDir;
			}
			set
			{
				updateManifest.Application.AppDir = value;
			}
		}

		public string ApplicationParams
		{
			get
			{
				return updateManifest.Application.MainExe.parameters;
			}
		}

		public ComponentsCollection Components
		{
			get
			{
				if (componentsToUpdate == null)
				{
					componentsToUpdate = new ComponentsCollection(updateManifest.Components);
				}
				return componentsToUpdate;
			}
		}

		public Manifest(Microsoft.VSPowerToys.Updater.Xsd.Manifest manifest)
		{
			updateManifest = manifest;
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public void Init(XmlNode config)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(Microsoft.VSPowerToys.Updater.Xsd.Manifest));
			updateManifest = (Microsoft.VSPowerToys.Updater.Xsd.Manifest)xmlSerializer.Deserialize(new XmlNodeReader(config));
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		protected Manifest(SerializationInfo info, StreamingContext context)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(Microsoft.VSPowerToys.Updater.Xsd.Manifest));
			updateManifest = (Microsoft.VSPowerToys.Updater.Xsd.Manifest)xmlSerializer.Deserialize(new StringReader(info.GetString("_manifest")));
			apply = info.GetBoolean("_apply");
		}

		public void SerializeXml(string filePath)
		{
			Microsoft.VSPowerToys.Updater.Xsd.Manifest manifest = new Microsoft.VSPowerToys.Updater.Xsd.Manifest();
			manifest.Application = new AppType();
			manifest.Application.ApplicationId = updateManifest.Application.ApplicationId;
			manifest.Application.AppDir = AppDir;
			manifest.Application.MainExe = new MainExeType();
			manifest.Application.MainExe.file = updateManifest.Application.MainExe.file;
			manifest.Application.MainExe.parameters = ApplicationParams;
			manifest.Description = Description;
			manifest.ManifestId = updateManifest.ManifestId;
			manifest.Components = new ManifestComponents();
			manifest.Components.Components = new ManifestComponent[Components.Count];
			manifest.Components.@base = Components.Base;
			for (int i = 0; i < Components.Count; i++)
			{
				manifest.Components.Components[i] = new ManifestComponent();
				ManifestComponent manifestComponent = manifest.Components.Components[i];
				manifestComponent.Name = Components[i].Name;
				manifestComponent.Description = Components[i].Description;
				manifestComponent.Updated = Components[i].LastUpdated;
				manifestComponent.Files = new ManifestFiles();
				manifestComponent.Files.Files = new ManifestFile[Components[i].Files.Count];
				manifestComponent.Files.@base = Components[i].Files.Base;
				for (int j = 0; j < Components[i].Files.Count; j++)
				{
					manifestComponent.Files.Files[j] = new ManifestFile();
					ManifestFile manifestFile = manifestComponent.Files.Files[j];
					manifestFile.Source = Components[i].Files[j].Source;
					manifestFile.query = Components[i].Files[j].Query;
					manifestFile.Hash = Components[i].Files[j].Hash;
					manifestFile.AnyAttr = Components[i].Files[j].Attributes;
					manifestFile.InstallTasks = new InstallTasks();
					manifestFile.InstallTasks.Items = new object[Components[i].Files[j].InstallTasks.Count];
					for (int k = 0; k < Components[i].Files[j].InstallTasks.Count; k++)
					{
						if (Components[i].Files[j].InstallTasks[k].GetType() == typeof(FileMoveTask))
						{
							manifestFile.InstallTasks.Items[k] = new Microsoft.VSPowerToys.Updater.Xsd.FileMoveTask();
							FileMoveTask fileMoveTask = Components[i].Files[j].InstallTasks[k] as FileMoveTask;
							Microsoft.VSPowerToys.Updater.Xsd.FileMoveTask fileMoveTask2 = manifestFile.InstallTasks.Items[k] as Microsoft.VSPowerToys.Updater.Xsd.FileMoveTask;
							fileMoveTask2.srcFile = fileMoveTask.SourceFile;
							fileMoveTask2.destFile = fileMoveTask.DestinationFile;
						}
						else if (Components[i].Files[j].InstallTasks[k].GetType() == typeof(FileUnzipTask))
						{
							manifestFile.InstallTasks.Items[k] = new Microsoft.VSPowerToys.Updater.Xsd.FileUnzipTask();
							FileUnzipTask fileUnzipTask = Components[i].Files[j].InstallTasks[k] as FileUnzipTask;
							Microsoft.VSPowerToys.Updater.Xsd.FileUnzipTask fileUnzipTask2 = manifestFile.InstallTasks.Items[k] as Microsoft.VSPowerToys.Updater.Xsd.FileUnzipTask;
							fileUnzipTask2.srcFile = fileUnzipTask.SourceFile;
							fileUnzipTask2.destDir = fileUnzipTask.DestinationDir;
						}
					}
				}
			}
			using (Stream stream = File.Open(filePath, FileMode.Truncate, FileAccess.ReadWrite))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(Microsoft.VSPowerToys.Updater.Xsd.Manifest));
				xmlSerializer.Serialize(stream, manifest);
				stream.Close();
			}
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringWriter textWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(Microsoft.VSPowerToys.Updater.Xsd.Manifest));
			xmlSerializer.Serialize(textWriter, updateManifest);
			info.AddValue("_manifest", stringBuilder.ToString());
			info.AddValue("_apply", apply);
		}
	}
}
