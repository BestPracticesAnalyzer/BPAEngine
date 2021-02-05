using System;

namespace Microsoft.VSPowerToys.Updater
{
	public class FileUri : Uri
	{
		private FileManifest fm;

		private string sourceFile;

		public string FileName
		{
			get
			{
				if (fm != null)
				{
					return fm.Source;
				}
				return sourceFile;
			}
		}

		public FileManifest FileManifest
		{
			get
			{
				return fm;
			}
		}

		public FileUri(string sUri, FileManifest fileManifest)
			: base(sUri)
		{
			fm = fileManifest;
		}

		public FileUri(string sUri, string manifestFileName)
			: base(sUri)
		{
			fm = null;
			if (string.IsNullOrEmpty(manifestFileName))
			{
				throw new ArgumentNullException(Updater.fileName);
			}
			sourceFile = manifestFileName;
		}
	}
}
