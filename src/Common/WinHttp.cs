using System;
using System.Runtime.InteropServices;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class WinHttp
	{
		[Flags]
		public enum WINHTTP_TYPE
		{
			WINHTTP_AUTO_DETECT_TYPE_DHCP = 0x1,
			WINHTTP_AUTO_DETECT_TYPE_DNS_A = 0x2
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct WINHTTP_AUTOPROXY_OPTIONS
		{
			public int dwFlags;

			public int dwAutoDetectFlags;

			[MarshalAs(UnmanagedType.LPTStr)]
			public string lpszAutoConfigUrl;

			private IntPtr lpvReserved;

			public int dwReserved;

			public bool fAutoLogonIfChallenged;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct WINHTTP_CURRENT_USER_IE_PROXY_CONFIG
		{
			public bool fAutoDetect;

			[MarshalAs(UnmanagedType.LPTStr)]
			public string lpszAutoConfigUrl;

			[MarshalAs(UnmanagedType.LPTStr)]
			public string lpszProxy;

			[MarshalAs(UnmanagedType.LPTStr)]
			public string lpszProxyBypass;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct WINHTTP_PROXY_INFO
		{
			public int dwAccessType;

			[MarshalAs(UnmanagedType.LPTStr)]
			public string lpszProxy;

			[MarshalAs(UnmanagedType.LPTStr)]
			public string lpszProxyBypass;
		}

		public const int WINHTTP_ACCESS_TYPE_NO_PROXY = 1;

		public const int WINHTTP_AUTOPROXY_AUTO_DETECT = 1;

		public const int WINHTTP_AUTOPROXY_CONFIG_URL = 2;

		[DllImport("winhttp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr WinHttpOpen(string pwszUserAgent, int dwAccessType, string pwszProxyName, string pwszProxyBypass, int dwFlags);

		[DllImport("winhttp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool WinHttpGetProxyForUrl(IntPtr hSession, string lpcwszUrl, [In] ref WINHTTP_AUTOPROXY_OPTIONS pAutoProxyOptions, [In][Out] ref WINHTTP_PROXY_INFO pProxyInfo);

		[DllImport("winhttp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool WinHttpGetIEProxyConfigForCurrentUser([In][Out] ref WINHTTP_CURRENT_USER_IE_PROXY_CONFIG pProxyConfig);

		[DllImport("winhttp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool WinHttpCloseHandle(IntPtr httpSession);
	}
}
