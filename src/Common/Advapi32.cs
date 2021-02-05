using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	[CLSCompliant(false)]
	public class Advapi32
	{
		[Flags]
		public enum SECURITY_DESCRIPTOR_CONTROL : ushort
		{
			SE_OWNER_DEFAULTED = 0x1,
			SE_GROUP_DEFAULTED = 0x2,
			SE_DACL_PRESENT = 0x4,
			SE_DACL_DEFAULTED = 0x8,
			SE_SACL_PRESENT = 0x10,
			SE_SACL_DEFAULTED = 0x20,
			SE_DACL_UNTRUSTED = 0x40,
			SE_SERVER_SECURITY = 0x80,
			SE_DACL_AUTO_INHERIT_REQ = 0x100,
			SE_SACL_AUTO_INHERIT_REQ = 0x200,
			SE_DACL_AUTO_INHERITED = 0x400,
			SE_SACL_AUTO_INHERITED = 0x800,
			SE_DACL_PROTECTED = 0x1000,
			SE_SACL_PROTECTED = 0x2000,
			SE_RM_CONTROL_VALID = 0x4000,
			SE_SELF_RELATIVE = 0x8000
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct STARTUPINFO
		{
			public uint cb;

			public string lpReserved;

			public string lpDesktop;

			public string lpTitle;

			public uint dwX;

			public uint dwY;

			public uint dwXSize;

			public uint dwYSize;

			public uint dwXCountChars;

			public uint dwYCountChars;

			public uint dwFillAttribute;

			public uint dwFlags;

			public short wShowWindow;

			public short cbReserved2;

			internal IntPtr lpReserved2;

			internal IntPtr hStdInput;

			internal IntPtr hStdOutput;

			internal IntPtr hStdError;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public class STARTUPINFOWrapper
		{
			private STARTUPINFO info;

			public STARTUPINFO GetStartUpInfo
			{
				get
				{
					return info;
				}
			}

			public STARTUPINFOWrapper()
			{
				info = default(STARTUPINFO);
			}

			public STARTUPINFOWrapper(IntPtr stdInput, IntPtr stdOutput, IntPtr stdError)
				: this()
			{
				info.hStdInput = stdInput;
				info.hStdOutput = stdOutput;
				info.hStdError = stdError;
			}
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct PROCESS_INFORMATION
		{
			internal IntPtr hProcess;

			internal IntPtr hThread;

			public uint dwProcessId;

			public uint dwThreadId;

			public IntPtr ProcessHandle
			{
				get
				{
					return hProcess;
				}
			}

			public IntPtr ThreadHandle
			{
				get
				{
					return hThread;
				}
			}
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct SECURITY_ATTRIBUTES
		{
			public int Length;

			internal IntPtr lpSecurityDescriptor;

			public bool bInheritHandle;
		}

		public const int SDDL_REVISION_1 = 1;

		public const int REG_NONE = 0;

		public const int REG_SZ = 1;

		public const int REG_EXPAND_SZ = 2;

		public const int REG_BINARY = 3;

		public const int REG_DWORD = 4;

		public const int REG_MULTI_SZ = 7;

		public const int REG_QWORD = 11;

		public static readonly IntPtr HKEY_CLASSES_ROOT = new IntPtr(int.MinValue);

		public static readonly IntPtr HKEY_CURRENT_USER = new IntPtr(-2147483647);

		public static readonly IntPtr HKEY_LOCAL_MACHINE = new IntPtr(-2147483646);

		public static readonly IntPtr HKEY_USERS = new IntPtr(-2147483645);

		public static readonly IntPtr HKEY_CURRENT_CONFIG = new IntPtr(-2147483643);

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern bool CheckTokenMembership(IntPtr TokenHandle, IntPtr SidToCheck, ref bool IsMember);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		public static extern bool CreateProcessWithLogonW(string lpUsername, string lpDomain, string lpPassword, uint dwLogonFlags, string lpApplicationName, string lpCommandLine, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, ref PROCESS_INFORMATION lpProcessInfo);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool CreateProcessAsUser(IntPtr phToken, string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool inheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, ref PROCESS_INFORMATION lpProcessInfo);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool LogonUser(string szUserName, string szDomain, string szPassword, int logonType, int logonProvider, ref IntPtr phToken);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, ref IntPtr TokenHandle);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool DuplicateToken(IntPtr ExistingTokenHandle, uint SECURITY_IMPERSONATION_LEVEL, ref IntPtr DuplicateTokenHandle);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool ImpersonateLoggedOnUser(IntPtr userToken);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool RevertToSelf();

		[DllImport("Advapi32.dll", SetLastError = true)]
		public static extern bool GetSecurityDescriptorControl(IntPtr pSecurityDescriptor, out SECURITY_DESCRIPTOR_CONTROL sdcontrol, out int dwRevision);

		[DllImport("Advapi32.dll", SetLastError = true)]
		public static extern bool GetSecurityDescriptorOwner(IntPtr pSecurityDescriptor, out IntPtr pOwner, out bool lpbOwnerDefaulted);

		[DllImport("Advapi32.dll", SetLastError = true)]
		public static extern bool GetSecurityDescriptorGroup(IntPtr pSecurityDescriptor, out IntPtr pGroup, out bool lpbGroupDefaulted);

		[DllImport("Advapi32.dll", SetLastError = true)]
		public static extern bool GetSecurityDescriptorDacl(IntPtr pSecurityDescriptor, out bool lpbDaclPresent, ref IntPtr pDacl, out bool lpbDaclDefaulted);

		[DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool ConvertStringSecurityDescriptorToSecurityDescriptor(string szSD, int version, out IntPtr pSecurityDescriptor, ref int sdSize);

		[DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool ConvertSidToStringSid(IntPtr sid, [In][Out][MarshalAs(UnmanagedType.LPTStr)] ref string pStringSid);

		[DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool ConvertStringSidToSid([In][MarshalAs(UnmanagedType.LPTStr)] string stringSid, out IntPtr sid);

		[DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool LookupAccountName(string lpSystemName, string lpAccountName, IntPtr Sid, ref int cbSid, StringBuilder DomainName, ref int cbDomainName, out int peUse);

		[DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool LookupAccountSid(string lpSystemName, IntPtr lpSid, StringBuilder lpName, ref int cchName, StringBuilder lpDomainName, ref int cchDomainName, out int peUse);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int RegConnectRegistry(string machineName, IntPtr hKey, out IntPtr phkResult);

		[DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int RegCloseKey(IntPtr hKey);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int RegEnumKeyEx(IntPtr hKey, int index, StringBuilder lpName, ref int lpcName, IntPtr lpReserved, IntPtr lpClass, IntPtr lpcClass, ref ulong lpftLastWriteTime);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto)]
		public static extern int RegEnumValue(IntPtr hKey, int index, StringBuilder lpValueName, ref int lpcValueName, IntPtr lpReserved, ref int lpType, IntPtr lpData, ref int lpcbData);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int RegOpenKey(IntPtr hKey, string lpSubKey, out IntPtr phkResult);

		[DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int RegOpenKeyEx(IntPtr hKey, string lpSubKey, int ulOptions, int samDesired, out IntPtr phkResult);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int RegQueryValueEx(IntPtr hKey, string subKey, IntPtr lpReserved, ref int lpType, IntPtr lpData, ref int lpcData);

		[DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int RegGetKeySecurity(IntPtr hKey, int securityInformation, IntPtr pSecurityDescriptor, ref int cbSecurityDescriptor);

		[DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int RegSetKeySecurity(IntPtr hKey, int SecurityInformation, IntPtr pSecurityDescriptor);

		[DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr RegisterEventSource(string lpUNCServerName, string lpSourceName);

		[DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool DeregisterEventSource(IntPtr hEventLog);

		[DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool ReportEvent(IntPtr hEventLog, short wType, short wCategory, int dwEventID, IntPtr lpUserSid, short wNumStrings, int dwDataSize, string[] lpStrings, IntPtr lpRawData);
	}
}
