using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace SqmInteropServices
{
	internal class SqmLibWrap
	{
		public delegate uint SqmUploadCallback(uint hr, [MarshalAs(UnmanagedType.LPWStr)] string filePath, uint dwHttpResponse);

		public const uint SQM_SESSION_CREATE_NEW = 1u;

		public const uint SQM_OVERWRITE_OLDEST_FILE = 2u;

		public const uint SQM_UPLOAD_ALL_FILES = 2u;

		public const uint SQM_SECURE_UPLOAD = 4u;

		public const uint SQM_MAX_SESSION_SIZE = 60000u;

		[DllImport("custsat.dll", EntryPoint = "#2")]
		public static extern uint SqmGetSession([MarshalAs(UnmanagedType.LPWStr)] string pszSessionIdentifier, uint cbMaxSessionSize, uint dwFlags);

		[DllImport("custsat.dll", EntryPoint = "#3")]
		public static extern void SqmStartSession(uint hSession);

		[DllImport("custsat.dll", EntryPoint = "#4")]
		public static extern void SqmEndSession(uint hSession, [MarshalAs(UnmanagedType.LPWStr)] string pszPattern, uint dwMaxFilesToQueue, uint dwFlags);

		[DllImport("custsat.dll", EntryPoint = "#5")]
		public static extern void SqmSet(uint hSession, uint dwId, uint dwVal);

		[DllImport("custsat.dll", EntryPoint = "#6")]
		public static extern void SqmSetBool(uint hSession, uint dwId, uint dwVal);

		[DllImport("custsat.dll", EntryPoint = "#7")]
		public static extern void SqmSetBits(uint hSession, uint dwId, uint dwOrBits);

		[DllImport("custsat.dll", EntryPoint = "#8")]
		public static extern void SqmSetIfMax(uint hSession, uint dwId, uint dwOrBits);

		[DllImport("custsat.dll", EntryPoint = "#9")]
		public static extern void SqmSetIfMin(uint hSession, uint dwId, uint dwOrBits);

		[DllImport("custsat.dll", EntryPoint = "#10")]
		public static extern void SqmIncrement(uint hSession, uint dwId, uint dwInc);

		[DllImport("custsat.dll", EntryPoint = "#11")]
		public static extern void SqmAddToAverage(uint hSession, uint dwId, uint dwVal);

		[DllImport("custsat.dll", EntryPoint = "#15")]
		public static extern bool SqmGet(uint hSession, uint dwId, out uint pdwVal);

		[DllImport("custsat.dll", EntryPoint = "#16")]
		public static extern void SqmSetEnabled(uint hSession, bool fEnabled);

		[DllImport("custsat.dll", EntryPoint = "#17")]
		public static extern bool SqmGetEnabled(uint hSession);

		[DllImport("custsat.dll", EntryPoint = "#18")]
		public static extern void SqmSetMachineId(uint hSession, ref Guid pGuid);

		[DllImport("custsat.dll", EntryPoint = "#19")]
		public static extern void SqmSetUserId(uint hSession, ref Guid pGuid);

		[DllImport("custsat.dll", EntryPoint = "#20")]
		public static extern void SqmGetMachineId(uint hSession, out Guid Guid);

		[DllImport("custsat.dll", EntryPoint = "#21")]
		public static extern void SqmGetUserId(uint hSession, out Guid Guid);

		[DllImport("custsat.dll", EntryPoint = "#22")]
		public static extern void SqmSetAppVersion(uint hSession, uint dwVersionHigh, uint dwVersionLow);

		[DllImport("custsat.dll", EntryPoint = "#23")]
		public static extern void SqmTimerStart(uint hSession, uint dwId);

		[DllImport("custsat.dll", EntryPoint = "#24")]
		public static extern void SqmTimerRecord(uint hSession, uint dwId);

		[DllImport("custsat.dll", EntryPoint = "#25")]
		public static extern void SqmTimerAccumulate(uint hSession, uint dwId);

		[DllImport("custsat.dll", EntryPoint = "#26")]
		public static extern System.Runtime.InteropServices.ComTypes.FILETIME SqmGetSessionStartTime(uint hSession);

		[DllImport("custsat.dll", EntryPoint = "#27")]
		public static extern void SqmSet64(uint hSession, uint dwId, ulong qwVal);

		[DllImport("custsat.dll", EntryPoint = "#28")]
		public static extern bool SqmGet64(uint hSession, uint dwId, out ulong qwVal);

		[DllImport("custsat.dll", EntryPoint = "#29")]
		public static extern bool SqmReadSharedMachineId(out Guid pGuid);

		[DllImport("custsat.dll", EntryPoint = "#30")]
		public static extern bool SqmReadSharedUserId(out Guid pGuid);

		[DllImport("custsat.dll", EntryPoint = "#31")]
		public static extern bool SqmWriteSharedMachineId(ref Guid pGuid);

		[DllImport("custsat.dll", EntryPoint = "#32")]
		public static extern bool SqmWriteSharedUserId(ref Guid pGuid);

		[DllImport("custsat.dll", EntryPoint = "#33")]
		public static extern bool SqmCreateNewId(out Guid pGuid);

		[DllImport("custsat.dll", EntryPoint = "#34")]
		public static extern void SqmSetFlags(uint hSession, uint dwFlags);

		[DllImport("custsat.dll", EntryPoint = "#35")]
		public static extern void SqmClearFlags(uint hSession, ref uint dwFlags);

		[DllImport("custsat.dll", EntryPoint = "#36")]
		public static extern void SqmGetFlags(uint hSession, out uint dwFlags);

		[DllImport("custsat.dll", EntryPoint = "#40")]
		public static extern bool SqmIsSharedMemoryConsistent(uint hSession);

		[DllImport("custsat.dll", EntryPoint = "#45")]
		public static extern void SqmTimerAddToAverage(uint hSession, uint dwId);

		[DllImport("custsat.dll", EntryPoint = "#46")]
		public static extern uint SqmStartUpload([MarshalAs(UnmanagedType.LPWStr)] string szPattern, [MarshalAs(UnmanagedType.LPWStr)] string szUrl, [MarshalAs(UnmanagedType.LPWStr)] string szSecureUrl, uint dwFlags, SqmUploadCallback pfnCallback);

		[DllImport("custsat.dll", EntryPoint = "#47")]
		public static extern void SqmAbortUpload(uint dwTimeoutMilliseconds);

		[DllImport("custsat.dll", EntryPoint = "#48")]
		public static extern void SqmSetCurrentTimeAsUploadTime([MarshalAs(UnmanagedType.LPWStr)] string pszSqmFileName);
	}
}
