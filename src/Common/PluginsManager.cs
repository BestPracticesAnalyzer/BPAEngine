#define TRACE
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Policy;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class PluginsManager<PluginType>
	{
		private Collection<PluginType> plugins = new Collection<PluginType>();

		public ReadOnlyCollection<PluginType> Plugins
		{
			get
			{
				return new ReadOnlyCollection<PluginType>(plugins);
			}
		}

		public ReadOnlyCollection<string> PluginNames
		{
			get
			{
				Collection<string> collection = new Collection<string>();
				foreach (PluginType plugin in plugins)
				{
					collection.Add(plugin.ToString());
				}
				return new ReadOnlyCollection<string>(collection);
			}
		}

		public PluginsManager(string[] args)
		{
			AppDomain currentDomain = AppDomain.CurrentDomain;
			Evidence evidence = AppDomain.CurrentDomain.Evidence;
			currentDomain.SetupInformation.PrivateBinPath = "plugins";
			string path = Path.Combine(currentDomain.BaseDirectory, "plugins");
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			FileInfo[] files = directoryInfo.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
			ArrayList arrayList = new ArrayList();
			FileInfo[] array = files;
			foreach (FileInfo fileInfo in array)
			{
				try
				{
					Assembly assembly = Assembly.LoadFrom(fileInfo.FullName);
					Type[] types = assembly.GetTypes();
					Type[] array2 = types;
					foreach (Type type in array2)
					{
						try
						{
							if (type.BaseType.Equals(typeof(PluginType)))
							{
								arrayList.Add(type);
							}
						}
						catch (NullReferenceException ex)
						{
							Console.WriteLine(ex.Message);
						}
					}
				}
				catch (BadImageFormatException ex2)
				{
					Trace.TraceWarning("Failed to load dll {0} due to error {1}", fileInfo.FullName, ex2.ToString());
				}
			}
			ArrayList arrayList2 = new ArrayList
			{
				args
			};
			foreach (Type item in arrayList)
			{
				plugins.Add((PluginType)Activator.CreateInstance(item, arrayList2.ToArray()));
			}
		}

		public PluginType Find(string pluginName)
		{
			PluginType result = default(PluginType);
			foreach (PluginType plugin in plugins)
			{
				if (string.Compare(pluginName, plugin.ToString(), true, CultureInfo.CurrentCulture) == 0)
				{
					return plugin;
				}
			}
			return result;
		}
	}
}
