using System;
using System.Collections;
using System.Threading;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class Collect
	{
		private ExecutionInterface execInterface;

		private ObjectProcessorThreadPool objProcThreadPool;

		public Collect(ExecutionInterface execInterface)
		{
			this.execInterface = execInterface;
			objProcThreadPool = new ObjectProcessorThreadPool(execInterface);
		}

		private void ProcessInstance(object instanceData)
		{
			ProcessInstanceData processInstanceData = (ProcessInstanceData)instanceData;
			try
			{
				bool flag = false;
				foreach (Node settingNode in processInstanceData.ObjInst.SettingNodes)
				{
					if (!PassedRestriction(settingNode, processInstanceData.ObjInst.OPD))
					{
						continue;
					}
					if (settingNode.GetNodes("Value") == null && Common.IsOptionSet(settingNode.GetAttribute("Retrieve"), "FailHard"))
					{
						flag = true;
						if (Common.IsOptionSet(processInstanceData.ObjInst.GetObjectAttribute("TrackProgress").ToString(), "TrackFailHard"))
						{
							processInstanceData.ObjInst.OPD.SetFailHard();
						}
						break;
					}
					lock (execInterface.Options.Data)
					{
						Node instanceAdded = processInstanceData.InstanceAdded;
						Node node2 = Common.FindSetting(instanceAdded, settingNode, processInstanceData.ObjInst.OPD);
						Node child = execInterface.Options.Data.ImportNode(settingNode, true);
						if (node2 != null)
						{
							node2.Delete();
						}
						instanceAdded.Add(child);
					}
				}
				if (flag)
				{
					processInstanceData.ObjInst.OPD.TrackProgressAdjust(processInstanceData.ChildObject, 0);
				}
				else
				{
					ProcessObjects(processInstanceData.ChildObject, processInstanceData.InstanceAdded, processInstanceData.ObjInst.OPD);
				}
			}
			finally
			{
				processInstanceData.Complete();
			}
		}

		public void ProcessObjects()
		{
			if (!execInterface.Aborting)
			{
				ObjectParentData opd = execInterface.Options.RootOPD();
				ProcessingInfo processingInfo = new ProcessingInfo(null, opd);
				processingInfo.TrackProgressStart();
				Document configuration = execInterface.Options.Configuration;
				Document data = execInterface.Options.Data;
				ProcessObjects(configuration.GetNode("/*"), data.GetNode("/*"), opd);
				processingInfo.TrackProgressComplete();
				objProcThreadPool.ExitWorkerThreads();
			}
		}

		private void ProcessObjects(Node cfgParent, Node dataParent, ObjectParentData opd)
		{
			if (execInterface.Aborting)
			{
				return;
			}
			ProcessingInfo processingInfo = null;
			Node[] array = null;
			lock (execInterface.Options.Data)
			{
				array = cfgParent.GetNodes("Object");
				processingInfo = ProcessingInfo.Parse(cfgParent, dataParent, opd);
			}
			Node[] array2 = array;
			foreach (Node node in array2)
			{
				ObjectInstanceList objectInstanceList = null;
				ObjectInstance objectInstance = null;
				lock (execInterface.Options.Data)
				{
					ObjectParentData objectParentData = opd.Clone();
					Node node2 = PrepareObject(node, objectParentData);
					if (ProcessThisObject(node2, objectParentData))
					{
						objectInstance = new ObjectInstance(execInterface, null);
						objectInstance.ObjectNode = node2;
						objectInstance.SettingNodes = GetSettings(node, objectParentData);
						objectInstance.OPD = objectParentData;
					}
				}
				TimeSpan timeToProcess = TimeSpan.Zero;
				ObjectProcessor objectProcessor = null;
				bool timedout = false;
				if (objectInstance != null)
				{
					string key = objectInstance.GetObjectAttribute("Type").ToString();
					string objectAttribute = objectInstance.GetObjectAttribute("LogText");
					if (objectAttribute.Length > 0)
					{
						execInterface.LogText(objectAttribute);
					}
					try
					{
						DateTime now = DateTime.Now;
						if (!execInterface.CodeLibraries.LoadedObjectProcessors.Contains(key))
						{
							throw new NotImplementedException();
						}
						LoadedProcessor loadedProcessor = (LoadedProcessor)execInterface.CodeLibraries.LoadedObjectProcessors[key];
						object[] parms = new object[2]
						{
							execInterface,
							objectInstance
						};
						objectProcessor = (ObjectProcessor)loadedProcessor.CreateInstance(parms);
						ObjectProcessorThread objectProcessorThread = objProcThreadPool.Dispatch(objectProcessor);
						bool delete = false;
						while (!objectProcessorThread.WaitForCompletion(TimeSpan.FromSeconds(execInterface.Options.Timeout)))
						{
							if (objectInstance.CheckTimeout() || execInterface.Aborting)
							{
								if (!execInterface.Aborting)
								{
									execInterface.LogText(CommonLoc.Info_ObjectTimeout(DisplayNode(objectInstance.ObjectNode)));
									timedout = true;
								}
								objectProcessorThread.Abort();
								delete = true;
								if (!objectProcessorThread.DispatchThread.Join(TimeSpan.FromSeconds(60.0)))
								{
									execInterface.LogText(CommonLoc.Error_ObjectTimeout(DisplayNode(objectInstance.ObjectNode)));
								}
								break;
							}
						}
						objProcThreadPool.Done(objectProcessorThread, delete);
						objectInstanceList = objectProcessor.ObjInstOutList;
						timeToProcess = DateTime.Now.Subtract(now);
					}
					catch (Exception ex)
					{
						execInterface.LogException(CommonLoc.Error_DispatchingObject(ex.Message, objectInstance.GetObjectAttribute("Type")), ex);
						objectInstanceList = null;
					}
					objectInstance.OPD.AddProcessingValues(objectInstance.GetObjectAttribute("Type").ToString(), objectInstanceList == null, timedout, timeToProcess);
				}
				opd.TrackProgressUpdate(node);
				if (objectInstance == null || objectInstanceList == null)
				{
					opd.TrackProgressAdjust(node, 0);
				}
				else if (objectInstanceList.Count != 1)
				{
					opd.TrackProgressAdjust(node, objectInstanceList.Count);
				}
				WaitHandle[] array3 = null;
				ProcessInstanceData[] array4 = null;
				Node node3 = null;
				SortedList matchedStrings = new SortedList();
				lock (execInterface.Options.Data)
				{
					node3 = Common.AddOrMatchObject(dataParent, node);
					node3.DeleteAttribute("ProcessTime");
					node3.SetAttribute("ProcessTime", timeToProcess.TotalMilliseconds.ToString());
					if (objectProcessor != null)
					{
						foreach (Node customNode in objectProcessor.CustomNodeList)
						{
							Node child = node3.OwnerDocument.ImportNode(customNode, true);
							node3.Add(child);
						}
					}
					if (objectInstanceList == null || objectInstanceList.Count == 0)
					{
						if (objectInstance != null && Common.IsOptionSet(objectInstance.GetObjectAttribute("TrackProgress").ToString(), "TrackFailHard"))
						{
							foreach (Node settingNode in objectInstance.SettingNodes)
							{
								if (Common.IsOptionSet(settingNode.GetAttribute("Retrieve"), "FailHard"))
								{
									opd.SetFailHard();
									break;
								}
							}
						}
					}
					else
					{
						string attribute = node.GetAttribute("Async");
						int num = 1;
						if (attribute.Length > 0)
						{
							num = ((attribute[0] != '%') ? int.Parse(attribute) : ((int)(float.Parse(attribute.Substring(1)) / 100f * (float)execInterface.Options.MaxThreads)));
						}
						if (num == 0 || num > objectInstanceList.Count)
						{
							num = objectInstanceList.Count;
						}
						if (num > 64)
						{
							num = 64;
						}
						if (num > 1)
						{
							array3 = new WaitHandle[num];
							for (int j = 0; j < num; j++)
							{
								array3[j] = new AutoResetEvent(true);
							}
						}
						array4 = new ProcessInstanceData[objectInstanceList.Count];
						for (int k = 0; k < objectInstanceList.Count; k++)
						{
							ObjectInstance objectInstance2 = objectInstanceList[k];
							objectInstance2.OPD.SetFilterStatus(objectInstance2.ObjectNode);
							Node node5 = null;
							if (PassedRestriction(objectInstance2.ObjectNode, objectInstance2.OPD))
							{
								node5 = Common.AddOrMatchInstance(node3, objectInstance2.ObjectNode, matchedStrings);
							}
							if (node5 == null)
							{
								array4[k] = null;
								opd.TrackProgressAdjust(node, 0);
							}
							else
							{
								array4[k] = new ProcessInstanceData(node, node5, objectInstance2);
							}
						}
					}
				}
				if (array4 == null)
				{
					continue;
				}
				ProcessInstanceData[] array5 = array4;
				foreach (ProcessInstanceData processInstanceData in array5)
				{
					if (processInstanceData != null)
					{
						if (array3 == null)
						{
							ProcessInstance(processInstanceData);
							continue;
						}
						int num2 = WaitHandle.WaitAny(array3);
						processInstanceData.CompletedEvent = (AutoResetEvent)array3[num2];
						ThreadPool.QueueUserWorkItem(ProcessInstance, processInstanceData);
					}
				}
				if (array3 != null)
				{
					WaitHandle.WaitAll(array3);
				}
			}
			if (processingInfo != null)
			{
				processingInfo.TrackProgressComplete();
			}
		}

		private static string DisplayNode(Node node)
		{
			string text = "";
			if (node != null)
			{
				int num = 0;
				for (Node parent = node.Parent; parent != null; parent = parent.Parent)
				{
					num++;
				}
				for (int i = 0; i < num; i++)
				{
					text += "  ";
				}
				text = text + "<" + node.Name;
				string[] attributes = node.Attributes;
				foreach (string text2 in attributes)
				{
					string text3 = text;
					text = text3 + " " + text2 + "=\"" + node.GetAttribute(text2) + "\"";
				}
				text += ((node.Children.Length == 0) ? "/>" : ">");
			}
			return text;
		}

		private ArrayList GetSettings(Node parent, ObjectParentData opd)
		{
			Node[] nodes = parent.GetNodes("Setting");
			ArrayList arrayList = new ArrayList();
			Node[] array = nodes;
			foreach (Node node in array)
			{
				Node node2 = PrepareSetting(node, opd);
				if (node2 != null)
				{
					arrayList.Add(node2);
				}
				string attribute = node.GetAttribute("Substitution");
				if (attribute.Length > 0)
				{
					opd.AddSubstitutionString(attribute, null);
				}
			}
			return arrayList;
		}

		private Node PrepareObject(Node obj, ObjectParentData opd)
		{
			string attribute = obj.GetAttribute("SecurityContext");
			if (obj.HasAttribute("SecurityContext"))
			{
				opd.SetInheritedProperty("SecurityContext", attribute);
			}
			Node node = execInterface.Options.Data.ImportNode(obj, false);
			Node[] nodes = obj.GetNodes("*[not(Setting | Rule | Object)]");
			Node[] array = nodes;
			foreach (Node srcNode in array)
			{
				Node node2 = execInterface.Options.Data.ImportNode(srcNode, true);
				ApplySubstitutionsRecursively(opd, node2);
				node.Add(node2);
			}
			return opd.ApplySubstitutions(node);
		}

		private void ApplySubstitutionsRecursively(ObjectParentData opd, Node node)
		{
			opd.ApplySubstitutions(node);
			Node[] nodes = node.GetNodes("*");
			Node[] array = nodes;
			foreach (Node node2 in array)
			{
				ApplySubstitutionsRecursively(opd, node2);
			}
		}

		private Node PrepareSetting(Node setting, ObjectParentData opd)
		{
			Node node = execInterface.Options.Data.ImportNode(setting, true);
			return opd.ApplySubstitutions(node);
		}

		private static bool ProcessThisObject(Node obj, ObjectParentData opd)
		{
			bool result = true;
			if (obj == null || opd.OverThresholds(obj.GetAttribute("Type")))
			{
				result = false;
			}
			return result;
		}

		private static bool PassedRestriction(Node node, ObjectParentData opd)
		{
			string attribute = node.GetAttribute("Restrict");
			if (attribute == "No" || (attribute.Length == 0 && opd.PassedFilter))
			{
				return true;
			}
			return false;
		}
	}
}
