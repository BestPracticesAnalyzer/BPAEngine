using System;
using System.Collections.ObjectModel;

namespace Microsoft.VSPowerToys.Updater
{
	public class DownloadManager
	{
		private HttpDownloader downloader = new HttpDownloader();

		public ReadOnlyCollection<string> DownloadAll(Collection<Uri> downloadUrls, string downloadDir)
		{
			Collection<string> collection = new Collection<string>();
			foreach (Uri downloadUrl in downloadUrls)
			{
				string item = downloader.DownloadFile(downloadUrl, downloadDir);
				collection.Add(item);
			}
			return new ReadOnlyCollection<string>(collection);
		}

		public void RegisterListener(IDownloadEventListener caller)
		{
			downloader.RegisterListener(caller);
		}

		public void UnRegisterListener(IDownloadEventListener caller)
		{
			downloader.UnRegisterListener(caller);
		}
	}
}
