using System;
using System.Runtime.InteropServices;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	[CLSCompliant(false)]
	public class NetApi32
	{
		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public class AT_INFO
		{
			public int JobTime;

			public int DaysOfMonth;

			public byte DaysOfWeek;

			public byte Flags;

			public string JobCommand = string.Empty;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public class AT_ENUM
		{
			public uint JobId;

			public int JobTime;

			public int DaysOfMonth;

			public byte DaysOfWeek;

			public byte Flags;

			public string JobCommand = string.Empty;
		}

		public const uint NERR_Success = 0u;

		public const uint JOB_RUN_PERIODICALLY = 1u;

		public const uint JOB_EXEC_ERROR = 2u;

		public const uint JOB_RUNS_TODAY = 4u;

		public const byte JOB_ADD_CURRENT_DATE = 8;

		public const uint JOB_NONINTERACTIVE = 16u;

		[DllImport("NetApi32.dll", CharSet = CharSet.Unicode)]
		public static extern int NetScheduleJobAdd(string serverName, AT_INFO atinfo, ref uint jobId);

		[DllImport("NetApi32.dll", CharSet = CharSet.Unicode)]
		public static extern int NetScheduleJobDel(string serverName, uint minJobId, uint maxJobId);

		[DllImport("NetApi32.dll", CharSet = CharSet.Unicode)]
		public static extern int NetScheduleJobGetInfo(string Servername, uint JobId, out IntPtr ptr);

		[DllImport("NetApi32.dll", CharSet = CharSet.Unicode)]
		public static extern int NetScheduleJobEnum(string Servername, out IntPtr Buffer, int prefMaxLength, ref int entriesRead, ref int totalEntries, out int resumeHandle);

		[DllImport("NetApi32.dll", CharSet = CharSet.Unicode)]
		public static extern int NetApiBufferFree(IntPtr ptr);
	}
}
