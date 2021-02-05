namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class GroupObjectProcessor : ObjectProcessor
	{
		public GroupObjectProcessor(ExecutionInterface executionInterface, ObjectInstance objInstIn)
			: base(executionInterface, objInstIn)
		{
		}

		public override void ProcessObject()
		{
			ObjectInstance objectInstance = new ObjectInstance(executionInterface, objInstIn);
			foreach (object settingNode in objInstIn.SettingNodes)
			{
				objectInstance.AddSettingNode(settingNode, new object[1]
				{
					string.Empty
				});
			}
			objInstOutList.Add(objectInstance);
		}
	}
}
