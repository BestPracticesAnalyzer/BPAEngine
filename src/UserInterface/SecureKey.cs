using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;
using Microsoft.Win32;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class SecureKey
	{
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct SECURITY_ATTRIBUTES
		{
			public int nLength;

			internal IntPtr lpSecurityDescriptor;

			public bool bInheritHandle;
		}

		private string secureKeyPath;

		private string secureKeyValue;

		private IntPtr hkeyRoot = IntPtr.Zero;

		private string szSecurityDescriptor;

		[DllImport("Advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int RegCreateKeyEx(IntPtr hKey, string subKeyName, int reserved, string lpClass, int option, int regSAM, ref SECURITY_ATTRIBUTES lpSA, out IntPtr phkResult, ref int disposition);

		public SecureKey(IntPtr hkeyRoot, string secureKeyPath, string secureKeyValue, string szSecurityDescriptor)
		{
			this.hkeyRoot = hkeyRoot;
			this.secureKeyPath = secureKeyPath;
			this.secureKeyValue = secureKeyValue;
			this.szSecurityDescriptor = szSecurityDescriptor;
		}

		private bool CreateSecureKey(ExecutionInterface execInterface, string subKey)
		{
			bool result = false;
			SECURITY_ATTRIBUTES lpSA = default(SECURITY_ATTRIBUTES);
			lpSA.nLength = Marshal.SizeOf(lpSA);
			lpSA.bInheritHandle = false;
			IntPtr phkResult = IntPtr.Zero;
			int disposition = 0;
			int sdSize = 0;
			try
			{
				if (Advapi32.ConvertStringSecurityDescriptorToSecurityDescriptor(szSecurityDescriptor, 1, out lpSA.lpSecurityDescriptor, ref sdSize))
				{
					if (RegCreateKeyEx(hkeyRoot, subKey, 0, null, 0, 0, ref lpSA, out phkResult, ref disposition) == 0)
					{
						Advapi32.RegCloseKey(phkResult);
						result = true;
						return result;
					}
					return result;
				}
				return result;
			}
			catch (Exception ex)
			{
				execInterface.LogTrace("CreateSecureKey : " + ex.Message);
				return result;
			}
			finally
			{
				if (lpSA.lpSecurityDescriptor != IntPtr.Zero)
				{
					Kernel32.LocalFree(lpSA.lpSecurityDescriptor);
				}
			}
		}

		private bool SetSecureKey(ExecutionInterface execInterface, string subKey)
		{
			bool result = false;
			int samDesired = 983040;
			int securityInformation = 4;
			SECURITY_ATTRIBUTES sECURITY_ATTRIBUTES = default(SECURITY_ATTRIBUTES);
			IntPtr phkResult = IntPtr.Zero;
			int sdSize = 0;
			int num = 0;
			try
			{
				num = Advapi32.RegOpenKeyEx(hkeyRoot, subKey, 0, samDesired, out phkResult);
				if (num == 0)
				{
					if (Advapi32.ConvertStringSecurityDescriptorToSecurityDescriptor(szSecurityDescriptor, 1, out sECURITY_ATTRIBUTES.lpSecurityDescriptor, ref sdSize))
					{
						num = Advapi32.RegSetKeySecurity(phkResult, securityInformation, sECURITY_ATTRIBUTES.lpSecurityDescriptor);
						if (num == 0)
						{
							result = true;
						}
					}
					else
					{
						num = Marshal.GetLastWin32Error();
					}
				}
				if (num != 0)
				{
					Win32Exception ex = new Win32Exception(num);
					throw ex;
				}
				return result;
			}
			catch (Exception ex2)
			{
				execInterface.LogTrace("SetSecureKey : " + ex2.Message);
				return result;
			}
			finally
			{
				if (sECURITY_ATTRIBUTES.lpSecurityDescriptor != IntPtr.Zero)
				{
					Kernel32.LocalFree(sECURITY_ATTRIBUTES.lpSecurityDescriptor);
				}
				if (phkResult != IntPtr.Zero)
				{
					Advapi32.RegCloseKey(phkResult);
				}
			}
		}

		public bool Save(ExecutionInterface execInterface, byte[] data)
		{
			bool flag = false;
			RegistryKey registryKey = null;
			try
			{
				registryKey = Registry.LocalMachine.OpenSubKey(secureKeyPath, true);
				flag = ((registryKey == null) ? CreateSecureKey(execInterface, secureKeyPath) : SetSecureKey(execInterface, secureKeyPath));
				if (flag)
				{
					registryKey = Registry.LocalMachine.OpenSubKey(secureKeyPath, true);
					if (registryKey != null)
					{
						registryKey.SetValue(secureKeyValue, data);
						return flag;
					}
					return false;
				}
				return flag;
			}
			catch (Exception ex)
			{
				execInterface.LogTrace("RegistryKeySave : " + ex.Message);
				return false;
			}
		}

		public bool View(ExecutionInterface execInterface, ref byte[] data)
		{
			bool result = false;
			RegistryKey registryKey = null;
			try
			{
				using (registryKey = Registry.LocalMachine.OpenSubKey(secureKeyPath))
				{
					if (registryKey != null)
					{
						data = (byte[])registryKey.GetValue(secureKeyValue, data);
						if (data != null)
						{
							if (data.Length > 0)
							{
								return true;
							}
							return result;
						}
						return result;
					}
					return result;
				}
			}
			catch (Exception ex)
			{
				execInterface.LogTrace("RegistryKeyView : " + ex.Message);
				return false;
			}
		}

		public bool Delete(ExecutionInterface execInterface)
		{
			bool result = false;
			RegistryKey registryKey = null;
			try
			{
				using (registryKey = Registry.LocalMachine.OpenSubKey(secureKeyPath, true))
				{
					if (registryKey != null)
					{
						registryKey.DeleteValue(secureKeyValue, true);
						result = true;
						return result;
					}
					return result;
				}
			}
			catch (Exception ex)
			{
				execInterface.LogTrace("RegistryKeyDel : " + ex.Message);
				return result;
			}
		}
	}
}
