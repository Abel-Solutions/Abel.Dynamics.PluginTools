using System;
using System.Linq;
using Abel.Dynamics.PluginTools.Attributes;
using Abel.Dynamics.PluginTools.Extensions;
using Microsoft.Xrm.Sdk;

namespace Abel.Dynamics.PluginTools
{
	public abstract class Plugin : IPlugin
	{
		public const int MaxDepth = 5; // todo config

		public string PluginName => GetType().Name;

		public abstract void Execute(PluginContext context);

		public void Execute(IServiceProvider serviceProvider)
		{
			var context = new PluginContext(serviceProvider);

			TraceMetadata(context);

			ValidateDepth(context);

			ValidateTrigger(context);

			TryToExecute(context);
		}

		private void TraceMetadata(PluginContext context)
		{
			context.Trace($"Start of {PluginName}");
			context.Trace($"Message name: {context.MessageName}");
			context.Trace($"Entity name: {context.EntityName}");
			context.Trace($"User ID: {context.UserId}");
			context.Trace($"Stage: {context.Stage}");
			context.Trace($"Depth: {context.Depth}");
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
			if (GetType().GetAttributes<PluginStepAttribute>() is var steps &&
				steps.Any() &&
				!steps.Any(step => string.Equals(step.MessageName, context.MessageName, StringComparison.InvariantCultureIgnoreCase) &&
								   string.Equals(step.EntityName, context.EntityName, StringComparison.InvariantCultureIgnoreCase)))
			{
				throw new InvalidPluginExecutionException(
					$"Error: {PluginName} does not have any {nameof(PluginStepAttribute)} " +
					$"with MessageName {context.MessageName} and EntityName {context.EntityName}");
			}
		}

		private void TryToExecute(PluginContext context)
		{
			try
			{
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
	}
}