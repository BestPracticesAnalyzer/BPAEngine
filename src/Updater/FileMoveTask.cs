using System;
using System.IO;

namespace Microsoft.VSPowerToys.Updater
{
	public class FileMoveTask : InstallTask
	{
		private string src;

		private string dest;

		public string SourceFile
		{
			get
			{
				return src;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException(Updater.srcFile);
				}
				src = value;
			}
		}

		public string DestinationFile
		{
			get
			{
				return dest;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException(Updater.destFile);
				}
				dest = value;
			}
		}

		public FileMoveTask()
		{
		}

		public FileMoveTask(string srcFile, string destFile)
		{
			if (string.IsNullOrEmpty(destFile))
			{
				throw new ArgumentNullException(Updater.destFile);
			}
			src = srcFile;
			dest = destFile;
		}

		public override void Execute()
		{
			FileInfo fileInfo = new FileInfo(src);
			if (!fileInfo.Exists)
			{
				throw new FileNotFoundException(src);
			}
			FileInfo fileInfo2 = new FileInfo(dest);
			if (fileInfo2.Exists)
			{
				if (File.Exists(fileInfo2.FullName + ".prev"))
				{
					File.Delete(fileInfo2.FullName + ".prev");
				}
				File.Move(fileInfo2.FullName, fileInfo2.FullName + ".prev");
			}
			if (!fileInfo2.Directory.Exists)
			{
				throw new DirectoryNotFoundException(Updater.DirectoryNotFound);
			}
			File.Move(src, dest);
		}
	}
}
