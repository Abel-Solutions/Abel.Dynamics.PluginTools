using Abel.Dynamics.PluginTools.Tests.Models;

namespace Abel.Dynamics.PluginTools.Tests.Plugins
{
	public class NoStepsPlugin : Plugin
	{
		public override void Execute(PluginContext context)
		{
			var account = context.GetTarget<Account>();

			account.Name = "foo";

			context.OrganizationService.Update(account);
		}
	}
}