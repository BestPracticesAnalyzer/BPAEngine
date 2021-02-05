using System;
using System.Reflection;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class GenericCommand<ReceiverType> : ICommand
	{
		protected ReceiverType Receiver;

		private MethodInfo mAction;

		private PropertyInfo pAction;

		protected object response;

		public GenericCommand(ReceiverType receivervar, string action)
		{
			Receiver = receivervar;
			mAction = typeof(ReceiverType).GetMethod(action);
			if (mAction == null)
			{
				pAction = typeof(ReceiverType).GetProperty(action);
			}
			if (pAction == null && mAction == null)
			{
				throw new ArgumentNullException("Action");
			}
		}

		public virtual void execute()
		{
			if (mAction != null)
			{
				response = mAction.Invoke(Receiver, null);
			}
			else if (pAction != null)
			{
				response = pAction.GetValue(Receiver, null);
			}
		}
	}
}
