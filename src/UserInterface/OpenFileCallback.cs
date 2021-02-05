using System;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class OpenFileCallback : ButtonCallbackBase
	{
		private string dialogTitle = string.Empty;

		private string filter = string.Empty;

		private string path = string.Empty;

		public OpenFileCallback(string dialogTitle, string filter)
		{
			this.dialogTitle = dialogTitle;
			this.filter = filter;
		}

		public override void ShowDialog()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			openFileDialog.RestoreDirectory = true;
			openFileDialog.Filter = filter;
			openFileDialog.CheckFileExists = true;
			openFileDialog.Title = dialogTitle;
			if (openFileDialog.ShowDialog(NewWrapper()) == DialogResult.OK)
			{
				path = openFileDialog.FileName;
				if (dialogCallback != null)
				{
					dialogCallback(path);
				}
			}
		}

		public override object[] Setting(Node node)
		{
			return new object[1]
			{
				path
			};
		}
	}
}
