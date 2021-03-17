using System;
using System.Collections.Generic;
using System.Linq;
using Abel.Dynamics.PluginTools.Attributes;
using Abel.Dynamics.PluginTools.Enums;
using Abel.Dynamics.PluginTools.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NSubstitute;

namespace Abel.Dynamics.PluginTools
{
	public class DynamicsContext
	{
		public ITracingService TracingService { get; } = Substitute.For<ITracingService>();

		public IPluginExecutionContext ExecutionContext { get; } = Substitute.For<IPluginExecutionContext>();

		public IOrganizationServiceFactory ServiceFactory { get; } = Substitute.For<IOrganizationServiceFactory>();

		public IServiceProvider ServiceProvider { get; } = Substitute.For<IServiceProvider>();

		public IOrganizationService OrganizationService { get; } = Substitute.For<IOrganizationService>();

		private readonly IDictionary<Guid, Entity> _records = new Dictionary<Guid, Entity>();

		private readonly IList<(string messageName, string entityName, Type pluginType)> _steps = new List<(string, string, Type)>();

		private readonly Guid _userId = Guid.NewGuid();

		public DynamicsContext()
		{
			SetupOrganizationService();

			SetupServiceFactory();

			SetupServiceProvider();

			ExecutionContext.Depth.Returns(-1); // todo ugly
		}

		public void ExecutePlugin<TPlugin>(object target, string messageName = "create", PluginStage? stage = PluginStage.PostOperation, Guid? userId = null)
			where TPlugin : IPlugin =>
			ExecutePlugin(typeof(TPlugin), target, messageName, stage, userId);

		public void Initialize(params Entity[] records) => records.ForEach(AddRecord);

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

		public void RegisterPlugin<TPlugin>() =>
			typeof(TPlugin).GetAttributes<PluginStepAttribute>()
				.ForEach(s => RegisterPlugin<TPlugin>(s.MessageName, s.EntityName));

		public void RegisterPlugin<TPlugin>(string messageName, string entityName) =>
			_steps.Add((messageName, entityName, typeof(TPlugin)));

		private void ExecutePlugin(Type pluginType, object target, string messageName = "create", PluginStage? stage = PluginStage.PostOperation, Guid? userId = null)
		{
			SetupExecutionContext(target, messageName, stage.Value, userId ?? _userId);

			((IPlugin)Activator.CreateInstance(pluginType)).Execute(ServiceProvider);
		}

		private void AddRecord(Entity entity)
		{
			entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
			_records[entity.Id] = entity;
		}

		private void SetupOrganizationService()
		{
			OrganizationService.Create(Arg.Do<Entity>(entity =>
			{
				AddRecord(entity);
				TriggerPlugins("create", entity);
			})).Returns((Entity entity) => entity.Id);

			OrganizationService.Update(Arg.Do<Entity>(entity =>
			{
				_records[entity.Id] = entity;
				TriggerPlugins("update", entity);
			}));

			OrganizationService.Delete(Arg.Any<string>(), Arg.Do<Guid>(id => _records.Remove(id)));

			OrganizationService.Retrieve(Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<ColumnSet>())
				.Returns((Guid id) => _records[id]);

			OrganizationService.RetrieveMultiple(Arg.Any<QueryByAttribute>())
				.Returns((QueryByAttribute query) => _records
					.Select(r => r.Value)
					.Where(entity => entity.LogicalName == query.EntityName &&
									 !query.Attributes.Where((t, i) => !entity.Contains(t) || entity[t] != query.Values[i]).Any())
					.ToEntityCollection());
		}

		private void TriggerPlugins(string messageName, Entity entity) => _steps
			.Where(step => string.Equals(step.messageName, messageName, StringComparison.InvariantCultureIgnoreCase) &&
						   string.Equals(step.entityName, entity.LogicalName, StringComparison.InvariantCultureIgnoreCase))
			.Select(s => s.pluginType)
			.ForEach(pluginType => ExecutePlugin(pluginType, entity, messageName));

		private void SetupServiceProvider()
		{
			ServiceProvider.GetService(typeof(ITracingService))
				.Returns(TracingService);

			ServiceProvider.GetService(typeof(IPluginExecutionContext))
				.Returns(ExecutionContext);

			ServiceProvider.GetService(typeof(IOrganizationServiceFactory))
				.Returns(ServiceFactory);
		}

		private void SetupExecutionContext(object target, string messageName, PluginStage stage, Guid userId)
		{
			ExecutionContext.InputParameters.Returns(new ParameterCollection { { "Target", target } });

			ExecutionContext.PrimaryEntityName.Returns(target.GetValue<string>("LogicalName"));

			ExecutionContext.PrimaryEntityId.Returns(target.GetValue<Guid>("Id"));

			ExecutionContext.MessageName.Returns(messageName);

			ExecutionContext.Stage.Returns((int)stage);

			ExecutionContext.UserId.Returns(userId);

			ExecutionContext.Depth.Returns(ExecutionContext.Depth + 1);
		}

		private void SetupServiceFactory() =>
			ServiceFactory.CreateOrganizationService(Arg.Any<Guid>())
				.Returns(OrganizationService);
	}
}