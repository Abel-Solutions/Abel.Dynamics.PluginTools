using Microsoft.Xrm.Sdk;
using System;

namespace Dynamocs.DevTools
{
	public abstract class PluginBase : IPlugin
	{
		public abstract void Execute(PluginContext context);

		public void Execute(IServiceProvider serviceProvider)
		{
			var context = new PluginContext(serviceProvider);

			context.Trace($"Start of {GetType().Name}");

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
				context.Trace($"End of {GetType().Name}");
			}
		}
	}
}