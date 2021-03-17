using System;
using Abel.Dynamics.PluginTools.Enums;
using Abel.Dynamics.PluginTools.Extensions;
using Microsoft.Xrm.Sdk;

namespace Abel.Dynamics.PluginTools
{
	public class PluginContext<TTarget>
	{
		public IOrganizationService OrganizationService { get; }

		public TTarget Target { get; }

		public string MessageName => _executionContext.MessageName;

		public string EntityName => _executionContext.PrimaryEntityName;

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

			Target = GetTarget();
		}

		public void Trace(string s) => _tracingService.Trace(s);

		public T GetInputParameter<T>(string name) =>
			_executionContext.InputParameters.ContainsKey(name)
				? (T)_executionContext.InputParameters[name]
				: default;

		private TTarget GetTarget() =>
			typeof(TTarget).IsSubclassOf(typeof(Entity)) ?
				GetInputParameter<Entity>("Target").ToEntity<TTarget>() :
				GetInputParameter<TTarget>("Target");
	}
}