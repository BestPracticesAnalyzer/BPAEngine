using System;
using System.Collections;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	[Serializable]
	public class ScheduleInfo
	{
		private uint mjobId;

		private NetApi32.AT_INFO atinfo = new NetApi32.AT_INFO();

		private ArrayList commandOptions;

		[CLSCompliant(false)]
		public uint JobId
		{
			get
			{
				return mjobId;
			}
			set
			{
				mjobId = value;
			}
		}

		public NetApi32.AT_INFO ATInfo
		{
			get
			{
				return atinfo;
			}
			set
			{
				atinfo = value;
			}
		}

		public ArrayList CommandOptions
		{
			get
			{
				return commandOptions;
			}
			set
			{
				commandOptions = value;
			}
		}

		public ScheduleInfo()
		{
			Reset();
		}

		public void Reset()
		{
			mjobId = 0u;
			atinfo.JobTime = 0;
			atinfo.DaysOfMonth = 0;
			atinfo.DaysOfWeek = 0;
			atinfo.Flags = 0;
			atinfo.JobCommand = null;
			commandOptions = null;
		}
	}
}
