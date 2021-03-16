using Abel.Dynamics.PluginTools.Tests.Models;

namespace Abel.Dynamics.PluginTools.Tests.Plugins
{
	public class NoStepsPlugin : Plugin<Account>
	{
		public override void Execute(PluginContext<Account> context)
		{
			var account = context.Target;

			account.Name = "foo";

			context.OrganizationService.Update(account);
		}
	}
}