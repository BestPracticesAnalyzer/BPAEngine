using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class Schedule
	{
		public const string BPAKeyName = "Software\\Microsoft\\BPA\\Schedule";

		public const string szSDDefault = "D:P(A;OICI;GA;;;SY)(A;OICI;GA;;;BA)";

		private IntPtr hkey = IntPtr.Zero;

		private string keyPath;

		private string keyValue;

		private string stringSecurityDescriptor;

		private SecureKey secureKey;

		public Schedule(IntPtr hkey, string keyPath, string keyValue, string securityDescriptor)
		{
			this.hkey = hkey;
			this.keyPath = keyPath;
			this.keyValue = keyValue;
			stringSecurityDescriptor = securityDescriptor;
			secureKey = new SecureKey(this.hkey, this.keyPath, this.keyValue, stringSecurityDescriptor);
		}

		public Schedule(string keyValue)
		{
			hkey = Advapi32.HKEY_LOCAL_MACHINE;
			keyPath = "Software\\Microsoft\\BPA\\Schedule";
			this.keyValue = keyValue;
			stringSecurityDescriptor = "D:P(A;OICI;GA;;;SY)(A;OICI;GA;;;BA)";
			secureKey = new SecureKey(hkey, keyPath, this.keyValue, stringSecurityDescriptor);
		}

		private bool UnpackData(ExecutionInterface execInterface, byte[] data, ref ScheduleInfo scheduleInfo)
		{
			bool result = false;
			byte[] pOut = null;
			try
			{
				if (data.Length > 0)
				{
					if (Crypt.DecryptData(execInterface, data, ref pOut))
					{
						MemoryStream serializationStream = new MemoryStream(pOut);
						BinaryFormatter binaryFormatter = new BinaryFormatter();
						scheduleInfo = (ScheduleInfo)binaryFormatter.Deserialize(serializationStream);
						result = true;
						return result;
					}
					return result;
				}
				return result;
			}
			catch (Exception ex)
			{
				execInterface.LogTrace("UnpackData : " + ex.Message);
				return result;
			}
		}

		private bool PackData(ExecutionInterface execInterface, ScheduleInfo scheduleInfo, ref byte[] pOut)
		{
			bool result = false;
			try
			{
				MemoryStream memoryStream = new MemoryStream();
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStream, scheduleInfo);
				byte[] buffer = memoryStream.GetBuffer();
				result = Crypt.EncryptData(execInterface, buffer, ref pOut);
				return result;
			}
			catch (Exception ex)
			{
				execInterface.LogTrace("PackData : " + ex.Message);
				return result;
			}
		}

		public bool Delete(ExecutionInterface execInterface)
		{
			bool result = false;
			ScheduleInfo scheduleInfo = new ScheduleInfo();
			try
			{
				if (View(execInterface, ref scheduleInfo))
				{
					if (scheduleInfo.JobId != 0)
					{
						ScheduleTask.Delete(scheduleInfo.JobId);
						secureKey.Delete(execInterface);
						result = true;
						return result;
					}
					return result;
				}
				return result;
			}
			catch (Exception ex)
			{
				execInterface.LogTrace("ScheduleDelete : " + ex.Message);
				return result;
			}
		}

		public bool View(ExecutionInterface execInterface, ref ScheduleInfo scheduleInfo)
		{
			return View(execInterface, ref scheduleInfo, true);
		}

		public bool View(ExecutionInterface execInterface, ref ScheduleInfo scheduleInfo, bool verifyTask)
		{
			bool result = false;
			scheduleInfo.Reset();
			try
			{
				byte[] data = null;
				if (secureKey.View(execInterface, ref data))
				{
					if (UnpackData(execInterface, data, ref scheduleInfo))
					{
						if (verifyTask)
						{
							if (ScheduleTask.VerifyScheduleTask(execInterface, scheduleInfo.JobId))
							{
								result = true;
								return result;
							}
							secureKey.Delete(execInterface);
							scheduleInfo.Reset();
							return result;
						}
						result = true;
						return result;
					}
					return result;
				}
				return result;
			}
			catch (Exception ex)
			{
				execInterface.LogTrace("ScheduleView : " + ex.Message);
				return result;
			}
		}

		public bool Save(ExecutionInterface execInterface, ScheduleInfo scheduleInfo)
		{
			bool flag = false;
			uint mjobId = 0u;
			try
			{
				Delete(execInterface);
				if (ScheduleTask.Add(execInterface, scheduleInfo.ATInfo, ref mjobId))
				{
					scheduleInfo.JobId = mjobId;
					byte[] pOut = null;
					if (PackData(execInterface, scheduleInfo, ref pOut) && secureKey.Save(execInterface, pOut))
					{
						flag = true;
					}
					if (!flag)
					{
						ScheduleTask.Delete(mjobId);
						return flag;
					}
					return flag;
				}
				return flag;
			}
			catch (Exception ex)
			{
				execInterface.LogTrace("ScheduleSubmit: " + ex.Message);
				return flag;
			}
		}

		public static string[] LoadSchedule(ExecutionInterface execInterface)
		{
			string[] array = null;
			ScheduleInfo scheduleInfo = new ScheduleInfo();
			Schedule schedule = new Schedule(execInterface.ApplicationName);
			if (schedule.View(execInterface, ref scheduleInfo, false))
			{
				if ((scheduleInfo.ATInfo.Flags & 1) == 0)
				{
					schedule.Delete(execInterface);
				}
				Directory.SetCurrentDirectory(execInterface.ExecutionDirectory);
				return (string[])scheduleInfo.CommandOptions.ToArray(typeof(string));
			}
			throw new ArgumentException(CommonLoc.Error_Parameters("S"));
		}
	}
}
