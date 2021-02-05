using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class CodeLibraries
	{
		private enum LibraryType
		{
			ObjectProcessor,
			IssueProcessor,
			ConfigPreprocessor,
			ExtFormat,
			ExtFunction,
			DataPostprocessor
		}

		private static LoadedProcessor.Callback[] callbacks = new LoadedProcessor.Callback[6]
		{
			null,
			null,
			null,
			ExtFormat.Add,
			ExtFunction.Add,
			null
		};

		private Hashtable libraries = new Hashtable();

		private ExecutionInterface executionInterface;

		public SortedList LoadedExtFormats
		{
			get
			{
				return (SortedList)libraries[LibraryType.ExtFormat];
			}
		}

		public SortedList LoadedExtFunctions
		{
			get
			{
				return (SortedList)libraries[LibraryType.ExtFunction];
			}
		}

		public SortedList LoadedObjectProcessors
		{
			get
			{
				return (SortedList)libraries[LibraryType.ObjectProcessor];
			}
		}

		public SortedList LoadedIssueProcessors
		{
			get
			{
				return (SortedList)libraries[LibraryType.IssueProcessor];
			}
		}

		public SortedList LoadedConfigPreprocessors
		{
			get
			{
				return (SortedList)libraries[LibraryType.ConfigPreprocessor];
			}
		}

		public SortedList LoadedDataPostprocessors
		{
			get
			{
				return (SortedList)libraries[LibraryType.DataPostprocessor];
			}
		}

		public CodeLibraries(ExecutionInterface executionInterface)
		{
			this.executionInterface = executionInterface;
			foreach (LibraryType value in Enum.GetValues(typeof(LibraryType)))
			{
				libraries[value] = new SortedList();
			}
		}

		public void Load(bool preprocessors)
		{
			string currentDirectory = Directory.GetCurrentDirectory();
			Directory.SetCurrentDirectory(executionInterface.ExecutionDirectory);
			Document configuration = executionInterface.Options.Configuration;
			ExtFunction.Reset();
			ExtFormat.Reset();
			foreach (LibraryType value in Enum.GetValues(typeof(LibraryType)))
			{
				if ((preprocessors && value == LibraryType.ConfigPreprocessor) || (!preprocessors && value != LibraryType.ConfigPreprocessor))
				{
					Node[] nodes = configuration.GetNodes("/*/Configuration/" + value);
					LoadedProcessor.Callback callback = callbacks[(int)value];
					LoadedProcessor.LoadProcessors((SortedList)libraries[value], nodes, executionInterface, callback);
				}
			}
			Directory.SetCurrentDirectory(currentDirectory);
		}

		public void CleanupProcessors()
		{
			foreach (LibraryType value in Enum.GetValues(typeof(LibraryType)))
			{
				foreach (LoadedProcessor value2 in ((SortedList)libraries[value]).Values)
				{
					try
					{
						if (value2.Loaded)
						{
							MethodInfo method = value2.ProcessorType.GetMethod("Cleanup", BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, new Type[0], null);
							if (method != null)
							{
								method.Invoke(null, null);
							}
						}
					}
					catch (Exception exception)
					{
						executionInterface.LogException(exception);
					}
				}
			}
		}
	}
}
