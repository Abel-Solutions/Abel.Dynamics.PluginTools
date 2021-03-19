using System;
using System.Linq;
using Abel.Dynamics.PluginTools.Attributes;
using Abel.Dynamics.PluginTools.Extensions;
using Microsoft.Xrm.Sdk;

namespace Abel.Dynamics.PluginTools
{
	public abstract class Plugin : Plugin<Entity>
	{
	}

	public abstract class Plugin<TEntity> : IPlugin
		where TEntity : Entity
	{
		public const int MaxDepth = 5; // todo config

		public string PluginName => GetType().Name;

		public abstract void Execute(PluginContext<TEntity> context);

		public void Execute(IServiceProvider serviceProvider)
		{
			var context = new PluginContext<TEntity>(serviceProvider);

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

		private static void ValidateDepth(PluginContext<TEntity> context)
		{
			if (context.Depth >= MaxDepth)
			{
				throw new InvalidPluginExecutionException($"Plugin depth is at or above max: {context.Depth}");
			}
		}

		private void ValidateTrigger(PluginContext<TEntity> context)
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
	}
}