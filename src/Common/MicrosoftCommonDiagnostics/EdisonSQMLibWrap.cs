using System;
using System.Runtime.InteropServices;

namespace Microsoft.WinSE.DiagnosticTools.MicrosoftCommonDiagnostics
{
	internal class EdisonSQMLibWrap
	{
		[DllImport("McdBpaSqmCWrapper.dll")]
		public static extern bool InitializeMcdSQM();

		[DllImport("McdBpaSqmCWrapper.dll")]
		public static extern bool CreateBpaSession(ref IntPtr ppSession);

		[DllImport("McdBpaSqmCWrapper.dll")]
		public static extern bool InitializeBpaSession(IntPtr pSession);

		[DllImport("McdBpaSqmCWrapper.dll")]
		public static extern bool StartBpaSession(IntPtr pSession);

		[DllImport("McdBpaSqmCWrapper.dll")]
		public static extern void SetRuleSetStatus(IntPtr pSession, uint RuleSetId, uint RuleSetVer, bool RuleSetResult, uint RuleSetTimeToExecute);

		[DllImport("McdBpaSqmCWrapper.dll")]
		public static extern void SetRuleStatus(IntPtr pSession, uint RuleId, bool RuleResult);

		[DllImport("McdBpaSqmCWrapper.dll")]
		public static extern bool EndBpaSession(IntPtr pSession);

		[DllImport("McdBpaSqmCWrapper.dll")]
		public static extern bool FreeBpaSession(ref IntPtr ppSession);
	}
}
