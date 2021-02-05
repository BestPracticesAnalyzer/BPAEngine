namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class Credentials
	{
		private string contextName = "";

		private string user = "";

		private string domain = "";

		private string password = "";

		public string ContextName
		{
			get
			{
				return contextName;
			}
		}

		public string User
		{
			get
			{
				return user;
			}
		}

		public string Domain
		{
			get
			{
				return domain;
			}
		}

		public string Password
		{
			get
			{
				return password;
			}
		}

		public Credentials(string contextName, string user, string domain, string password)
		{
			this.contextName = contextName;
			this.user = user;
			this.domain = domain;
			this.password = password;
		}
	}
}
