using System;
using System.Diagnostics;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class EventProcessor : IssueProcessor
	{
		private EventLog eventLog;

		private IntPtr eventLogHandle = IntPtr.Zero;

		private static string sourceName = "BPA";

		public EventProcessor(ExecutionInterface executionInterface)
			: base(executionInterface)
		{
			eventLog = new EventLog("Application");
			if (!EventLog.SourceExists(sourceName))
			{
				EventLog.CreateEventSource(sourceName, "Application");
				if (executionInterface.Trace)
				{
					executionInterface.LogText("New source created");
				}
			}
			eventLog.Source = sourceName;
			eventLogHandle = Advapi32.RegisterEventSource(null, sourceName);
		}

		~EventProcessor()
		{
			if (eventLogHandle != IntPtr.Zero)
			{
				Advapi32.DeregisterEventSource(eventLogHandle);
			}
		}

		public override void ProcessIssue(Node messageNode, Node ruleNode, params object[] args)
		{
			if (messageNode.HasAttribute("EventID"))
			{
				EventLogEntryType eventLogEntryType;
				switch (messageNode.GetAttribute("Error"))
				{
				case "Error":
					eventLogEntryType = EventLogEntryType.Error;
					break;
				case "Warning":
				case "BestPractice":
				case "Baseline":
					eventLogEntryType = EventLogEntryType.Warning;
					break;
				default:
					eventLogEntryType = EventLogEntryType.Information;
					break;
				}
				Advapi32.ReportEvent(eventLogHandle, (short)eventLogEntryType, 0, Convert.ToInt32(messageNode.GetAttribute("EventID")), IntPtr.Zero, 3, 0, new string[3]
				{
					messageNode.Value,
					messageNode.GetAttribute("GUID"),
					messageNode.GetAttribute("Error")
				}, IntPtr.Zero);
				GC.KeepAlive(this);
			}
		}

		[Obsolete("Use ProcessIssue(Node, Node, object[]) instead.")]
		public override void ProcessIssue(Node messageNode, Node ruleNode, params string[] args)
		{
		}
	}
}
