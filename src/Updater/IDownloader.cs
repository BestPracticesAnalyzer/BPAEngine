using System;

namespace Microsoft.VSPowerToys.Updater
{
	public interface IDownloader
	{
		string DownloadFile(Uri fromUrl, string downloadDir);
	}
}
