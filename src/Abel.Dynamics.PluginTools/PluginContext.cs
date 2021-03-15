using System;
using Abel.Dynamics.PluginTools.Enums;
using Abel.Dynamics.PluginTools.Extensions;
using Microsoft.Xrm.Sdk;

namespace Abel.Dynamics.PluginTools
{
	public class PluginContext<TEntity>
		where TEntity : Entity
	{
		public IOrganizationService OrganizationService { get; }

		public TEntity Target { get; }

		public string MessageName => _executionContext.MessageName;

		public string EntityName => Target.LogicalName;

		public Guid UserId => _executionContext.UserId;

		public int Depth => _executionContext.Depth;

		public PluginStage Stage => (PluginStage)_executionContext.Stage;

		private readonly IPluginExecutionContext _executionContext;

		private readonly ITracingService _tracingService;

		public PluginContext(IServiceProvider serviceProvider)
		{
			_tracingService = serviceProvider.GetService<ITracingService>();

			_executionContext = serviceProvider.GetService<IPluginExecutionContext>();

			OrganizationService = serviceProvider.GetService<IOrganizationServiceFactory>()
				.CreateOrganizationService(UserId);

			Target = GetInputParameter<Entity>("Target")
				.ToEntity<TEntity>();
		}

		public T GetInputParameter<T>(string name) =>
			_executionContext.InputParameters.ContainsKey(name)
				? (T)_executionContext.InputParameters[name]
				: default;

		public void Trace(string s) => _tracingService.Trace(s);
	}
}