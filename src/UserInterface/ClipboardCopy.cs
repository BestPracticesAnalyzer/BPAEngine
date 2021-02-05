using System.Threading;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class ClipboardCopy
	{
		private static object lockAccess = new object();

		private object objToCopy;

		private ClipboardCopy(object objToCopy)
		{
			this.objToCopy = objToCopy;
		}

		public static void StartCopy(object objToCopy)
		{
			lock (lockAccess)
			{
				ClipboardCopy @object = new ClipboardCopy(objToCopy);
				Thread thread = new Thread(@object.Copy);
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();
				thread.Join();
			}
		}

		private void Copy()
		{
			Clipboard.SetDataObject(objToCopy, true);
		}
	}
}
