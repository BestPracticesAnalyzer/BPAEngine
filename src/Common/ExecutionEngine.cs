using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Xsl;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	internal class ExecutionEngine
	{
		public delegate void ExecuteStep();

		private ExecutionStatus status;

		private ExecutionInterface execInterface;

		private ArrayList executeSteps;

		private Rules rules;

		private Collect collect;

		public ExecutionStatus Status
		{
			get
			{
				return status;
			}
		}

		public ExecutionEngine(ExecutionInterface execInterface)
		{
			this.execInterface = execInterface;
			collect = new Collect(execInterface);
		}

		public void Start()
		{
			Document configuration = execInterface.Options.Configuration;
			try
			{
				EventLog eventLog = null;
				if (execInterface.Options.StartStopEvents)
				{
					try
					{
						eventLog = new EventLog("Application", ".", "BPA");
						eventLog.WriteEntry(CommonLoc.Info_Status(execInterface.ApplicationName, 0), EventLogEntryType.Information, 100);
					}
					catch (Exception exception)
					{
						execInterface.LogException(exception);
					}
				}
				status = ExecutionStatus.InProgress;
				Compile();
				foreach (ExecuteStep executeStep in executeSteps)
				{
					try
					{
						executeStep();
					}
					catch (Exception ex)
					{
						execInterface.LogException(CommonLoc.Error_ExecuteStep(ex.Message, executeStep.Method.ToString()), ex);
						execInterface.Abort();
					}
				}
				if (eventLog != null)
				{
					try
					{
						eventLog.WriteEntry(CommonLoc.Info_Status(execInterface.ApplicationName, 100), EventLogEntryType.Information, 101);
					}
					catch (Exception exception2)
					{
						execInterface.LogException(exception2);
					}
				}
				if (status != ExecutionStatus.Aborted)
				{
					status = ExecutionStatus.Okay;
				}
			}
			catch (Exception exception3)
			{
				status = ExecutionStatus.Failed;
				execInterface.LogException(exception3);
			}
			finally
			{
				execInterface.Options.Configuration = configuration;
				execInterface.Completed(status);
			}
		}

		public void Compile()
		{
			executeSteps = new ArrayList();
			executeSteps.Add(new ExecuteStep(LoadDataOnRun));
			executeSteps.Add(new ExecuteStep(PrepareOutput));
			executeSteps.Add(new ExecuteStep(CreateRun));
			executeSteps.Add(new ExecuteStep(LoadPreprocessorLibraries));
			executeSteps.Add(new ExecuteStep(RunPreprocessors));
			executeSteps.Add(new ExecuteStep(LoadOtherLibraries));
			executeSteps.Add(new ExecuteStep(TransformConfig));
			if (execInterface.Options.IsOperationSet(OperationsFlags.Collect) || execInterface.Options.IsOperationSet(OperationsFlags.Analyze))
			{
				executeSteps.Add(new ExecuteStep(StartPeriodicSave));
			}
			if (execInterface.Options.IsOperationSet(OperationsFlags.Collect))
			{
				executeSteps.Add(new ExecuteStep(Collect));
			}
			if (execInterface.Options.IsOperationSet(OperationsFlags.Analyze))
			{
				executeSteps.Add(new ExecuteStep(PreprocessRules));
				executeSteps.Add(new ExecuteStep(PostprocessRules));
			}
			if (execInterface.Options.IsOperationSet(OperationsFlags.Collect) || execInterface.Options.IsOperationSet(OperationsFlags.Analyze))
			{
				executeSteps.Add(new ExecuteStep(StopPeriodicSave));
			}
			executeSteps.Add(new ExecuteStep(ProcessParameters));
			executeSteps.Add(new ExecuteStep(RunPostprocessors));
			executeSteps.Add(new ExecuteStep(CleanupProcessors));
			if (execInterface.Options.IsOperationSet(OperationsFlags.Export))
			{
				executeSteps.Add(new ExecuteStep(PrepareForExport));
			}
			executeSteps.Add(new ExecuteStep(UpdateRunData));
			executeSteps.Add(new ExecuteStep(UpdateConfigData));
			if (execInterface.Options.IsOperationSet(OperationsFlags.Report))
			{
				executeSteps.Add(new ExecuteStep(GenerateReport));
			}
		}

		private void LoadDataOnRun()
		{
			if (execInterface.Options.LoadDataOnRun)
			{
				execInterface.Options.Data.Load();
			}
		}

		private void ProcessParameters()
		{
			execInterface.Parameters.ProcessParameters();
		}

		private void StartPeriodicSave()
		{
			if (execInterface.Options.SaveInterval > 0)
			{
				execInterface.Options.Data.StartPeriodicSave(execInterface.Options.SaveInterval);
			}
		}

		private void StopPeriodicSave()
		{
			execInterface.Options.Data.StopPeriodicSave();
		}

		private void Collect()
		{
			if (!execInterface.Aborting)
			{
				execInterface.LoadSecurityContexts();
				execInterface.LogText(CommonLoc.Info_StepStart(CommonLoc.Info_StepCollecting));
				collect.ProcessObjects();
				execInterface.LogText(CommonLoc.Info_StepEnd(CommonLoc.Info_StepCollecting));
			}
		}

		private void CreateRun()
		{
			Node node = execInterface.Options.Data.CreateNode("Run");
			node.SetAttribute("ConfigName", execInterface.Options.Configuration.ConfigurationNode.GetAttribute("ConfigName"));
			node.SetAttribute("ConfigVersion", execInterface.Options.Configuration.ConfigurationNode.GetAttribute("ConfigVersion"));
			node.SetAttribute("Label", execInterface.Options.Label);
			node.SetAttribute("Timeout", execInterface.Options.Timeout.ToString());
			node.SetAttribute("Threads", execInterface.Options.MaxThreads.ToString());
			string text = "";
			if (execInterface.Options.IsOperationSet(OperationsFlags.Collect))
			{
				text += "collect,";
			}
			if (execInterface.Options.IsOperationSet(OperationsFlags.Analyze))
			{
				text += "analyze,";
			}
			if (execInterface.Options.IsOperationSet(OperationsFlags.Export))
			{
				text += "export,";
			}
			if (execInterface.Options.IsOperationSet(OperationsFlags.Report))
			{
				text += "report,";
			}
			text = text.TrimEnd(',');
			node.SetAttribute("Operations", text);
			string text2 = "";
			foreach (RestrictionType value in execInterface.Options.Restrictions.Types.Values)
			{
				string text3 = execInterface.Options.Restrictions.CommaList(value.Name, false, true, true);
				if (text3.Length > 0)
				{
					text2 = text2 + "," + text3;
				}
			}
			node.SetAttribute("Restriction", text2.TrimStart(','));
			node.SetAttribute("StartTime", DateTime.Now.ToString("G", DateTimeFormatInfo.InvariantInfo));
			node.SetAttribute("AppVersion", execInterface.EngineVersion.ToString());
			node.SetAttribute("Completed", "Aborted");
			execInterface.Options.Data.ConfigurationNode.Add(node);
			execInterface.Options.Data.RunNode = node;
			UpdateConfigData();
		}

		private void LoadPreprocessorLibraries()
		{
			execInterface.CodeLibraries.Load(true);
		}

		private void LoadOtherLibraries()
		{
			execInterface.CodeLibraries.Load(false);
		}

		private void OverwriteContent(Node[] childNodes)
		{
			foreach (Node node in childNodes)
			{
				if (node.Value != null)
				{
					node.Value = Regex.Replace(node.Value, ".", "*");
				}
				OverwriteContent(node.Children);
			}
		}

		private void PostprocessRules()
		{
			if (execInterface.Aborting)
			{
				return;
			}
			execInterface.LogText(CommonLoc.Info_StepStart(CommonLoc.Info_StepPostprocessing));
			ArrayList arrayList = new ArrayList();
			execInterface.LogTrace("Rule Postprocessing: Evaluating Rules");
			Hashtable hashtable = new Hashtable();
			lock (execInterface.Options.Data)
			{
				Node[] nodes = execInterface.Options.Data.GetNodes("//Pivot[true()] | //Rule[true()]");
				Node[] array = nodes;
				foreach (Node node in array)
				{
					string attribute = node.GetAttribute("Name");
					if (!hashtable.Contains(attribute))
					{
						hashtable[attribute] = new ArrayList();
					}
					((ArrayList)hashtable[attribute]).Add(node);
				}
				foreach (PreNode rule in rules)
				{
					string attribute = rule.Name;
					execInterface.LogTrace("\tEvaluating '{0}' nodes", attribute);
					if (!hashtable.Contains(attribute) || !rule.Valid)
					{
						continue;
					}
					foreach (Node item in (ArrayList)hashtable[attribute])
					{
						if (rule is PrePivot)
						{
							PostPivot postPivot = new PostPivot((XmlElement)item.UnderlyingNode, (PrePivot)rule, execInterface);
							postPivot.EvaluateAttributes();
							continue;
						}
						PostRule postRule = new PostRule((XmlElement)item.UnderlyingNode, (PreRule)rule, execInterface);
						bool flag = false;
						try
						{
							flag = postRule.EvaluateQuery();
						}
						catch (Exception ex)
						{
							execInterface.LogException(string.Format("Error '{0}' in evaluation of rule {1}", ex.Message, attribute), ex);
							continue;
						}
						if (flag)
						{
							arrayList.Add(postRule);
						}
					}
				}
				execInterface.LogTrace("Rule Postprocessing: Creating Messages");
				foreach (PostRule item2 in arrayList)
				{
					try
					{
						item2.CreateMessage();
					}
					catch (Exception ex2)
					{
						execInterface.LogException(string.Format("Error '{0}' in creating Message for rule {1}", ex2.Message, item2.Name), ex2);
					}
				}
			}
			execInterface.LogText(CommonLoc.Info_StepEnd(CommonLoc.Info_StepPostprocessing));
			execInterface.LogTrace("Rule Postprocessing: Done");
		}

		private void PrepareForExport()
		{
			lock (execInterface.Options.Data)
			{
				Node[] nodes = execInterface.Options.Data.GetNodes("//Setting[@NotForExport='True']");
				Node[] array = nodes;
				foreach (Node node in array)
				{
					Node[] nodes2 = node.GetNodes("Value");
					Node[] array2 = nodes2;
					foreach (Node node2 in array2)
					{
						OverwriteContent(node2.Children);
					}
				}
			}
		}

		private void GenerateReport()
		{
			XsltArgumentList args = new XsltArgumentList();
			XmlDocument xmlDocument = Transforms.Apply((XmlDocument)execInterface.Options.Data.UnderlyingDocument, "Report", args);
			Hashtable hashtable = new Hashtable();
			ArrayList arrayList = new ArrayList();
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("//Message");
			foreach (XmlElement item in xmlNodeList)
			{
				string attribute = item.GetAttribute("Name");
				if (!hashtable.Contains(attribute))
				{
					hashtable[attribute] = 1;
					arrayList.Add(new string[3]
					{
						attribute,
						item.GetAttribute("Title"),
						item.GetAttribute("Error")
					});
				}
			}
			XmlNode xmlNode = xmlDocument.SelectSingleNode("/*/Configuration");
			XmlElement xmlElement2 = xmlDocument.CreateElement("Rules");
			xmlNode.AppendChild(xmlElement2);
			foreach (string[] item2 in arrayList)
			{
				XmlElement xmlElement3 = xmlDocument.CreateElement("Rule");
				xmlElement3.SetAttribute("Name", item2[0]);
				xmlElement3.SetAttribute("Title", item2[1]);
				xmlElement3.SetAttribute("Error", item2[2]);
				xmlElement2.AppendChild(xmlElement3);
			}
			XmlDocument doc = Transforms.Apply(xmlDocument, "Format", args);
			new Regex("\\.xml", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
			string defaultOutputFileName = execInterface.GetDefaultOutputFileName("report.htm");
			execInterface.LogTrace("Writing report to {0}", defaultOutputFileName);
			BPADocument.SaveXml(doc, defaultOutputFileName);
		}

		private void PrepareOutput()
		{
			ProcessingInfo.ObjectsTracked = 0;
			if (execInterface.Options.Data.ConfigurationNode == null)
			{
				Node node = execInterface.Options.Data.ImportNode(execInterface.Options.Configuration.GetNode("/*"), false);
				node.AddToRoot();
				execInterface.Options.Data.ConfigurationNode = execInterface.Options.Data.CreateNode("Configuration");
				node.Add(execInterface.Options.Data.ConfigurationNode);
			}
			else
			{
				if (!execInterface.Options.IsOperationSet(OperationsFlags.Collect) && !execInterface.Options.IsOperationSet(OperationsFlags.Analyze))
				{
					return;
				}
				string[] array = new string[3]
				{
					"//Rule",
					"//Message",
					"//Pivot"
				};
				foreach (string query in array)
				{
					Node[] nodes = execInterface.Options.Data.GetNodes(query);
					Node[] array2 = nodes;
					foreach (Node node2 in array2)
					{
						node2.Delete();
					}
				}
				string[] array3 = new string[3]
				{
					"//Object[@Error]",
					"//Instance[@Error]",
					"//Setting[@Error]"
				};
				foreach (string query2 in array3)
				{
					Node[] nodes2 = execInterface.Options.Data.GetNodes(query2);
					Node[] array4 = nodes2;
					foreach (Node node3 in array4)
					{
						node3.DeleteAttribute("Error");
					}
				}
			}
		}

		private void PreprocessRules()
		{
			if (execInterface.Aborting)
			{
				return;
			}
			execInterface.LogTrace("Rule Preprocessing: Selecting Rules");
			rules = new Rules(execInterface, ((XmlDocument)execInterface.Options.Configuration.UnderlyingDocument).SelectNodes("//Rule"), ((XmlDocument)execInterface.Options.Configuration.UnderlyingDocument).SelectNodes("//Pivot"));
			execInterface.LogTrace("Rule Preprocessing: Expanding Rules");
			foreach (PreNode rule in rules)
			{
				try
				{
					rule.ExpandAttributes();
				}
				catch (ExDiagAnalyzerException ex)
				{
					execInterface.LogException(CommonLoc.Error_RuleFormat(ex.Message), ex);
					rule.Valid = false;
				}
			}
			rules.ProcessRules();
			execInterface.LogTrace("Rule Preprocessing: Sorting Rule List");
			rules.SortList();
			execInterface.LogTrace("Rule Preprocessing: Done");
		}

		private void TransformConfig()
		{
			if (!execInterface.Aborting)
			{
				XmlDocument doc = (XmlDocument)execInterface.Options.Configuration.UnderlyingDocument;
				try
				{
					doc = Transforms.Apply(doc, "CopyTemplates", null);
					((BPADocument)execInterface.Options.Configuration).ReplaceUnderlyingDocument(doc);
					execInterface.Options.Restrictions.MarkRestrictions(execInterface.Options.Configuration);
				}
				catch (Exception exception)
				{
					execInterface.LogException(exception);
				}
			}
		}

		private void UpdateRunData()
		{
			lock (execInterface.Options.Data)
			{
				if (execInterface.Options.Data.RunNode != null)
				{
					execInterface.Options.Data.RunNode.SetAttribute("EndTime", DateTime.Now.ToString("G", DateTimeFormatInfo.InvariantInfo));
					if (execInterface.Aborting)
					{
						execInterface.Options.Data.RunNode.SetAttribute("Completed", "Aborted");
					}
					else
					{
						execInterface.Options.Data.RunNode.SetAttribute("Completed", "OK");
					}
					execInterface.Options.Data.RunNode.SetAttribute("ObjectsTracked", ProcessingInfo.ObjectsTracked.ToString());
				}
			}
		}

		private void UpdateConfigData()
		{
			lock (execInterface.Options.Data)
			{
				if (execInterface.Options.Data.ConfigurationNode == null)
				{
					return;
				}
				execInterface.Options.Data.ConfigurationNode.SetAttribute("ConfigVersion", execInterface.Options.Data.RunNode.GetAttribute("ConfigVersion"));
				if (execInterface.Options.Data.RunNode.GetAttribute("Label").Length > 0)
				{
					execInterface.Options.Data.ConfigurationNode.SetAttribute("Label", execInterface.Options.Data.RunNode.GetAttribute("Label"));
				}
				execInterface.Options.Data.ConfigurationNode.SetAttribute("Operations", execInterface.Options.Data.RunNode.GetAttribute("Operations"));
				if (execInterface.Options.IsOperationSet(OperationsFlags.Collect))
				{
					string[] array = new string[3]
					{
						"StartTime",
						"ObjectsTracked",
						"Completed"
					};
					foreach (string attrName in array)
					{
						execInterface.Options.Data.ConfigurationNode.SetAttribute(attrName, execInterface.Options.Data.RunNode.GetAttribute(attrName));
					}
				}
				execInterface.Options.Data.ConfigurationNode.SetAttribute("Restriction", execInterface.Options.Data.RunNode.GetAttribute("Restriction"));
			}
		}

		private void RunPreprocessors()
		{
			if (execInterface.Aborting)
			{
				return;
			}
			foreach (LoadedProcessor value in execInterface.CodeLibraries.LoadedConfigPreprocessors.Values)
			{
				try
				{
					object[] parms = new object[2]
					{
						execInterface,
						execInterface.Options.Configuration
					};
					ConfigPreprocessor configPreprocessor = (ConfigPreprocessor)value.CreateInstance(parms);
					configPreprocessor.ProcessConfiguration();
				}
				catch (Exception exception)
				{
					execInterface.LogException(exception);
				}
			}
		}

		private void RunPostprocessors()
		{
			if (execInterface.Aborting)
			{
				return;
			}
			foreach (LoadedProcessor value in execInterface.CodeLibraries.LoadedDataPostprocessors.Values)
			{
				try
				{
					object[] parms = new object[2]
					{
						execInterface,
						execInterface.Options.Data
					};
					DataPostprocessor dataPostprocessor = (DataPostprocessor)value.CreateInstance(parms);
					dataPostprocessor.ProcessData();
				}
				catch (Exception exception)
				{
					execInterface.LogException(exception);
				}
			}
		}

		private void CleanupProcessors()
		{
			execInterface.CodeLibraries.CleanupProcessors();
		}
	}
}
