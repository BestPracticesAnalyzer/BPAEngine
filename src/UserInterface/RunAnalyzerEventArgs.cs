using System;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class RunAnalyzerEventArgs : EventArgs
	{
		private string analyzerName;

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
