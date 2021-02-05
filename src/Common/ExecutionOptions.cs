using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	[CLSCompliant(false)]
	public class ExecutionOptions
	{
		[ComImport]
		[Guid("CB2F6723-AB3A-11D2-9C40-00C04FA30A3E")]
		public class CorRuntimeHost
		{
		}

		[Guid("84680D3A-B2C1-46e8-ACC2-DBC0A359159A")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface ICLRThreadPool
		{
			void RegisterWaitForSingleObject();

			void UnregisterWait();

			void QueueUserWorkItem();

			void CreateTimer();

			void ChangeTimer();

			void DeleteTimer();

			void BindIoCompletionCallback();

			void CallOrQueueUserWorkItem();

			void CorSetMaxThreads(uint MaxWorkerThreads, uint MaxIOCompletionThreads);

			void CorGetMaxThreads(out uint MaxWorkerThreads, out uint MaxIOCompletionThreads);

			void CorGetAvailableThreads(out uint AvailableWorkerThreads, out uint AvailableIOCompletionThreads);
		}

		public const OperationsFlags DefaultOperations = OperationsFlags.Collect | OperationsFlags.Analyze;

		private const string InvalidFileChars = "<>|\"\\/?*:";

		private ExecutionInterface execInterface;

		private Document config;

		private Document data;

		private int saveInterval;

		private int timeOut = 300;

		private bool trace;

		private bool debug;

		private bool loadDataOnRun;

		private OperationsFlags operations = OperationsFlags.Collect | OperationsFlags.Analyze;

		private SortedList credentialsList = new SortedList();

		private ProgressCallback progress;

		private CompletedCallback completed;

		private LogCallback log;

		private string label = string.Empty;

		private string dc = string.Empty;

		private ICLRThreadPool objCLRInternalThreadPool;

		private SortedList substitutions;

		private Restrictions restrictions;

		private Hashtable processorOptions = new Hashtable();

		private bool reloadConfiguration = true;

		private bool startStopEvents = true;

		private bool stripLogTimestamps;

		private bool enableLogFileCreation = true;

		private bool checkAssemblySignature = true;

		private bool checkConfigurationSignature = true;

		private string configurationPublicKey = "<RSAKeyValue><Modulus>zzj34aQnXvjwzKrO+zreO2IxRgVGu/W2BRrTs6zCnw9MZwgoxEMQ9Tt1eX9qkfTWM8hhv/qRkAB68HkdXWhw9pCymHe1Ax0vm52bdYkvSgFBfJ58y4dDm/SWdJmemMHPQFdYFvbA1ZIW5SSFcY+ZSe1VfGXJHzgAI8U+qxHWKWzGnqBwW33VN9RndyDDBs6F+E40gKA1xBxTMgFX77EovWwB461AvICpCUnbNuM39B1Jqiqna9AZ08yOndaGRnoTStZFGaVTs+J4Ly41l2tMxugasNPRJJBpq878aW4+TPsCQWLcB5hdflynTCcxa1ZM4ZjY4NEdcY09KsB/cU3/zw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

		public Document Configuration
		{
			get
			{
				return config;
			}
			set
			{
				config = value;
			}
		}

		public Document Data
		{
			get
			{
				return data;
			}
			set
			{
				data = value;
			}
		}

		public bool LoadDataOnRun
		{
			get
			{
				return loadDataOnRun;
			}
			set
			{
				loadDataOnRun = value;
			}
		}

		public int SaveInterval
		{
			get
			{
				return saveInterval;
			}
			set
			{
				saveInterval = value;
			}
		}

		public uint MaxThreads
		{
			get
			{
				int workerThreads = 0;
				int completionPortThreads = 0;
				ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
				return (uint)workerThreads;
			}
			set
			{
				uint num = value;
				if (num < 10)
				{
					num = 10u;
				}
				int workerThreads = 0;
				int completionPortThreads = 0;
				ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
				if (num != (uint)workerThreads)
				{
					objCLRInternalThreadPool.CorSetMaxThreads(num, (uint)completionPortThreads);
				}
			}
		}

		public bool ReloadConfiguration
		{
			get
			{
				return reloadConfiguration;
			}
			set
			{
				reloadConfiguration = value;
			}
		}

		public bool StartStopEvents
		{
			get
			{
				return startStopEvents;
			}
			set
			{
				startStopEvents = value;
			}
		}

		public int Timeout
		{
			get
			{
				return timeOut;
			}
			set
			{
				timeOut = value;
			}
		}

		public bool Trace
		{
			get
			{
				return trace;
			}
			set
			{
				trace = value;
			}
		}

		public bool Debug
		{
			get
			{
				return debug;
			}
			set
			{
				debug = value;
			}
		}

		public OperationsFlags Operations
		{
			get
			{
				return operations;
			}
			set
			{
				operations = value;
			}
		}

		public SortedList CredentialsList
		{
			get
			{
				return credentialsList;
			}
		}

		public ProgressCallback Progress
		{
			get
			{
				return progress;
			}
			set
			{
				progress = value;
			}
		}

		public CompletedCallback Completed
		{
			get
			{
				return completed;
			}
			set
			{
				completed = value;
			}
		}

		public LogCallback Log
		{
			get
			{
				return log;
			}
			set
			{
				log = value;
			}
		}

		public string Label
		{
			get
			{
				return label;
			}
			set
			{
				label = value;
				char[] array = "<>|\"\\/?*:".ToCharArray();
				foreach (char oldChar in array)
				{
					label = label.Replace(oldChar, '_');
				}
			}
		}

		public string DC
		{
			get
			{
				return dc;
			}
			set
			{
				dc = value;
			}
		}

		public SortedList Substitutions
		{
			get
			{
				return substitutions;
			}
			set
			{
				substitutions = value;
			}
		}

		public Restrictions Restrictions
		{
			get
			{
				return restrictions;
			}
		}

		public Hashtable ProcessorOptions
		{
			get
			{
				return processorOptions;
			}
		}

		public bool StripLogTimestamps
		{
			get
			{
				return stripLogTimestamps;
			}
			set
			{
				stripLogTimestamps = value;
			}
		}

		public bool EnableLogFileCreation
		{
			get
			{
				return enableLogFileCreation;
			}
			set
			{
				enableLogFileCreation = false;
			}
		}

		public bool CheckAssemblySignature
		{
			get
			{
				return checkAssemblySignature;
			}
			set
			{
				checkAssemblySignature = value;
			}
		}

		public bool CheckConfigurationSignature
		{
			get
			{
				return checkConfigurationSignature;
			}
			set
			{
				checkConfigurationSignature = value;
			}
		}

		public string ConfigurationPublicKey
		{
			get
			{
				return configurationPublicKey;
			}
			set
			{
				configurationPublicKey = value;
			}
		}

		internal ExecutionOptions(ExecutionInterface execInterface)
		{
			objCLRInternalThreadPool = (ICLRThreadPool)new CorRuntimeHost();
			this.execInterface = execInterface;
			config = new BPADocument(execInterface);
			data = new BPADocument(execInterface);
			restrictions = new Restrictions(execInterface);
			substitutions = new SortedList();
		}

		public void AddCredentials(string contextName, string domainName, string userName, string password)
		{
			if (!credentialsList.Contains(contextName))
			{
				Credentials value = new Credentials(contextName, userName, domainName, password);
				credentialsList.Add(contextName, value);
			}
		}

		public bool IsOperationSet(OperationsFlags op)
		{
			return (operations & op) != 0;
		}

		internal ObjectParentData RootOPD()
		{
			ObjectParentData objectParentData = new ObjectParentData();
			foreach (RestrictionOption value2 in restrictions.Options.Values)
			{
				string value = value2.Value;
				if (value != null)
				{
					objectParentData.AddSubstitutionString(value2.Name, value2.Value);
					if (execInterface.Trace)
					{
						execInterface.LogText("Global substitution {0}={1}", value2.Name, value2.Value);
					}
				}
			}
			Node[] nodes = config.GetNodes("/*/Configuration/Substitution");
			Node[] array = nodes;
			foreach (Node node in array)
			{
				string attribute = node.GetAttribute("Name");
				objectParentData.AddSubstitutionString(attribute, node.Value);
				if (trace)
				{
					execInterface.LogText("Default Global substitution {0}={1}", attribute, node.Value);
				}
			}
			foreach (string key in substitutions.Keys)
			{
				objectParentData.AddSubstitutionString(key, (string)substitutions[key]);
			}
			return objectParentData;
		}

		public void SetProcessorOptions(ArrayList optionStrings)
		{
			processorOptions = new Hashtable();
			if (optionStrings == null)
			{
				return;
			}
			foreach (string optionString in optionStrings)
			{
				string[] array = optionString.Split(new char[1]
				{
					':'
				}, 2);
				Hashtable hashtable = new Hashtable();
				processorOptions[array[0]] = hashtable;
				if (array.Length != 2)
				{
					continue;
				}
				string[] array2 = array[1].Split(',');
				string[] array3 = array2;
				foreach (string text2 in array3)
				{
					array = text2.Split(new char[1]
					{
						'='
					}, 2);
					if (array.Length == 2)
					{
						hashtable[array[0]] = array[1];
					}
					else
					{
						hashtable[array[0]] = "";
					}
				}
			}
		}
	}
}
