namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public abstract class TreeNodeInfo
	{
		public string text = "";

		public virtual string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}

		public virtual TreeNodeInfo Add(string text)
		{
			return null;
		}
	}
}
