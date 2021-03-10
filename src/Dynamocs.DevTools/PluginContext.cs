using Microsoft.Xrm.Sdk;
using System;

namespace Dynamocs.DevTools
{
	public class PluginContext
	{
		public IOrganizationService OrgService { get; }

		public Entity Target { get; }

		private readonly ITracingService _tracingService;

		public PluginContext(IServiceProvider serviceProvider)
		{
			_tracingService = serviceProvider.GetService<ITracingService>();

			var context = serviceProvider.GetService<IPluginExecutionContext>();

			Target = context.InputParameters.ContainsKey("Target")
				? (Entity)context.InputParameters["Target"]
				: null;

			var serviceFactory = serviceProvider.GetService<IOrganizationServiceFactory>();

			OrgService = serviceFactory.CreateOrganizationService(context.UserId);
		}

		public TEntity GetTarget<TEntity>()
			where TEntity : Entity =>
			Target.ToEntity<TEntity>();

		public void Trace(string s) => _tracingService.Trace(s);
	}
}