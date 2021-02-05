using System;
using System.Text;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class StepID
	{
		private int[] parts;

		public StepID(int[] parts)
		{
			this.parts = parts;
		}

		public StepID(string stepString)
		{
			string[] array = stepString.Split('.');
			parts = new int[array.Length];
			for (int i = 0; i < parts.Length; i++)
			{
				parts[i] = Convert.ToInt32(array[i]);
			}
		}

		public bool Equals(StepID other)
		{
			if (parts.Length != other.parts.Length)
			{
				return false;
			}
			for (int i = 0; i < parts.Length; i++)
			{
				if (parts[i] != other.parts[i])
				{
					return false;
				}
			}
			return true;
		}

		public bool Under(StepID other)
		{
			if (parts.Length < other.parts.Length)
			{
				return false;
			}
			for (int i = 0; i < other.parts.Length; i++)
			{
				if (parts[i] != other.parts[i])
				{
					return false;
				}
			}
			return true;
		}

		public bool Before(StepID other)
		{
			for (int i = 0; i < parts.Length; i++)
			{
				if (i == other.parts.Length)
				{
					return false;
				}
				if (parts[i] < other.parts[i])
				{
					return true;
				}
				if (parts[i] > other.parts[i])
				{
					return false;
				}
			}
			return true;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < parts.Length; i++)
			{
				stringBuilder.Append(parts[i]);
				if (i < parts.Length - 1)
				{
					stringBuilder.Append(".");
				}
			}
			return stringBuilder.ToString();
		}
	}
}
