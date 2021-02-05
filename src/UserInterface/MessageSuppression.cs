using System.Collections;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class MessageSuppression
	{
		private SortedList suppressionList;

		public MessageSuppression()
		{
			suppressionList = new SortedList();
		}

		public override string ToString()
		{
			string text = "";
			foreach (string key in suppressionList.Keys)
			{
				if (text.Length > 0)
				{
					text += ",";
				}
				text += key;
			}
			return text;
		}

		public bool IsSuppressed(string msgid)
		{
			return suppressionList.Contains(msgid);
		}

		public void Suppress(string msgid)
		{
			if (!suppressionList.Contains(msgid))
			{
				suppressionList.Add(msgid, true);
			}
		}

		public void Unsuppress(string msgid)
		{
			if (suppressionList.Contains(msgid))
			{
				suppressionList.Remove(msgid);
			}
		}
	}
}
