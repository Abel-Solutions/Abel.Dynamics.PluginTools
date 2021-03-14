using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using Dynamocs.DevTools.Attributes;
using Dynamocs.DevTools.Extensions;

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

			context.Trace($"Message name: {context.MessageName}");
			context.Trace($"Entity name: {context.EntityName}");
			context.Trace($"User ID: {context.UserId}");
			context.Trace($"Stage: {context.Stage}");
			context.Trace($"Depth: {context.Depth}");

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
				throw new InvalidPluginExecutionException($"Plugin depth is at or above max: {context.Depth}");
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