using System;
using System.Diagnostics;
using System.IO;

namespace Microsoft.VSPowerToys.Updater
{
	public class FileUnzipTask : InstallTask
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

		public string DestinationDir
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

		public FileUnzipTask()
		{
		}

		public FileUnzipTask(string srcFile, string destDir)
		{
			if (string.IsNullOrEmpty(destDir))
			{
				throw new ArgumentNullException(Updater.destFile);
			}
			src = srcFile;
			dest = destDir;
		}

		public override void Execute()
		{
			FileInfo fileInfo = new FileInfo(dest);
			if (!fileInfo.Directory.Exists)
			{
				throw new DirectoryNotFoundException(Updater.DirectoryNotFound);
			}
			string text = Path.Combine(Environment.SystemDirectory, "Expand.exe");
			if (!File.Exists(text))
			{
				throw new InvalidOperationException(Updater.CannotFindTool + "expand.exe");
			}
			string text2 = "\"" + src + "\" \"" + dest + "\"";
			ProcessStartInfo processStartInfo = new ProcessStartInfo(text, text2);
			processStartInfo.UseShellExecute = false;
			processStartInfo.CreateNoWindow = true;
			Process process = Process.Start(processStartInfo);
			process.WaitForExit(base.TimeOut);
			if (!process.HasExited)
			{
				if (process.Responding)
				{
					process.CloseMainWindow();
				}
				else
				{
					process.Kill();
				}
			}
			if (process.ExitCode != 0)
			{
				throw new Exception(Updater.UnzipFailed + process.ExitCode + " " + text2);
			}
		}
	}
}
