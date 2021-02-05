using System;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class CommonUIFunctions
	{
		public static TimeSpan ComputeTimeLeft(DateTime startTime, int pctDone)
		{
			TimeSpan result = TimeSpan.Zero;
			if (pctDone > 0)
			{
				TimeSpan timeSpan = DateTime.Now.Subtract(startTime);
				result = TimeSpan.FromSeconds(timeSpan.TotalSeconds * 100.0 / (double)pctDone - timeSpan.TotalSeconds);
			}
			return result;
		}

		public static string ComputeTimeLeftStringNoSeconds(TimeSpan timeLeft)
		{
			string text = "";
			if (timeLeft.TotalMinutes < 60.0)
			{
				return BPALoc.Label_IPTimeInMinutesNoSeconds(timeLeft.Minutes);
			}
			return BPALoc.Label_IPTimeInHoursNoSeconds((int)timeLeft.TotalHours, timeLeft.Minutes);
		}

		public static string ComputeTimeLeftString(TimeSpan timeLeft)
		{
			string text = "";
			if (timeLeft.TotalMinutes < 60.0)
			{
				return BPALoc.Label_IPTimeInMinutes(timeLeft.Minutes, timeLeft.Seconds);
			}
			return BPALoc.Label_IPTimeInHours((int)timeLeft.TotalHours, timeLeft.Minutes, timeLeft.Seconds);
		}
	}
}
