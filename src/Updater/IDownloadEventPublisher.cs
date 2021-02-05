namespace Microsoft.VSPowerToys.Updater
{
	public interface IDownloadEventPublisher
	{
		void RegisterListener<T>(T listener) where T : IDownloadEventListener;

		void UnRegisterListener<T>(T listener) where T : IDownloadEventListener;

		void NotifyListeners(DownloadEventArgs e);
	}
}
