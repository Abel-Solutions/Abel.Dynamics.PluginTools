using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using Reflex;

namespace NotDynamocs
{
	public class Dynamocs
	{
		public Mock<ITracingService> TracingService { get; } = new Mock<ITracingService>();

		public Mock<IPluginExecutionContext> ExecutionContext { get; } = new Mock<IPluginExecutionContext>();

		public Mock<IOrganizationServiceFactory> ServiceFactory { get; } = new Mock<IOrganizationServiceFactory>();

		public Mock<IServiceProvider> ServiceProvider { get; } = new Mock<IServiceProvider>();

		public Mock<IOrganizationService> OrganizationService { get; } = new Mock<IOrganizationService>();

		public Dictionary<Guid, Entity> Records { get; private set; } = new Dictionary<Guid, Entity>();

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

			Activator.CreateInstance<TPlugin>().Execute(ServiceProvider.Object);
		}

		public void Initialize(params Entity[] records) => Records = records.ToDictionary(r => r.Id, r => r);

		public TEntity GetRecord<TEntity>()
			where TEntity : Entity =>
			GetRecord(Activator.CreateInstance<TEntity>().LogicalName)?.ToEntity<TEntity>();

		public TEntity GetRecord<TEntity>(Guid id)
			where TEntity : Entity =>
			GetRecord(id)?.ToEntity<TEntity>();

		public Entity GetRecord(Guid id) =>
			Records.ContainsKey(id) ? Records[id] : null;

		public Entity GetRecord(string entityName) =>
			Records
				.Select(r => r.Value)
				.FirstOrDefault(r => r.LogicalName == entityName);

		private void SetupOrganizationService()
		{
			OrganizationService.Setup(s => s.Create(It.IsAny<Entity>()))
				.Callback((Entity e) =>
				{
					e.Id = e.Id == Guid.Empty ? Guid.NewGuid() : e.Id;
					Records[e.Id] = e;
				})
				.Returns((Entity e) => e.Id);

			OrganizationService.Setup(s => s.Update(It.IsAny<Entity>()))
				.Callback((Entity e) => Records[e.Id] = e);

			OrganizationService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<Guid>()))
				.Callback((string entityName, Guid id) => Records.Remove(id));

			OrganizationService.Setup(s => s.Retrieve(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<ColumnSet>()))
				.Returns((string entityName, Guid id, ColumnSet columnSet) => Records[id]);

			OrganizationService.Setup(s => s.RetrieveMultiple(It.IsAny<QueryBase>()))
				.Returns((QueryBase query) =>
				{
					var entityName = query.GetValue<string>("EntityName");
					var records = Records
						.Where(r => r.Value.LogicalName == entityName) // todo this is crap
						.Select(r => r.Value)
						.ToList();
					return new EntityCollection(records);
				});
		}

		private void SetupServiceProvider()
		{
			ServiceProvider.Setup(s => s.GetService(typeof(ITracingService)))
				.Returns(TracingService.Object);

			ServiceProvider.Setup(s => s.GetService(typeof(IPluginExecutionContext)))
				.Returns(ExecutionContext.Object);

			ServiceProvider.Setup(s => s.GetService(typeof(IOrganizationServiceFactory)))
				.Returns(ServiceFactory.Object);
		}

		private void SetupExecutionContext(Entity target, string messageName, Guid? userId)
		{
			ExecutionContext.Setup(s => s.InputParameters)
				.Returns(new ParameterCollection { { "Target", target } });

			ExecutionContext.Setup(s => s.MessageName)
				.Returns(messageName);

			ExecutionContext.Setup(s => s.UserId)
				.Returns(userId ?? Guid.NewGuid());
		}

		private void SetupTracingService() =>
			TracingService.Setup(s => s.Trace(It.IsAny<string>(), It.IsAny<object[]>()))
				.Callback((string s, object[] p) => Console.WriteLine(s));

		private void SetupServiceFactory() =>
			ServiceFactory.Setup(s => s.CreateOrganizationService(It.IsAny<Guid>()))
				.Returns(OrganizationService.Object);
	}
}