using System.Threading;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class TimerThread
	{
		public delegate void StatusDelegate();

		public delegate void DoneDelegate();

		private BaseProgressPanel baseProgPanel;

		public TimerThread(BaseProgressPanel baseProgPanel)
		{
			this.baseProgPanel = baseProgPanel;
		}

		public void Start()
		{
			try
			{
				while (!baseProgPanel.FinishTimer)
				{
					Thread.Sleep(1000);
					baseProgPanel.TimerStatus();
				}
				for (int i = 0; i < 10; i++)
				{
					Thread.Sleep(100);
					baseProgPanel.TimerStatus();
				}
			}
			catch
			{
			}
			finally
			{
				baseProgPanel.TimerDone();
			}
			Application.ExitThread();
		}
	}
}
