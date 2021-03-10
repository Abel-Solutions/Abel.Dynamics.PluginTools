using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;
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
		public void Retrieve_VerifyRetrieveWasCalled()
		{
			var account = new Entity("account")
			{
				Id = Guid.NewGuid()
			};

			var dynamocs = new Dynamocs.TestTools.Dynamocs();
			dynamocs.Initialize(account);

			var retrievedAccount = dynamocs.OrgService.Retrieve("account", account.Id);
			Assert.Equal(account.Id, retrievedAccount.Id);

			dynamocs.MockOrganizationService.Verify(o => o.Retrieve("account", account.Id, It.IsAny<ColumnSet>()), Times.Once);
		}
	}
}