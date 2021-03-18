using System;
using Abel.Dynamics.PluginTools.Extensions;
using Abel.Dynamics.PluginTools.Tests.Models;
using Abel.Dynamics.PluginTools.Tests.Plugins;
using FluentAssertions;
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
		public void ExecutePlugin_Latebound_UpdatesAccountName()
		{
			var account = new Entity("account")
			{
				Id = Guid.NewGuid()
			};

			var dynamicsContext = new DynamicsContext();
			dynamicsContext.Initialize(account);

			dynamicsContext.ExecutePlugin<LateboundPlugin>(account, "update");

			account["name"].Should().Be("foo");
			dynamicsContext.OrganizationService.Received().Update(Arg.Is<Entity>(a => a.GetAttributeValue<string>("name") == "foo"));

			WriteTraces(dynamicsContext);
		}

		[Fact]
		public void ExecutePlugin_InvalidTrigger_Throws()
		{
			var account = new Account
			{
				Id = Guid.NewGuid(),
				Name = "lol"
			};

			var dynamicsContext = new DynamicsContext();
			dynamicsContext.Initialize(account);

			FluentActions.Invoking(() => dynamicsContext.ExecutePlugin<EarlyboundPlugin>(account, "woop")).Should().Throw<InvalidPluginExecutionException>()
				.WithMessage($"Error: {nameof(EarlyboundPlugin)} does not have any PluginStepAttribute with MessageName woop and EntityName account");

			WriteTraces(dynamicsContext);
		}

		[Fact]
		public void ExecutePlugin_NoStepAttributes_UpdatesAccountName()
		{
			var account = new Account
			{
				Id = Guid.NewGuid(),
				Name = "lol"
			};

			var dynamicsContext = new DynamicsContext();
			dynamicsContext.Initialize(account);

			dynamicsContext.ExecutePlugin<NoStepsPlugin>(account, "update");

			dynamicsContext.OrganizationService.Received().Update(Arg.Is<Account>(a => a.Name == "foo"));

			WriteTraces(dynamicsContext);
		}

		[Fact]
		public void RegisterPlugin_NoStepsPlugin_UpdatesAccountName()
		{
			var account = new Account
			{
				Id = Guid.NewGuid(),
				Name = "lol"
			};

			var dynamicsContext = new DynamicsContext();

			dynamicsContext.RegisterPlugin<NoStepsPlugin>("create", "account");
			dynamicsContext.OrganizationService.Create(account);

			account.Name.Should().Be("foo");
			dynamicsContext.OrganizationService.Received().Update(Arg.Is<Account>(a => a.Name == "foo"));

			WriteTraces(dynamicsContext);
		}

		[Fact]
		public void RegisterPlugin_TriggersItself_Throws()
		{
			var account = new Account
			{
				Id = Guid.NewGuid(),
				Name = "bar"
			};

			var dynamicsContext = new DynamicsContext();

			dynamicsContext.RegisterPlugin<EarlyboundPlugin>();

			FluentActions.Invoking(() => dynamicsContext.OrganizationService.Create(account)).Should().Throw<InvalidPluginExecutionException>()
				.WithMessage("Plugin depth is at or above max: 5");
			dynamicsContext.OrganizationService.Received(5).Update(Arg.Is<Account>(a => a.Name == "foo"));

			WriteTraces(dynamicsContext);
		}

		private void WriteTraces(DynamicsContext dynamicsContext) =>
			dynamicsContext.TracingService.GetTraces().ForEach(_output.WriteLine);
	}
}