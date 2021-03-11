using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace Dynamocs.DevTools.Tests
{
	[EntityLogicalName("account")]
	public class Account : Entity
	{
		public Account() : base("account")
		{
		}

		public string Name
		{
			get => GetAttributeValue<string>("name");
			set => Attributes["name"] = value;
		}
	}
}
