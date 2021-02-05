using System;
using System.Collections;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPAScanInfo
	{
		private ArrayList objectsToBeProcessed;

		private TimeSpan estimatedTime = TimeSpan.Zero;

		private ExecutionOptions options;

		public ArrayList ObjectsToBeProcessed
		{
			get
			{
				return objectsToBeProcessed;
			}
		}

		public TimeSpan EstimatedTime
		{
			get
			{
				return estimatedTime;
			}
			set
			{
				estimatedTime = value;
			}
		}

		public ExecutionOptions Options
		{
			get
			{
				return options;
			}
		}

		internal BPAScanInfo(ExecutionOptions options)
		{
			this.options = options;
			objectsToBeProcessed = new ArrayList();
			estimatedTime = TimeSpan.FromMinutes(5.0);
		}
	}
}
