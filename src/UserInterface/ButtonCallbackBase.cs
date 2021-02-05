using System;
using System.Threading;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class ButtonCallbackBase
	{
		protected ShowDialogCallback dialogCallback;

		private IntPtr handle = IntPtr.Zero;

		public virtual void Callback(ShowDialogCallback dialogCallback, IntPtr handle)
		{
			this.dialogCallback = dialogCallback;
			this.handle = handle;
			ThreadStart start = ShowDialog;
			Thread thread = new Thread(start);
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}

		public virtual void ShowDialog()
		{
		}

		public virtual object[] Setting(Node node)
		{
			return null;
		}

		protected Win32WindowWrapper NewWrapper()
		{
			return new Win32WindowWrapper(handle);
		}
	}
}
