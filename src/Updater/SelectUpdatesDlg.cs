using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.Updater
{
	public class SelectUpdatesDlg : Form
	{
		private IContainer components;

		private Button btnCancelUpdates;

		private Button btnApplyUpdates;

		private GradientFlowLayOutPanel flowLayoutPanel1;

		private BackgroundWorker manifestDownload;

		private SplitContainer splitContainer;

		private PrettyLabel gradientLabel1;

		private PrettyLabel gradientLabel2;

		private ListView selectComponentsListBox;

		private ColumnHeader NameCol;

		private ColumnHeader State;

		private WebBrowser webBrowser;

		private Manifest updateManifest;

		public ReadOnlyCollection<string> SelectedComponents
		{
			get
			{
				Collection<string> collection = new Collection<string>();
				foreach (ListViewItem checkedItem in selectComponentsListBox.CheckedItems)
				{
					collection.Add(checkedItem.Text);
				}
				return new ReadOnlyCollection<string>(collection);
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
			manifestDownload = new System.ComponentModel.BackgroundWorker();
			splitContainer = new System.Windows.Forms.SplitContainer();
			selectComponentsListBox = new System.Windows.Forms.ListView();
			NameCol = new System.Windows.Forms.ColumnHeader();
			State = new System.Windows.Forms.ColumnHeader();
			gradientLabel1 = new Microsoft.VSPowerToys.Updater.PrettyLabel();
			webBrowser = new System.Windows.Forms.WebBrowser();
			gradientLabel2 = new Microsoft.VSPowerToys.Updater.PrettyLabel();
			btnApplyUpdates = new System.Windows.Forms.Button();
			btnCancelUpdates = new System.Windows.Forms.Button();
			flowLayoutPanel1 = new Microsoft.VSPowerToys.Updater.GradientFlowLayOutPanel();
			splitContainer.Panel1.SuspendLayout();
			splitContainer.Panel2.SuspendLayout();
			splitContainer.SuspendLayout();
			flowLayoutPanel1.SuspendLayout();
			SuspendLayout();
			splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			splitContainer.Location = new System.Drawing.Point(0, 0);
			splitContainer.Name = "splitContainer";
			splitContainer.Panel1.Controls.Add(selectComponentsListBox);
			splitContainer.Panel1.Controls.Add(gradientLabel1);
			splitContainer.Panel2.BackColor = System.Drawing.SystemColors.Control;
			splitContainer.Panel2.Controls.Add(webBrowser);
			splitContainer.Panel2.Controls.Add(gradientLabel2);
			splitContainer.Size = new System.Drawing.Size(540, 328);
			splitContainer.SplitterDistance = 180;
			splitContainer.SplitterWidth = 1;
			splitContainer.TabIndex = 2;
			selectComponentsListBox.AutoArrange = false;
			selectComponentsListBox.CheckBoxes = true;
			selectComponentsListBox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[2]
			{
				NameCol,
				State
			});
			selectComponentsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			selectComponentsListBox.Font = new System.Drawing.Font("Courier New", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			selectComponentsListBox.FullRowSelect = true;
			selectComponentsListBox.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			selectComponentsListBox.HideSelection = false;
			selectComponentsListBox.Location = new System.Drawing.Point(0, 24);
			selectComponentsListBox.Name = "selectComponentsListBox";
			selectComponentsListBox.ShowGroups = false;
			selectComponentsListBox.ShowItemToolTips = true;
			selectComponentsListBox.Size = new System.Drawing.Size(180, 304);
			selectComponentsListBox.TabIndex = 1;
			selectComponentsListBox.UseCompatibleStateImageBehavior = false;
			selectComponentsListBox.View = System.Windows.Forms.View.Details;
			selectComponentsListBox.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(selectComponentsListBox_ItemSelectionChanged);
			NameCol.Text = "Name";
			NameCol.Width = 25;
			State.Text = "";
			State.Width = 12;
			gradientLabel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			gradientLabel1.Dock = System.Windows.Forms.DockStyle.Top;
			gradientLabel1.EndGradientColor = System.Drawing.Color.FromArgb(25, 65, 165);
			gradientLabel1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			gradientLabel1.Location = new System.Drawing.Point(0, 0);
			gradientLabel1.Name = "gradientLabel1";
			gradientLabel1.Size = new System.Drawing.Size(180, 24);
			gradientLabel1.StartGradientColor = System.Drawing.Color.FromArgb(90, 160, 255);
			gradientLabel1.TabIndex = 0;
			gradientLabel1.Text = "Components";
			webBrowser.AllowWebBrowserDrop = false;
			webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			webBrowser.Location = new System.Drawing.Point(0, 24);
			webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			webBrowser.Name = "webBrowser";
			webBrowser.ScriptErrorsSuppressed = true;
			webBrowser.Size = new System.Drawing.Size(359, 304);
			webBrowser.TabIndex = 2;
			gradientLabel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			gradientLabel2.Dock = System.Windows.Forms.DockStyle.Top;
			gradientLabel2.EndGradientColor = System.Drawing.Color.FromArgb(25, 65, 165);
			gradientLabel2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			gradientLabel2.Location = new System.Drawing.Point(0, 0);
			gradientLabel2.Name = "gradientLabel2";
			gradientLabel2.Size = new System.Drawing.Size(359, 24);
			gradientLabel2.StartGradientColor = System.Drawing.Color.FromArgb(90, 160, 255);
			gradientLabel2.TabIndex = 1;
			gradientLabel2.Text = "Properties";
			btnApplyUpdates.DialogResult = System.Windows.Forms.DialogResult.OK;
			btnApplyUpdates.Dock = System.Windows.Forms.DockStyle.Bottom;
			btnApplyUpdates.FlatStyle = System.Windows.Forms.FlatStyle.System;
			btnApplyUpdates.Location = new System.Drawing.Point(381, 3);
			btnApplyUpdates.Name = "btnApplyUpdates";
			btnApplyUpdates.Size = new System.Drawing.Size(75, 23);
			btnApplyUpdates.TabIndex = 1;
			btnApplyUpdates.Text = "&Apply";
			btnApplyUpdates.UseVisualStyleBackColor = true;
			btnApplyUpdates.Click += new System.EventHandler(btnApplyUpdates_Click);
			btnCancelUpdates.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			btnCancelUpdates.Dock = System.Windows.Forms.DockStyle.Bottom;
			btnCancelUpdates.FlatStyle = System.Windows.Forms.FlatStyle.System;
			btnCancelUpdates.Location = new System.Drawing.Point(462, 3);
			btnCancelUpdates.Name = "btnCancelUpdates";
			btnCancelUpdates.Size = new System.Drawing.Size(75, 23);
			btnCancelUpdates.TabIndex = 0;
			btnCancelUpdates.Text = "&Cancel";
			btnCancelUpdates.UseVisualStyleBackColor = true;
			btnCancelUpdates.Click += new System.EventHandler(btnCancelUpdates_Click);
			flowLayoutPanel1.Controls.Add(btnCancelUpdates);
			flowLayoutPanel1.Controls.Add(btnApplyUpdates);
			flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			flowLayoutPanel1.Location = new System.Drawing.Point(0, 328);
			flowLayoutPanel1.Name = "flowLayoutPanel1";
			flowLayoutPanel1.Size = new System.Drawing.Size(540, 33);
			flowLayoutPanel1.TabIndex = 1;
			base.AcceptButton = btnApplyUpdates;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			BackColor = System.Drawing.SystemColors.Control;
			base.CancelButton = btnCancelUpdates;
			base.ClientSize = new System.Drawing.Size(540, 361);
			base.Controls.Add(splitContainer);
			base.Controls.Add(flowLayoutPanel1);
			base.Name = "SelectUpdatesDlg";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Select Updates to apply";
			base.Load += new System.EventHandler(SelectUpdatesDlg_Load);
			splitContainer.Panel1.ResumeLayout(false);
			splitContainer.Panel2.ResumeLayout(false);
			splitContainer.ResumeLayout(false);
			flowLayoutPanel1.ResumeLayout(false);
			ResumeLayout(false);
		}

		public SelectUpdatesDlg(Manifest m)
		{
			InitializeComponent();
			updateManifest = m;
		}

		private void btnApplyUpdates_Click(object sender, EventArgs e)
		{
		}

		private void btnCancelUpdates_Click(object sender, EventArgs e)
		{
		}

		private void SelectUpdatesDlg_Load(object sender, EventArgs e)
		{
			Font font = new Font(new FontFamily("Courier New"), 8.25f, FontStyle.Bold | FontStyle.Italic);
			foreach (ComponentManifest component in updateManifest.Components)
			{
				ListViewItem listViewItem = selectComponentsListBox.Items.Add(component.Name);
				listViewItem.UseItemStyleForSubItems = false;
				Color foreColor = ((component.UpdateState != 0) ? Color.Blue : Color.Green);
				ListViewItem.ListViewSubItem listViewSubItem = listViewItem.SubItems.Add(component.UpdateState.ToString());
				listViewSubItem.ForeColor = foreColor;
				listViewSubItem.Font = font;
			}
			selectComponentsListBox.Columns[0].Width = -1;
			selectComponentsListBox.Columns[1].Width = -1;
		}

		private void selectComponentsListBox_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if (selectComponentsListBox.FocusedItem == null)
			{
				return;
			}
			webBrowser.Navigate("about:blank");
			HtmlDocument document = webBrowser.Document;
			string text = selectComponentsListBox.FocusedItem.Text;
			ComponentManifest componentManifest = updateManifest.Components[text];
			document.Write(string.Empty);
			StringBuilder stringBuilder = new StringBuilder("<html><body><h4>");
			stringBuilder.Append(componentManifest.Name);
			stringBuilder.Append("</h4><h6>");
			if (componentManifest.UpdateState == ComponentManifest.State.New)
			{
				stringBuilder.Append("New!");
			}
			else
			{
				stringBuilder.Append("Update!");
			}
			stringBuilder.Append("</h6><br>");
			stringBuilder.Append(componentManifest.Description);
			stringBuilder.Append("<br><br><b>Last Updated:</b> ");
			stringBuilder.Append(componentManifest.LastUpdated.ToString());
			stringBuilder.Append("<br><br><b>Updated Files:</b><br>");
			foreach (FileManifest file in componentManifest.Files)
			{
				stringBuilder.Append("<h5>");
				stringBuilder.Append(file.Source);
				stringBuilder.Append("</h5>");
			}
			stringBuilder.Append("</body></html>");
			webBrowser.DocumentText = stringBuilder.ToString();
		}
	}
}
