using System;
using System.Threading;
using System.Xml.XPath;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public abstract class Document : IXPathNavigable
	{
		protected ExecutionInterface execInterface;

		protected object doc;

		protected Node configNode;

		protected Node runNode;

		protected string fileName = "";

		private AutoResetEvent doneSignal;

		private object currentlySaving = new object();

		private Thread saveThread;

		private int saveInterval;

		public virtual object UnderlyingDocument
		{
			get
			{
				return doc;
			}
		}

		public string FileName
		{
			get
			{
				return fileName;
			}
			set
			{
				fileName = value;
			}
		}

		public string Name
		{
			get
			{
				return fileName.Substring(fileName.LastIndexOf("\\") + 1);
			}
		}

		public virtual Node ConfigurationNode
		{
			get
			{
				return configNode;
			}
			set
			{
				configNode = value;
			}
		}

		public virtual Node RunNode
		{
			get
			{
				return runNode;
			}
			set
			{
				runNode = value;
			}
		}

		public virtual int SaveInterval
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

		internal Document(ExecutionInterface execInterface, object doc)
		{
			this.execInterface = execInterface;
			this.doc = doc;
		}

		public virtual Node[] GetNodes(string query)
		{
			return null;
		}

		public virtual Node GetNode(string query)
		{
			return null;
		}

		public virtual Node CreateNode(string name)
		{
			return null;
		}

		public virtual Node ImportNode(Node srcNode, bool deep)
		{
			return null;
		}

		public virtual void ClearDocument()
		{
		}

		public virtual void Load()
		{
		}

		public virtual void Save()
		{
		}

		public virtual bool IsEmpty()
		{
			return true;
		}

		public void StartPeriodicSave(int saveInterval)
		{
			this.saveInterval = saveInterval;
			doneSignal = new AutoResetEvent(false);
			saveThread = new Thread(SavePeriodically);
			saveThread.Start();
		}

		public void StopPeriodicSave()
		{
			if (saveThread != null)
			{
				doneSignal.Set();
				Monitor.Enter(currentlySaving);
				Monitor.Exit(currentlySaving);
				saveThread = null;
			}
		}

		internal virtual void Log(string time, string text)
		{
		}

		private void SavePeriodically()
		{
			bool flag = false;
			Monitor.Enter(currentlySaving);
			while (!flag)
			{
				try
				{
					flag = doneSignal.WaitOne(TimeSpan.FromSeconds(saveInterval), false);
					if (!flag)
					{
						Save();
					}
				}
				catch (Exception ex)
				{
					if (execInterface != null)
					{
						execInterface.LogException(CommonLoc.Error_XMLWrite(ex.Message), ex);
					}
				}
			}
			Monitor.Exit(currentlySaving);
		}

		public virtual XPathNavigator CreateNavigator()
		{
			return null;
		}
	}
}
