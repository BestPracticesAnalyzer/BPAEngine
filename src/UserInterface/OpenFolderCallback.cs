using System;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class OpenFolderCallback : ButtonCallbackBase
	{
		private string dialogTitle = string.Empty;

		private string path = string.Empty;

		private bool allowNewFolder;

		public OpenFolderCallback(string dialogTitle)
			: this(dialogTitle, false)
		{
		}

		public OpenFolderCallback(string dialogTitle, bool allowNewFolder)
		{
			this.dialogTitle = dialogTitle;
			this.allowNewFolder = allowNewFolder;
		}

		public override void ShowDialog()
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.ShowNewFolderButton = allowNewFolder;
			folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
			folderBrowserDialog.Description = dialogTitle;
			if (folderBrowserDialog.ShowDialog(NewWrapper()) == DialogResult.OK)
			{
				path = folderBrowserDialog.SelectedPath;
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
