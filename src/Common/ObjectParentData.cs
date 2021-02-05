using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	internal class ObjectParentData
	{
		private SortedList inheritedProps;

		private SortedList substitutions;

		private bool passedFilter = true;

		private bool trackProgressSetOnThisObject;

		private ObjectParentData parentOpd;

		private ArrayList processingInfos;

		internal ObjectParentData ParentOpd
		{
			get
			{
				return parentOpd;
			}
		}

		public bool PassedFilter
		{
			get
			{
				return passedFilter;
			}
		}

		internal ObjectParentData()
		{
			inheritedProps = new SortedList();
			substitutions = new SortedList();
			processingInfos = new ArrayList();
		}

		internal void AddProcessingInfo(ProcessingInfo pInfo)
		{
			processingInfos.Add(pInfo);
		}

		internal void AddProcessingValues(string objType, bool failed, bool timedout, TimeSpan timeToProcess)
		{
			foreach (ProcessingInfo processingInfo in processingInfos)
			{
				processingInfo.AddValues(objType, failed, timedout, timeToProcess);
			}
		}

		internal void AddSubstitutionString(string substsString, string val)
		{
			if (substsString.Length > 0)
			{
				if (!substitutions.ContainsKey(substsString))
				{
					substitutions.Add(substsString, val);
				}
				else if (val != null)
				{
					substitutions[substsString] = val;
				}
			}
		}

		internal ObjectParentData Clone()
		{
			ObjectParentData objectParentData = new ObjectParentData();
			foreach (string key3 in substitutions.Keys)
			{
				objectParentData.substitutions.Add(key3, substitutions[key3]);
			}
			foreach (string key4 in inheritedProps.Keys)
			{
				objectParentData.inheritedProps.Add(key4, inheritedProps[key4]);
			}
			foreach (ProcessingInfo processingInfo in processingInfos)
			{
				objectParentData.processingInfos.Add(processingInfo);
			}
			objectParentData.passedFilter = passedFilter;
			objectParentData.parentOpd = this;
			return objectParentData;
		}

		public object InheritedProperty(string propName)
		{
			object obj = inheritedProps[propName];
			if (obj == null)
			{
				return string.Empty;
			}
			return obj;
		}

		public void SetFilterStatus(Node node)
		{
			if (node.HasAttribute("Filter"))
			{
				passedFilter = Regex.Match(node.GetAttribute("Name"), node.GetAttribute("Filter"), RegexOptions.IgnoreCase).Success;
			}
		}

		internal string MakeSubstitutions(string str)
		{
			if (str.IndexOf("%") == -1)
			{
				return str;
			}
			for (int i = 0; i < substitutions.Count; i++)
			{
				string text = "%" + substitutions.GetKey(i).ToString() + "%";
				if (substitutions.GetByIndex(i) == null)
				{
					if (str.IndexOf(text) != -1)
					{
						return null;
					}
				}
				else
				{
					string newValue = substitutions.GetByIndex(i).ToString();
					str = str.Replace(text, newValue);
				}
			}
			return str;
		}

		public bool HasSubstitution(string val)
		{
			if (val.IndexOf("%") == -1)
			{
				return false;
			}
			for (int i = 0; i < substitutions.Count; i++)
			{
				string value = "%" + substitutions.GetKey(i).ToString() + "%";
				if (val.IndexOf(value) != -1)
				{
					return true;
				}
			}
			return false;
		}

		internal bool OverThresholds(string objType)
		{
			bool result = false;
			foreach (ProcessingInfo processingInfo in processingInfos)
			{
				if (processingInfo.OverThreshold(objType))
				{
					return true;
				}
			}
			return result;
		}

		public void SetInheritedProperty(string propName, object propValue)
		{
			if (!inheritedProps.ContainsKey(propName))
			{
				inheritedProps.Add(propName, propValue);
			}
			else
			{
				inheritedProps[propName] = propValue;
			}
		}

		internal void SetFailHard()
		{
			ObjectParentData objectParentData = this;
			while (objectParentData != null && !objectParentData.trackProgressSetOnThisObject)
			{
				objectParentData = objectParentData.parentOpd;
			}
			if (objectParentData != null && objectParentData.trackProgressSetOnThisObject)
			{
				((ProcessingInfo)objectParentData.processingInfos[objectParentData.processingInfos.Count - 1]).SetFailHard();
			}
		}

		internal void TrackProgressAdjust(Node cfgParent, int instanceCount)
		{
			foreach (ProcessingInfo processingInfo in processingInfos)
			{
				processingInfo.TrackProgressAdjust(cfgParent, instanceCount);
			}
		}

		internal void TrackProgressStart(Node cfgParent)
		{
			trackProgressSetOnThisObject = true;
			((ProcessingInfo)processingInfos[processingInfos.Count - 1]).TrackProgressStart(cfgParent);
		}

		internal void TrackProgressUpdate(Node processedNode)
		{
			foreach (ProcessingInfo processingInfo in processingInfos)
			{
				processingInfo.TrackProgressUpdate(processedNode);
			}
		}

		public Node ApplySubstitutions(Node node)
		{
			string[] attributes = node.Attributes;
			foreach (string attrName in attributes)
			{
				string text = MakeSubstitutions(node.GetAttribute(attrName));
				if (text == null)
				{
					return null;
				}
				node.SetAttribute(attrName, text);
			}
			return node;
		}

		public void CaptureSubstitution(Node setting)
		{
			string attribute = setting.GetAttribute("Substitution");
			if (attribute.Length == 0)
			{
				return;
			}
			string attribute2 = setting.GetAttribute("SubstitutionFormat");
			string text = "";
			Node[] nodes = setting.GetNodes("Value");
			Node[] array = nodes;
			foreach (Node node in array)
			{
				if (node.Value.Length > 0)
				{
					text += ExtFormat.ConvertValueToString(node.Value, attribute2);
				}
			}
			if (attribute.Length > 0)
			{
				AddSubstitutionString(attribute, text);
			}
		}
	}
}
