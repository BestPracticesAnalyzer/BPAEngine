using System.Collections.Generic;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class MacroCommand : ICommand
	{
		private List<ICommand> commands;

		public MacroCommand()
		{
			commands = new List<ICommand>();
		}

		public void Add(ICommand cmd)
		{
			commands.Add(cmd);
		}

		public void Remove(ICommand cmd)
		{
			commands.Remove(cmd);
		}

		public virtual void execute()
		{
			foreach (ICommand command in commands)
			{
				command.execute();
			}
		}
	}
}
