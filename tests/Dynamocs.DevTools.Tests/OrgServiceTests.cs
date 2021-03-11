using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace Dynamocs.DevTools.Tests
{
	public class OrgServiceTests : TestBase
	{
		public OrgServiceTests(ITestOutputHelper output)
			: base(output)
		{
		}

		[Fact]
		public void Retrieve_Verify()
		{
			var account = new Entity("account")
			{
				Id = Guid.NewGuid()
			};

			var dynamocs = new TestTools.Dynamocs();
			dynamocs.Initialize(account);

			var retrievedAccount = dynamocs.OrganizationService.Retrieve("account", account.Id);
			Assert.Equal(account.Id, retrievedAccount.Id);

			dynamocs.OrganizationService.Received().Retrieve("account", account.Id, Arg.Any<ColumnSet>());
		}

		[Fact]
		public void RetrieveByAttribute_Verify()
		{
			var account = new Entity("account")
			{
				["name"] = "lol"
			};

			var dynamocs = new TestTools.Dynamocs();
			dynamocs.Initialize(account);

			var retrievedAccount = dynamocs.OrganizationService.RetrieveByAttribute("account", "name", "lol");
			Assert.Equal(account.Id, retrievedAccount.Id);

			dynamocs.OrganizationService.Received().RetrieveMultiple(Arg.Is<QueryBase>(q => q.EntityName() == "account"));
		}
	}
}