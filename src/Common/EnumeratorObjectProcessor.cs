using System;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class EnumeratorObjectProcessor : ObjectProcessor
	{
		public EnumeratorObjectProcessor(ExecutionInterface executionInterface, ObjectInstance objInstIn)
			: base(executionInterface, objInstIn)
		{
		}

		public override void ProcessObject()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			try
			{
				num = int.Parse(objInstIn.GetObjectAttribute("Key1"));
				num2 = int.Parse(objInstIn.GetObjectAttribute("Key2"));
				if (objInstIn.GetObjectAttribute("Key3").Length > 0)
				{
					num3 = int.Parse(objInstIn.GetObjectAttribute("Key3"));
				}
				if (num3 < 1)
				{
					num3 = 1;
				}
				for (int i = num; i <= num2; i += num3)
				{
					ObjectInstance objectInstance = new ObjectInstance(executionInterface, objInstIn);
					objectInstance.SetObjectAttribute("Name", i.ToString());
					foreach (object settingNode in objInstIn.SettingNodes)
					{
						objectInstance.AddSettingNode(settingNode, new object[1]
						{
							i.ToString()
						});
					}
					objInstOutList.Add(objectInstance);
				}
			}
			catch (Exception exception)
			{
				objInstOutList = null;
				executionInterface.LogException(exception);
			}
		}
	}
}
