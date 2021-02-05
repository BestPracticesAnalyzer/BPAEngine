using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.VSPowerToys.Updater.Xsd;

namespace Microsoft.VSPowerToys.Updater
{
	public class FileManifest
	{
		public enum State
		{
			New,
			Update
		}

		private string sourceLocation;

		private string fileHashValue;

		private string query;

		private XmlAttribute[] otherAttributes;

		private Collection<ITask> iTasks;

		private State updateState;

		public State UpdateState
		{
			get
			{
				return updateState;
			}
			set
			{
				updateState = value;
			}
		}

		public string Source
		{
			get
			{
				return sourceLocation;
			}
		}

		public string Hash
		{
			get
			{
				return fileHashValue;
			}
		}

		public string Query
		{
			get
			{
				return query;
			}
		}

		public ReadOnlyCollection<ITask> InstallTasks
		{
			get
			{
				return new ReadOnlyCollection<ITask>(iTasks);
			}
		}

		public XmlAttribute[] Attributes
		{
			get
			{
				return otherAttributes;
			}
		}

		public FileManifest(ManifestFile file)
		{
			otherAttributes = file.AnyAttr;
			sourceLocation = file.Source;
			fileHashValue = file.Hash;
			query = file.query;
			iTasks = new Collection<ITask>();
			object[] items = file.InstallTasks.Items;
			foreach (object obj in items)
			{
				if (obj.GetType() == typeof(Microsoft.VSPowerToys.Updater.Xsd.FileMoveTask))
				{
					Microsoft.VSPowerToys.Updater.Xsd.FileMoveTask fileMoveTask = obj as Microsoft.VSPowerToys.Updater.Xsd.FileMoveTask;
					string srcFile = fileMoveTask.srcFile;
					FileMoveTask item = new FileMoveTask(srcFile, fileMoveTask.destFile);
					iTasks.Add(item);
				}
				else if (obj.GetType() == typeof(Microsoft.VSPowerToys.Updater.Xsd.FileUnzipTask))
				{
					Microsoft.VSPowerToys.Updater.Xsd.FileUnzipTask fileUnzipTask = obj as Microsoft.VSPowerToys.Updater.Xsd.FileUnzipTask;
					string srcFile2 = fileUnzipTask.srcFile;
					FileUnzipTask item2 = new FileUnzipTask(srcFile2, fileUnzipTask.destDir);
					iTasks.Add(item2);
				}
			}
		}
	}
}
