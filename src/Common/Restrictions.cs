using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class Restrictions
	{
		private class EnabledType
		{
			public string Name;

			public ArrayList Options;
		}

		private ExecutionInterface executionInterface;

		private Hashtable types;

		private Hashtable options;

		private RestrictionOption firstOption;

		private EnabledType[] etypes;

		private uint hideMask;

		private uint filterMask;

		public Hashtable Types
		{
			get
			{
				return types;
			}
		}

		public Hashtable Options
		{
			get
			{
				return options;
			}
		}

		public RestrictionOption FirstOption
		{
			get
			{
				return firstOption;
			}
		}

		public Restrictions(ExecutionInterface executionInterface)
		{
			this.executionInterface = executionInterface;
			types = new Hashtable();
			options = new Hashtable();
			firstOption = null;
		}

		public string CommaList(string typeName, bool useDisplayName, bool enabledOnly, bool showValues)
		{
			string text = "";
			foreach (RestrictionOption option in ((RestrictionType)types[typeName]).OptionList)
			{
				if (!enabledOnly || option.Enabled)
				{
					text = ((text.Length != 0) ? (text + "," + (useDisplayName ? option.Display : option.Name)) : (useDisplayName ? option.Display : option.Name));
					if (showValues && option.Value != null)
					{
						text = text + "=" + option.Value;
					}
				}
			}
			return text;
		}

		private RestrictionType AddType(string typeName)
		{
			if (!types.Contains(typeName))
			{
				types[typeName] = new RestrictionType(typeName, this);
			}
			return (RestrictionType)types[typeName];
		}

		private RestrictionOption AddOption(string optionName, RestrictionType type)
		{
			if (!options.Contains(optionName))
			{
				options[optionName] = new RestrictionOption(optionName, type);
			}
			RestrictionOption restrictionOption = (RestrictionOption)options[optionName];
			restrictionOption.Type.OptionList.Add(restrictionOption);
			return restrictionOption;
		}

		public void SetValue(string optionName, string optionValue)
		{
			if (options.Contains(optionName))
			{
				((RestrictionOption)options[optionName]).Value = optionValue;
			}
		}

		public void Enable(string optionName)
		{
			if (!options.Contains(optionName))
			{
				return;
			}
			RestrictionOption restrictionOption = (RestrictionOption)options[optionName];
			if (restrictionOption.Enabled)
			{
				return;
			}
			restrictionOption.Enabled = true;
			restrictionOption.Type.Enabled = true;
			foreach (string include in restrictionOption.Includes)
			{
				Enable(include);
			}
		}

		public void DisableAll()
		{
			foreach (RestrictionOption value in Options.Values)
			{
				value.Enabled = false;
				value.Value = null;
			}
			foreach (RestrictionType value2 in Types.Values)
			{
				value2.Enabled = false;
			}
		}

		public void LoadValid(Document cfg)
		{
			types = new Hashtable();
			options = new Hashtable();
			firstOption = null;
			Node[] nodes = cfg.GetNodes("/*/Configuration/RestrictionType");
			Node[] array = nodes;
			foreach (Node node in array)
			{
				string attribute = node.GetAttribute("Name");
				RestrictionType restrictionType = AddType(attribute);
				if (node.HasAttribute("Display"))
				{
					restrictionType.Display = node.GetAttribute("Display");
				}
				if (node.HasAttribute("Description"))
				{
					restrictionType.Description = node.GetAttribute("Description");
				}
				if (node.GetAttribute("Collapse") == "True")
				{
					restrictionType.Collapse = true;
				}
				if (node.GetAttribute("Filter") == "True")
				{
					restrictionType.Filter = true;
				}
				if (executionInterface.Trace)
				{
					executionInterface.LogText("Restriction Type {0}", restrictionType.Name);
				}
				Node[] nodes2 = node.GetNodes("Option");
				Node[] array2 = nodes2;
				foreach (Node node2 in array2)
				{
					string attribute2 = node2.GetAttribute("Name");
					RestrictionOption restrictionOption = AddOption(attribute2, restrictionType);
					if (firstOption == null)
					{
						firstOption = restrictionOption;
					}
					if (node2.HasAttribute("Display"))
					{
						restrictionOption.Display = node2.GetAttribute("Display");
					}
					if (node2.HasAttribute("Description"))
					{
						restrictionOption.Description = node2.GetAttribute("Description");
					}
					if (node2.HasAttribute("Cost"))
					{
						restrictionOption.Cost = node2.GetAttribute("Cost");
					}
					if (node2.HasAttribute("Group"))
					{
						restrictionOption.Group = node2.GetAttribute("Group");
					}
					if (node2.HasAttribute("Includes"))
					{
						string[] array3 = node2.GetAttribute("Includes").Split(',');
						foreach (string value in array3)
						{
							restrictionOption.Includes.Add(value);
						}
					}
					if (executionInterface.Trace)
					{
						executionInterface.LogText("    Restriction Option {0}", restrictionOption.Name);
					}
				}
			}
			DisableAll();
		}

		public void EnableOptions(string optionListString)
		{
			if (optionListString == null || optionListString.Length == 0)
			{
				if (firstOption != null)
				{
					Enable(firstOption.Name);
				}
				return;
			}
			string[] array = optionListString.Split(',');
			foreach (string text in array)
			{
				if (text.Length == 0)
				{
					continue;
				}
				if (executionInterface.Trace)
				{
					executionInterface.LogText("Adding option {0}", text);
				}
				RestrictionOption restrictionOption = null;
				string[] array2 = text.Split(new char[1]
				{
					'='
				}, 2);
				foreach (RestrictionOption value in options.Values)
				{
					if (value.Name.ToUpper(CultureInfo.InvariantCulture) == array2[0].ToUpper(CultureInfo.InvariantCulture))
					{
						restrictionOption = value;
						Enable(value.Name);
						break;
					}
				}
				if (restrictionOption == null)
				{
					throw new ArgumentException(CommonLoc.Error_BadRestrictionParam(array2[0]));
				}
				if (array2.Length > 1)
				{
					restrictionOption.Value = array2[1];
				}
			}
		}

		public void MarkRestrictions(Document cfg)
		{
			int num = 0;
			foreach (RestrictionType value in types.Values)
			{
				if (value.Enabled)
				{
					num++;
				}
			}
			etypes = new EnabledType[num];
			hideMask = 0u;
			filterMask = 0u;
			int num2 = 0;
			foreach (RestrictionType value2 in types.Values)
			{
				if (!value2.Enabled)
				{
					continue;
				}
				etypes[num2] = new EnabledType();
				etypes[num2].Name = value2.Name;
				etypes[num2].Options = new ArrayList();
				if (value2.Collapse)
				{
					hideMask |= (uint)(1 << num2);
				}
				if (value2.Filter)
				{
					filterMask |= (uint)(1 << num2);
				}
				foreach (RestrictionOption option in value2.OptionList)
				{
					if (option.Enabled)
					{
						etypes[num2].Options.Add(option.Name);
					}
				}
				num2++;
			}
			Node[] nodes = cfg.GetNodes("*/Object");
			foreach (Node element in nodes)
			{
				SetDeferred(element, uint.MaxValue);
			}
		}

		private uint SetDeferred(Node element, uint status)
		{
			uint num = 0u;
			uint num2 = 0u;
			for (int i = 0; i < etypes.Length; i++)
			{
				EnabledType enabledType = etypes[i];
				if (!element.HasAttribute(enabledType.Name))
				{
					continue;
				}
				bool flag = MatchValues(enabledType.Options, element.GetAttribute(enabledType.Name));
				uint num3 = (uint)(1 << i);
				if (flag)
				{
					num2 |= num3;
				}
				switch (element.GetAttribute(enabledType.Name).Substring(0, 1))
				{
				case ",":
					if (flag)
					{
						status |= num3;
					}
					break;
				case "+":
					if (!flag)
					{
						status &= ~num3;
					}
					break;
				default:
					status = ((!flag) ? (status & ~num3) : (status | num3));
					break;
				}
			}
			Node[] nodes = element.GetNodes("*");
			foreach (Node element2 in nodes)
			{
				num |= SetDeferred(element2, status);
			}
			if ((hideMask & num & ~status) != 0)
			{
				element.SetAttribute("Display", "Hide");
			}
			if (element.HasAttribute("Substitution"))
			{
				element.SetAttribute("Restrict", "No");
				if ((hideMask & ~status) != 0)
				{
					element.SetAttribute("Display", "Hide");
				}
			}
			else
			{
				if ((status | num) != uint.MaxValue)
				{
					element.Delete();
					return 0u;
				}
				if ((num & filterMask) != 0)
				{
					element.SetAttribute("Restrict", "No");
				}
			}
			if ((filterMask & num2) != 0)
			{
				uint num4 = filterMask & num2;
				string text = "";
				for (int k = 0; k < etypes.Length; k++)
				{
					if ((num4 & (uint)(1 << k)) == 0)
					{
						continue;
					}
					string[] array = element.GetAttribute(etypes[k].Name).TrimStart(',', '+').Split(',');
					foreach (string text2 in array)
					{
						if (((RestrictionOption)options[text2]).Value != null)
						{
							text = text + "^%" + text2 + "%$|";
						}
					}
				}
				if (text.Length > 0)
				{
					element.SetAttribute("Filter", text.TrimEnd('|'));
				}
			}
			return num | (num2 & status);
		}

		private bool MatchValues(ArrayList options, string attribute)
		{
			string[] array = attribute.TrimStart(',', '+').Split(',');
			foreach (string option in options)
			{
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					if (option.ToUpper() == text2.ToUpper())
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
