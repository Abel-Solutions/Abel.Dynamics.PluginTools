using Abel.Dynamics.PluginTools.Attributes;
using Microsoft.Xrm.Sdk;

namespace Abel.Dynamics.PluginTools.Tests.Plugins
{
	[PluginStep("update", "account")]
	public class NonGenericPlugin : Plugin
	{
		public override void Execute(PluginContext<Entity> context)
		{
			var account = context.Target;

			account["name"] = "foo";

			context.OrganizationService.Update(account);
		}
	}
}