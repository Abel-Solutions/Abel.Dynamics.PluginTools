using Abel.Dynamics.PluginTools.Attributes;
using Microsoft.Xrm.Sdk;

namespace Abel.Dynamics.PluginTools.Tests.Plugins
{
	[PluginStep("update", "account")]
	public class LateboundPlugin : Plugin
	{
		public override void Execute(PluginContext context)
		{
			var account = context.GetTarget<Entity>();

			account["name"] = "foo";

			context.OrganizationService.Update(account);
		}
	}
}