using Microsoft.Xrm.Sdk;
using System;

namespace Dynamocs.DevTools
{
	public abstract class PluginBase : IPlugin
	{
		public abstract void Execute(PluginContext context);

		public void Execute(IServiceProvider serviceProvider) => Execute(new PluginContext(serviceProvider));
	}
}