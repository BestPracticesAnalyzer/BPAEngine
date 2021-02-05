using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class LoadedProcessor
	{
		public delegate void Callback(string name, Node node, LoadedProcessor loadedProcessor);

		private string name;

		private Node node;

		private Type processorType;

		private static Hashtable assemblies = new Hashtable();

		private static ExecutionInterface executionInterface = null;

		private Hashtable processorOptions;

		private bool loaded;

		private string assemblyName = "";

		private string className = "";

		private bool checkAssemblySignature;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public Type ProcessorType
		{
			get
			{
				return processorType;
			}
		}

		public Hashtable ProcessorOptions
		{
			get
			{
				return processorOptions;
			}
		}

		public Node Node
		{
			get
			{
				return node;
			}
		}

		public bool Loaded
		{
			get
			{
				return loaded;
			}
		}

		[Obsolete("This constructor is deprecated.  Use LoadedProcessor(string,Node,Hashtable,bool) instead.")]
		private LoadedProcessor(string name, Node node, Hashtable processorOptions)
			: this(name, node, processorOptions, true)
		{
		}

		private LoadedProcessor(string name, Node node, Hashtable processorOptions, bool checkAssemblySignature)
		{
			this.name = name;
			this.node = node;
			this.checkAssemblySignature = checkAssemblySignature;
			if (processorOptions != null)
			{
				this.processorOptions = processorOptions;
			}
			else
			{
				this.processorOptions = new Hashtable();
			}
			assemblyName = node.GetAttribute("Assembly");
			className = node.GetAttribute("Class");
			string[] attributes = node.Attributes;
			foreach (string text in attributes)
			{
				if (!this.processorOptions.Contains(text))
				{
					this.processorOptions[text] = node.GetAttribute(text);
				}
			}
		}

		public static void LoadProcessors(SortedList list, Node[] nodes, ExecutionInterface executionInterface, Callback callback)
		{
			LoadedProcessor.executionInterface = executionInterface;
			foreach (Node node in nodes)
			{
				string text = string.Empty;
				try
				{
					text = node.GetAttribute("ObjectType");
					if (text.Length == 0)
					{
						text = node.GetAttribute("Name");
					}
					Hashtable hashtable = (Hashtable)executionInterface.Options.ProcessorOptions[text];
					LoadedProcessor loadedProcessor = new LoadedProcessor(text, node, hashtable, executionInterface.Options.CheckAssemblySignature);
					if (list.Contains(loadedProcessor.Name))
					{
						list.Remove(loadedProcessor.Name);
					}
					list.Add(loadedProcessor.Name, loadedProcessor);
					if (executionInterface.Trace)
					{
						executionInterface.LogText("Created entry for processor {0}", text);
					}
					if (callback != null)
					{
						loadedProcessor.LoadClass();
						callback(text, node, loadedProcessor);
					}
				}
				catch (Exception ex)
				{
					executionInterface.LogText(CommonLoc.Error_LoadingAssembly(ex.Message, node.Name, text, node.GetAttribute("Assembly"), node.GetAttribute("Class")));
				}
			}
		}

		public object CreateInstance(object[] parms)
		{
			if (!loaded)
			{
				loaded = true;
				try
				{
					LoadClass();
					MethodInfo method = ProcessorType.GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, new Type[0], null);
					if (method != null)
					{
						method.Invoke(null, null);
					}
				}
				catch (Exception ex)
				{
					executionInterface.LogText(CommonLoc.Error_LoadingAssembly(ex.Message, node.Name, name, node.GetAttribute("Assembly"), node.GetAttribute("Class")));
				}
			}
			if (processorType == null)
			{
				throw new NotImplementedException();
			}
			return Activator.CreateInstance(processorType, parms);
		}

		private void LoadClass()
		{
			if (!assemblies.Contains(this.assemblyName))
			{
				AssemblyName assemblyName = new AssemblyName();
				assemblyName.Name = Regex.Replace(this.assemblyName, ".dll", "");
				if (checkAssemblySignature)
				{
					assemblyName.SetPublicKeyToken(GetType().Assembly.GetName().GetPublicKeyToken());
				}
				assemblies[this.assemblyName] = Assembly.Load(assemblyName);
			}
			processorType = ((Assembly)assemblies[this.assemblyName]).GetType(className, true);
			if (executionInterface.Trace)
			{
				executionInterface.LogText("Loaded assemblyName {0}, className {1}", ProcessorType.Assembly.FullName, ProcessorType.FullName);
			}
		}
	}
}
