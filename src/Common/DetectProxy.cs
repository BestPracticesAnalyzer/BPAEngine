using System;
using System.Net;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class DetectProxy
	{
		public static bool CheckProxyServer(string urlTarget, ref WebProxy proxy)
		{
			bool result = true;
			HttpWebRequest httpWebRequest = null;
			HttpWebResponse httpWebResponse = null;
			try
			{
				httpWebRequest = (HttpWebRequest)WebRequest.Create(urlTarget);
				if (proxy != null)
				{
					httpWebRequest.Proxy = proxy;
				}
				else
				{
					proxy = (WebProxy)httpWebRequest.Proxy;
				}
				httpWebRequest.Timeout = 30000;
				httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				return result;
			}
			catch
			{
				return false;
			}
			finally
			{
				if (httpWebResponse != null)
				{
					httpWebResponse.Close();
				}
			}
		}

		public static string GetProxyList(string agent, string urlTarget)
		{
			string text = null;
			WinHttp.WINHTTP_AUTOPROXY_OPTIONS pAutoProxyOptions = default(WinHttp.WINHTTP_AUTOPROXY_OPTIONS);
			WinHttp.WINHTTP_PROXY_INFO pProxyInfo = default(WinHttp.WINHTTP_PROXY_INFO);
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				WinHttp.WINHTTP_PROXY_INFO pProxyInfo2 = default(WinHttp.WINHTTP_PROXY_INFO);
				WinHttp.WINHTTP_CURRENT_USER_IE_PROXY_CONFIG pProxyConfig = default(WinHttp.WINHTTP_CURRENT_USER_IE_PROXY_CONFIG);
				intPtr = WinHttp.WinHttpOpen(agent, 1, null, null, 0);
				if (intPtr != IntPtr.Zero)
				{
					pAutoProxyOptions.dwFlags = 1;
					pAutoProxyOptions.dwAutoDetectFlags = 3;
					pAutoProxyOptions.fAutoLogonIfChallenged = true;
					pAutoProxyOptions.lpszAutoConfigUrl = null;
					if (WinHttp.WinHttpGetProxyForUrl(intPtr, urlTarget, ref pAutoProxyOptions, ref pProxyInfo))
					{
						text = pProxyInfo.lpszProxy;
						return text;
					}
					if (WinHttp.WinHttpGetIEProxyConfigForCurrentUser(ref pProxyConfig))
					{
						if (pProxyConfig.lpszAutoConfigUrl != null && pProxyConfig.lpszAutoConfigUrl.Length > 0)
						{
							pAutoProxyOptions.dwFlags = 2;
							pAutoProxyOptions.dwAutoDetectFlags = 0;
							pAutoProxyOptions.lpszAutoConfigUrl = pProxyConfig.lpszAutoConfigUrl;
							if (WinHttp.WinHttpGetProxyForUrl(intPtr, urlTarget, ref pAutoProxyOptions, ref pProxyInfo2))
							{
								text = pProxyInfo2.lpszProxy;
							}
						}
						if (text == null)
						{
							if (pProxyConfig.lpszProxy != null)
							{
								if (pProxyConfig.lpszProxy.Length > 0)
								{
									text = pProxyConfig.lpszProxy;
									return text;
								}
								return text;
							}
							return text;
						}
						return text;
					}
					return text;
				}
				return text;
			}
			catch
			{
				return text;
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					WinHttp.WinHttpCloseHandle(intPtr);
				}
			}
		}

		public static WebProxy GetProxy(ExecutionInterface execInterface, string agent, string urlTarget)
		{
			WebProxy proxy = null;
			bool flag = false;
			char[] separator = new char[2]
			{
				';',
				' '
			};
			string text = null;
			try
			{
				if (!CheckProxyServer(urlTarget, ref proxy))
				{
					proxy = new WebProxy();
					string proxyList = GetProxyList(agent, urlTarget);
					if (proxyList != null)
					{
						string[] array = proxyList.Split(separator);
						string[] array2 = array;
						foreach (string text2 in array2)
						{
							text = ((text2 == null || !text2.StartsWith("http://")) ? ("http://" + text2) : text2);
							Uri uri2 = (proxy.Address = new Uri(text));
							proxy.Credentials = CredentialCache.DefaultCredentials;
							if (CheckProxyServer(urlTarget, ref proxy))
							{
								flag = true;
								break;
							}
						}
					}
				}
				else
				{
					flag = true;
				}
			}
			catch (Exception exception)
			{
				execInterface.LogException(exception);
			}
			if (!flag)
			{
				return null;
			}
			return proxy;
		}
	}
}
