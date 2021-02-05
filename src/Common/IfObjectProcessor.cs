using System.Xml;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class IfObjectProcessor : ObjectProcessor
	{
		public IfObjectProcessor(ExecutionInterface executionInterface, ObjectInstance objInstIn)
			: base(executionInterface, objInstIn)
		{
		}

		public override void ProcessObject()
		{
			object obj = ExtFunction.Evaluate((XmlNode)objInstIn.ObjectNode.UnderlyingNode, objInstIn.GetObjectAttribute("Key1"));
			if (!ExtFunction.Boolify(obj))
			{
				return;
			}
			ObjectInstance objectInstance = new ObjectInstance(executionInterface, objInstIn);
			foreach (Node settingNode in objInstIn.SettingNodes)
			{
				object obj2 = ExtFunction.Evaluate((XmlNode)settingNode.UnderlyingNode, objInstIn.GetSettingAttribute(settingNode, "Key1"));
				objectInstance.AddSettingNode(settingNode, new object[1]
				{
					ExtFunction.Stringify(obj2)
				});
			}
			objInstOutList.Add(objectInstance);
		}
	}
}
