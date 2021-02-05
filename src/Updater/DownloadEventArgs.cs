using System;

namespace Microsoft.VSPowerToys.Updater
{
	public class DownloadEventArgs : EventArgs
	{
		private Uri fileurl;

		private long filesize;

		private long downloadedsize;

		private string fileName;

		public long Size
		{
			get
			{
				return filesize;
			}
		}

		public long Downloaded
		{
			get
			{
				return downloadedsize;
			}
		}

		public int PercentDownloaded
		{
			get
			{
				if (filesize > 0)
				{
					return (int)(downloadedsize * 100 / filesize);
				}
				return 0;
			}
		}

		public Uri Url
		{
			get
			{
				return fileurl;
			}
		}

		public string FileName
		{
			get
			{
				return fileName;
			}
		}

		public DownloadEventArgs(long sz, long bytesDownloaded, Uri from, string downloadFileName)
		{
			fileurl = from;
			filesize = sz;
			downloadedsize = bytesDownloaded;
			fileName = downloadFileName;
		}
	}
}
