using System;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class Win32WindowWrapper : IWin32Window
	{
		private IntPtr _hwnd = IntPtr.Zero;

		public IntPtr Handle
		{
			get
			{
				return _hwnd;
			}
		}

		public Win32WindowWrapper(IntPtr handle)
		{
			_hwnd = handle;
		}
	}
}
