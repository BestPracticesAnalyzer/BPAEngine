using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Microsoft.VSPowerToys.Updater;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	internal class SelectAnalyzer : BPAScreen
	{
		private BPAListBox analyzerListBox;

		private BPAPanel listPanel;

		private BPAPanel fixedPanel;

		private BPALabel totalAnalyzers;

		private int startingTabIndex;

		private int nextTabIndex;

		private BPALabel footerTitle;

		private BPAAnalyzerDataGridView analyzerListGrid;

		private DownloadGuiHelper helper;

		private Collection<Uri> downloadUrls = new Collection<Uri>();

		private event EventHandler<DownloadEventArgs> ReportProgressEvent;

		public SelectAnalyzer(MainGUI mainGUI)
			: base(mainGUI)
		{
			Point borderCornerPoint = MainGUI.BorderCornerPoint;
			Dock = DockStyle.Fill;
			AutoScroll = true;
			BackColor = Color.White;
			base.mainGUI = mainGUI;
			nextTabIndex = mainGUI.StartingTabIndex;
			borderCornerPoint = Navigate.Below(new BPATitle(BPALoc.Label_SelectAnalyzer, borderCornerPoint, mainGUI.FullWidth, Color.White, Color.FromArgb(49, 85, 156), MainGUI.TitleFont, this)
			{
				TabIndex = nextTabIndex++
			});
			listPanel = new BPAPanel(borderCornerPoint, mainGUI.FullWidth - 50, mainGUI.FullHeight - (borderCornerPoint.Y - MainGUI.BorderCornerPoint.Y), this);
			listPanel.TabStop = false;
			listPanel.ResizeFlags = 4;
			listPanel.MinimumSize = new Size(300, 300);
			listPanel.SetOrigRect();
			analyzerListGrid = new BPAAnalyzerDataGridView();
			analyzerListGrid.Initialize();
			analyzerListGrid.Dock = DockStyle.Fill;
			analyzerListGrid.AutoSize = true;
			listPanel.Controls.Add(analyzerListGrid);
			BPAPictureBox bPAPictureBox = new BPAPictureBox(MainGUI.DarkGray, new Point(0, 0), new Size(0, 1), listPanel)
			{
				Dock = DockStyle.Top
			};
			BPAPanel bPAPanel = new BPAPanel(new Point(0, 0), 0, MainGUI.DefaultFont.Height * 2, listPanel);
			bPAPanel.Dock = DockStyle.Top;
			bPAPanel.BorderStyle = BorderStyle.None;
			bPAPanel.TabStop = false;
			bPAPanel.SetOrigRect();
			totalAnalyzers = new BPALabel(location: new Point(MainGUI.DefaultFont.Height / 2, MainGUI.DefaultFont.Height / 2), text: " " + string.Format(Resource.AvailableAnalyzerFormatString, MainGUI.Analyzers.Count), width: listPanel.Width, parent: bPAPanel);
			totalAnalyzers.TabIndex = nextTabIndex++;
			fixedPanel = new BPAPanel(new Point(0, 0), 0, 0, listPanel);
			fixedPanel.BorderStyle = BorderStyle.None;
			fixedPanel.Dock = DockStyle.Top;
			fixedPanel.SetOrigRect();
			fixedPanel.BackColor = MainGUI.DarkGray;
			fixedPanel.TabStop = false;
			startingTabIndex = nextTabIndex;
			helper = new DownloadGuiHelper(this);
			helper.DownloadedManifestEvent += helper_DownloadedManifestEvent;
			helper.DownloadCompleteEvent += helper_DownloadCompleteEvent;
			helper.InstallCompleteEvent += helper_InstallCompleteEvent;
			helper.StartDownloadingServerManifest();
			this.ReportProgressEvent = (EventHandler<DownloadEventArgs>)Delegate.Combine(this.ReportProgressEvent, new EventHandler<DownloadEventArgs>(SelectAnalyzer_ReportProgressEvent));
		}

		private void DrawUnexpandedAnalyzer(ListBox listBox, DrawItemEventArgs e, Rectangle bounds, bool toExpand)
		{
			try
			{
				e.Graphics.PageUnit = GraphicsUnit.Pixel;
				Rectangle rectangle = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
				if (listBox.SelectedIndex == e.Index)
				{
					e.Graphics.FillRectangle(new SolidBrush(MainGUI.MediumGray), rectangle);
				}
				rectangle = new Rectangle(bounds.X + 2, bounds.Y, bounds.Width - 2, bounds.Height);
				if (e.Index != 0)
				{
					rectangle.Height = 1;
					e.Graphics.FillRectangle(new LinearGradientBrush(rectangle, MainGUI.DarkGray, MainGUI.LightGray, 0f, false), rectangle);
				}
				mainGUI.DefaultAppIcon.ToBitmap();
				rectangle = new Rectangle(bounds.X + 4, bounds.Y + 6, 16, 16);
				e.Graphics.DrawIcon(mainGUI.DefaultAppIcon, rectangle);
				string s = (string)listBox.Items[e.Index];
				StringFormat stringFormat = new StringFormat(StringFormatFlags.FitBlackBox | StringFormatFlags.NoWrap | StringFormatFlags.NoClip);
				stringFormat.Trimming = StringTrimming.EllipsisCharacter;
				RectangleF layoutRectangle = new RectangleF(bounds.X + 24, bounds.Y + 8, bounds.Width / 2 - 24, listBox.Font.Height);
				e.Graphics.DrawString(s, MainGUI.DefaultFont, new SolidBrush(SystemColors.ControlText), layoutRectangle, stringFormat);
				if (!toExpand)
				{
					DrawLastLine(e, bounds);
				}
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		private void DrawExpandedAnalyzer(ListBox listBox, DrawItemEventArgs e, Rectangle bounds, bool toExpand)
		{
			DrawUnexpandedAnalyzer(listBox, e, bounds, toExpand);
			DrawLastLine(e, bounds);
		}

		private int MeasureExpandedAnalyzer(ListBox listBox, MeasureItemEventArgs e)
		{
			int result = 0;
			try
			{
				result = MeasureUnexpandedAnalyzer(listBox, e);
				return result;
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
				return result;
			}
		}

		private int MeasureUnexpandedAnalyzer(ListBox listBox, MeasureItemEventArgs e)
		{
			return MainGUI.DefaultFont.Height + 12;
		}

		private void ShowAnalyzerList()
		{
			if (analyzerListGrid.Rows.Count > 0)
			{
				return;
			}
			foreach (object analyzer in MainGUI.Analyzers)
			{
				analyzerListGrid.AddNewDataSetRow(analyzer.ToString(), Resource.RunAnalyzerLabel, PluginStatus.Installed);
			}
			ShowAnalyzerListHeader();
		}

		private void ShowAnalyzerListHeader()
		{
			fixedPanel.Controls.Clear();
			new Point(0, MainGUI.DefaultFont.Height / 2);
			BPALabel bPALabel = new BPALabel(string.Format(Resource.AvailableAnalyzerString, mainGUI.Customizations.ShortName), new Point(5, 0), fixedPanel.Width, fixedPanel);
			bPALabel.Font = new Font("Microsoft Sans Serif", 14f, FontStyle.Regular, GraphicsUnit.Point, 0);
			bPALabel.Size = bPALabel.GetSizeToFit();
			bPALabel.Height += 4;
			bPALabel.SetOrigRect();
			bPALabel.ForeColor = Color.White;
			bPALabel.BackColor = MainGUI.DarkGray;
			bPALabel.TabIndex = nextTabIndex++;
			fixedPanel.Height = bPALabel.Height + MainGUI.DefaultFont.Height / 2;
			if (fixedPanel.Height < 28)
			{
				fixedPanel.Height = 28;
			}
			bPALabel.Top = (fixedPanel.Height - bPALabel.Height) / 2;
			totalAnalyzers.Text = " " + string.Format(Resource.AnalyzerCountFormatString, MainGUI.Analyzers.Count);
			analyzerListGrid.TabIndex = nextTabIndex++;
			analyzerListGrid.RunAnalyzerEvent += mainGUI.RunAnalyzer;
			analyzerListGrid.StatusButtonClickEvent += OnStatusClickEvent;
			footerTitle = new BPALabel(Resource.ManifestUpdating, new Point(5, listPanel.Height - base.Height), fixedPanel.Width, listPanel);
			footerTitle.Dock = DockStyle.Bottom;
			footerTitle.BorderStyle = BorderStyle.FixedSingle;
			footerTitle.Font = new Font("Microsoft Sans Serif", 14f, FontStyle.Regular, GraphicsUnit.Point, 0);
			footerTitle.Size = bPALabel.GetSizeToFit();
			footerTitle.Height += 4;
			footerTitle.SetOrigRect();
			footerTitle.ForeColor = Color.White;
			footerTitle.BackColor = MainGUI.DarkGray;
			footerTitle.TabIndex = nextTabIndex++;
		}

		public override bool Start()
		{
			nextTabIndex = startingTabIndex;
			ShowAnalyzerList();
			return true;
		}

		private void DrawLastLine(DrawItemEventArgs e, Rectangle bounds)
		{
			if (e.Index + 1 >= analyzerListBox.Items.Count || analyzerListBox.Items[e.Index + 1].GetType() == typeof(string))
			{
				Rectangle rect = new Rectangle(bounds.X + 2, bounds.Y + bounds.Height - 1, bounds.Width - 2, 1);
				e.Graphics.FillRectangle(new LinearGradientBrush(rect, MainGUI.DarkGray, MainGUI.LightGray, 0f, false), rect);
			}
		}

		private void UpdateFooterStatusText(string text)
		{
			if (!base.InvokeRequired)
			{
				if (footerTitle != null)
				{
					footerTitle.Text = text;
				}
			}
			else
			{
				Invoke(new UpdateTextHandler(UpdateFooterStatusText), text);
			}
		}

		private void helper_DownloadedManifestEvent(object sender, EventArgs e)
		{
			if (helper.DownloadStatus == DownloadGuiHelper.Status.DownloadFailed)
			{
				UpdateFooterStatusText(Resource.ManifestDownloadFailed);
			}
			else if (helper.DownloadStatus == DownloadGuiHelper.Status.DownloadedManifest)
			{
				UpdateFooterStatusText(Resource.FooterLabel + DateTime.Now.ToShortDateString());
			}
			if (helper.UpdateManifest == null || !helper.UpdateManifest.Apply)
			{
				return;
			}
			foreach (ComponentManifest component in helper.UpdateManifest.Components)
			{
				if (component.UpdateState == ComponentManifest.State.Update)
				{
					PluginStatus status = PluginStatus.UpdatesAvailable;
					analyzerListGrid.UpdateStatus(component.Name, status);
				}
				else if (component.UpdateState == ComponentManifest.State.New)
				{
					PluginStatus status = PluginStatus.New;
					analyzerListGrid.AddNewDataSetRow(component.Name, Resource.RunAnalyzerLabel, status);
				}
			}
		}

		public void OnStatusClickEvent(object sender, StatusButtonClickEventArgs e)
		{
			string analyzerName = e.AnalyzerName;
			Manifest manifest = ((helper.UpdateManifest == null) ? helper.ClientManifest : ((helper.UpdateManifest.Components.Count != 0) ? helper.UpdateManifest : helper.ClientManifest));
			if (manifest == null || manifest.Components == null)
			{
				return;
			}
			ComponentManifest componentManifest = manifest.Components[analyzerName];
			if (componentManifest == null)
			{
				componentManifest = helper.ClientManifest.Components[analyzerName];
			}
			if (componentManifest == null)
			{
				return;
			}
			PluginInfoDlg pluginInfoDlg = new PluginInfoDlg(componentManifest);
			if (componentManifest.UpdateState == ComponentManifest.State.Installed || pluginInfoDlg.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			downloadUrls.Clear();
			foreach (FileManifest file in componentManifest.Files)
			{
				FileUri item = ((!string.IsNullOrEmpty(file.Query)) ? new FileUri(componentManifest.Files.Base + "?" + file.Query, file) : new FileUri(componentManifest.Files.Base + file.Source, file));
				downloadUrls.Add(item);
			}
			analyzerListGrid.UpdateStatus(componentManifest.Name, PluginStatus.Downloading);
			helper.DownloadComponent = componentManifest.Name;
			helper.DownloadFiles(downloadUrls, Application.UserAppDataPath);
		}

		public void ReportProgress(object sender, DownloadEventArgs e)
		{
			analyzerListGrid.UpdateProgress(helper.DownloadComponent, e.PercentDownloaded);
		}

		private void SelectAnalyzer_ReportProgressEvent(object sender, DownloadEventArgs e)
		{
			footerTitle.Text = Resource.DownloadingString + e.FileName;
		}

		public virtual void OnReportProgressEvent(DownloadEventArgs e)
		{
			if (this.ReportProgressEvent != null)
			{
				this.ReportProgressEvent(this, e);
			}
		}

		private void helper_DownloadCompleteEvent(object sender, EventArgs e)
		{
			analyzerListGrid.UpdateStatus(helper.DownloadComponent, PluginStatus.Installing);
		}

		private void helper_InstallCompleteEvent(object sender, EventArgs e)
		{
			analyzerListGrid.UpdateStatus(helper.DownloadComponent, PluginStatus.Installed);
			MainGUI.UpdatePluginList();
		}
	}
}
