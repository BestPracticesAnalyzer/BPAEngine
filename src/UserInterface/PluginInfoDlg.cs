using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.VSPowerToys.Updater;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class PluginInfoDlg : Form
	{
		private IContainer components;

		private GradientFlowLayOutPanel flowLayoutPanel1;

		private PrettyLabel gradientLabel2;

		private Button btnApplyUpdates;

		private WebBrowser webBrowser;

		private Button btnCancelUpdates;

		private ComponentManifest pluginInfo;

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
			flowLayoutPanel1 = new Microsoft.VSPowerToys.Updater.GradientFlowLayOutPanel();
			btnApplyUpdates = new System.Windows.Forms.Button();
			gradientLabel2 = new Microsoft.VSPowerToys.Updater.PrettyLabel();
			webBrowser = new System.Windows.Forms.WebBrowser();
			btnCancelUpdates = new System.Windows.Forms.Button();
			flowLayoutPanel1.SuspendLayout();
			SuspendLayout();
			flowLayoutPanel1.Controls.Add(btnApplyUpdates);
			flowLayoutPanel1.Controls.Add(btnCancelUpdates);
			flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			flowLayoutPanel1.Location = new System.Drawing.Point(0, 328);
			flowLayoutPanel1.Name = "flowLayoutPanel1";
			flowLayoutPanel1.Size = new System.Drawing.Size(540, 33);
			flowLayoutPanel1.TabIndex = 1;
			btnApplyUpdates.DialogResult = System.Windows.Forms.DialogResult.OK;
			btnApplyUpdates.Dock = System.Windows.Forms.DockStyle.Bottom;
			btnApplyUpdates.FlatStyle = System.Windows.Forms.FlatStyle.System;
			btnApplyUpdates.Location = new System.Drawing.Point(462, 3);
			btnApplyUpdates.Name = "btnApplyUpdates";
			btnApplyUpdates.Size = new System.Drawing.Size(75, 23);
			btnApplyUpdates.TabIndex = 0;
			btnApplyUpdates.Text = "&Download";
			btnApplyUpdates.UseVisualStyleBackColor = true;
			btnApplyUpdates.Click += new System.EventHandler(btnApplyUpdates_Click_1);
			gradientLabel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			gradientLabel2.Dock = System.Windows.Forms.DockStyle.Top;
			gradientLabel2.EndGradientColor = System.Drawing.Color.FromArgb(25, 65, 165);
			gradientLabel2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			gradientLabel2.Location = new System.Drawing.Point(0, 0);
			gradientLabel2.Name = "gradientLabel2";
			gradientLabel2.Size = new System.Drawing.Size(540, 24);
			gradientLabel2.StartGradientColor = System.Drawing.Color.FromArgb(90, 160, 255);
			gradientLabel2.TabIndex = 3;
			webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			webBrowser.IsWebBrowserContextMenuEnabled = false;
			webBrowser.Location = new System.Drawing.Point(0, 24);
			webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			webBrowser.Name = "webBrowser";
			webBrowser.ScriptErrorsSuppressed = true;
			webBrowser.Size = new System.Drawing.Size(540, 304);
			webBrowser.TabIndex = 4;
			btnCancelUpdates.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			btnCancelUpdates.Dock = System.Windows.Forms.DockStyle.Bottom;
			btnCancelUpdates.FlatStyle = System.Windows.Forms.FlatStyle.System;
			btnCancelUpdates.Location = new System.Drawing.Point(381, 3);
			btnCancelUpdates.Name = "btnCancelUpdates";
			btnCancelUpdates.Size = new System.Drawing.Size(75, 23);
			btnCancelUpdates.TabIndex = 3;
			btnCancelUpdates.Text = "&Cancel";
			btnCancelUpdates.UseVisualStyleBackColor = true;
			base.AcceptButton = btnApplyUpdates;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			BackColor = System.Drawing.SystemColors.Control;
			base.CancelButton = btnCancelUpdates;
			base.ClientSize = new System.Drawing.Size(540, 361);
			base.Controls.Add(webBrowser);
			base.Controls.Add(gradientLabel2);
			base.Controls.Add(flowLayoutPanel1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Name = "PluginInfoDlg";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Update Details";
			base.Load += new System.EventHandler(PluginInfoDlg_Load);
			flowLayoutPanel1.ResumeLayout(false);
			ResumeLayout(false);
		}

		public PluginInfoDlg(ComponentManifest cm)
		{
			InitializeComponent();
			pluginInfo = cm;
		}

		private void btnApplyUpdates_Click(object sender, EventArgs e)
		{
		}

		private void btnCancelUpdates_Click(object sender, EventArgs e)
		{
		}

		private void ShowDetails()
		{
			webBrowser.Navigate("about:blank");
			HtmlDocument document = webBrowser.Document;
			document.Write(string.Empty);
			StringBuilder stringBuilder = new StringBuilder("<html><body style=\"font-family:'" + Control.DefaultFont.Name + "'; font-size:" + Control.DefaultFont.SizeInPoints + "pt;\">");
			stringBuilder.Append("<h4>");
			stringBuilder.Append(pluginInfo.Name);
			stringBuilder.Append("</h4>");
			stringBuilder.Append("<ul>");
			foreach (FileManifest file in pluginInfo.Files)
			{
				stringBuilder.Append("<li>" + file.Source + "</li>");
			}
			stringBuilder.Append("</ul>");
			stringBuilder.Append("<h5><b>Date Released: </b>");
			stringBuilder.Append(pluginInfo.LastUpdated.ToShortDateString());
			stringBuilder.Append("</h5><p>");
			stringBuilder.Append(pluginInfo.Description);
			stringBuilder.Append("</p></body></html>");
			webBrowser.DocumentText = stringBuilder.ToString();
		}

		private void PluginInfoDlg_Load(object sender, EventArgs e)
		{
			ShowDetails();
		}

		private void btnApplyUpdates_Click_1(object sender, EventArgs e)
		{
		}
	}
}
