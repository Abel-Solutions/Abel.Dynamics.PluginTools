using Microsoft.Xrm.Sdk;
using System;

namespace Dynamocs.DevTools
{
	public class PluginContext
	{
		public IOrgService OrgService { get; }

		public Entity Target { get; }

		private readonly ITracingService _tracingService;

		public PluginContext(IServiceProvider serviceProvider)
		{
			_tracingService = GetService<ITracingService>(serviceProvider);

			var context = GetService<IPluginExecutionContext>(serviceProvider);

			Target = context.InputParameters.ContainsKey("Target")
				? (Entity)context.InputParameters["Target"]
				: null;

			var serviceFactory = GetService<IOrganizationServiceFactory>(serviceProvider);

			OrgService = new OrgService(serviceFactory.CreateOrganizationService(context.UserId));
		}

		public TEntity GetTarget<TEntity>()
			where TEntity : Entity =>
			Target.ToEntity<TEntity>();

		public void Trace(string s) => _tracingService.Trace(s);

		private static TService GetService<TService>(IServiceProvider serviceProvider) => (TService)serviceProvider.GetService(typeof(TService));
	}
}