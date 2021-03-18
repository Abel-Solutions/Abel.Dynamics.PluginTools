using Abel.Dynamics.PluginTools.Attributes;
using Abel.Dynamics.PluginTools.Tests.Models;

namespace Abel.Dynamics.PluginTools.Tests.Plugins
{
	[PluginStep("create", "account")]
	[PluginStep("update", "account")]
	public class EarlyboundPlugin : Plugin
	{
		public override void Execute(PluginContext context)
		{
			var account = context.GetTarget<Account>();

			account.Name = "foo";

			context.OrganizationService.Update(account);
		}
	}
}