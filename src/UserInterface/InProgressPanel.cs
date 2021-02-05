using System;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class InProgressPanel : BaseProgressPanel
	{
		private BPALabel pctDone;

		private BPALabel timeLeft;

		private ProgressBar progBar;

		private DateTime startTime;

		private TimeSpan estimatedTimeTotal;

		private TimeSpan estimatedTimeLeft;

		private int finishPct;

		private DateTime lastStatusUpdate;

		private object lockTimeUpdate = new object();

		private TimeSpan finishTimeLeft;

		private DateTime timeSinceZero;

		private bool disjoinProgressBar;

		protected string EstimatedTime
		{
			set
			{
				BPALabel bPALabel = new BPALabel(value, nextLocation, 0, this);
				bPALabel.TabIndex = nextTabIndex++;
				nextLocation = Navigate.NextTo(bPALabel);
				timeLeft = new BPALabel(CommonUIFunctions.ComputeTimeLeftString(estimatedTimeTotal), nextLocation, 300, this);
				timeLeft.TabIndex = nextTabIndex++;
				nextLocation = Navigate.Below(bPALabel);
			}
		}

		public TimeSpan EstimatedTimeTotal
		{
			get
			{
				return estimatedTimeTotal;
			}
			set
			{
				lock (lockTimeUpdate)
				{
					estimatedTimeTotal = value;
					timeLeft.Text = CommonUIFunctions.ComputeTimeLeftString(estimatedTimeTotal);
				}
			}
		}

		public int PercentComplete
		{
			get
			{
				return progBar.Value;
			}
		}

		public InProgressPanel()
		{
		}

		public InProgressPanel(int width)
			: base(width)
		{
		}

		public InProgressPanel(MainGUI mainGUI)
			: base(mainGUI)
		{
		}

		public override void Initialize(MainGUI mainGUI)
		{
			base.Initialize(mainGUI);
			estimatedTimeTotal = TimeSpan.FromMinutes(1.0);
		}

		protected void DisjoinProgressBar()
		{
			disjoinProgressBar = true;
		}

		protected override void InitializeTimer()
		{
			startTime = DateTime.Now;
			lastStatusUpdate = DateTime.Now;
			estimatedTimeLeft = estimatedTimeTotal;
			timeLeft.Text = CommonUIFunctions.ComputeTimeLeftString(estimatedTimeLeft);
			pctDone.Text = string.Format("0%");
			progBar.Value = 0;
			timeSinceZero = DateTime.MinValue;
		}

		public void BaseStatus(int pctDoneSoFar)
		{
			if (!(DateTime.Now.Subtract(lastStatusUpdate).TotalSeconds > 5.0))
			{
				return;
			}
			lock (lockTimeUpdate)
			{
				lastStatusUpdate = DateTime.Now;
				if (pctDoneSoFar > 0)
				{
					if (pctDoneSoFar > 100)
					{
						pctDoneSoFar = 100;
					}
					pctDone.Text = string.Format("{0}%", pctDoneSoFar.ToString());
					progBar.Value = pctDoneSoFar;
					if (!disjoinProgressBar)
					{
						TimeSpan value = CommonUIFunctions.ComputeTimeLeft(startTime, pctDoneSoFar);
						estimatedTimeTotal = DateTime.Now.Add(value).Subtract(startTime);
					}
				}
			}
		}

		protected override void CompleteTimer()
		{
			finishPct = 100 - progBar.Value;
			finishTimeLeft = estimatedTimeLeft;
		}

		protected override void UpdateTimer()
		{
			lock (lockTimeUpdate)
			{
				int num = progBar.Value;
				if (base.FinishTimer)
				{
					num = progBar.Value + finishPct / 10;
					if (num > 100)
					{
						num = 100;
					}
					if (num == 100)
					{
						estimatedTimeLeft = TimeSpan.FromSeconds(0.0);
					}
					else
					{
						estimatedTimeLeft = estimatedTimeLeft.Subtract(TimeSpan.FromSeconds(finishTimeLeft.TotalSeconds / 10.0));
						if (estimatedTimeLeft.TotalSeconds < 0.0)
						{
							estimatedTimeLeft = TimeSpan.Zero;
						}
					}
				}
				else
				{
					estimatedTimeLeft = estimatedTimeTotal.Subtract(DateTime.Now.Subtract(startTime));
					if (estimatedTimeLeft.TotalSeconds < 0.0)
					{
						estimatedTimeLeft = TimeSpan.FromSeconds(0.0);
					}
					if (!disjoinProgressBar)
					{
						num = ((estimatedTimeTotal.Ticks != 0) ? (100 - (int)(estimatedTimeLeft.Ticks * 100 / estimatedTimeTotal.Ticks)) : 100);
					}
				}
				if (estimatedTimeLeft.TotalSeconds == 0.0 && timeSinceZero.CompareTo(DateTime.MinValue) == 0)
				{
					timeSinceZero = DateTime.Now;
				}
				if (estimatedTimeLeft.TotalSeconds == 0.0 && DateTime.Now.Subtract(timeSinceZero).TotalSeconds > 3.0)
				{
					if (timeLeft.Text.StartsWith("0"))
					{
						timeLeft.Text = "-*********";
					}
					else
					{
						timeLeft.Text = timeLeft.Text.Substring(timeLeft.Text.Length - 1, 1) + timeLeft.Text.Substring(0, timeLeft.Text.Length - 1);
					}
				}
				else
				{
					timeLeft.Text = CommonUIFunctions.ComputeTimeLeftString(estimatedTimeLeft);
				}
				pctDone.Text = string.Format("{0}%", num.ToString());
				progBar.Value = num;
			}
		}

		protected override void AddProgressBar()
		{
			progBar = new ProgressBar();
			progBar.Location = nextLocation;
			progBar.Size = new Size(350, 16);
			progBar.Tag = new Rectangle(progBar.Location, progBar.Size);
			progBar.TabIndex = nextTabIndex++;
			base.Controls.Add(progBar);
			nextLocation = Navigate.NextTo(progBar);
			pctDone = new BPALabel("0%", nextLocation, 100, this);
			pctDone.TabIndex = nextTabIndex++;
			nextLocation = Navigate.Below(progBar, 2f);
		}
	}
}
