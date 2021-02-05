using System;
using System.Runtime.InteropServices;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	internal class Crypt32
	{
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DATA_BLOB
		{
			public int cbData;

			internal IntPtr pbData;
		}

		public const uint CRYPTPROTECT_UI_FORBIDDEN = 1u;

		public const uint CRYPTPROTECT_LOCAL_MACHINE = 4u;

		[DllImport("Crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool CryptProtectData(ref DATA_BLOB pDataIn, string szDataDescr, IntPtr pOptionalEntropy, IntPtr pvReserved, string szPrompt, uint dwFlags, ref DATA_BLOB pDataOut);

		[DllImport("Crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool CryptUnprotectData(ref DATA_BLOB pDataIn, IntPtr ppszDataDescr, IntPtr pOptionalEntropy, IntPtr pvReserved, string szPrompt, uint dwFlags, ref DATA_BLOB pDataOut);
	}
}
