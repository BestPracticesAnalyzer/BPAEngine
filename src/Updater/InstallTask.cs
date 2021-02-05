namespace Microsoft.VSPowerToys.Updater
{
	public abstract class InstallTask : ITask
	{
		private int timeOutMilliSecs = 50000;

		public int TimeOut
		{
			get
			{
				return timeOutMilliSecs;
			}
			set
			{
				timeOutMilliSecs = value;
			}
		}

		public abstract void Execute();
	}
}
