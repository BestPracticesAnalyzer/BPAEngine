using System.Collections;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	internal class ObjectProcessorThreadPool
	{
		private ArrayList idleObjectProcessorThreads;

		private ArrayList busyObjectProcessorThreads;

		private object poolLock;

		private ExecutionInterface executionInterface;

		public ObjectProcessorThreadPool(ExecutionInterface executionInterface)
		{
			this.executionInterface = executionInterface;
			idleObjectProcessorThreads = new ArrayList();
			busyObjectProcessorThreads = new ArrayList();
			poolLock = new object();
		}

		public ObjectProcessorThread Dispatch(ObjectProcessor objProcClass)
		{
			ObjectProcessorThread objectProcessorThread = null;
			lock (poolLock)
			{
				if (idleObjectProcessorThreads.Count > 0)
				{
					objectProcessorThread = (ObjectProcessorThread)idleObjectProcessorThreads[0];
					idleObjectProcessorThreads.RemoveAt(0);
				}
				else
				{
					objectProcessorThread = new ObjectProcessorThread(executionInterface, this);
					objectProcessorThread.DispatchThread.Start();
				}
				objectProcessorThread.QueueWork(objProcClass);
				busyObjectProcessorThreads.Add(objectProcessorThread);
				return objectProcessorThread;
			}
		}

		public void Done(ObjectProcessorThread objProcThread, bool delete)
		{
			lock (poolLock)
			{
				busyObjectProcessorThreads.Remove(objProcThread);
				if (!delete)
				{
					idleObjectProcessorThreads.Add(objProcThread);
				}
			}
		}

		public void ExitWorkerThreads()
		{
			foreach (ObjectProcessorThread idleObjectProcessorThread in idleObjectProcessorThreads)
			{
				idleObjectProcessorThread.QueueWork(null);
			}
			idleObjectProcessorThreads.Clear();
			foreach (ObjectProcessorThread busyObjectProcessorThread in busyObjectProcessorThreads)
			{
				busyObjectProcessorThread.Abort();
			}
			busyObjectProcessorThreads.Clear();
		}
	}
}
