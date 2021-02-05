using System;
using System.Collections;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	internal class ProcessingInfo
	{
		private static int objectsTracked;

		private TimeSpan maxTotalTime = TimeSpan.Zero;

		private SortedList maxTimePerType = new SortedList();

		private int maxTotalFailures;

		private SortedList maxFailuresPerType = new SortedList();

		private int maxTotalTimeouts;

		private SortedList maxTimeoutsPerType = new SortedList();

		private TimeSpan curTotalTime = TimeSpan.Zero;

		private SortedList curTimePerType = new SortedList();

		private int curTotalFailures;

		private SortedList curFailuresPerType = new SortedList();

		private int curTotalTimeouts;

		private SortedList curTimeoutsPerType = new SortedList();

		private static ExecutionInterface executionInterface;

		private object thresholdLock = new object();

		private string name = "";

		private bool loggedMessage;

		private float totalObjectsToProcess;

		private object totalObjectsToProcessLock = new object();

		private float totalObjectsProcessed;

		private object totalObjectsProcessedLock = new object();

		private bool trackProgress;

		private bool anyOverThreshold;

		private bool failHard;

		private bool globalProgress;

		public static ExecutionInterface ExecutionInterface
		{
			get
			{
				return executionInterface;
			}
			set
			{
				executionInterface = value;
			}
		}

		public bool TrackProgress
		{
			get
			{
				return trackProgress;
			}
		}

		public static int ObjectsTracked
		{
			get
			{
				return objectsTracked;
			}
			set
			{
				objectsTracked = value;
			}
		}

		public ProcessingInfo(string name, ObjectParentData opd)
		{
			if (name == null)
			{
				this.name = "Full Run";
				globalProgress = true;
			}
			else
			{
				this.name = name;
				globalProgress = false;
			}
			maxTimePerType = new SortedList();
			maxFailuresPerType = new SortedList();
			maxTimeoutsPerType = new SortedList();
			curTimePerType = new SortedList();
			curFailuresPerType = new SortedList();
			curTimeoutsPerType = new SortedList();
			opd.AddProcessingInfo(this);
		}

		public void AddValues(string objType, bool failed, bool timedout, TimeSpan timeToProcess)
		{
			lock (thresholdLock)
			{
				curTotalTime = curTotalTime.Add(timeToProcess);
				if (curTimePerType.Contains(objType))
				{
					((TimeSpan)curTimePerType[objType]).Add(timeToProcess);
				}
				else
				{
					curTimePerType.Add(objType, timeToProcess);
				}
				if (failed && !timedout)
				{
					curTotalFailures++;
					if (curFailuresPerType.Contains(objType))
					{
						curFailuresPerType[objType] = (int)curFailuresPerType[objType] + 1;
					}
					else
					{
						curFailuresPerType.Add(objType, 1);
					}
				}
				if (timedout)
				{
					curTotalTimeouts++;
					if (curTimeoutsPerType.Contains(objType))
					{
						curTimeoutsPerType[objType] = (int)curTimeoutsPerType[objType] + 1;
					}
					else
					{
						curTimeoutsPerType.Add(objType, 1);
					}
				}
			}
		}

		public bool OverThreshold(string objType)
		{
			bool flag = false;
			string arg = "";
			lock (thresholdLock)
			{
				if (maxTotalTime.Ticks != 0 && maxTotalTime.CompareTo(curTotalTime) <= 0)
				{
					flag = true;
					arg = "Max Total Time";
				}
				else if (maxTimePerType.Contains(objType) && curTimePerType.Contains(objType) && ((TimeSpan)maxTimePerType[objType]).Ticks != 0 && ((TimeSpan)maxTimePerType[objType]).CompareTo((TimeSpan)curTimePerType[objType]) <= 0)
				{
					flag = true;
					arg = string.Format("Max Time for {0} objects", objType);
				}
				else if (maxTotalFailures != 0 && maxTotalFailures <= curTotalFailures)
				{
					flag = true;
					arg = "Max Total Failures";
				}
				else if (maxFailuresPerType.Contains(objType) && curFailuresPerType.Contains(objType) && (int)maxFailuresPerType[objType] != 0 && (int)maxFailuresPerType[objType] <= (int)curFailuresPerType[objType])
				{
					flag = true;
					arg = string.Format("Max Failures for {0} objects", objType);
				}
				else if (maxTotalTimeouts != 0 && maxTotalTimeouts <= curTotalTimeouts)
				{
					flag = true;
					arg = "Max Total Timeouts";
				}
				else if (maxTimeoutsPerType.Contains(objType) && curTimeoutsPerType.Contains(objType) && (int)maxTimeoutsPerType[objType] != 0 && (int)maxTimeoutsPerType[objType] <= (int)curTimeoutsPerType[objType])
				{
					flag = true;
					arg = string.Format("Max Timeouts for {0} objects", objType);
				}
			}
			if (flag)
			{
				anyOverThreshold = true;
				if (!loggedMessage)
				{
					executionInterface.LogText(CommonLoc.Error_ThresholdExceeded(arg, name));
					loggedMessage = true;
				}
			}
			return flag;
		}

		public void SetFailHard()
		{
			failHard = true;
		}

		public void SetMaxFailures(string objType, int maxFailures)
		{
			if (objType.Length == 0 || objType.CompareTo("ALL") == 0)
			{
				maxTotalFailures = maxFailures;
			}
			else if (!maxFailuresPerType.Contains(objType))
			{
				maxFailuresPerType.Add(objType, maxFailures);
			}
		}

		public void SetMaxTime(string objType, TimeSpan maxTime)
		{
			if (objType.Length == 0 || objType.CompareTo("ALL") == 0)
			{
				maxTotalTime = maxTime;
			}
			else if (!maxTimePerType.Contains(objType))
			{
				maxTimePerType.Add(objType, maxTime);
			}
		}

		public void SetMaxTimeouts(string objType, int maxTimeouts)
		{
			if (objType.Length == 0 || objType.CompareTo("ALL") == 0)
			{
				maxTotalTimeouts = maxTimeouts;
			}
			else if (!maxTimeoutsPerType.Contains(objType))
			{
				maxTimeoutsPerType.Add(objType, maxTimeouts);
			}
		}

		public void TrackProgressAdjust(Node cfgParent, int instanceCount)
		{
			if (trackProgress)
			{
				lock (totalObjectsToProcessLock)
				{
					totalObjectsToProcess += CountNodeTimes(cfgParent) * (instanceCount - 1);
				}
				if (executionInterface.Trace)
				{
					executionInterface.LogText("Tracking adjusted: {0} out of {1} objects for {2}", totalObjectsProcessed, totalObjectsToProcess, name);
				}
				ReportProgress(ObjectProgress.ObjectStatus.InProgress);
			}
		}

		public void TrackProgressComplete()
		{
			if (trackProgress)
			{
				ObjectProgress.ObjectStatus status = ObjectProgress.ObjectStatus.CompletedOkay;
				if (failHard)
				{
					status = ObjectProgress.ObjectStatus.CompletedWithError;
				}
				else if (anyOverThreshold)
				{
					status = ObjectProgress.ObjectStatus.CompletedWithWarning;
				}
				if (executionInterface.Trace)
				{
					executionInterface.LogText("Tracking completed: {0} out of {1} objects for {2}", totalObjectsProcessed, totalObjectsToProcess, name);
				}
				ReportProgress(status);
				trackProgress = false;
			}
		}

		public void TrackProgressStart()
		{
			trackProgress = true;
			totalObjectsProcessed = 0f;
			totalObjectsToProcess = 0f;
			Node[] nodes = executionInterface.Options.Configuration.GetNodes("*/Object");
			Node[] array = nodes;
			foreach (Node cfgParent in array)
			{
				totalObjectsToProcess += CountNodeTimes(cfgParent);
			}
			if (executionInterface.Trace)
			{
				executionInterface.LogText("Tracking started: {0} out of {1} objects for {2}", totalObjectsProcessed, totalObjectsToProcess, name);
			}
			ReportProgress(ObjectProgress.ObjectStatus.InProgress);
		}

		public void TrackProgressStart(Node cfgParent)
		{
			if (!trackProgress)
			{
				trackProgress = true;
				totalObjectsProcessed = CountSingleObjectNode(cfgParent);
				totalObjectsToProcess = totalObjectsProcessed + (float)CountNodeTimes(cfgParent);
				if (executionInterface.Trace)
				{
					executionInterface.LogText("Tracking started: {0} out of {1} objects for {2}", totalObjectsProcessed, totalObjectsToProcess, name);
				}
				ReportProgress(ObjectProgress.ObjectStatus.InProgress);
			}
		}

		public void TrackProgressUpdate(Node processedObject)
		{
			if (trackProgress)
			{
				lock (totalObjectsProcessedLock)
				{
					totalObjectsProcessed += CountSingleObjectNode(processedObject);
				}
				if (executionInterface.Trace)
				{
					executionInterface.LogText("Tracking updated: {0} out of {1} objects for {2}", totalObjectsProcessed, totalObjectsToProcess, name);
				}
				ReportProgress(ObjectProgress.ObjectStatus.InProgress);
			}
		}

		private void ReportProgress(ObjectProgress.ObjectStatus status)
		{
			ObjectProgress objectProgress = new ObjectProgress();
			objectProgress.GlobalProgress = globalProgress;
			objectProgress.Status = status;
			if (totalObjectsToProcess == 0f)
			{
				objectProgress.PercentageDone = 100;
			}
			else
			{
				objectProgress.PercentageDone = (int)(totalObjectsProcessed / totalObjectsToProcess * 100f);
				if (objectProgress.PercentageDone > 100)
				{
					objectProgress.PercentageDone = 100;
				}
			}
			if (executionInterface.Options.Progress != null)
			{
				executionInterface.Options.Progress(name, objectProgress);
			}
		}

		private int CountNodeTimes(Node cfgParent)
		{
			int num = 0;
			Node[] nodes = cfgParent.GetNodes(".//Object");
			Node[] array = nodes;
			foreach (Node obj in array)
			{
				num += CountSingleObjectNode(obj);
			}
			return num;
		}

		internal static int CountSingleObjectNode(Node obj)
		{
			return GetTimeout(obj) / ExecutionInterface.Options.Timeout + 1;
		}

		internal static int GetTimeout(Node objectNode)
		{
			int num = ExecutionInterface.Options.Timeout;
			string attribute = objectNode.GetAttribute("Timeout");
			if (attribute.Length > 0)
			{
				num = ((attribute[0] != '%') ? int.Parse(attribute) : ((int)(float.Parse(attribute.Substring(1)) / 100f * (float)num)));
			}
			return num;
		}

		private static bool ValidateProcessingInfoAttribute(string[] parms)
		{
			bool result = true;
			if (parms.Length % 2 != 0)
			{
				result = false;
			}
			else
			{
				foreach (string text in parms)
				{
					if (text.Length == 0)
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}

		public static ProcessingInfo Parse(Node cfgParent, Node dataParent, ObjectParentData opd)
		{
			ProcessingInfo processingInfo = null;
			string attribute = cfgParent.GetAttribute("MaxTime");
			string attribute2 = cfgParent.GetAttribute("MaxFailures");
			string attribute3 = cfgParent.GetAttribute("MaxTimeouts");
			string attribute4 = cfgParent.GetAttribute("TrackProgress");
			if (attribute.Length > 0 || attribute2.Length > 0 || attribute3.Length > 0 || attribute4 == "TrackProgress")
			{
				processingInfo = new ProcessingInfo(dataParent.GetAttribute("Name"), opd);
			}
			char[] separator = new char[2]
			{
				',',
				';'
			};
			if (attribute.Length > 0)
			{
				string[] array = attribute.Split(separator);
				if (!ValidateProcessingInfoAttribute(array))
				{
					executionInterface.LogText(CommonLoc.Error_BadFormat("MaxTime", attribute));
				}
				else
				{
					for (int i = 0; i < array.Length; i += 2)
					{
						processingInfo.SetMaxTime(array[i], TimeSpan.FromMinutes(double.Parse(array[i + 1])));
					}
				}
			}
			if (attribute2.Length > 0)
			{
				string[] array2 = attribute2.Split(separator);
				if (!ValidateProcessingInfoAttribute(array2))
				{
					executionInterface.LogText(CommonLoc.Error_BadFormat("MaxFailures", attribute2));
				}
				else
				{
					for (int j = 0; j < array2.Length; j += 2)
					{
						processingInfo.SetMaxFailures(array2[j], int.Parse(array2[j + 1]));
					}
				}
			}
			if (attribute3.Length > 0)
			{
				string[] array3 = attribute3.Split(separator);
				if (!ValidateProcessingInfoAttribute(array3))
				{
					executionInterface.LogText(CommonLoc.Error_BadFormat("MaxTimeouts", attribute3));
				}
				else
				{
					for (int k = 0; k < array3.Length; k += 2)
					{
						processingInfo.SetMaxTimeouts(array3[k], int.Parse(array3[k + 1]));
					}
				}
			}
			if (attribute4 == "TrackProgress")
			{
				opd.TrackProgressStart(cfgParent);
				objectsTracked++;
			}
			return processingInfo;
		}
	}
}
