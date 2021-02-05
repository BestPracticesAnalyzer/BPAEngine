using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public abstract class BaseProgressPanel : BPAScreen
	{
		private Thread timer;

		private bool finishTimer;

		private int fullWidth;

		protected int nextTabIndex;

		protected Point nextLocation;

		protected string Title
		{
			set
			{
				BPATitle bPATitle = new BPATitle(value, nextLocation, fullWidth, this);
				bPATitle.TabIndex = nextTabIndex++;
				nextLocation = Navigate.Below(bPATitle);
			}
		}

		protected string PleaseWait
		{
			set
			{
				BPALabel bPALabel = new BPALabel(value, nextLocation, fullWidth, this);
				bPALabel.TabIndex = nextTabIndex++;
				nextLocation = Navigate.Below(bPALabel, 2f);
				AddProgressBar();
			}
		}

		protected string Description
		{
			set
			{
				BPALabel bPALabel = new BPALabel(value, nextLocation, fullWidth, this);
				bPALabel.TabIndex = nextTabIndex++;
				nextLocation = Navigate.Below(bPALabel);
			}
		}

		public bool FinishTimer
		{
			get
			{
				return finishTimer;
			}
		}

		protected Point NextLocationPoint
		{
			get
			{
				return nextLocation;
			}
		}

		public BaseProgressPanel()
		{
			Initialize(null);
		}

		public BaseProgressPanel(int width)
			: this()
		{
			fullWidth = width;
		}

		public BaseProgressPanel(MainGUI mainGUI)
			: base(mainGUI)
		{
			Initialize(mainGUI);
		}

		public override void Initialize(MainGUI mainGUI)
		{
			base.Initialize(mainGUI);
			Dock = DockStyle.Fill;
			AutoScroll = true;
			BackColor = Color.White;
			nextLocation = MainGUI.BorderCornerPoint;
			if (mainGUI != null)
			{
				fullWidth = mainGUI.FullWidth;
				nextTabIndex = mainGUI.StartingTabIndex;
			}
		}

		protected void ResetControls()
		{
			nextTabIndex = 0;
			base.Controls.Clear();
		}

		public override bool Start()
		{
			return true;
		}

		protected void StartTimerThread()
		{
			InitializeTimer();
			finishTimer = false;
			TimerThread @object = new TimerThread(this);
			timer = new Thread(@object.Start);
			timer.Start();
		}

		public void StopTimerThread()
		{
			CompleteTimer();
			finishTimer = true;
		}

		public virtual void TimerDone()
		{
		}

		public void TimerStatus()
		{
			if (!base.InvokeRequired)
			{
				UpdateTimer();
				return;
			}
			TimerThread.StatusDelegate method = TimerStatus;
			BeginInvoke(method, null);
		}

		protected virtual void AddProgressBar()
		{
		}

		protected virtual void InitializeTimer()
		{
		}

		protected virtual void CompleteTimer()
		{
		}

		protected virtual void UpdateTimer()
		{
		}
	}
}
