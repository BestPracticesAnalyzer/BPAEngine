using System.Net;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class ResolveObjectProcessor : ObjectProcessor
	{
		public ResolveObjectProcessor(ExecutionInterface executionInterface, ObjectInstance objInstIn)
			: base(executionInterface, objInstIn)
		{
		}

		public override void ProcessObject()
		{
			string[] array = objInstIn.GetObjectAttribute("Key1").Split(',');
			string text = string.Empty;
			string[] array2 = array;
			foreach (string hostNameOrAddress in array2)
			{
				try
				{
					IPHostEntry hostEntry = Dns.GetHostEntry(hostNameOrAddress);
					text = hostEntry.HostName;
				}
				catch
				{
					continue;
				}
				break;
			}
			ObjectInstance objectInstance = new ObjectInstance(executionInterface, objInstIn);
			objectInstance.SetObjectAttribute("Name", text);
			object[] propVals = new object[1]
			{
				text
			};
			foreach (object settingNode in objInstIn.SettingNodes)
			{
				objectInstance.AddSettingNode(settingNode, propVals);
			}
			objInstOutList.Add(objectInstance);
		}
	}
}
