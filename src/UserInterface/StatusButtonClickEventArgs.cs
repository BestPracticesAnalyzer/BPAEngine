using System;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class StatusButtonClickEventArgs : EventArgs
	{
		private string analyzerName;

		private PluginStatus currentStatus;

		public PluginStatus AnalyzerStatus
		{
			get
			{
				return currentStatus;
			}
			set
			{
				currentStatus = value;
			}
		}

		public string AnalyzerName
		{
			get
			{
				return analyzerName;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException(Resource.AnalyzerNameNull);
				}
				analyzerName = value;
			}
		}
	}
}
