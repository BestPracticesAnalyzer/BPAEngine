using System;
using System.Runtime.InteropServices;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	[CLSCompliant(false)]
	public class Kernel32
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int WaitForSingleObject(IntPtr handle, int dwMilliseconds);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool CloseHandle(IntPtr handle);

		[DllImport("kernel32.dll")]
		public static extern void ExitProcess(int uExitCode);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetStdHandle(int dwHandleId);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool GetConsoleMode(IntPtr hConsoleHandle, ref int dwMode);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int dwMode);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr LocalFree(IntPtr hMem);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GlobalFree(IntPtr ptr);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern int ResumeThread(IntPtr ptr);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool CreatePipe(ref IntPtr hReadPipe, ref IntPtr hWritePipe, ref Advapi32.SECURITY_ATTRIBUTES lpPipeAttributes, int nSize);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool GetExitCodeProcess(IntPtr hProcess, ref int lpExitCode);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref Advapi32.STARTUPINFO lpStartupInfo, ref Advapi32.PROCESS_INFORMATION lpProcessInfo);
	}
}
