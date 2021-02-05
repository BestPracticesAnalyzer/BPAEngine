using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VSPowerToys.Updater;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class DownloadGuiHelper : IDownloadEventListener
	{
		public enum Status
		{
			DownloadingManifest,
			DownloadedManifest,
			DownloadingFiles,
			DownloadFailed
		}

		private Status currentStatus;

		private Control parentForm;

		private Manifest updateManifest;

		private Manifest clientManifest;

		private DownloadManager dm = new DownloadManager();

		private string currentComponentDownload = string.Empty;

		private FileUri serverManifestUri;

		private string localManifestFile;

		private string manifestFileName = ConfigurationManager.AppSettings["ManifestFileName"];

		public Status DownloadStatus
		{
			get
			{
				return currentStatus;
			}
		}

		public string DownloadComponent
		{
			get
			{
				return currentComponentDownload;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException("DownloadComponent");
				}
				currentComponentDownload = value;
			}
		}

		[CLSCompliant(false)]
		public Manifest UpdateManifest
		{
			get
			{
				lock (this)
				{
					return updateManifest;
				}
			}
		}

		[CLSCompliant(false)]
		public Manifest ClientManifest
		{
			get
			{
				lock (this)
				{
					return clientManifest;
				}
			}
		}

		public event EventHandler<EventArgs> DownloadedManifestEvent;

		public event EventHandler<EventArgs> DownloadCompleteEvent;

		public event EventHandler<EventArgs> InstallCompleteEvent;

		public virtual void OnDownloadedManifestEvent(EventArgs e)
		{
			if (this.DownloadedManifestEvent != null)
			{
				this.DownloadedManifestEvent(this, e);
			}
		}

		public DownloadGuiHelper(Control parentFrm)
		{
			if (parentFrm == null)
			{
				throw new ArgumentNullException("parentFrm");
			}
			parentForm = parentFrm;
			serverManifestUri = new FileUri(ConfigurationManager.AppSettings["ManifestUri"], manifestFileName);
			localManifestFile = Application.StartupPath + "\\" + manifestFileName;
		}

		public void StartDownloadingServerManifest()
		{
			WaitCallback callBack = DownloadServerManifest;
			ThreadPool.QueueUserWorkItem(callBack);
		}

		private void DownloadServerManifest(object state)
		{
			lock (this)
			{
				clientManifest = ManifestManager.GetClientManifest(localManifestFile);
			}
			ManifestManager manifestManager = new ManifestManager(serverManifestUri, Application.UserAppDataPath, Application.StartupPath);
			Manifest validManifest = manifestManager.GetValidManifest(localManifestFile);
			lock (this)
			{
				updateManifest = validManifest;
				if (validManifest != null)
				{
					currentStatus = Status.DownloadedManifest;
				}
				else
				{
					currentStatus = Status.DownloadFailed;
				}
			}
			OnDownloadedManifestEvent(new EventArgs());
		}

		public void DownloadFiles(Collection<Uri> downloadUrls, string localDownloadDir)
		{
			dm.RegisterListener(this);
			Pair<Collection<Uri>, string> pair = new Pair<Collection<Uri>, string>(downloadUrls, localDownloadDir);
			WaitCallback callBack = DownloadAll;
			ThreadPool.QueueUserWorkItem(callBack, pair);
		}

		private void DownloadAll(object state)
		{
			Pair<Collection<Uri>, string> pair = (Pair<Collection<Uri>, string>)state;
			ReadOnlyCollection<string> readOnlyCollection = dm.DownloadAll(pair.First, pair.Second);
			int num = 0;
			foreach (FileUri item in pair.First)
			{
				foreach (ITask installTask in item.FileManifest.InstallTasks)
				{
					if (installTask.GetType() == typeof(FileMoveTask))
					{
						FileMoveTask fileMoveTask = installTask as FileMoveTask;
						fileMoveTask.SourceFile = readOnlyCollection[num];
					}
					else if (installTask.GetType() == typeof(FileUnzipTask))
					{
						FileUnzipTask fileUnzipTask = installTask as FileUnzipTask;
						fileUnzipTask.SourceFile = readOnlyCollection[num];
					}
				}
				num++;
			}
			this.DownloadCompleteEvent(this, new EventArgs());
			InstallFiles(pair.First);
			this.InstallCompleteEvent(this, new EventArgs());
		}

		public void OnDownloadEvent(object sender, DownloadEventArgs e)
		{
			SelectAnalyzer selectAnalyzer = (SelectAnalyzer)parentForm;
			selectAnalyzer.ReportProgress(sender, e);
		}

		private void InstallFiles(Collection<Uri> downloadUrls)
		{
			foreach (FileUri downloadUrl in downloadUrls)
			{
				foreach (ITask installTask in downloadUrl.FileManifest.InstallTasks)
				{
					if (installTask.GetType() == typeof(FileMoveTask))
					{
						FileMoveTask fileMoveTask = installTask as FileMoveTask;
						FileInfo fileInfo = new FileInfo(fileMoveTask.DestinationFile);
						if (fileInfo.Extension.Length == 0)
						{
							fileMoveTask.DestinationFile = fileMoveTask.DestinationFile + "\\" + downloadUrl.FileManifest.Source;
						}
						fileMoveTask.Execute();
					}
					else if (installTask.GetType() == typeof(FileUnzipTask))
					{
						FileUnzipTask fileUnzipTask = installTask as FileUnzipTask;
						fileUnzipTask.Execute();
					}
				}
			}
			UpdateClientManifest();
		}

		private void UpdateClientManifest()
		{
			Manifest manifest = ManifestManager.GetClientManifest(Application.StartupPath + "\\" + manifestFileName);
			ComponentManifest componentManifest = updateManifest.Components[currentComponentDownload];
			ComponentManifest componentManifest2 = manifest.Components[currentComponentDownload];
			if (componentManifest2 == null)
			{
				manifest.Components.Add(componentManifest);
			}
			else
			{
				FilesCollection files = componentManifest2.Files;
				FilesCollection files2 = componentManifest.Files;
				foreach (FileManifest item in files2)
				{
					FileManifest fileManifest2 = files[item.Source];
					if (fileManifest2 != null)
					{
						files.Remove(fileManifest2);
					}
					files.Add(item);
				}
			}
			manifest.AppDir = Application.StartupPath;
			manifest.SerializeXml(Application.StartupPath + "\\" + manifestFileName);
		}
	}
}
