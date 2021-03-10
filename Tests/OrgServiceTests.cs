using System;
using NotDynamocs;
using Microsoft.Xrm.Sdk;
using OrgServiz;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
	public class OrgServiceTests : TestBase
	{
		public OrgServiceTests(ITestOutputHelper output)
			: base(output)
		{
		}

		[Fact]
		public void Retrieve()
		{
			var account = new Entity("account")
			{
				Id = Guid.NewGuid()
			};

			var dynamocs = new Dynamocs();
			dynamocs.Initialize(account);

			var orgService = new OrgService(dynamocs.OrganizationService.Object);

			var retrievedAccount = orgService.Retrieve("account", account.Id);
			Assert.Equal(account.Id, retrievedAccount.Id);
		}
	}
}