using System;
using Microsoft.Xrm.Sdk;

namespace PluginBase
{
    public abstract class PluginBase : IPlugin
    {
        public abstract void Execute(PluginContext context);

        public void Execute(IServiceProvider serviceProvider) => Execute(new PluginContext(serviceProvider));
    }
}