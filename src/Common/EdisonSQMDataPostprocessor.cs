using System;
using Microsoft.WinSE.DiagnosticTools.MicrosoftCommonDiagnostics;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class EdisonSQMDataPostprocessor : DataPostprocessor
	{
		private IntPtr pBpaHandle = IntPtr.Zero;

		public EdisonSQMDataPostprocessor(ExecutionInterface execInterface, Document data)
			: base(execInterface, data)
		{
		}

		public override void ProcessData()
		{
			string attribute = executionInterface.Options.Configuration.ConfigurationNode.GetAttribute("SQMSet");
			uint ruleSetId = 0u;
			if (attribute.Length > 0)
			{
				ruleSetId = uint.Parse(attribute);
			}
			string attribute2 = executionInterface.Options.Configuration.ConfigurationNode.GetAttribute("SQMVersion");
			uint ruleSetVer = 0u;
			if (attribute2.Length > 0)
			{
				ruleSetVer = uint.Parse(attribute2);
			}
			executionInterface.LogTrace("SQM session initializing...");
			if (!EdisonSQMLibWrap.InitializeMcdSQM())
			{
				executionInterface.LogTrace("SQM object initialization failed.");
				return;
			}
			if (!EdisonSQMLibWrap.CreateBpaSession(ref pBpaHandle))
			{
				executionInterface.LogTrace("SQM session creation failed.");
				return;
			}
			if (!EdisonSQMLibWrap.InitializeBpaSession(pBpaHandle))
			{
				executionInterface.LogTrace("SQM session initialization failed.");
				return;
			}
			if (!EdisonSQMLibWrap.StartBpaSession(pBpaHandle))
			{
				executionInterface.LogTrace("SQM session start failed.");
				return;
			}
			executionInterface.LogTrace("SQM data point processing...");
			DateTime now = DateTime.Now;
			bool ruleSetResult = true;
			Node[] nodes = data.GetNodes("//Rule[@SQM]");
			Node[] array = nodes;
			foreach (Node node in array)
			{
				try
				{
					uint num = uint.Parse(node.GetAttribute("SQM"));
					bool flag = node.GetAttribute("Pass") == "True";
					executionInterface.LogTrace("SQM data point {0} set to {1}.", num, flag);
					EdisonSQMLibWrap.SetRuleStatus(pBpaHandle, num, flag);
				}
				catch
				{
					executionInterface.LogTrace("SQM data point set failure occured.");
					ruleSetResult = false;
				}
			}
			executionInterface.LogTrace("SQM session completion.");
			EdisonSQMLibWrap.SetRuleSetStatus(pBpaHandle, ruleSetId, ruleSetVer, ruleSetResult, (uint)DateTime.Now.Subtract(now).TotalMilliseconds);
			EdisonSQMLibWrap.EndBpaSession(pBpaHandle);
			EdisonSQMLibWrap.FreeBpaSession(ref pBpaHandle);
		}
	}
}
