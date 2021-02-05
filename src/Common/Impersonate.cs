using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class Impersonate
	{
		private class SecurityContextThreadInfo
		{
			public string securityContextName;

			public WindowsImpersonationContext impersonatedUser;
		}

		public class SecurityContext
		{
			private Credentials credentials;

			private IntPtr dupeTokenHandle = IntPtr.Zero;

			public string Name
			{
				get
				{
					return credentials.ContextName;
				}
			}

			public Credentials Credentials
			{
				get
				{
					return credentials;
				}
			}

			public SecurityContext(Credentials credentials)
			{
				this.credentials = credentials;
				dupeTokenHandle = IntPtr.Zero;
				if (credentials.User.Length == 0)
				{
					throw new ArgumentException();
				}
				dupeTokenHandle = CreateImpersonationToken(credentials.User, credentials.Domain, credentials.Password);
			}

			~SecurityContext()
			{
				Dispose();
			}

			public void Dispose()
			{
				if (dupeTokenHandle != IntPtr.Zero)
				{
					Kernel32.CloseHandle(dupeTokenHandle);
				}
				dupeTokenHandle = IntPtr.Zero;
				GC.KeepAlive(this);
			}

			public bool Impersonate()
			{
				bool result = Advapi32.ImpersonateLoggedOnUser(dupeTokenHandle);
				GC.KeepAlive(this);
				return result;
			}
		}

		private LocalDataStoreSlot securityContextSlot = Thread.AllocateDataSlot();

		private SortedList securityContexts = new SortedList();

		public SortedList SecurityContexts
		{
			get
			{
				return securityContexts;
			}
		}

		public void Clear()
		{
			foreach (SecurityContext value in securityContexts.Values)
			{
				value.Dispose();
			}
			securityContexts.Clear();
		}

		public void AddSecurityContext(Credentials credentials)
		{
			if (!securityContexts.Contains(credentials.ContextName))
			{
				SecurityContext securityContext = new SecurityContext(credentials);
				securityContexts.Add(securityContext.Name, securityContext);
			}
		}

		public SecurityContext GetCurrentSecurityContext()
		{
			SecurityContextThreadInfo securityContextThreadInfo = (SecurityContextThreadInfo)Thread.GetData(securityContextSlot);
			if (securityContextThreadInfo == null)
			{
				return null;
			}
			return (SecurityContext)securityContexts[securityContextThreadInfo.securityContextName];
		}

		public static WindowsImpersonationContext ImpersonateUser(SecurityContext securityContext)
		{
			securityContext.Impersonate();
			return null;
		}

		internal void SetSecurityContext(ObjectParentData opd)
		{
			SecurityContext securityContext = null;
			string text = opd.InheritedProperty("SecurityContext").ToString();
			if (securityContexts.ContainsKey(text))
			{
				securityContext = (SecurityContext)securityContexts[text];
			}
			SecurityContextThreadInfo securityContextThreadInfo = (SecurityContextThreadInfo)Thread.GetData(securityContextSlot);
			if (securityContextThreadInfo == null && securityContext != null && securityContext.Credentials.User.Length > 0)
			{
				securityContextThreadInfo = new SecurityContextThreadInfo();
				securityContextThreadInfo.impersonatedUser = ImpersonateUser(securityContext);
				securityContextThreadInfo.securityContextName = text;
				Thread.SetData(securityContextSlot, securityContextThreadInfo);
			}
			else if (securityContextThreadInfo != null && (securityContext == null || securityContext.Credentials.User.Length == 0))
			{
				UnimpersonateUser(securityContextThreadInfo.impersonatedUser);
				Thread.SetData(securityContextSlot, null);
			}
			else if (securityContextThreadInfo != null && securityContext != null && securityContextThreadInfo.securityContextName.CompareTo(text) != 0)
			{
				UnimpersonateUser(securityContextThreadInfo.impersonatedUser);
				if (securityContext.Credentials.User.Length > 0)
				{
					securityContextThreadInfo.impersonatedUser = ImpersonateUser(securityContext);
					securityContextThreadInfo.securityContextName = text;
				}
				else
				{
					securityContextThreadInfo = null;
				}
				Thread.SetData(securityContextSlot, securityContextThreadInfo);
			}
		}

		public static IntPtr CreateImpersonationToken(string strUser, string strDomain, string strPassword)
		{
			Advapi32.STARTUPINFO lpStartupInfo = default(Advapi32.STARTUPINFO);
			Advapi32.PROCESS_INFORMATION lpProcessInfo = default(Advapi32.PROCESS_INFORMATION);
			IntPtr TokenHandle = IntPtr.Zero;
			IntPtr DuplicateTokenHandle = IntPtr.Zero;
			uint dwLogonFlags = 2u;
			uint dwCreationFlags = 4u;
			uint sECURITY_IMPERSONATION_LEVEL = 2u;
			uint desiredAccess = 2u;
			int logonType = 2;
			int logonType2 = 9;
			int logonProvider = 0;
			int logonProvider2 = 3;
			IntPtr phToken = IntPtr.Zero;
			lpStartupInfo.cb = (uint)Marshal.SizeOf(typeof(Advapi32.STARTUPINFO));
			try
			{
				if (Advapi32.LogonUser(strUser, strDomain, strPassword, logonType, logonProvider, ref phToken))
				{
					if (Advapi32.DuplicateToken(phToken, sECURITY_IMPERSONATION_LEVEL, ref DuplicateTokenHandle))
					{
						return DuplicateTokenHandle;
					}
					Kernel32.CloseHandle(phToken);
					phToken = IntPtr.Zero;
				}
				WindowsIdentity current = WindowsIdentity.GetCurrent();
				if (!current.IsSystem)
				{
					if (!Advapi32.CreateProcessWithLogonW(strUser, strDomain, strPassword, dwLogonFlags, null, "cmd.exe", dwCreationFlags, IntPtr.Zero, null, ref lpStartupInfo, ref lpProcessInfo))
					{
						throw new Exception("CreateProcessWithLogonW Failed with " + Marshal.GetLastWin32Error());
					}
				}
				else
				{
					if (!Advapi32.LogonUser(strUser, strDomain, strPassword, logonType2, logonProvider2, ref phToken))
					{
						throw new Exception("LogonUser Failed with " + Marshal.GetLastWin32Error());
					}
					if (!Advapi32.CreateProcessAsUser(phToken, null, "cmd.exe", IntPtr.Zero, IntPtr.Zero, false, dwCreationFlags, IntPtr.Zero, null, ref lpStartupInfo, ref lpProcessInfo))
					{
						throw new Exception("CreateProcessAsUser Failed with " + Marshal.GetLastWin32Error());
					}
				}
				if (!Advapi32.OpenProcessToken(lpProcessInfo.hProcess, desiredAccess, ref TokenHandle))
				{
					throw new Exception("OpenProcessToken Failed with " + Marshal.GetLastWin32Error());
				}
				if (!Advapi32.DuplicateToken(TokenHandle, sECURITY_IMPERSONATION_LEVEL, ref DuplicateTokenHandle))
				{
					throw new Exception("DuplicateToken Failed with " + Marshal.GetLastWin32Error());
				}
				if (lpProcessInfo.hProcess != IntPtr.Zero)
				{
					Kernel32.TerminateProcess(lpProcessInfo.hProcess, 0u);
				}
				int num = Kernel32.WaitForSingleObject(lpProcessInfo.hProcess, 15000);
				if (num != 0)
				{
					throw new Exception("WaitForSingleObject Failed with " + num);
				}
				return DuplicateTokenHandle;
			}
			finally
			{
				if (phToken != IntPtr.Zero)
				{
					Kernel32.CloseHandle(phToken);
				}
				if (TokenHandle != IntPtr.Zero)
				{
					Kernel32.CloseHandle(TokenHandle);
				}
				if (lpProcessInfo.hThread != IntPtr.Zero)
				{
					Kernel32.CloseHandle(lpProcessInfo.hThread);
				}
				if (lpProcessInfo.hProcess != IntPtr.Zero)
				{
					Kernel32.CloseHandle(lpProcessInfo.hProcess);
				}
			}
		}

		private void UnimpersonateUser(WindowsImpersonationContext impersonatedUser)
		{
			impersonatedUser = null;
			Advapi32.RevertToSelf();
		}
	}
}
