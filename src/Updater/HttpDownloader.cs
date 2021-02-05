using System;
using System.IO;
using System.Net;
using System.Security;

namespace Microsoft.VSPowerToys.Updater
{
	public class HttpDownloader : IDownloader, IDownloadEventPublisher
	{
		private const int BLOCK = 20000;

		private int failures;

		private int retry = 3;

		public int RetryAttempts
		{
			get
			{
				return retry;
			}
			set
			{
				retry = value;
			}
		}

		private event EventHandler<DownloadEventArgs> DownloadEvent;

		public void RegisterListener<T>(T listener) where T : IDownloadEventListener
		{
			this.DownloadEvent = (EventHandler<DownloadEventArgs>)Delegate.Combine(this.DownloadEvent, new EventHandler<DownloadEventArgs>(((IDownloadEventListener)listener).OnDownloadEvent));
		}

		public void UnRegisterListener<T>(T listener) where T : IDownloadEventListener
		{
			this.DownloadEvent = (EventHandler<DownloadEventArgs>)Delegate.Remove(this.DownloadEvent, new EventHandler<DownloadEventArgs>(((IDownloadEventListener)listener).OnDownloadEvent));
		}

		public void NotifyListeners(DownloadEventArgs e)
		{
			if (this.DownloadEvent != null)
			{
				this.DownloadEvent(this, e);
			}
		}

		public string DownloadFile(Uri fromUrl, string downloadDir)
		{
			BinaryReader binaryReader = null;
			FileStream fileStream = null;
			BinaryWriter binaryWriter = null;
			bool flag = true;
			HttpWebResponse httpWebResponse = null;
			string fileName = ((FileUri)fromUrl).FileName;
			string text = downloadDir + "\\" + fileName;
			while (flag)
			{
				try
				{
					flag = false;
					HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(fromUrl);
					httpWebRequest.PreAuthenticate = true;
					httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
					httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
					if (httpWebResponse.StatusCode != HttpStatusCode.OK)
					{
						failures = retry;
						throw new FileNotFoundException();
					}
					long contentLength = httpWebResponse.ContentLength;
					long num = 0L;
					binaryReader = new BinaryReader(httpWebResponse.GetResponseStream());
					fileStream = new FileStream(text, FileMode.Create);
					binaryWriter = new BinaryWriter(fileStream);
					byte[] buffer = new byte[20000];
					int num2 = 0;
					while ((num2 = binaryReader.Read(buffer, 0, 20000)) > 0)
					{
						num += num2;
						NotifyListeners(new DownloadEventArgs(contentLength, num, fromUrl, fileName));
						binaryWriter.Write(buffer, 0, num2);
					}
				}
				catch (Exception ex)
				{
					if (ex is IOException || ex is ArgumentException || ex is SecurityException || ex is WebException || ex is NotSupportedException || ex is UriFormatException)
					{
						flag = true;
						failures++;
						if (failures >= retry)
						{
							return null;
						}
					}
				}
				finally
				{
					if (binaryWriter != null)
					{
						binaryWriter.Close();
					}
					binaryWriter = null;
					if (binaryReader != null)
					{
						binaryReader.Close();
					}
					binaryReader = null;
					if (fileStream != null)
					{
						fileStream.Close();
					}
					fileStream = null;
					if (httpWebResponse != null)
					{
						httpWebResponse.Close();
					}
					httpWebResponse = null;
				}
			}
			return text;
		}
	}
}
