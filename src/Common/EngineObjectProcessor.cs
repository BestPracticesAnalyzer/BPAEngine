using System;
using System.Globalization;
using System.IO;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class EngineObjectProcessor : ObjectProcessor
	{
		public EngineObjectProcessor(ExecutionInterface executionInterface, ObjectInstance objInstIn)
			: base(executionInterface, objInstIn)
		{
		}

		public override void ProcessObject()
		{
			ObjectInstance objectInstance = new ObjectInstance(executionInterface, objInstIn);
			foreach (object settingNode in objInstIn.SettingNodes)
			{
				string settingAttribute = objInstIn.GetSettingAttribute(settingNode, "Key1");
				string settingAttribute2 = objInstIn.GetSettingAttribute(settingNode, "Key2");
				object[] array = null;
				string text = null;
				FileInfo fileInfo = null;
				switch (settingAttribute)
				{
				case "AppVersion":
					text = executionInterface.EngineVersion.ToString();
					break;
				case "ConfigVersion":
					text = executionInterface.Options.Configuration.ConfigurationNode.GetAttribute("ConfigVersion");
					break;
				case "DataFileName":
					fileInfo = new FileInfo(executionInterface.Options.Data.FileName);
					text = fileInfo.FullName;
					break;
				case "ExecutionDirectory":
					text = executionInterface.ExecutionDirectory;
					break;
				case "ApplicationName":
					text = executionInterface.ApplicationName;
					break;
				case "StartTime":
					text = executionInterface.ExecTime.ToString(CultureInfo.InvariantCulture);
					break;
				case "ExecutionCulture":
					text = executionInterface.Culture.ToString();
					break;
				case "EnvironmentVariable":
					text = Environment.GetEnvironmentVariable(settingAttribute2);
					break;
				default:
					if (executionInterface.Trace)
					{
						executionInterface.LogText("The value for Key1 ({0}) is invalid", settingAttribute);
					}
					break;
				}
				if (text != null)
				{
					array = new object[1]
					{
						text
					};
					objectInstance.AddSettingNode(settingNode, array);
				}
			}
			objInstOutList.Add(objectInstance);
		}
	}
}
