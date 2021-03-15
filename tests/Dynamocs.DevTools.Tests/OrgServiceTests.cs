using System;
using Abel.Dynamics.PluginTools.Extensions;
using Abel.Dynamics.PluginTools.Tests.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NSubstitute;
using Xunit;

namespace Abel.Dynamics.PluginTools.Tests
{
	public class OrgServiceTests
	{
		[Fact]
		public void Retrieve_Verify()
		{
			var account = new Entity("account")
			{
				Id = Guid.NewGuid()
			};

			var dynamicsContext = new DynamicsContext();
			dynamicsContext.Initialize(account);

			var retrievedAccount = dynamicsContext.OrganizationService.Retrieve("account", account.Id);
			Assert.Equal(account.Id, retrievedAccount.Id);

			dynamicsContext.OrganizationService.Received().Retrieve("account", account.Id, Arg.Any<ColumnSet>());
		}

		[Fact]
		public void RetrieveByAttribute_Verify()
		{
			var account = new Entity("account")
			{
				["name"] = "lol"
			};

			var dynamicsContext = new DynamicsContext();
			dynamicsContext.Initialize(account);

			var retrievedAccount = dynamicsContext.OrganizationService.RetrieveByAttribute("account", "name", "lol");
			Assert.Equal(account.Id, retrievedAccount.Id);

			dynamicsContext.OrganizationService.Received().RetrieveMultiple(Arg.Is<QueryByAttribute>(q => q.EntityName == "account"));
		}

		[Fact]
		public void RetrieveByAttribute_ShouldNotMatch()
		{
			var account = new Entity("account")
			{
				["name"] = "lol"
			};

			var dynamicsContext = new DynamicsContext();
			dynamicsContext.Initialize(account);

			var retrievedAccount = dynamicsContext.OrganizationService.RetrieveByAttribute("account", "name", "foo");
			Assert.Null(retrievedAccount);

			dynamicsContext.OrganizationService.Received().RetrieveMultiple(Arg.Is<QueryByAttribute>(q => q.EntityName == "account"));
		}

		[Fact]
		public void RetrieveByAttributeGeneric_ShouldMatch()
		{
			var account = new Account
			{
				Name = "Kåre"
			};

			var dynamicsContext = new DynamicsContext();
			dynamicsContext.Initialize(account);

			var retrievedAccount = dynamicsContext.OrganizationService.RetrieveByAttribute<Account>("name", "Kåre");
			Assert.Equal(account.Name, retrievedAccount.Name);

			dynamicsContext.OrganizationService.Received().RetrieveMultiple(Arg.Is<QueryByAttribute>(q =>
				q.EntityName == "account" && q.Attributes.Contains("name") && q.Values.Contains("Kåre")));
		}
	}
}