using System;
using System.Collections;
using System.Drawing;
using System.Security.Principal;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	internal class ScheduleOptions : BPAScreen
	{
		private enum ScheduleStatus
		{
			Allowed,
			NotLocalAdmin,
			NoContext
		}

		public struct VIEWINFO
		{
			public DateTime dtTime;

			public int SelectIndex;
		}

		[Flags]
		public enum RUNFREQUENCY
		{
			RunOnce = 0x0,
			RunDaily = 0x1,
			RunWeekly = 0x2,
			RunMonthly = 0x3
		}

		private BPACheckBox scheduleEnabled;

		private BPADateTimePicker startTimePicker;

		private BPADateTimePicker startDatePicker;

		private BPAComboBox runFrequencyEntries;

		private int nextTabIndex;

		private Schedule scheduler;

		private ScheduleInfo scheduleInfo;

		private bool scheduled;

		private ScheduleStatus scheduleStatus;

		private VIEWINFO viewInfo = default(VIEWINFO);

		private BPALink linkSaveExit;

		private BPAScanInfo scanInfo;

		private BPALabel scheduleStatusText;

		public ScheduleOptions(MainGUI mainGUI)
			: base(mainGUI)
		{
			Point borderCornerPoint = MainGUI.BorderCornerPoint;
			Dock = DockStyle.Fill;
			AutoScroll = true;
			BackColor = Color.White;
			base.mainGUI = mainGUI;
			nextTabIndex = mainGUI.StartingTabIndex;
			scheduler = new Schedule(mainGUI.Customizations.ApplicationName);
			scheduleInfo = new ScheduleInfo();
			if (scheduler.View(mainGUI.ExecInterface, ref scheduleInfo))
			{
				scheduled = true;
			}
			string commandLinePostFix = " -s";
			ScheduleTask.CleanOrphanTasks(mainGUI.ExecInterface, mainGUI.Customizations.CommandLineApplicationName, commandLinePostFix, scheduleInfo.JobId);
			borderCornerPoint = Navigate.Below(new BPATitle(BPALoc.Label_SHTitle, borderCornerPoint, mainGUI.FullWidth, this)
			{
				TabIndex = nextTabIndex++
			});
			borderCornerPoint = Navigate.Below(new BPALabel(BPALoc.Label_SHDesc, borderCornerPoint, mainGUI.FullWidth, this)
			{
				TabIndex = nextTabIndex++
			});
			scheduleEnabled = new BPACheckBox(BPALoc.CheckBox_SHEnable, borderCornerPoint, mainGUI.FullWidth, this);
			scheduleEnabled.Checked = scheduled;
			scheduleEnabled.TabIndex = nextTabIndex++;
			scheduleEnabled.Click += ScheduleEnabledClicked;
			borderCornerPoint = Navigate.Below(scheduleEnabled);
			BPAGradiatedBox control = new BPAGradiatedBox(MainGUI.DarkGray, MainGUI.LightGray, borderCornerPoint, new Size(mainGUI.FullWidth, 1), this);
			borderCornerPoint = Navigate.Below(control);
			BPALabel control2 = new BPALabel(BPALoc.Label_SHStartTime, borderCornerPoint, 200, this)
			{
				TabIndex = nextTabIndex++
			};
			borderCornerPoint = Navigate.NextTo(control2);
			startTimePicker = new BPADateTimePicker(borderCornerPoint, 66, this);
			startTimePicker.CustomFormat = "HH:mm";
			startTimePicker.Format = DateTimePickerFormat.Custom;
			startTimePicker.ShowUpDown = true;
			startTimePicker.TabIndex = nextTabIndex++;
			startTimePicker.ValueChanged += StartTimeChanged;
			borderCornerPoint = Navigate.NextTo(startTimePicker);
			startDatePicker = new BPADateTimePicker(borderCornerPoint, 200, this);
			startDatePicker.MinDate = DateTime.Now;
			startDatePicker.TabIndex = nextTabIndex++;
			startDatePicker.ValueChanged += StartDateChanged;
			borderCornerPoint = Navigate.Below(control2);
			BPALabel control3 = new BPALabel(BPALoc.Label_SHRunFrequency, borderCornerPoint, 200, this)
			{
				TabIndex = nextTabIndex++
			};
			borderCornerPoint = Navigate.NextTo(control3);
			runFrequencyEntries = new BPAComboBox(borderCornerPoint, 1f, this);
			runFrequencyEntries.Width = startDatePicker.Right - startTimePicker.Left;
			runFrequencyEntries.SetOrigRect();
			runFrequencyEntries.Items.Clear();
			runFrequencyEntries.Items.Add(BPALoc.Label_SHRunFrequencyOnce);
			runFrequencyEntries.Items.Add(BPALoc.Label_SHRunFrequencyDaily);
			runFrequencyEntries.Items.Add(BPALoc.Label_SHRunFrequencyWeekly);
			runFrequencyEntries.Items.Add(BPALoc.Label_SHRunFrequencyMonthly);
			runFrequencyEntries.SelectedItem = BPALoc.Label_SHRunFrequencyWeekly;
			runFrequencyEntries.SelectedIndexChanged += RunFrequencyChanged;
			runFrequencyEntries.TabIndex = nextTabIndex++;
			borderCornerPoint = Navigate.Below(control3);
			if (scheduled)
			{
				MapScheduleToViewInfo();
				startTimePicker.Value = viewInfo.dtTime;
				startDatePicker.Value = viewInfo.dtTime;
				runFrequencyEntries.SelectedIndex = viewInfo.SelectIndex;
			}
			EnableScheduleControls(scheduleEnabled.Checked);
			BPAGradiatedBox control4 = new BPAGradiatedBox(MainGUI.DarkGray, MainGUI.LightGray, borderCornerPoint, new Size(mainGUI.FullWidth, 1), this);
			borderCornerPoint = Navigate.Below(control4);
			borderCornerPoint = Navigate.Indent(borderCornerPoint);
			BPALink bPALink = new BPALink(mainGUI, MainGUI.Actions.Exit, false, BPALoc.LinkLabel_SHExit, mainGUI.ArrowPic, borderCornerPoint, 0, this);
			bPALink.SetTabIndex(nextTabIndex++);
			borderCornerPoint = Navigate.Below(bPALink, 0.4f);
			linkSaveExit = new BPALink(mainGUI, MainGUI.Actions.SaveScheduleAndExit, false, BPALoc.LinkLabel_SHSaveAndExit, mainGUI.ArrowPic, borderCornerPoint, 0, this);
			linkSaveExit.SetTabIndex(nextTabIndex++);
			borderCornerPoint = Navigate.Below(linkSaveExit, 1.2f);
			borderCornerPoint = Navigate.UnIndent(borderCornerPoint);
			scheduleStatusText = new BPALabel(BPALoc.Label_SHNotAllowedNoContext, borderCornerPoint, 0, this);
			scheduleStatusText.ForeColor = Color.Red;
			scheduleStatusText.Font = new Font("Microsoft Sans Serif", 14f, FontStyle.Regular, GraphicsUnit.Point, 0);
			scheduleStatusText.TabIndex = nextTabIndex++;
			borderCornerPoint = Navigate.NextTo(scheduleStatusText);
			linkSaveExit.Enabled = scheduleEnabled.Checked;
		}

		public override bool Start()
		{
			scanInfo = new BPAScanInfo(mainGUI.ExecInterface.Options);
			mainGUI.Customizations.GetBPAScanInfo(scanInfo, null);
			scheduleStatus = ScheduleAllowed();
			scheduleEnabled.Enabled = scheduleStatus == ScheduleStatus.Allowed;
			linkSaveExit.Enabled = scheduleStatus == ScheduleStatus.Allowed && scheduleEnabled.Checked;
			EnableScheduleControls(scheduleStatus == ScheduleStatus.Allowed && scheduleEnabled.Checked);
			if (scheduleStatus == ScheduleStatus.NoContext)
			{
				scheduleStatusText.Text = BPALoc.Label_SHNotAllowedNoContext;
			}
			else if (scheduleStatus == ScheduleStatus.NotLocalAdmin)
			{
				scheduleStatusText.Text = BPALoc.Label_SHNotAllowedNotLocalAdmin;
			}
			else
			{
				scheduleStatusText.Text = "";
			}
			return true;
		}

		public void SaveAndExit()
		{
			if (!scheduleEnabled.Checked)
			{
				return;
			}
			viewInfo.dtTime = new DateTime(startDatePicker.Value.Year, startDatePicker.Value.Month, startDatePicker.Value.Day, startTimePicker.Value.Hour, startTimePicker.Value.Minute, startTimePicker.Value.Second);
			viewInfo.SelectIndex = runFrequencyEntries.SelectedIndex;
			MapViewInfoToSchedule();
			NetApi32.AT_INFO aTInfo = scheduleInfo.ATInfo;
			string text = string.Format("{0}\\{1}", mainGUI.ExecInterface.ExecutionDirectory, mainGUI.Customizations.CommandLineApplicationName);
			if (text.IndexOf(' ') != -1)
			{
				text = "\"" + text + "\"";
			}
			aTInfo.JobCommand = string.Format("{0} {1}", text, "-s");
			scheduleInfo.ATInfo = aTInfo;
			scheduleInfo.CommandOptions = new ArrayList();
			if (mainGUI.Customizations.UpdateScheduleInfo != null)
			{
				mainGUI.Customizations.UpdateScheduleInfo(scheduleInfo);
			}
			scheduleInfo.CommandOptions.Add("-dat");
			string text2 = mainGUI.RegSettings.DataDirectory;
			if (text2 == null || text2.Length == 0)
			{
				text2 = mainGUI.Customizations.DefaultDataDirectory;
			}
			scheduleInfo.CommandOptions.Add(text2);
			if (mainGUI.ExecInterface.Trace)
			{
				scheduleInfo.CommandOptions.Add("-t");
			}
			if (mainGUI.ExecInterface.Debug)
			{
				scheduleInfo.CommandOptions.Add("-b");
			}
			if (scanInfo.Options.Label.Length > 0)
			{
				scheduleInfo.CommandOptions.Add("-l");
				scheduleInfo.CommandOptions.Add(scanInfo.Options.Label);
			}
			if (mainGUI.ExecInterface.Options.Timeout > 0)
			{
				scheduleInfo.CommandOptions.Add("-to");
				scheduleInfo.CommandOptions.Add(mainGUI.ExecInterface.Options.Timeout.ToString());
			}
			if (scanInfo.Options.CredentialsList.Count > 0)
			{
				scheduleInfo.CommandOptions.Add("-u");
				foreach (Credentials value in scanInfo.Options.CredentialsList.Values)
				{
					scheduleInfo.CommandOptions.Add(value.ContextName);
					scheduleInfo.CommandOptions.Add(string.Format("{0}\\{1}", value.Domain, value.User));
					scheduleInfo.CommandOptions.Add(value.Password);
				}
			}
			scheduleInfo.CommandOptions.Add("-uk");
			scheduleInfo.CommandOptions.Add(WindowsIdentity.GetCurrent().Name);
			string text3 = string.Empty;
			foreach (string key in mainGUI.ExecInterface.Options.Restrictions.Types.Keys)
			{
				string text4 = mainGUI.ExecInterface.Options.Restrictions.CommaList(key, false, true, true);
				if (text4.Length > 0)
				{
					if (text3.Length > 0)
					{
						text3 += ",";
					}
					text3 += text4;
				}
			}
			if (text3.Length > 0)
			{
				scheduleInfo.CommandOptions.Add("-r");
				scheduleInfo.CommandOptions.Add(text3);
			}
			if (!scheduler.Save(mainGUI.ExecInterface, scheduleInfo))
			{
				MessageBox.Show(BPALoc.Label_PSHSubmitError, mainGUI.Customizations.ShortName);
			}
		}

		private void EnableScheduleControls(bool enable)
		{
			startTimePicker.Enabled = enable;
			startDatePicker.Enabled = enable;
			runFrequencyEntries.Enabled = enable;
		}

		private void MapScheduleToViewInfo()
		{
			int num = 0;
			if (((uint)scheduleInfo.ATInfo.Flags & (true ? 1u : 0u)) != 0)
			{
				if (scheduleInfo.ATInfo.DaysOfMonth != 0)
				{
					viewInfo.SelectIndex = 3;
				}
				else if (scheduleInfo.ATInfo.DaysOfWeek == 127)
				{
					viewInfo.SelectIndex = 1;
				}
				else
				{
					viewInfo.SelectIndex = 2;
				}
			}
			else
			{
				viewInfo.SelectIndex = 0;
			}
			viewInfo.dtTime = DateTime.Now.Date;
			viewInfo.dtTime = viewInfo.dtTime.AddMilliseconds(scheduleInfo.ATInfo.JobTime);
			switch (viewInfo.SelectIndex)
			{
			case 1:
				if (viewInfo.dtTime < DateTime.Now)
				{
					viewInfo.dtTime = viewInfo.dtTime.AddDays(1.0);
				}
				break;
			case 0:
			case 2:
			case 3:
				if (scheduleInfo.ATInfo.DaysOfWeek != 0)
				{
					num = GetSetBit(scheduleInfo.ATInfo.DaysOfWeek);
					num %= 7;
					if ((int)viewInfo.dtTime.DayOfWeek < num || (viewInfo.dtTime.DayOfWeek == (DayOfWeek)num && viewInfo.dtTime > DateTime.Now))
					{
						viewInfo.dtTime = viewInfo.dtTime.AddDays((double)(num - viewInfo.dtTime.DayOfWeek));
					}
					else
					{
						viewInfo.dtTime = viewInfo.dtTime.AddDays((double)(7 - viewInfo.dtTime.DayOfWeek + num));
					}
				}
				else
				{
					if (scheduleInfo.ATInfo.DaysOfMonth == 0)
					{
						break;
					}
					num = GetSetBit((uint)scheduleInfo.ATInfo.DaysOfMonth);
					viewInfo.dtTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, num, viewInfo.dtTime.Hour, viewInfo.dtTime.Minute, viewInfo.dtTime.Second, viewInfo.dtTime.Millisecond);
					if (viewInfo.dtTime <= DateTime.Now)
					{
						do
						{
							viewInfo.dtTime = viewInfo.dtTime.AddMonths(1);
						}
						while (DateTime.DaysInMonth(viewInfo.dtTime.Year, viewInfo.dtTime.Month) < num);
						viewInfo.dtTime = viewInfo.dtTime.AddDays(num - viewInfo.dtTime.Day);
					}
				}
				break;
			}
		}

		private int GetSetBit(uint b)
		{
			int num = 1;
			while (b != 0 && (b & 1) == 0)
			{
				b >>= 1;
				num++;
			}
			return num;
		}

		private void MapViewInfoToSchedule()
		{
			int num = 0;
			scheduleInfo.Reset();
			NetApi32.AT_INFO aTInfo = scheduleInfo.ATInfo;
			aTInfo.JobTime = ((viewInfo.dtTime.Hour * 60 + viewInfo.dtTime.Minute) * 60 + viewInfo.dtTime.Second) * 1000;
			aTInfo.Flags = 16;
			switch (viewInfo.SelectIndex)
			{
			case 0:
				num = viewInfo.dtTime.Day - 1;
				aTInfo.DaysOfMonth = 1 << num;
				break;
			case 1:
				aTInfo.Flags |= 1;
				aTInfo.DaysOfWeek = 127;
				break;
			case 2:
				aTInfo.Flags |= 1;
				num = (int)viewInfo.dtTime.DayOfWeek;
				num = (num + 6) % 7;
				aTInfo.DaysOfWeek = (byte)(1 << num);
				break;
			case 3:
				aTInfo.Flags |= 1;
				num = viewInfo.dtTime.Day - 1;
				aTInfo.DaysOfMonth = 1 << num;
				break;
			}
			scheduleInfo.ATInfo = aTInfo;
		}

		private ScheduleStatus ScheduleAllowed()
		{
			SortedList sortedList = CommandLineParameters.AttributeValues(mainGUI.ExecInterface.Options.Configuration, "SecurityContext");
			foreach (object key in sortedList.Keys)
			{
				if (!mainGUI.ExecInterface.Options.CredentialsList.Contains(key) && ((string)key).Length > 0)
				{
					return ScheduleStatus.NoContext;
				}
			}
			try
			{
				WindowsIdentity current2 = WindowsIdentity.GetCurrent();
				WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current2);
				if (!windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator))
				{
					return ScheduleStatus.NotLocalAdmin;
				}
			}
			catch
			{
			}
			return ScheduleStatus.Allowed;
		}

		private void RunFrequencyChanged(object sender, EventArgs e)
		{
		}

		private void ScheduleEnabledClicked(object sender, EventArgs e)
		{
			try
			{
				if (!scheduleEnabled.Checked && scheduled)
				{
					scheduler.Delete(mainGUI.ExecInterface);
					scheduleInfo.Reset();
					scheduled = false;
				}
				EnableScheduleControls(scheduleEnabled.Checked);
				linkSaveExit.Enabled = scheduleStatus == ScheduleStatus.Allowed && scheduleEnabled.Checked;
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		private void StartDateChanged(object sender, EventArgs e)
		{
		}

		private void StartTimeChanged(object sender, EventArgs e)
		{
		}
	}
}
