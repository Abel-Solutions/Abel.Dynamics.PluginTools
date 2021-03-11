using System;
using System.Collections.Generic;
using System.Linq;
using Dynamocs.DevTools;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NSubstitute;

namespace Dynamocs.TestTools
{
	public class Dynamocs
	{
		public ITracingService TracingService { get; } = Substitute.For<ITracingService>();

		public IPluginExecutionContext ExecutionContext { get; } = Substitute.For<IPluginExecutionContext>();

		public IOrganizationServiceFactory ServiceFactory { get; } = Substitute.For<IOrganizationServiceFactory>();

		public IServiceProvider ServiceProvider { get; } = Substitute.For<IServiceProvider>();

		public IOrganizationService OrganizationService { get; } = Substitute.For<IOrganizationService>();

		private readonly Dictionary<Guid, Entity> _records = new Dictionary<Guid, Entity>();

		public Dynamocs()
		{
			SetupOrganizationService();

			SetupServiceFactory();

			SetupServiceProvider();

			SetupTracingService();
		}

		public void ExecutePlugin<TPlugin>(Entity target, string messageName = "create", Guid? userId = null)
			where TPlugin : IPlugin
		{
			SetupExecutionContext(target, messageName, userId);

			Activator.CreateInstance<TPlugin>().Execute(ServiceProvider);
		}

		public void Initialize(params Entity[] records) => records.ToList().ForEach(AddRecord);

		public TEntity GetRecord<TEntity>()
			where TEntity : Entity =>
			GetRecord(Activator.CreateInstance<TEntity>().LogicalName)?.ToEntity<TEntity>();

		public TEntity GetRecord<TEntity>(Guid id)
			where TEntity : Entity =>
			GetRecord(id)?.ToEntity<TEntity>();

		public Entity GetRecord(Guid id) =>
			_records.ContainsKey(id) ? _records[id] : null;

		public Entity GetRecord(string entityName) =>
			GetRecords(entityName).FirstOrDefault();

		public IEnumerable<Entity> GetRecords(string entityName) =>
			_records.Select(r => r.Value).Where(r => r.LogicalName == entityName);

		private void AddRecord(Entity entity)
		{
			entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
			_records[entity.Id] = entity;
		}

		private void SetupOrganizationService()
		{
			OrganizationService.Create(Arg.Do<Entity>(AddRecord))
				.Returns(args => args.Arg<Entity>().Id);

			OrganizationService.Update(Arg.Do<Entity>(e => _records[e.Id] = e));

			OrganizationService.Delete(Arg.Any<string>(), Arg.Do<Guid>(id => _records.Remove(id)));

			OrganizationService.Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>())
				.Returns(args => _records[args.Arg<Guid>()]);

			OrganizationService.RetrieveMultiple(Arg.Any<QueryByAttribute>())
				.Returns(args => _records.Select(r => r.Value).Where(r => IsMatch(r, args.Arg<QueryByAttribute>())).ToEntityCollection());
		}

		private static bool IsMatch(Entity entity, QueryByAttribute query) =>
			entity.LogicalName == query.EntityName &&
			!query.Attributes.Where((t, i) => !entity.Contains(t) || entity[t] != query.Values[i]).Any();

		private void SetupServiceProvider()
		{
			ServiceProvider.GetService(typeof(ITracingService))
				.Returns(TracingService);

			ServiceProvider.GetService(typeof(IPluginExecutionContext))
				.Returns(ExecutionContext);

			ServiceProvider.GetService(typeof(IOrganizationServiceFactory))
				.Returns(ServiceFactory);
		}

		private void SetupExecutionContext(Entity target, string messageName, Guid? userId)
		{
			ExecutionContext.InputParameters.Returns(new ParameterCollection { { "Target", target } });

			ExecutionContext.MessageName.Returns(messageName);

			ExecutionContext.UserId.Returns(userId ?? Guid.NewGuid());
		}

		private void SetupTracingService() =>
			TracingService.Trace(Arg.Do<string>(Console.WriteLine), Arg.Any<object[]>());

		private void SetupServiceFactory() =>
			ServiceFactory.CreateOrganizationService(Arg.Any<Guid>())
				.Returns(OrganizationService);
	}
}