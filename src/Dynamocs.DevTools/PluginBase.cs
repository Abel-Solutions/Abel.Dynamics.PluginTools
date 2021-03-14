using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace Dynamocs.DevTools
{
	public abstract class PluginBase : IPlugin
	{
		public const int MaxDepth = 5; // todo config

		public string PluginName => GetType().Name;

		public abstract void Execute(PluginContext context);

		public void Execute(IServiceProvider serviceProvider)
		{
			var context = new PluginContext(serviceProvider);

			context.Trace($"Start of {PluginName}");

			try
			{
				ValidateDepth(context);

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

		private static void ValidateDepth(PluginContext context)
		{
			if (context.Depth >= MaxDepth)
			{
				throw new InvalidPluginExecutionException($"Plugin depth is at or above max: {context.Depth} (max is {MaxDepth})");
			}
		}

		private void ValidateTrigger(PluginContext context)
		{
			if (!GetType().GetAttributes<PluginStepAttribute>()
				.Any(step => string.Equals(step.MessageName, context.MessageName, StringComparison.InvariantCultureIgnoreCase) &&
							 string.Equals(step.EntityName, context.EntityName, StringComparison.InvariantCultureIgnoreCase)))
			{
				throw new InvalidPluginExecutionException(
					$"Error: {PluginName} does not have any {nameof(PluginStepAttribute)} " +
					$"with MessageName {context.MessageName} and EntityName {context.EntityName}");
			}
		}
	}
}