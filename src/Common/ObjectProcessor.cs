using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	[CLSCompliant(false)]
	public abstract class ObjectProcessor
	{
		protected ExecutionInterface executionInterface;

		protected ObjectInstance objInstIn;

		protected ObjectInstanceList objInstOutList;

		protected NodeList customNodeList;

		public ObjectInstanceList ObjInstOutList
		{
			get
			{
				return objInstOutList;
			}
		}

		public ObjectInstance ObjInstIn
		{
			get
			{
				return objInstIn;
			}
		}

		public NodeList CustomNodeList
		{
			get
			{
				return customNodeList;
			}
		}

		public ObjectProcessor(ExecutionInterface executionInterface, ObjectInstance objInstIn)
		{
			this.executionInterface = executionInterface;
			this.objInstIn = objInstIn;
			objInstOutList = new ObjectInstanceList(executionInterface, objInstIn);
			customNodeList = new NodeList();
		}

		public Node AddExceptionNode(Exception exception)
		{
			string val = string.Format("{0:HH:mm:ss.fff}", DateTime.Now);
			Node node = objInstIn.ObjectNode.OwnerDocument.CreateNode("Exception");
			int hRForException = Marshal.GetHRForException(exception);
			node.SetAttribute("Time", val);
			node.SetAttribute("Message", exception.Message);
			node.SetAttribute("HResult", hRForException.ToString(null, CultureInfo.InvariantCulture));
			customNodeList.Add(node);
			return node;
		}

		public virtual void ProcessObject()
		{
		}
	}
}
