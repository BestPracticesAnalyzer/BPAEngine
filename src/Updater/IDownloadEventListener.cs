namespace Microsoft.VSPowerToys.Updater
{
	public interface IDownloadEventListener
	{
		void OnDownloadEvent(object sender, DownloadEventArgs e);
	}
}
