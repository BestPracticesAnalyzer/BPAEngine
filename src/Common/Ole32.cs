using System;
using System.Runtime.InteropServices;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class Ole32
	{
		public const int RPC_C_AUTHN_GSS_NEGOTIATE = 9;

		public const int RPC_C_AUTHN_LEVEL_CONNECT = 2;

		public const int EOAC_DEFAULT = 2048;

		public const int EOAC_STATIC_CLOAKING = 32;

		public const int EOAC_DYNAMIC_CLOAKING = 64;

		public const int RPC_C_IMP_LEVEL_DEFAULT = 0;

		public const int RPC_C_IMP_LEVEL_ANONYMOUS = 1;

		public const int RPC_C_IMP_LEVEL_IDENTITY = 2;

		public const int RPC_C_IMP_LEVEL_IMPERSONATE = 3;

		public const int RPC_C_IMP_LEVEL_DELEGATE = 4;

		[DllImport("ole32.dll")]
		public static extern int CoInitializeSecurity(IntPtr pVoid, int cAuthSvc, IntPtr asAuthSvc, IntPtr pReserved1, int dwAuthnLevel, int dwImpLevel, IntPtr pAuthList, int dwCapabilities, IntPtr pReserved3);
	}
}
