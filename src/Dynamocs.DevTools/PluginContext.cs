using Microsoft.Xrm.Sdk;
using System;
using Dynamocs.DevTools.Enums;
using Dynamocs.DevTools.Extensions;

namespace Dynamocs.DevTools
{
	public class PluginContext<TEntity>
		where TEntity : Entity
	{
		public IOrganizationService OrgService { get; }

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

			Target = _executionContext.InputParameters.ContainsKey("Target")
				? ((Entity)_executionContext.InputParameters["Target"]).ToEntity<TEntity>()
				: null;

			var serviceFactory = serviceProvider.GetService<IOrganizationServiceFactory>();

			OrgService = serviceFactory.CreateOrganizationService(UserId);
		}

		public void Trace(string s) => _tracingService.Trace(s);
	}
}