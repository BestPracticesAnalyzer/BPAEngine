using System;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class SaveFileCallback : ButtonCallbackBase
	{
		private string dialogTitle = string.Empty;

		private string filter = string.Empty;

		private string saveFileName = string.Empty;

		private string defaultExt = string.Empty;

		private bool addExt;

		private string path = string.Empty;

		public SaveFileCallback(string dialogTitle, string filter, string saveFileName, string defaultExt, bool addExt)
		{
			this.dialogTitle = dialogTitle;
			this.filter = filter;
			this.saveFileName = saveFileName;
			this.defaultExt = defaultExt;
			this.addExt = addExt;
		}

		public override void ShowDialog()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			saveFileDialog.RestoreDirectory = true;
			saveFileDialog.Filter = filter;
			saveFileDialog.Title = dialogTitle;
			saveFileDialog.FileName = saveFileName;
			saveFileDialog.DefaultExt = defaultExt;
			saveFileDialog.AddExtension = addExt;
			if (saveFileDialog.ShowDialog(NewWrapper()) == DialogResult.OK)
			{
				path = saveFileDialog.FileName;
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
