using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.Updater
{
	public class UpdateYesNoDlg : Form
	{
		private IContainer components;

		private PrettyPanel prettyPanel1;

		private Button btnCheckUpdates;

		private Button btnCancel;

		private PrettyLabel prettyLabel2;

		private Manifest updatedManifest;

		private ReadOnlyCollection<string> scomponents;

		private string manifestFileName = ConfigurationManager.AppSettings["ManifestFileName"];

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Microsoft.VSPowerToys.Updater.UpdateYesNoDlg));
			btnCheckUpdates = new System.Windows.Forms.Button();
			btnCancel = new System.Windows.Forms.Button();
			prettyPanel1 = new Microsoft.VSPowerToys.Updater.PrettyPanel();
			prettyLabel2 = new Microsoft.VSPowerToys.Updater.PrettyLabel();
			prettyPanel1.SuspendLayout();
			SuspendLayout();
			btnCheckUpdates.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			btnCheckUpdates.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			btnCheckUpdates.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			btnCheckUpdates.Image = (System.Drawing.Image)resources.GetObject("btnCheckUpdates.Image");
			btnCheckUpdates.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			btnCheckUpdates.Location = new System.Drawing.Point(302, 122);
			btnCheckUpdates.Name = "btnCheckUpdates";
			btnCheckUpdates.Size = new System.Drawing.Size(89, 38);
			btnCheckUpdates.TabIndex = 2;
			btnCheckUpdates.Text = "&Update";
			btnCheckUpdates.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			btnCheckUpdates.UseVisualStyleBackColor = true;
			btnCheckUpdates.Click += new System.EventHandler(btnCheckUpdates_Click);
			btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
			btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			btnCancel.Image = (System.Drawing.Image)resources.GetObject("btnCancel.Image");
			btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			btnCancel.Location = new System.Drawing.Point(302, 184);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new System.Drawing.Size(89, 38);
			btnCancel.TabIndex = 3;
			btnCancel.Text = "&Cancel";
			btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			btnCancel.UseVisualStyleBackColor = true;
			btnCancel.Click += new System.EventHandler(btnCancel_Click);
			prettyPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			prettyPanel1.Controls.Add(btnCancel);
			prettyPanel1.Controls.Add(btnCheckUpdates);
			prettyPanel1.Controls.Add(prettyLabel2);
			prettyPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			prettyPanel1.EndGradientColor = System.Drawing.Color.FromArgb(160, 191, 245);
			prettyPanel1.Font = new System.Drawing.Font("Trebuchet MS", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			prettyPanel1.FooterGradientEndColor = System.Drawing.Color.FromArgb(25, 65, 165);
			prettyPanel1.FooterGradientStartColor = System.Drawing.Color.FromArgb(90, 160, 255);
			prettyPanel1.FooterText = null;
			prettyPanel1.HeaderGradientEndColor = System.Drawing.Color.FromArgb(25, 65, 165);
			prettyPanel1.HeaderGradientStartColor = System.Drawing.Color.FromArgb(90, 160, 255);
			prettyPanel1.HeaderText = "Do you want to check for updates?";
			prettyPanel1.Location = new System.Drawing.Point(0, 0);
			prettyPanel1.Name = "prettyPanel1";
			prettyPanel1.Size = new System.Drawing.Size(540, 361);
			prettyPanel1.StartGradientColor = System.Drawing.Color.White;
			prettyPanel1.TabIndex = 1;
			prettyLabel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			prettyLabel2.EndGradientColor = System.Drawing.SystemColors.Info;
			prettyLabel2.Font = new System.Drawing.Font("Trebuchet MS", 9.75f, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, 0);
			prettyLabel2.Location = new System.Drawing.Point(62, 74);
			prettyLabel2.Name = "prettyLabel2";
			prettyLabel2.Size = new System.Drawing.Size(192, 175);
			prettyLabel2.StartGradientColor = System.Drawing.Color.White;
			prettyLabel2.TabIndex = 4;
			prettyLabel2.Text = "Clicking on update would fetch the manifest from the update server. Cancel would cancel the update and start the application";
			base.AcceptButton = btnCheckUpdates;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			base.CancelButton = btnCancel;
			base.ClientSize = new System.Drawing.Size(540, 361);
			base.ControlBox = false;
			base.Controls.Add(prettyPanel1);
			DoubleBuffered = true;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "UpdateYesNoDlg";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Welcome to VS Power Toy Update";
			base.TopMost = true;
			base.Load += new System.EventHandler(UpdateYesNoDlg_Load);
			prettyPanel1.ResumeLayout(false);
			ResumeLayout(false);
		}

		public UpdateYesNoDlg()
		{
			InitializeComponent();
			DoubleBuffered = true;
		}

		private void btnCheckUpdates_Click(object sender, EventArgs e)
		{
			FileUri manifestLoc = new FileUri(ConfigurationManager.AppSettings["ManifestUri"], manifestFileName);
			DownloadManifestDlg downloadManifestDlg = new DownloadManifestDlg(manifestLoc, Application.StartupPath + "\\" + manifestFileName);
			base.Visible = false;
			downloadManifestDlg.ShowDialog();
			updatedManifest = downloadManifestDlg.UpdateManifest;
			updatedManifest.AppDir = Application.StartupPath;
			if (updatedManifest != null && updatedManifest.Apply)
			{
				SelectUpdatesDlg selectUpdatesDlg = new SelectUpdatesDlg(updatedManifest);
				base.Visible = false;
				if (selectUpdatesDlg.ShowDialog() != DialogResult.Cancel && selectUpdatesDlg.SelectedComponents.Count > 0)
				{
					scomponents = selectUpdatesDlg.SelectedComponents;
					selectUpdatesDlg.Close();
					Collection<Uri> collection = new Collection<Uri>();
					foreach (string scomponent in scomponents)
					{
						FilesCollection files = updatedManifest.Components[scomponent].Files;
						foreach (FileManifest item2 in files)
						{
							FileUri item = ((!string.IsNullOrEmpty(item2.Query)) ? new FileUri(files.Base + "?" + item2.Query, item2) : new FileUri(files.Base + item2.Source, item2));
							collection.Add(item);
						}
					}
					DownloadFilesDlg downloadFilesDlg = new DownloadFilesDlg(collection, Application.UserAppDataPath);
					if (downloadFilesDlg.ShowDialog() == DialogResult.OK)
					{
						foreach (FileUri item3 in collection)
						{
							foreach (ITask installTask in item3.FileManifest.InstallTasks)
							{
								if (installTask.GetType() == typeof(FileMoveTask))
								{
									FileMoveTask fileMoveTask = installTask as FileMoveTask;
									FileInfo fileInfo = new FileInfo(fileMoveTask.DestinationFile);
									if (fileInfo.Extension.Length == 0)
									{
										fileMoveTask.DestinationFile = fileMoveTask.DestinationFile + "\\" + item3.FileManifest.Source;
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
				}
			}
			RunApplication();
		}

		private void UpdateYesNoDlg_Load(object sender, EventArgs e)
		{
			btnCheckUpdates.BringToFront();
			btnCancel.BringToFront();
			base.Visible = true;
		}

		private void RunApplication()
		{
			ProcessStartInfo processStartInfo = new ProcessStartInfo(updatedManifest.Application, updatedManifest.ApplicationParams);
			processStartInfo.UseShellExecute = false;
			processStartInfo.CreateNoWindow = true;
			Process.Start(processStartInfo);
			base.Visible = false;
			Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			updatedManifest = ManifestManager.GetClientManifest(Application.StartupPath + "\\" + manifestFileName);
			updatedManifest.AppDir = Application.StartupPath;
			RunApplication();
		}

		private void UpdateClientManifest()
		{
			Manifest clientManifest = ManifestManager.GetClientManifest(Application.StartupPath + "\\" + manifestFileName);
			foreach (string scomponent in scomponents)
			{
				ComponentManifest componentManifest = updatedManifest.Components[scomponent];
				ComponentManifest componentManifest2 = clientManifest.Components[scomponent];
				if (componentManifest2 == null)
				{
					clientManifest.Components.Add(componentManifest);
					continue;
				}
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
			clientManifest.AppDir = Application.StartupPath;
			clientManifest.SerializeXml(Application.StartupPath + "\\" + manifestFileName);
		}
	}
}
