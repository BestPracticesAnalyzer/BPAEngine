using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.Updater
{
	public class DownloadFilesDlg : Form, IDownloadEventListener
	{
		private Collection<Uri> fileUrlsToDownload;

		private string localDownloadDir;

		private DownloadManager dm;

		private IContainer components;

		private BackgroundWorker DownloadThread;

		private PrettyPanel prettyPanel1;

		private Button btnCancelUpdates;

		private Button btnInstall;

		private ListView downloadFilesListView;

		private ColumnHeader serial;

		private ColumnHeader File;

		private ColumnHeader Status;

		private ProgressBar downloadProgress;

		public event EventHandler<DownloadEventArgs> UpdateProgress;

		public DownloadFilesDlg(Collection<Uri> downloadUrls, string downloadDir)
		{
			InitializeComponent();
			fileUrlsToDownload = downloadUrls;
			localDownloadDir = downloadDir;
			dm = new DownloadManager();
		}

		private void DownloadThread_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker backgroundWorker = (BackgroundWorker)sender;
			if (backgroundWorker.CancellationPending)
			{
				e.Cancel = true;
				return;
			}
			try
			{
				dm.DownloadAll(fileUrlsToDownload, localDownloadDir);
			}
			catch (Exception ex)
			{
				e.Result = null;
				MessageBox.Show(ex.StackTrace);
			}
		}

		private void DownloadThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			this.UpdateProgress(sender, (DownloadEventArgs)e.UserState);
		}

		private void DownloadThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				MessageBox.Show(this, "Download of files failed with errors:" + e.Error.Message, "Error!");
			}
			else if (e.Cancelled)
			{
				Close();
			}
			else
			{
				btnInstall.Visible = true;
			}
		}

		public void OnDownloadEvent(object sender, DownloadEventArgs e)
		{
			try
			{
				DownloadThread.ReportProgress(e.PercentDownloaded, e);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.StackTrace);
			}
		}

		private void DownloadFilesDlg_Load(object sender, EventArgs e)
		{
			downloadProgress.Style = ProgressBarStyle.Blocks;
			dm.RegisterListener(this);
			this.UpdateProgress = (EventHandler<DownloadEventArgs>)Delegate.Combine(this.UpdateProgress, new EventHandler<DownloadEventArgs>(DownloadFilesDlg_UpdateProgress));
			int num = 1;
			foreach (Uri item in fileUrlsToDownload)
			{
				FileUri fileUri = item as FileUri;
				string fileName = fileUri.FileName;
				ListViewItem listViewItem = new ListViewItem(fileName);
				listViewItem.SubItems.Add(num.ToString());
				listViewItem.SubItems.Add(Updater.Pending);
				downloadFilesListView.Items.Add(listViewItem);
				num++;
			}
			downloadFilesListView.BringToFront();
			btnCancelUpdates.BringToFront();
			btnInstall.BringToFront();
			btnInstall.Visible = false;
			downloadFilesListView.Items[0].Selected = true;
			DownloadThread.RunWorkerAsync();
		}

		private void btnCancelUpdates_Click(object sender, EventArgs e)
		{
			btnCancelUpdates.Enabled = false;
			DownloadThread.CancelAsync();
		}

		private void btnInstall_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void DownloadFilesDlg_UpdateProgress(object sender, DownloadEventArgs e)
		{
			try
			{
				downloadProgress.Value = e.PercentDownloaded;
				prettyPanel1.HeaderText = Updater.Downloading + e.FileName + Updater.from + e.Url.DnsSafeHost;
				ListViewItem listViewItem = downloadFilesListView.FindItemWithText(e.FileName, false, 0, false);
				if (listViewItem != null)
				{
					if (e.PercentDownloaded == 100)
					{
						listViewItem.SubItems[2].Text = Updater.Done;
					}
					else
					{
						listViewItem.Selected = true;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.StackTrace);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Microsoft.VSPowerToys.Updater.DownloadFilesDlg));
			DownloadThread = new System.ComponentModel.BackgroundWorker();
			prettyPanel1 = new Microsoft.VSPowerToys.Updater.PrettyPanel();
			btnInstall = new System.Windows.Forms.Button();
			btnCancelUpdates = new System.Windows.Forms.Button();
			downloadProgress = new System.Windows.Forms.ProgressBar();
			downloadFilesListView = new System.Windows.Forms.ListView();
			File = new System.Windows.Forms.ColumnHeader();
			serial = new System.Windows.Forms.ColumnHeader();
			Status = new System.Windows.Forms.ColumnHeader();
			prettyPanel1.SuspendLayout();
			SuspendLayout();
			DownloadThread.WorkerReportsProgress = true;
			DownloadThread.WorkerSupportsCancellation = true;
			DownloadThread.DoWork += new System.ComponentModel.DoWorkEventHandler(DownloadThread_DoWork);
			DownloadThread.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(DownloadThread_RunWorkerCompleted);
			DownloadThread.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(DownloadThread_ProgressChanged);
			resources.ApplyResources(prettyPanel1, "prettyPanel1");
			prettyPanel1.Controls.Add(btnInstall);
			prettyPanel1.Controls.Add(btnCancelUpdates);
			prettyPanel1.Controls.Add(downloadProgress);
			prettyPanel1.Controls.Add(downloadFilesListView);
			prettyPanel1.EndGradientColor = System.Drawing.Color.FromArgb(160, 191, 245);
			prettyPanel1.FooterGradientEndColor = System.Drawing.Color.FromArgb(25, 65, 165);
			prettyPanel1.FooterGradientStartColor = System.Drawing.Color.FromArgb(90, 160, 255);
			prettyPanel1.FooterText = null;
			prettyPanel1.HeaderGradientEndColor = System.Drawing.Color.FromArgb(25, 65, 165);
			prettyPanel1.HeaderGradientStartColor = System.Drawing.Color.FromArgb(90, 160, 255);
			prettyPanel1.HeaderText = Microsoft.VSPowerToys.Updater.Updater.Downloading;
			prettyPanel1.Name = "prettyPanel1";
			prettyPanel1.StartGradientColor = System.Drawing.Color.White;
			btnInstall.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(btnInstall, "btnInstall");
			btnInstall.Name = "btnInstall";
			btnInstall.UseVisualStyleBackColor = true;
			btnInstall.Click += new System.EventHandler(btnInstall_Click);
			btnCancelUpdates.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(btnCancelUpdates, "btnCancelUpdates");
			btnCancelUpdates.Name = "btnCancelUpdates";
			btnCancelUpdates.UseVisualStyleBackColor = true;
			downloadProgress.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
			downloadProgress.ForeColor = System.Drawing.Color.SpringGreen;
			resources.ApplyResources(downloadProgress, "downloadProgress");
			downloadProgress.Name = "downloadProgress";
			downloadProgress.Step = 1;
			downloadFilesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[3]
			{
				File,
				serial,
				Status
			});
			resources.ApplyResources(downloadFilesListView, "listView1");
			downloadFilesListView.FullRowSelect = true;
			downloadFilesListView.GridLines = true;
			downloadFilesListView.Name = "listView1";
			downloadFilesListView.UseCompatibleStateImageBehavior = false;
			downloadFilesListView.View = System.Windows.Forms.View.Details;
			resources.ApplyResources(File, "File");
			resources.ApplyResources(serial, "serial");
			resources.ApplyResources(Status, "Status");
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(prettyPanel1);
			base.Name = "DownloadFilesDlg";
			base.Load += new System.EventHandler(DownloadFilesDlg_Load);
			prettyPanel1.ResumeLayout(false);
			ResumeLayout(false);
		}
	}
}
