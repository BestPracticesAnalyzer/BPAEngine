using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	internal class DataParser
	{
		internal enum LookupHead
		{
			Condition,
			Step,
			Field,
			Query,
			String,
			XPath
		}

		internal class LookupNode
		{
			private LookupHead head;

			private object[] args;

			public LookupHead Head
			{
				get
				{
					return head;
				}
			}

			public object[] Args
			{
				get
				{
					return args;
				}
				set
				{
					args = value;
				}
			}

			public LookupNode(LookupHead head, params object[] args)
			{
				this.head = head;
				this.args = args;
			}
		}

		private string lookup;

		private bool dump;

		private int pos;

		private LookupNode root;

		private DataInstanceSet initial;

		private StepID stepID;

		private Node node;

		internal DataParser(string lookup, bool dump, Node node)
		{
			this.lookup = lookup;
			this.dump = dump;
			this.node = node;
			root = ParseStep(dump);
		}

		public void Parse()
		{
			root = ParseStep(dump);
		}

		internal LookupNode ParseStep(bool dump)
		{
			ArrayList arrayList = new ArrayList();
			if (lookup[pos] == '/')
			{
				arrayList.Add("/");
				pos++;
				arrayList.Add(null);
				if (pos < lookup.Length && lookup[pos] != ']' && lookup[pos] != '|')
				{
					arrayList[1] = ParseStep(dump);
				}
			}
			else
			{
				arrayList.Add(ParseString("(\\w[\\w\\d]*|\\.\\.)"));
				arrayList.Add(null);
				while (pos < lookup.Length && lookup[pos] == '[')
				{
					pos++;
					LookupNode value = ParseCondition();
					arrayList.Add(value);
					pos++;
				}
				if (pos >= lookup.Length)
				{
					if (!dump)
					{
						if (arrayList.Count > 2)
						{
							throw new Exception("Can't have a condition on the final step of a lookup when setting data");
						}
						return new LookupNode(LookupHead.Field, arrayList[0]);
					}
				}
				else if (lookup[pos] == '/')
				{
					pos++;
					arrayList[1] = ParseStep(dump);
				}
			}
			return new LookupNode(LookupHead.Step, arrayList.ToArray());
		}

		internal LookupNode ParseCondition()
		{
			ArrayList arrayList = new ArrayList();
			while (true)
			{
				LookupNode value;
				switch (lookup[pos])
				{
				case '\'':
				{
					string text = ParseString("'(.*?)'");
					value = new LookupNode(LookupHead.String, text);
					break;
				}
				case '"':
				{
					string text = ParseString("\"(.*?)\"");
					value = new LookupNode(LookupHead.String, text);
					break;
				}
				case '{':
				{
					string text = ParseString("{(.*?)}");
					value = new LookupNode(LookupHead.XPath, text);
					break;
				}
				default:
					value = ParseStep(true);
					break;
				}
				arrayList.Add(value);
				if (lookup[pos] != '|')
				{
					break;
				}
				pos++;
			}
			return new LookupNode(LookupHead.Condition, arrayList.ToArray());
		}

		internal string ParseString(string reString)
		{
			Regex regex = new Regex(reString);
			Match match = regex.Match(lookup, pos);
			if (match.Success)
			{
				pos += match.Length;
				if (match.Groups.Count > 0)
				{
					return match.Groups[1].Value;
				}
				return match.Groups[0].Value;
			}
			throw new Exception("Error parsing regex " + reString + " in Data lookup");
		}

		public DataInstanceSet Evaluate(DataInstanceSet dataInstanceSet, StepID stepID)
		{
			initial = dataInstanceSet;
			this.stepID = stepID;
			return EvaluateStep(root, dataInstanceSet);
		}

		private bool EvaluateCondition(LookupNode lookupNode, DataInstance instance)
		{
			string a = instance.ToString();
			object[] args = lookupNode.Args;
			for (int i = 0; i < args.Length; i++)
			{
				LookupNode lookupNode2 = (LookupNode)args[i];
				switch (lookupNode2.Head)
				{
				case LookupHead.String:
					if (a == (string)lookupNode2.Args[0])
					{
						return true;
					}
					break;
				case LookupHead.XPath:
				{
					object obj = ExtFunction.Evaluate(node, (string)lookupNode2.Args[0]);
					if (obj is XPathNodeIterator)
					{
						XPathNodeIterator xPathNodeIterator = (XPathNodeIterator)obj;
						while (xPathNodeIterator.MoveNext())
						{
							if (a == ExtFunction.Stringify(xPathNodeIterator.Current))
							{
								return true;
							}
						}
					}
					else if (a == ExtFunction.Stringify(obj))
					{
						return true;
					}
					break;
				}
				case LookupHead.Step:
				{
					ArrayList arrayList = EvaluateStep(lookupNode2, new DataInstanceSet(instance)).ConvertToStrings();
					if (arrayList.Count > 0)
					{
						return true;
					}
					break;
				}
				default:
					throw new Exception();
				}
			}
			return false;
		}

		private DataInstanceSet EvaluateCondition(LookupNode lookupNode, DataInstanceSet input)
		{
			DataInstanceSet dataInstanceSet = new DataInstanceSet(input.DataObject);
			foreach (DataInstance item in input)
			{
				if (EvaluateCondition(lookupNode, item))
				{
					dataInstanceSet.Add(item);
				}
			}
			return dataInstanceSet;
		}

		private DataInstanceSet EvaluateStep(LookupNode lookupNode, DataInstanceSet input)
		{
			DataInstanceSet dataInstanceSet = null;
			string text = (string)lookupNode.Args[0];
			if (lookupNode.Head == LookupHead.Field)
			{
				input.FinalName = (string)lookupNode.Args[0];
				return input;
			}
			if (text == "/")
			{
				dataInstanceSet = new DataInstanceSet(DataObjectProcessor.GetRootInstance());
			}
			else
			{
				foreach (DataInstance item in input)
				{
					if (dataInstanceSet == null)
					{
						dataInstanceSet = item.GetChildren(text, stepID);
					}
					else
					{
						dataInstanceSet.Merge(item.GetChildren(text, stepID));
					}
				}
			}
			for (int i = 2; i < lookupNode.Args.Length; i++)
			{
				dataInstanceSet = EvaluateCondition((LookupNode)lookupNode.Args[i], dataInstanceSet);
			}
			if (lookupNode.Args[1] != null)
			{
				dataInstanceSet = EvaluateStep((LookupNode)lookupNode.Args[1], dataInstanceSet);
			}
			if (dataInstanceSet == null)
			{
				return new DataInstanceSet();
			}
			return dataInstanceSet;
		}
	}
}
