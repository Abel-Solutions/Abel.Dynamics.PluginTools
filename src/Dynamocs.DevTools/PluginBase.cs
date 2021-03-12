using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace Dynamocs.DevTools
{
	public abstract class PluginBase : IPlugin
	{
		public string PluginName => GetType().Name;

		public abstract void Execute(PluginContext context);

		public void Execute(IServiceProvider serviceProvider)
		{
			var context = new PluginContext(serviceProvider);

			context.Trace($"Start of {PluginName}");

			try
			{
				ValidateTrigger(context);

				Execute(context);
			}
			catch (InvalidPluginExecutionException ex)
			{
				context.Trace(ex.ToString());
				throw;
			}
			catch (Exception ex)
			{
				context.Trace(ex.ToString());
				throw new InvalidPluginExecutionException(ex.Message, ex);
			}
			finally
			{
				context.Trace($"End of {PluginName}");
			}
		}

		private void ValidateTrigger(PluginContext context)
		{
			if (GetType().GetAttributes<PluginStepAttribute>()
				.All(step => step.MessageName != context.MessageName || step.EntityName != context.EntityName))
			{
				throw new InvalidPluginExecutionException(
					$"Error: {PluginName} does not have any {nameof(PluginStepAttribute)} " +
					$"with MessageName {context.MessageName} and EntityName {context.EntityName}");
			}
		}
	}
}