using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class ScheduleTask
	{
		[CLSCompliant(false)]
		public static bool Add(ExecutionInterface execInterface, NetApi32.AT_INFO atInfo, ref uint mjobId)
		{
			bool result = false;
			try
			{
				result = NetApi32.NetScheduleJobAdd(null, atInfo, ref mjobId) == 0;
				return result;
			}
			catch (Exception ex)
			{
				execInterface.LogTrace("ScheduleTask.Add: " + ex.Message);
				return result;
			}
		}

		[CLSCompliant(false)]
		public static int Delete(uint jobId)
		{
			return NetApi32.NetScheduleJobDel(null, jobId, jobId);
		}

		[CLSCompliant(false)]
		public static bool VerifyScheduleTask(ExecutionInterface execInterface, uint jobId)
		{
			IntPtr ptr = IntPtr.Zero;
			bool result = false;
			try
			{
				if (NetApi32.NetScheduleJobGetInfo(null, jobId, out ptr) == 0)
				{
					result = true;
					return result;
				}
				return result;
			}
			catch (Exception ex)
			{
				execInterface.LogTrace("ScheduleTask.VerifyScheduleTask: " + ex.Message);
				return result;
			}
			finally
			{
				if (ptr != IntPtr.Zero)
				{
					NetApi32.NetApiBufferFree(ptr);
				}
			}
		}

		[CLSCompliant(false)]
		public static void CleanOrphanTasks(ExecutionInterface execInterface, string commandLineApplicationName, string commandLinePostFix, uint jobId)
		{
			IntPtr Buffer = IntPtr.Zero;
			int prefMaxLength = 8192;
			int entriesRead = 0;
			int totalEntries = 0;
			int resumeHandle = 0;
			int num = 0;
			int num2 = 0;
			try
			{
				do
				{
					num2 = NetApi32.NetScheduleJobEnum(null, out Buffer, prefMaxLength, ref entriesRead, ref totalEntries, out resumeHandle);
					if (num2 != 0)
					{
						Win32Exception ex = new Win32Exception(num2);
						throw ex;
					}
					if (entriesRead <= 0)
					{
						break;
					}
					num = 0;
					IntPtr intPtr = Buffer;
					while (num < entriesRead)
					{
						NetApi32.AT_ENUM aT_ENUM = (NetApi32.AT_ENUM)Marshal.PtrToStructure(intPtr, typeof(NetApi32.AT_ENUM));
						num++;
						if (aT_ENUM.JobId != jobId && aT_ENUM.JobCommand.IndexOf(commandLineApplicationName) > 0 && aT_ENUM.JobCommand.EndsWith(commandLinePostFix))
						{
							Delete(aT_ENUM.JobId);
						}
						intPtr = (IntPtr)((long)intPtr + Marshal.SizeOf(aT_ENUM));
					}
					NetApi32.NetApiBufferFree(Buffer);
					Buffer = IntPtr.Zero;
				}
				while (num < totalEntries && resumeHandle > 0);
			}
			catch (Exception ex2)
			{
				execInterface.LogTrace("ScheduleTask.CleanScheduleTasks: " + ex2.Message);
			}
			finally
			{
				if (Buffer != IntPtr.Zero)
				{
					NetApi32.NetApiBufferFree(Buffer);
				}
			}
		}
	}
}
