using System.Threading;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	internal class ProcessInstanceData
	{
		private Node childObject;

		private Node instanceAdded;

		private ObjectInstance objInst;

		private AutoResetEvent completedEvent;

		public Node ChildObject
		{
			get
			{
				return childObject;
			}
		}

		public Node InstanceAdded
		{
			get
			{
				return instanceAdded;
			}
		}

		public ObjectInstance ObjInst
		{
			get
			{
				return objInst;
			}
		}

		public AutoResetEvent CompletedEvent
		{
			set
			{
				completedEvent = value;
			}
		}

		public bool Async
		{
			get
			{
				return completedEvent != null;
			}
		}

		public ProcessInstanceData(Node childObject, Node instanceAdded, ObjectInstance objInst)
		{
			this.childObject = childObject;
			this.instanceAdded = instanceAdded;
			this.objInst = objInst;
		}

		public void Complete()
		{
			if (completedEvent != null)
			{
				completedEvent.Set();
			}
		}
	}
}
