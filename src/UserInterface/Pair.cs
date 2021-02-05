namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public struct Pair<T, U>
	{
		public readonly T First;

		public readonly U Second;

		public Pair(T fst, U scnd)
		{
			First = fst;
			Second = scnd;
		}
	}
}
