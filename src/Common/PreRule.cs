using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class PreRule : PreNode
	{
		private readonly Regex paramMatch = new Regex("^[SP]\\d+$");

		private RuleExpression query;

		private RuleExpression[] param;

		private bool checkCondition;

		private bool conditionToTrigger = true;

		public RuleExpression Query
		{
			get
			{
				return query;
			}
		}

		public RuleExpression[] Param
		{
			get
			{
				return param;
			}
		}

		public bool CheckCondition
		{
			get
			{
				return checkCondition;
			}
		}

		public bool ConditionToTrigger
		{
			get
			{
				return conditionToTrigger;
			}
		}

		[Obsolete("Title is no longer part of the PreRule.  Read the attribute directly from the element.")]
		public string Title
		{
			get
			{
				return null;
			}
		}

		[Obsolete("Text is no longer part of the PreRule.  Read the attribute directly from the element.")]
		public string Text
		{
			get
			{
				return null;
			}
		}

		[Obsolete("The Attribute hashtable is no longer part of the PreRule.  Read the attributes directly from the element.")]
		public Hashtable Attribute
		{
			get
			{
				return null;
			}
		}

		public PreRule(XmlElement element, ExecutionInterface executionInterface, Rules rules)
			: base(element, executionInterface, rules)
		{
			if (name.Length == 0)
			{
				name = allRules.UniqueName();
				element.SetAttribute("Name", name);
			}
			if (allRules.RuleHash.Contains(name))
			{
				throw new ExDiagRuleFormatException("The rule name " + name + " is used more than once in input file");
			}
			if (element.HasAttribute("Text"))
			{
				checkCondition = true;
			}
		}

		public override void ExpandAttributes()
		{
			if (executionInterface.Trace)
			{
				executionInterface.LogText("\tPreprocessing rule '{0}'", base.name);
			}
			query = new RuleExpression(element.HasAttribute("Query") ? element.GetAttribute("Query") : "true()", true, !element.HasAttribute("AlwaysEvaluate"));
			if (query.Text.Length == 0)
			{
				throw new ExDiagRuleFormatException("Empty query in rule " + base.name);
			}
			FindReferences(query);
			element.SetAttribute("Query", query.Text);
			Hashtable hashtable = new Hashtable();
			int num = -1;
			foreach (XmlAttribute attribute in element.Attributes)
			{
				if (paramMatch.IsMatch(attribute.Name))
				{
					int num2 = int.Parse(attribute.Name.Substring(1));
					if (num2 > num)
					{
						num = num2;
					}
					if (attribute.Name[0] == 'P')
					{
						hashtable[num2] = attribute.Value;
					}
				}
			}
			param = new RuleExpression[num + 1];
			for (int i = 0; i < param.Length; i++)
			{
				string name = "P" + i;
				if (hashtable.Contains(i))
				{
					param[i] = new RuleExpression((string)hashtable[i], false, false);
					FindReferences(param[i]);
					element.SetAttribute(name, param[i].Text);
				}
			}
		}

		public override string ToString()
		{
			return "  Rule       " + name + "\n    Query =  " + query.Text + "\n";
		}
	}
}
