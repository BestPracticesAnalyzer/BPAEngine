using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class AppCmdLineArguments
	{
		private StringDictionary Params;

		private Regex Splitter = new Regex("^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private Regex TrimQuotes = new Regex("^['\"]?(.*?)['\"]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public string this[string parameter]
		{
			get
			{
				return Params[parameter];
			}
		}

		public AppCmdLineArguments(string[] args)
		{
			Params = new StringDictionary();
			string text = null;
			foreach (string input in args)
			{
				string[] array = Splitter.Split(input, 3);
				switch (array.Length)
				{
				case 3:
					if (text != null && !Params.ContainsKey(text))
					{
						Params.Add(text, "true");
					}
					text = array[1];
					if (!Params.ContainsKey(text))
					{
						Trim(ref array[2]);
						Params.Add(text, array[2]);
					}
					text = null;
					break;
				case 2:
					if (text != null && !Params.ContainsKey(text))
					{
						Params.Add(text, "true");
					}
					text = array[1];
					break;
				case 1:
					if (text != null && !Params.ContainsKey(text))
					{
						Trim(ref array[0]);
						Params.Add(text, array[0]);
					}
					break;
				}
			}
			if (text != null && !Params.ContainsKey(text))
			{
				Params.Add(text, "true");
			}
		}

		private void Trim(ref string Value)
		{
			Value = TrimQuotes.Replace(Value, "$1");
		}

		public void UpdateParams(object App)
		{
			PropertyInfo[] properties = App.GetType().GetProperties();
			for (int i = 0; i < properties.Length; i++)
			{
				object[] customAttributes = properties[i].GetCustomAttributes(false);
				for (int j = 0; j < customAttributes.Length; j++)
				{
					Attribute attribute = (Attribute)customAttributes[j];
					if (!(attribute is AppCmdLineArgumentAttribute))
					{
						continue;
					}
					AppCmdLineArgumentAttribute appCmdLineArgumentAttribute = (AppCmdLineArgumentAttribute)attribute;
					if (Params[appCmdLineArgumentAttribute.Name] != null)
					{
						object[] array = new object[1];
						if (properties[i].PropertyType == typeof(string))
						{
							array[0] = Params[appCmdLineArgumentAttribute.Name];
						}
						else if (properties[i].PropertyType == typeof(int))
						{
							array[0] = int.Parse(Params[appCmdLineArgumentAttribute.Name]);
						}
						else if (properties[i].PropertyType == typeof(bool))
						{
							bool result;
							if (!bool.TryParse(Params[appCmdLineArgumentAttribute.Name], out result))
							{
								throw new ArgumentException(string.Format(Strings.BooleanArgumentError, appCmdLineArgumentAttribute.Name));
							}
							array[0] = result;
						}
						properties[i].GetSetMethod().Invoke(App, array);
					}
					else if (appCmdLineArgumentAttribute.Required)
					{
						throw new ArgumentException(string.Format(Strings.ArgumentRequiredError, appCmdLineArgumentAttribute.Name));
					}
				}
			}
		}
	}
}
