using System;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace Microsoft.VSPowerToys.Updater
{
	public class DownloadManifestDlg : Form
	{
		private Uri serverManifestLoc;

		private string localManifestFile;

		private Manifest updateManifest;

		private IContainer components;

		private BackgroundWorker DownloadThread;

		private PrettyPanel prettyPanel1;

		private ProgressBar downloadProgress;

		private Button btnCancelUpdates;

		public Manifest UpdateManifest
		{
			get
			{
				return updateManifest;
			}
		}

		public DownloadManifestDlg(Uri manifestLoc, string localManifest)
		{
			serverManifestLoc = manifestLoc;
			localManifestFile = localManifest;
			InitializeComponent();
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
				ManifestManager manifestManager = new ManifestManager(serverManifestLoc, Application.UserAppDataPath, Application.StartupPath);
				Manifest manifest = (Manifest)(e.Result = manifestManager.GetValidManifest(localManifestFile));
			}
			catch (WebException ex)
			{
				e.Result = null;
				MessageBox.Show(this, Updater.DownloadManifestFailure + ex.Message, Updater.ErrorCaption);
			}
			catch (XmlException ex2)
			{
				MessageBox.Show(this, Updater.DownloadManifestFailure + ex2.Message, Updater.ErrorCaption);
				e.Result = null;
			}
		}

		private void DownloadThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				MessageBox.Show(this, Updater.DownloadManifestFailure + e.Error.Message, Updater.ErrorCaption);
				return;
			}
			if (e.Cancelled)
			{
				Close();
				return;
			}
			Manifest manifest = (Manifest)e.Result;
			if (manifest == null)
			{
				MessageBox.Show(Updater.DownloadFailed);
				updateManifest = null;
			}
			else
			{
				updateManifest = manifest;
			}
			btnCancelUpdates.Enabled = false;
			prettyPanel1.HeaderText = Updater.ManifestDownloaded;
			downloadProgress.Style = ProgressBarStyle.Blocks;
			downloadProgress.Increment(100);
			Thread.Sleep(1000);
			Close();
		}

		private void btnApplyUpdates_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnCancelUpdates_Click(object sender, EventArgs e)
		{
			btnCancelUpdates.Enabled = false;
			DownloadThread.CancelAsync();
		}

		private void DownloadManifestDlg_Load(object sender, EventArgs e)
		{
			btnCancelUpdates.BringToFront();
			DownloadThread.RunWorkerAsync();
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
			DownloadThread = new System.ComponentModel.BackgroundWorker();
			prettyPanel1 = new Microsoft.VSPowerToys.Updater.PrettyPanel();
			btnCancelUpdates = new System.Windows.Forms.Button();
			downloadProgress = new System.Windows.Forms.ProgressBar();
			prettyPanel1.SuspendLayout();
			SuspendLayout();
			DownloadThread.WorkerReportsProgress = true;
			DownloadThread.WorkerSupportsCancellation = true;
			DownloadThread.DoWork += new System.ComponentModel.DoWorkEventHandler(DownloadThread_DoWork);
			DownloadThread.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(DownloadThread_RunWorkerCompleted);
			prettyPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			prettyPanel1.Controls.Add(btnCancelUpdates);
			prettyPanel1.Controls.Add(downloadProgress);
			prettyPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			prettyPanel1.EndGradientColor = System.Drawing.Color.FromArgb(160, 191, 245);
			prettyPanel1.Font = new System.Drawing.Font("Trebuchet MS", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			prettyPanel1.FooterGradientEndColor = System.Drawing.Color.FromArgb(25, 65, 165);
			prettyPanel1.FooterGradientStartColor = System.Drawing.Color.FromArgb(90, 160, 255);
			prettyPanel1.FooterText = null;
			prettyPanel1.HeaderGradientEndColor = System.Drawing.Color.FromArgb(25, 65, 165);
			prettyPanel1.HeaderGradientStartColor = System.Drawing.Color.FromArgb(90, 160, 255);
			prettyPanel1.HeaderText = "Checking for updates. Please wait.....";
			prettyPanel1.Location = new System.Drawing.Point(0, 0);
			prettyPanel1.Name = "prettyPanel1";
			prettyPanel1.Size = new System.Drawing.Size(540, 361);
			prettyPanel1.StartGradientColor = System.Drawing.Color.White;
			prettyPanel1.TabIndex = 6;
			btnCancelUpdates.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			btnCancelUpdates.FlatStyle = System.Windows.Forms.FlatStyle.System;
			btnCancelUpdates.Font = new System.Drawing.Font("Trebuchet MS", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			btnCancelUpdates.Location = new System.Drawing.Point(462, 338);
			btnCancelUpdates.Name = "btnCancelUpdates";
			btnCancelUpdates.Size = new System.Drawing.Size(75, 23);
			btnCancelUpdates.TabIndex = 9;
			btnCancelUpdates.Text = "&Cancel";
			btnCancelUpdates.UseVisualStyleBackColor = true;
			downloadProgress.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
			downloadProgress.ForeColor = System.Drawing.Color.SpringGreen;
			downloadProgress.Location = new System.Drawing.Point(4, 35);
			downloadProgress.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
			downloadProgress.MarqueeAnimationSpeed = 10;
			downloadProgress.Name = "progressBar1";
			downloadProgress.Size = new System.Drawing.Size(527, 23);
			downloadProgress.Step = 20;
			downloadProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			downloadProgress.TabIndex = 6;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			base.ClientSize = new System.Drawing.Size(540, 361);
			base.Controls.Add(prettyPanel1);
			base.Name = "DownloadManifestDlg";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Download";
			base.Load += new System.EventHandler(DownloadManifestDlg_Load);
			prettyPanel1.ResumeLayout(false);
			ResumeLayout(false);
		}
	}
}
