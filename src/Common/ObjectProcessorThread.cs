using System;
using System.Threading;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	internal class ObjectProcessorThread
	{
		private ObjectProcessorThreadPool objProcThreadPool;

		private AutoResetEvent gotWork;

		private AutoResetEvent workCompleted;

		private ObjectProcessor objProcClass;

		private Thread dispatchThread;

		private ExecutionInterface executionInterface;

		public Thread DispatchThread
		{
			get
			{
				return dispatchThread;
			}
		}

		public ObjectProcessorThread(ExecutionInterface executionInterface, ObjectProcessorThreadPool objProcThreadPool)
		{
			this.executionInterface = executionInterface;
			this.objProcThreadPool = objProcThreadPool;
			gotWork = new AutoResetEvent(false);
			workCompleted = new AutoResetEvent(false);
			dispatchThread = new Thread(DoWork);
		}

		private void DoWork()
		{
			while (true)
			{
				gotWork.WaitOne();
				if (objProcClass == null)
				{
					break;
				}
				try
				{
					executionInterface.ImpersonateInstance.SetSecurityContext(objProcClass.ObjInstIn.OPD);
					objProcClass.ProcessObject();
				}
				catch (Exception ex)
				{
					executionInterface.LogException(ex.Message, ex);
				}
				workCompleted.Set();
			}
		}

		public void QueueWork(ObjectProcessor objProc)
		{
			objProcClass = objProc;
			gotWork.Set();
		}

		public bool WaitForCompletion(TimeSpan timeout)
		{
			return workCompleted.WaitOne(timeout, true);
		}

		private void DoAbort()
		{
			dispatchThread.Abort();
		}

		public void Abort()
		{
			Thread thread = new Thread(DoAbort);
			thread.Start();
		}
	}
}
