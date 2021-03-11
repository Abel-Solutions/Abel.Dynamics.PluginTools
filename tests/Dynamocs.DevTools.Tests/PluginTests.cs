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
			var account = new Account
			{
				Id = Guid.NewGuid(),
				Name = "lol"
			};

			var dynamocs = new TestTools.Dynamocs();
			dynamocs.Initialize(account);

			dynamocs.ExecutePlugin<TestPlugin>(account, "update");

			var maybeUpdatedAccount = dynamocs.GetRecord<Account>(account.Id);
			Assert.Equal("foo", maybeUpdatedAccount.Name);

			var maybeCreatedLol = dynamocs.OrganizationService.RetrieveByAttribute<Account>(nameof(Account.Name).ToLower(), "bar2");
			Assert.Equal("bar2", maybeCreatedLol.Name);
			Assert.NotEqual(Guid.Empty, maybeCreatedLol.Id);

			dynamocs.OrganizationService.Received().Update(Arg.Any<Entity>());
		}
	}
}