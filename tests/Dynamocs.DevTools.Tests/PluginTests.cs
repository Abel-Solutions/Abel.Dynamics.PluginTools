using System;
using Dynamocs.DevTools.Tests.Models;
using Dynamocs.DevTools.Tests.Plugins;
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
		public void ExecutePlugin_VerifyUpdateWasCalled()
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

		[Fact]
		public void TriggerPlugin_TriggersItself_Throws()
		{
			var account = new Account
			{
				Id = Guid.NewGuid(),
				Name = "lol"
			};

			var dynamocs = new TestTools.Dynamocs();
			dynamocs.Initialize(account);

			dynamocs.RegisterPlugin<TestPlugin>();

			account.Name = "foo";

			var ex = Assert.Throws<InvalidPluginExecutionException>(() => dynamocs.OrganizationService.Update(account));
			Assert.Equal("Plugin depth is at or above max: 5 (max is 5)", ex.Message);

			dynamocs.OrganizationService.Received(PluginBase.MaxDepth).Update(Arg.Is<Account>(a => a.Name == "foo"));
		}
	}
}