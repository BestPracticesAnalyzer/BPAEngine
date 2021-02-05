using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	internal class StartScan : InProgressPanel
	{
		private BPAPanel statusPanel;

		private BPALabel scanObjectCount;

		private BPAListBox objectListBox;

		private SortedList objectListToProcess;

		private SortedList sortedDisplayList;

		private int objectsPending;

		private int objectsInProgress;

		private int objectsCompleted;

		private int topOfStatusPanel;

		private ManualResetEvent stopSaveEvent;

		public BPAPanel StatusPanel
		{
			get
			{
				return statusPanel;
			}
		}

		public StartScan(MainGUI mainGUI)
			: base(mainGUI)
		{
			objectListToProcess = new SortedList();
			stopSaveEvent = new ManualResetEvent(false);
			base.Title = BPALoc.Label_IPTitle;
			base.PleaseWait = BPALoc.Label_IPPleaseWait(mainGUI.Customizations.ShortName);
			base.EstimatedTime = BPALoc.Label_IPEstimatedTime;
			Point nextLocationPoint = base.NextLocationPoint;
			nextLocationPoint = Navigate.Indent(nextLocationPoint);
			BPALink bPALink = new BPALink(mainGUI, MainGUI.Actions.StopScan, false, BPALoc.LinkLabel_IPStopScan, mainGUI.ArrowPic, nextLocationPoint, 0, this);
			bPALink.SetTabIndex(nextTabIndex++);
			nextLocationPoint = Navigate.Below(bPALink);
			nextLocationPoint = Navigate.UnIndent(nextLocationPoint);
			nextLocationPoint = Navigate.Below(new BPALabel(BPALoc.Label_IPDetails, nextLocationPoint, 0, this)
			{
				TabIndex = nextTabIndex++
			});
			statusPanel = new BPAPanel(nextLocationPoint, mainGUI.FullWidth, mainGUI.FullHeight - (nextLocationPoint.Y - MainGUI.BorderCornerPoint.Y), this);
			statusPanel.TabStop = false;
			statusPanel.ResizeFlags = 4;
			topOfStatusPanel = statusPanel.Top;
			nextLocationPoint = new Point(0, 0);
			objectListBox = new BPAListBox(mainGUI, DrawServer, null, MeasureServer, null, false, statusPanel);
			objectListBox.TabIndex = nextTabIndex++;
			objectListBox.Dock = DockStyle.Fill;
			BPAPictureBox bPAPictureBox = new BPAPictureBox(MainGUI.DarkGray, new Point(0, 0), new Size(0, 1), statusPanel)
			{
				Dock = DockStyle.Top
			};
			BPAPanel bPAPanel = new BPAPanel(new Point(0, 0), 0, MainGUI.DefaultFont.Height * 2, statusPanel);
			bPAPanel.BorderStyle = BorderStyle.None;
			bPAPanel.Dock = DockStyle.Top;
			bPAPanel.TabStop = false;
			bPAPanel.SetOrigRect();
			scanObjectCount = new BPALabel(location: new Point(MainGUI.DefaultFont.Height / 2, MainGUI.DefaultFont.Height / 2), text: BPALoc.Label_IPTotalServers(BPALoc.Label_IPTotalServersPending(0)), width: statusPanel.Width, parent: bPAPanel);
			scanObjectCount.TabIndex = nextTabIndex++;
		}

		public void Done(ExecutionStatus status)
		{
			if (!base.InvokeRequired)
			{
				StopTimerThread();
				return;
			}
			CompletedCallback method = Done;
			BeginInvoke(method, status);
		}

		public override bool Start()
		{
			try
			{
				mainGUI.ExecInterface.Options.Operations = OperationsFlags.Collect | OperationsFlags.Analyze;
				mainGUI.ExecInterface.Options.SaveInterval = 1800;
				mainGUI.ExecInterface.Options.LoadDataOnRun = false;
				Directory.SetCurrentDirectory(mainGUI.RegSettings.DataDirectory);
				mainGUI.ExecInterface.Options.Completed = Done;
				mainGUI.ExecInterface.Options.Progress = Status;
				mainGUI.ExecInterface.Options.Data.FileName = "";
				BPAScanInfo bPAScanInfo = new BPAScanInfo(mainGUI.ExecInterface.Options);
				mainGUI.Customizations.GetBPAScanInfo(bPAScanInfo, null);
				if (mainGUI.RegSettings.SQMEnabled)
				{
					ArrayList arrayList = new ArrayList();
					arrayList.Add("0 SQM Data Generation:Enable");
					mainGUI.ExecInterface.Options.SetProcessorOptions(arrayList);
				}
				base.EstimatedTimeTotal = bPAScanInfo.EstimatedTime;
				StartTimerThread();
				DisjoinProgressBar();
				if (statusPanel.Parent != null)
				{
					statusPanel.Parent.Controls.Remove(statusPanel);
				}
				statusPanel.Top = topOfStatusPanel;
				statusPanel.Height = mainGUI.FullHeight - (statusPanel.Top - MainGUI.BorderCornerPoint.Y);
				base.Controls.Add(statusPanel);
				PopulateScanObjects(bPAScanInfo);
				statusPanel.SetOrigRect();
				objectListBox.SetOrigRect();
				mainGUI.ExecInterface.Start(true);
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
				mainGUI.TakeAction(MainGUI.Actions.ShowCustomScreen, null, BPALoc.Label_SSStartError);
				return false;
			}
			return true;
		}

		public void Status(string name, ObjectProgress progress)
		{
			if (!base.InvokeRequired)
			{
				if (mainGUI.ExecInterface.Aborting)
				{
					return;
				}
				if (progress.GlobalProgress)
				{
					int percentageDone = progress.PercentageDone;
					if (percentageDone > 95)
					{
						percentageDone = 95;
					}
					BaseStatus(progress.PercentageDone);
				}
				else
				{
					if (name.Length == 0)
					{
						return;
					}
					BPAScanObjectInfo bPAScanObjectInfo = null;
					if (objectListToProcess.Contains(name))
					{
						bPAScanObjectInfo = (BPAScanObjectInfo)objectListToProcess[name];
					}
					else
					{
						bPAScanObjectInfo = new BPAScanObjectInfo(name);
						InsertScanObjectIntoList(bPAScanObjectInfo);
						objectsPending++;
					}
					bPAScanObjectInfo.PctDone = progress.PercentageDone;
					if (bPAScanObjectInfo.ScanStatus == MainGUI.ScanStatus.Pending)
					{
						bPAScanObjectInfo.ScanStatus = MainGUI.ScanStatus.InProgress;
						objectsPending--;
						objectsInProgress++;
					}
					if (progress.Status != 0)
					{
						switch (progress.Status)
						{
						case ObjectProgress.ObjectStatus.CompletedWithWarning:
							bPAScanObjectInfo.ScanStatus = MainGUI.ScanStatus.CompletedWithWarning;
							break;
						case ObjectProgress.ObjectStatus.CompletedWithError:
							bPAScanObjectInfo.ScanStatus = MainGUI.ScanStatus.CompletedWithError;
							break;
						default:
							bPAScanObjectInfo.ScanStatus = MainGUI.ScanStatus.CompletedOk;
							break;
						}
						objectsInProgress--;
						objectsCompleted++;
					}
					SetTotal(false);
					objectListBox.Invalidate(new Region(objectListBox.GetItemRectangle(objectListBox.Items.IndexOf(bPAScanObjectInfo))));
				}
			}
			else
			{
				ProgressCallback method = Status;
				BeginInvoke(method, name, progress);
			}
		}

		public void Stop()
		{
			mainGUI.ExecInterface.Abort();
		}

		public override void TimerDone()
		{
			if (!base.InvokeRequired)
			{
				try
				{
					DataInfo dataInfo = SaveData();
					ScanCompleted.CompletionInfo completionInfo = new ScanCompleted.CompletionInfo();
					completionInfo.dataInfo = dataInfo;
					completionInfo.completedOk = !mainGUI.ExecInterface.Aborting && dataInfo != null;
					SetTotal(true);
					mainGUI.TakeAction(MainGUI.Actions.ScanCompleted, completionInfo);
				}
				catch (Exception exception)
				{
					mainGUI.TraceError(exception);
				}
			}
			else
			{
				TimerThread.DoneDelegate method = TimerDone;
				BeginInvoke(method, null);
			}
		}

		private void PopulateScanObjects(BPAScanInfo scanInfo)
		{
			objectListToProcess = new SortedList();
			sortedDisplayList = new SortedList();
			objectListBox.Items.Clear();
			objectsPending = 0;
			objectsInProgress = 0;
			objectsCompleted = 0;
			foreach (BPAScanObjectInfo item in scanInfo.ObjectsToBeProcessed)
			{
				InsertScanObjectIntoList(item);
			}
			objectsPending = scanInfo.ObjectsToBeProcessed.Count;
			SetTotal(false);
		}

		private void InsertScanObjectIntoList(BPAScanObjectInfo scanObjectInfo)
		{
			string text = scanObjectInfo.Group;
			if (text.Length < 40)
			{
				text.PadRight(40, ' ');
			}
			else
			{
				text = text.Substring(0, 40);
			}
			if (!sortedDisplayList.Contains(text))
			{
				sortedDisplayList.Add(text, 0);
				objectListBox.Items.Insert(sortedDisplayList.IndexOfKey(text), text);
			}
			text += scanObjectInfo.Name;
			if (!sortedDisplayList.Contains(text))
			{
				objectListToProcess.Add(scanObjectInfo.Name, scanObjectInfo);
				sortedDisplayList.Add(text, 0);
				objectListBox.Items.Insert(sortedDisplayList.IndexOfKey(text), scanObjectInfo);
			}
		}

		private DataInfo SaveData()
		{
			try
			{
				mainGUI.ExecInterface.Options.Data.Save();
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
				return null;
			}
			finally
			{
				mainGUI.ExecInterface.Options.Data.ClearDocument();
			}
			DataInfo dataInfo = new DataInfo(mainGUI.ExecInterface, mainGUI.ExecInterface.Options.Data.FileName);
			if (mainGUI.SelectScanScreen != null)
			{
				CommonData.DataInfoList.Add(dataInfo);
			}
			return dataInfo;
		}

		private void SetTotal(bool failAll)
		{
			if (failAll)
			{
				foreach (BPAScanObjectInfo value in objectListToProcess.Values)
				{
					if (value.ScanStatus == MainGUI.ScanStatus.Pending)
					{
						objectsPending--;
						objectsCompleted++;
						value.ScanStatus = MainGUI.ScanStatus.CompletedWithError;
						objectListBox.Invalidate(new Region(objectListBox.GetItemRectangle(objectListBox.Items.IndexOf(value))));
					}
					else if (value.ScanStatus == MainGUI.ScanStatus.InProgress)
					{
						objectsInProgress--;
						objectsCompleted++;
						value.ScanStatus = MainGUI.ScanStatus.CompletedWithError;
						objectListBox.Invalidate(new Region(objectListBox.GetItemRectangle(objectListBox.Items.IndexOf(value))));
					}
				}
			}
			string text = "";
			if (objectsPending > 0)
			{
				text = text + BPALoc.Label_IPTotalServersPending(objectsPending.ToString()) + ", ";
			}
			if (objectsInProgress > 0)
			{
				text = text + BPALoc.Label_IPTotalServersInProgress(objectsInProgress.ToString()) + ", ";
			}
			if (objectsCompleted > 0)
			{
				text = text + BPALoc.Label_IPTotalServersCompleted(objectsCompleted.ToString()) + ", ";
			}
			if (text.Length > 1)
			{
				text = text.Substring(0, text.Length - 2);
			}
			scanObjectCount.Text = BPALoc.Label_IPTotalServers(text);
		}

		private void DrawServer(ListBox listBox, DrawItemEventArgs e, Rectangle bounds, bool toExpand)
		{
			try
			{
				BPAScanObjectInfo bPAScanObjectInfo = (BPAScanObjectInfo)objectListBox.Items[e.Index];
				Rectangle rect = new Rectangle(bounds.X + 2, bounds.Y, bounds.Width - 2, 1);
				if (e.Index != 0)
				{
					e.Graphics.FillRectangle(new LinearGradientBrush(rect, MainGUI.DarkGray, MainGUI.LightGray, 0f, false), rect);
				}
				e.Graphics.DrawImage(mainGUI.Customizations.ObjectPic, bounds.X + 4, bounds.Y + 8);
				e.Graphics.DrawString(bPAScanObjectInfo.Name, MainGUI.DefaultFont, new SolidBrush(SystemColors.ControlText), bounds.X + mainGUI.Customizations.ObjectPic.Width + 8, bounds.Y + 10);
				int num = rect.Width - 100;
				switch (bPAScanObjectInfo.ScanStatus)
				{
				case MainGUI.ScanStatus.Pending:
					e.Graphics.DrawString(BPALoc.Label_IPServerStatusPending, MainGUI.DefaultFont, new SolidBrush(MainGUI.DarkGray), num + CommonData.ErrorPic.Width, bounds.Y + 12);
					break;
				case MainGUI.ScanStatus.InProgress:
				{
					e.Graphics.FillRectangle(new SolidBrush(SystemColors.ActiveBorder), num, bounds.Y + 12, 62, 12);
					e.Graphics.DrawRectangle(new Pen(MainGUI.Darken(SystemColors.ActiveBorder, 50)), num, bounds.Y + 12, 62, 12);
					e.Graphics.FillRectangle(new SolidBrush(SystemColors.ActiveCaption), num + 2, bounds.Y + 14, bPAScanObjectInfo.PctDone * 62 / 100, 9);
					string s = string.Format("{0}%", bPAScanObjectInfo.PctDone);
					e.Graphics.DrawString(s, MainGUI.DefaultFont, new SolidBrush(MainGUI.DarkGray), num + 64, bounds.Y + 12);
					break;
				}
				case MainGUI.ScanStatus.CompletedOk:
					e.Graphics.DrawIcon(mainGUI.OkIcon, num, bounds.Y + 12);
					e.Graphics.DrawString(BPALoc.Label_IPServerStatusCompleted, MainGUI.DefaultFont, new SolidBrush(MainGUI.DarkGray), num + mainGUI.OkIcon.Width, bounds.Y + 12);
					break;
				case MainGUI.ScanStatus.CompletedWithWarning:
					e.Graphics.DrawIcon(CommonData.WarningIcon, num, bounds.Y + 12);
					e.Graphics.DrawString(BPALoc.Label_IPServerStatusCompleted, MainGUI.DefaultFont, new SolidBrush(MainGUI.DarkGray), num + CommonData.WarningIcon.Width, bounds.Y + 12);
					break;
				case MainGUI.ScanStatus.CompletedWithError:
					e.Graphics.DrawIcon(CommonData.ErrorIcon, num, bounds.Y + 12);
					e.Graphics.DrawString(BPALoc.Label_IPServerStatusCompleted, MainGUI.DefaultFont, new SolidBrush(MainGUI.DarkGray), num + CommonData.ErrorIcon.Width, bounds.Y + 12);
					break;
				case MainGUI.ScanStatus.Aborted:
					e.Graphics.DrawIcon(CommonData.ErrorIcon, num, bounds.Y + 12);
					e.Graphics.DrawString(BPALoc.Label_IPServerStatusAborted, MainGUI.DefaultFont, new SolidBrush(MainGUI.DarkGray), num + CommonData.ErrorIcon.Width, bounds.Y + 12);
					break;
				case MainGUI.ScanStatus.NotStarted:
					e.Graphics.DrawIcon(CommonData.ErrorIcon, num, bounds.Y + 12);
					e.Graphics.DrawString(BPALoc.Label_IPServerStatusNotStarted, MainGUI.DefaultFont, new SolidBrush(MainGUI.DarkGray), num + CommonData.ErrorIcon.Width, bounds.Y + 12);
					break;
				}
				if (e.Index + 1 >= objectListBox.Items.Count || objectListBox.Items[e.Index + 1].GetType() == typeof(string))
				{
					rect = new Rectangle(bounds.X + 2, bounds.Y + bounds.Height - 1, bounds.Width - 2, 1);
					e.Graphics.FillRectangle(new LinearGradientBrush(rect, MainGUI.DarkGray, MainGUI.LightGray, 0f, false), rect);
				}
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		private int MeasureServer(ListBox listBox, MeasureItemEventArgs e)
		{
			return MainGUI.DefaultFont.Height * 2 + 4;
		}
	}
}
