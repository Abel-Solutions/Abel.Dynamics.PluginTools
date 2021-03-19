using System;
using Abel.Dynamics.PluginTools.Extensions;
using Abel.Dynamics.PluginTools.Tests.Models;
using Abel.Dynamics.PluginTools.Tests.Plugins;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace Abel.Dynamics.PluginTools.Tests
{
	public class PluginTests
	{
		private readonly ITestOutputHelper _output;

		public PluginTests(ITestOutputHelper output) => _output = output;

		[Fact]
		public void ExecutePlugin_VerifyUpdateWasCalled()
		{
			var account = new Account
			{
				Id = Guid.NewGuid(),
				Name = "lol"
			};

			var dynamicsContext = new DynamicsContext();
			dynamicsContext.Initialize(account);

			dynamicsContext.ExecutePlugin<GenericPlugin>(account, "update");

			var maybeUpdatedAccount = dynamicsContext.GetRecord<Account>(account.Id);
			Assert.Equal("foo", maybeUpdatedAccount.Name);

			var maybeCreatedLol = dynamicsContext.OrganizationService.RetrieveByAttribute<Account>(nameof(Account.Name).ToLower(), "bar2");
			Assert.Equal("bar2", maybeCreatedLol.Name);
			Assert.NotEqual(Guid.Empty, maybeCreatedLol.Id);

			dynamicsContext.OrganizationService.Received().Update(Arg.Any<Entity>());

			dynamicsContext.TracingService.GetTraces().ForEach(_output.WriteLine);
		}

		[Fact]
		public void TriggerPlugin_TriggersItself_Throws()
		{
			var account = new Account
			{
				Id = Guid.NewGuid(),
				Name = "lol"
			};

			var dynamicsContext = new DynamicsContext();
			dynamicsContext.Initialize(account);

			dynamicsContext.RegisterPlugin<StepPlugin>();

			account.Name = "foo";

			var ex = Assert.Throws<InvalidPluginExecutionException>(() => dynamicsContext.OrganizationService.Update(account));
			Assert.Equal("Plugin depth is at or above max: 5", ex.Message);

			dynamicsContext.OrganizationService.Received(5).Update(Arg.Is<Account>(a => a.Name == "foo"));

			dynamicsContext.TracingService.GetTraces().ForEach(_output.WriteLine);
		}

		[Fact]
		public void ExecutePlugin_NoStepAttributes_DoesNotThrow()
		{
			var account = new Account
			{
				Id = Guid.NewGuid(),
				Name = "lol"
			};

			var dynamicsContext = new DynamicsContext();
			dynamicsContext.Initialize(account);

			dynamicsContext.ExecutePlugin<NoStepPlugin>(account, "update");
			
			dynamicsContext.OrganizationService.Received().Update(Arg.Any<Entity>());

			dynamicsContext.TracingService.GetTraces().ForEach(_output.WriteLine);
		}
	}
}