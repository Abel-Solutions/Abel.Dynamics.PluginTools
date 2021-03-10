using System;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace Dynamocs.DevTools.Tests
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

			var dynamocs = new TestTools.Dynamocs();
			dynamocs.Initialize(account);

			dynamocs.ExecutePlugin<TestPlugin>(account, "update");

			var maybeUpdatedAccount = dynamocs.GetRecord(account.Id);
			Assert.Equal("foo", maybeUpdatedAccount["name"]);

			var maybeCreatedLol = dynamocs.GetRecord("lol");
			Assert.Equal("bar2", maybeCreatedLol["name"]);
			Assert.NotEqual(Guid.Empty, maybeCreatedLol.Id);

			dynamocs.OrganizationService.Received().Update(Arg.Any<Entity>());
		}
	}
}