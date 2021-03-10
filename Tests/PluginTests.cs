using System;
using Plugins;
using NotDynamocs;
using Microsoft.Xrm.Sdk;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
	public class PluginTests : TestBase
	{
		public PluginTests(ITestOutputHelper output)
			: base(output)
		{
		}

		[Fact]
		public void RunPlugin_VerifyUpdateWasCalled()
		{
			var account = new Entity("account")
			{
				Id = Guid.NewGuid()
			};

			var dynamocs = new Dynamocs();
			dynamocs.Initialize(account);

			dynamocs.ExecutePlugin<Plugin>(account, "update");

			var maybeUpdatedAccount = dynamocs.GetRecord(account.Id);
			Assert.Equal("foo", maybeUpdatedAccount["name"]);

			var maybeCreatedLol = dynamocs.GetRecord("lol");
			Assert.Equal("bar2", maybeCreatedLol["name"]);
			Assert.NotEqual(Guid.Empty, maybeCreatedLol.Id);

			dynamocs.OrganizationService.Verify(s => s.Update(It.IsAny<Entity>()), Times.Once);
		}
	}
}