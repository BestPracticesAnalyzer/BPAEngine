using System;
using System.IO;
using System.Net;
using SqmInteropServices;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class SQMDataPostprocessor : DataPostprocessor
	{
		private uint sqmSessionId;

		private string sqmFilePattern = "";

		private string sqmUploadDirectory = "";

		private string sqmUploadPattern = "";

		private string sqmUploadUrl = "";

		private static bool sqmEnabled = true;

		public SQMDataPostprocessor(ExecutionInterface execInterface, Document data)
			: base(execInterface, data)
		{
		}

		public override void ProcessData()
		{
			executionInterface.LogTrace("SQM Datapostprocessor starting");
			executionInterface.LogTrace("SQM registry setting: {0}", sqmEnabled);
			bool flag = sqmEnabled;
			foreach (LoadedProcessor value in executionInterface.CodeLibraries.LoadedDataPostprocessors.Values)
			{
				if (value.Name == "0 SQM Data Generation" && value.ProcessorOptions != null)
				{
					flag = value.ProcessorOptions.Contains("Enable");
					break;
				}
			}
			if (!flag)
			{
				executionInterface.LogTrace("SQM reporting disabled - exiting");
				return;
			}
			executionInterface.LogTrace("Setting global SQM values");
			SetGlobalSQMData();
			executionInterface.LogTrace("Looking for SQM objects");
			Node[] nodes = data.GetNodes("//Object[@SQM]");
			Node[] array = nodes;
			foreach (Node node in array)
			{
				if (!(node.GetAttribute("SQM") == "PerInstance"))
				{
					continue;
				}
				executionInterface.LogTrace("Processing SQM object {0}", node.GetAttribute("Name"));
				Node[] nodes2 = node.GetNodes("Instance");
				Node[] array2 = nodes2;
				foreach (Node node2 in array2)
				{
					StartSQMSession();
					Node[] nodes3 = node2.GetNodes(".//Setting[@SQM]");
					Node[] array3 = nodes3;
					foreach (Node sqmNode in array3)
					{
						ProcessSQMDatapoint(sqmNode, "Value");
					}
					Node[] nodes4 = node2.GetNodes(".//Rule[@SQM]");
					Node[] array4 = nodes4;
					foreach (Node sqmNode2 in array4)
					{
						ProcessSQMDatapoint(sqmNode2, "Result");
					}
					EndSQMSession();
				}
			}
			executionInterface.LogTrace("Uploading SQM data files");
			UploadSQMData();
		}

		public static void Enable(bool enable)
		{
			sqmEnabled = enable;
		}

		private void SetGlobalSQMData()
		{
			sqmFilePattern = executionInterface.GetDefaultOutputFileName("%02d.sqm");
			sqmUploadPattern = "*.sqm";
			sqmUploadDirectory = Directory.GetParent(sqmFilePattern).FullName;
			sqmUploadUrl = executionInterface.Options.Configuration.ConfigurationNode.GetAttribute("SQMUploadURL");
		}

		private void StartSQMSession()
		{
			sqmSessionId = SqmLibWrap.SqmGetSession(null, 60000u, 1u);
			Guid pGuid;
			if (!SqmLibWrap.SqmReadSharedMachineId(out pGuid))
			{
				SqmLibWrap.SqmCreateNewId(out pGuid);
				SqmLibWrap.SqmWriteSharedMachineId(ref pGuid);
				SqmLibWrap.SqmReadSharedMachineId(out pGuid);
			}
			SqmLibWrap.SqmSetMachineId(sqmSessionId, ref pGuid);
			Guid pGuid2;
			if (!SqmLibWrap.SqmReadSharedUserId(out pGuid2))
			{
				SqmLibWrap.SqmCreateNewId(out pGuid2);
				SqmLibWrap.SqmWriteSharedUserId(ref pGuid2);
				SqmLibWrap.SqmReadSharedUserId(out pGuid2);
			}
			SqmLibWrap.SqmSetUserId(sqmSessionId, ref pGuid2);
			SqmLibWrap.SqmSetAppVersion(sqmSessionId, (uint)executionInterface.EngineVersion.Major, (uint)executionInterface.EngineVersion.Minor);
			uint dwFlags = uint.MaxValue;
			SqmLibWrap.SqmClearFlags(sqmSessionId, ref dwFlags);
			if (sqmUploadUrl.ToLower().StartsWith("https:"))
			{
				SqmLibWrap.SqmSetFlags(sqmSessionId, 4u);
			}
			SqmLibWrap.SqmStartSession(sqmSessionId);
		}

		private void ProcessSQMDatapoint(Node sqmNode, string valueNodeName)
		{
			try
			{
				string attribute = sqmNode.GetAttribute("SQM");
				int num = attribute.IndexOf(";");
				string[] array = attribute.Substring(0, num).Split(',');
				uint[] array2 = new uint[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = uint.Parse(array[i]);
				}
				string text = attribute.Substring(num + 1);
				Node node = sqmNode.GetNode(valueNodeName);
				string text2 = "";
				if (node != null)
				{
					text2 = node.Value;
				}
				else if (sqmNode.Name == "Rule")
				{
					text2 = ((sqmNode.GetAttribute("Pass") == "True") ? "1" : "0");
				}
				switch (text)
				{
				case "Set":
					SqmLibWrap.SqmSet(sqmSessionId, array2[0], uint.Parse(text2));
					break;
				case "Increment":
					SqmLibWrap.SqmIncrement(sqmSessionId, array2[0], uint.Parse(text2));
					break;
				case "SetIfMax":
					SqmLibWrap.SqmSetIfMax(sqmSessionId, array2[0], uint.Parse(text2));
					break;
				case "SetIfMin":
					SqmLibWrap.SqmSetIfMin(sqmSessionId, array2[0], uint.Parse(text2));
					break;
				case "AddToAverage":
					SqmLibWrap.SqmAddToAverage(sqmSessionId, array2[0], uint.Parse(text2));
					break;
				case "SetGUID":
				{
					if (array2.Length != 4)
					{
						executionInterface.LogTrace("SetGUID specified with wrong number of data points provided.");
						break;
					}
					byte[] bytes = new Guid(text2).ToByteArray();
					uint dwVal = CreateUintFromBytes(bytes, 0);
					uint dwVal2 = CreateUintFromBytes(bytes, 4);
					uint dwVal3 = CreateUintFromBytes(bytes, 8);
					uint dwVal4 = CreateUintFromBytes(bytes, 12);
					SqmLibWrap.SqmSet(sqmSessionId, array2[0], dwVal);
					SqmLibWrap.SqmSet(sqmSessionId, array2[1], dwVal2);
					SqmLibWrap.SqmSet(sqmSessionId, array2[2], dwVal3);
					SqmLibWrap.SqmSet(sqmSessionId, array2[3], dwVal4);
					break;
				}
				default:
					executionInterface.LogTrace("Unknown operation specified: {0}", text);
					break;
				}
			}
			catch (Exception exception)
			{
				executionInterface.LogException(exception);
			}
		}

		private static uint CreateUintFromBytes(byte[] bytes, int offset)
		{
			return (uint)(bytes[offset] | (bytes[offset + 1] << 8) | (bytes[offset + 2] << 16) | (bytes[offset + 3] << 24));
		}

		private void EndSQMSession()
		{
			SqmLibWrap.SqmEndSession(sqmSessionId, sqmFilePattern, 10u, 2u);
		}

		private void UploadSQMData()
		{
			string[] files = Directory.GetFiles(sqmUploadDirectory, sqmUploadPattern);
			WebProxy wp = null;
			if (files.Length > 0)
			{
				wp = DetectProxy.GetProxy(executionInterface, executionInterface.ApplicationName, sqmUploadUrl);
			}
			string[] array = files;
			foreach (string file in array)
			{
				UploadSQMFile(file, wp);
			}
		}

		private void UploadSQMFile(string file, WebProxy wp)
		{
			HttpWebResponse httpWebResponse = null;
			Stream stream = null;
			Stream stream2 = null;
			try
			{
				SqmLibWrap.SqmSetCurrentTimeAsUploadTime(file);
				stream = File.OpenRead(file);
				long num = stream.Length;
				WebRequest webRequest = WebRequest.Create(sqmUploadUrl);
				webRequest.Method = "POST";
				if (wp != null)
				{
					webRequest.Proxy = wp;
				}
				webRequest.ContentLength = num;
				stream2 = webRequest.GetRequestStream();
				byte[] array = new byte[8192];
				while (num > 0)
				{
					int num2 = stream.Read(array, 0, (int)Math.Min(num, array.Length));
					stream2.Write(array, 0, num2);
					num -= num2;
				}
				stream2.Close();
				stream2 = null;
				httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
				if (httpWebResponse.StatusCode == HttpStatusCode.OK)
				{
					executionInterface.LogTrace("Uploaded file {0} successfully.", file);
				}
				else
				{
					executionInterface.LogText("Upload of file {0} failed with error {1}.", file, httpWebResponse.StatusCode);
				}
			}
			catch (Exception exception)
			{
				executionInterface.LogException(exception);
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
				if (stream2 != null)
				{
					stream2.Close();
				}
				if (httpWebResponse != null)
				{
					httpWebResponse.Close();
				}
				if (!executionInterface.Debug)
				{
					File.Delete(file);
				}
			}
		}
	}
}
