using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class Crypt
	{
		private const uint cryptFlags = 5u;

		private const string szDataDescr = "Microsoft";

		public static bool DecryptData(ExecutionInterface execInterface, byte[] pIn, ref byte[] pOut)
		{
			bool result = false;
			if (pIn.Length > 0)
			{
				Crypt32.DATA_BLOB pDataIn = default(Crypt32.DATA_BLOB);
				Crypt32.DATA_BLOB pDataOut = default(Crypt32.DATA_BLOB);
				pDataIn.cbData = pIn.Length;
				pDataIn.pbData = Marshal.AllocHGlobal(pDataIn.cbData);
				try
				{
					Marshal.Copy(pIn, 0, pDataIn.pbData, pDataIn.cbData);
					if (Crypt32.CryptUnprotectData(ref pDataIn, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, null, 5u, ref pDataOut))
					{
						pOut = new byte[pDataOut.cbData];
						Marshal.Copy(pDataOut.pbData, pOut, 0, pDataOut.cbData);
						result = true;
						return result;
					}
					int lastWin32Error = Marshal.GetLastWin32Error();
					Win32Exception ex = new Win32Exception(lastWin32Error);
					throw ex;
				}
				catch (Exception ex2)
				{
					execInterface.LogTrace("DecryptData : " + ex2.Message);
					return result;
				}
				finally
				{
					if (pDataOut.pbData != IntPtr.Zero)
					{
						Kernel32.LocalFree(pDataOut.pbData);
					}
					if (pDataIn.pbData != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(pDataIn.pbData);
					}
				}
			}
			return result;
		}

		public static bool EncryptData(ExecutionInterface execInterface, byte[] pIn, ref byte[] pOut)
		{
			bool result = false;
			if (pIn.Length > 0)
			{
				Crypt32.DATA_BLOB pDataIn = default(Crypt32.DATA_BLOB);
				Crypt32.DATA_BLOB pDataOut = default(Crypt32.DATA_BLOB);
				pDataIn.cbData = pIn.Length;
				pDataIn.pbData = Marshal.AllocHGlobal(pDataIn.cbData);
				try
				{
					Marshal.Copy(pIn, 0, pDataIn.pbData, pDataIn.cbData);
					if (Crypt32.CryptProtectData(ref pDataIn, "Microsoft", IntPtr.Zero, IntPtr.Zero, null, 5u, ref pDataOut))
					{
						pOut = new byte[pDataOut.cbData];
						Marshal.Copy(pDataOut.pbData, pOut, 0, pDataOut.cbData);
						result = true;
						return result;
					}
					int lastWin32Error = Marshal.GetLastWin32Error();
					Win32Exception ex = new Win32Exception(lastWin32Error);
					throw ex;
				}
				catch (Exception ex2)
				{
					execInterface.LogTrace("EncryptData : " + ex2.Message);
					return result;
				}
				finally
				{
					if (pDataOut.pbData != IntPtr.Zero)
					{
						Kernel32.LocalFree(pDataOut.pbData);
					}
					if (pDataIn.pbData != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(pDataIn.pbData);
					}
				}
			}
			return result;
		}
	}
}
