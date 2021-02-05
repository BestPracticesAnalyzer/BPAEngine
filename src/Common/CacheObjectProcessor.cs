using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class CacheObjectProcessor : ObjectProcessor
	{
		private static Hashtable data = new Hashtable();

		public CacheObjectProcessor(ExecutionInterface executionInterface, ObjectInstance objInstIn)
			: base(executionInterface, objInstIn)
		{
		}

		public override void ProcessObject()
		{
			string text = objInstIn.GetObjectAttribute("Key1").ToString();
			string key = objInstIn.GetObjectAttribute("Key2").ToString();
			string text2 = objInstIn.GetObjectAttribute("Key3").ToString();
			string text3 = objInstIn.GetObjectAttribute("Key4").ToString();
			string key2 = objInstIn.GetObjectAttribute("Name").ToString();
			Hashtable hashtable;
			lock (data)
			{
				if (data.ContainsKey(key2))
				{
					hashtable = (Hashtable)data[key2];
				}
				else
				{
					hashtable = new Hashtable();
					data[key2] = hashtable;
				}
			}
			SortedList sortedList2;
			lock (hashtable)
			{
				sortedList2 = (SortedList)(hashtable.ContainsKey(key) ? ((SortedList)hashtable[key]) : (hashtable[key] = new SortedList()));
			}
			lock (sortedList2)
			{
				switch (text)
				{
				case "Add":
				{
					if (text3.Length == 0)
					{
						if (!sortedList2.ContainsKey(text2))
						{
							sortedList2.Add(text2, 1);
							AddInstance(text2);
						}
						break;
					}
					Regex regex = new Regex(text3);
					string[] array = regex.Split(text2);
					string[] array2 = array;
					foreach (string text5 in array2)
					{
						if (text5.Length > 0 && !sortedList2.ContainsKey(text5))
						{
							sortedList2.Add(text5, 1);
							AddInstance(text5);
						}
					}
					break;
				}
				case "AddValue":
					if (!sortedList2.ContainsKey(text2))
					{
						sortedList2.Add(text2, 1);
					}
					break;
				case "Delete":
					if (sortedList2.ContainsKey(text2))
					{
						sortedList2.Remove(text2);
					}
					else if (text2.Length == 0)
					{
						lock (hashtable)
						{
							hashtable.Remove(key);
						}
					}
					AddInstance(text2);
					break;
				case "Dump":
					foreach (string key3 in sortedList2.Keys)
					{
						AddInstance(key3);
					}
					break;
				case "DumpValue":
				{
					string text4 = "";
					foreach (string key4 in sortedList2.Keys)
					{
						text4 = text4 + key4 + text3;
					}
					if (text4.EndsWith(text3))
					{
						text4 = text4.Substring(0, text4.Length - text3.Length);
					}
					AddInstance(text4);
					break;
				}
				}
			}
		}

		private void AddInstance(string value)
		{
			ObjectInstance objectInstance = new ObjectInstance(executionInterface, objInstIn);
			objectInstance.SetObjectAttribute("Name", value);
			object[] propVals = new object[1]
			{
				value
			};
			foreach (object settingNode in objInstIn.SettingNodes)
			{
				objectInstance.AddSettingNode(settingNode, propVals);
			}
			objInstOutList.Add(objectInstance);
		}

		public static void Initialize()
		{
			data = new Hashtable();
		}

		public static string ToLower(object val)
		{
			return val.ToString().ToLower(CultureInfo.InvariantCulture);
		}
	}
}
