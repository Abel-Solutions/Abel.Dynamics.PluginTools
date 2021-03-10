using System;
using System.Collections.Generic;
using System.Linq;
using Dynamocs.DevTools;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;

namespace Dynamocs.TestTools
{
	public class Dynamocs
	{
		public Mock<ITracingService> MockTracingService { get; } = new Mock<ITracingService>();

		public Mock<IPluginExecutionContext> MockExecutionContext { get; } = new Mock<IPluginExecutionContext>();

		public Mock<IOrganizationServiceFactory> MockServiceFactory { get; } = new Mock<IOrganizationServiceFactory>();

		public Mock<IServiceProvider> MockServiceProvider { get; } = new Mock<IServiceProvider>();

		public Mock<IOrganizationService> MockOrganizationService { get; } = new Mock<IOrganizationService>();

		public IOrgService OrgService { get; }

		private readonly Dictionary<Guid, Entity> _records = new Dictionary<Guid, Entity>();

		public Dynamocs()
		{
			OrgService = new OrgService(MockOrganizationService.Object);

			SetupOrganizationService();

			SetupServiceFactory();

			SetupServiceProvider();

			SetupTracingService();
		}

		public void ExecutePlugin<TPlugin>(Entity target, string messageName = "create", Guid? userId = null)
			where TPlugin : IPlugin
		{
			SetupExecutionContext(target, messageName, userId);

			Activator.CreateInstance<TPlugin>().Execute(MockServiceProvider.Object);
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
			_records
				.Select(r => r.Value)
				.FirstOrDefault(r => r.LogicalName == entityName);

		private void AddRecord(Entity entity)
		{
			entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
			_records[entity.Id] = entity;
		}

		private void SetupOrganizationService()
		{
			MockOrganizationService.Setup(s => s.Create(It.IsAny<Entity>()))
				.Callback((Entity e) => AddRecord(e))
				.Returns((Entity e) => e.Id);

			MockOrganizationService.Setup(s => s.Update(It.IsAny<Entity>()))
				.Callback((Entity e) => _records[e.Id] = e);

			MockOrganizationService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<Guid>()))
				.Callback((string entityName, Guid id) => _records.Remove(id));

			MockOrganizationService.Setup(s => s.Retrieve(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<ColumnSet>()))
				.Returns((string entityName, Guid id, ColumnSet columnSet) => _records[id]);

			MockOrganizationService.Setup(s => s.RetrieveMultiple(It.IsAny<QueryBase>()))
				.Returns((QueryBase query) =>
				{
					var entityName = query.GetValue<string>("EntityName");
					var records = _records
						.Where(r => r.Value.LogicalName == entityName) // todo this is crap
						.Select(r => r.Value)
						.ToList();
					return new EntityCollection(records);
				});
		}

		private void SetupServiceProvider()
		{
			MockServiceProvider.Setup(s => s.GetService(typeof(ITracingService)))
				.Returns(MockTracingService.Object);

			MockServiceProvider.Setup(s => s.GetService(typeof(IPluginExecutionContext)))
				.Returns(MockExecutionContext.Object);

			MockServiceProvider.Setup(s => s.GetService(typeof(IOrganizationServiceFactory)))
				.Returns(MockServiceFactory.Object);
		}

		private void SetupExecutionContext(Entity target, string messageName, Guid? userId)
		{
			MockExecutionContext.Setup(s => s.InputParameters)
				.Returns(new ParameterCollection { { "Target", target } });

			MockExecutionContext.Setup(s => s.MessageName)
				.Returns(messageName);

			MockExecutionContext.Setup(s => s.UserId)
				.Returns(userId ?? Guid.NewGuid());
		}

		private void SetupTracingService() =>
			MockTracingService.Setup(s => s.Trace(It.IsAny<string>(), It.IsAny<object[]>()))
				.Callback((string s, object[] p) => Console.WriteLine(s));

		private void SetupServiceFactory() =>
			MockServiceFactory.Setup(s => s.CreateOrganizationService(It.IsAny<Guid>()))
				.Returns(MockOrganizationService.Object);
	}
}