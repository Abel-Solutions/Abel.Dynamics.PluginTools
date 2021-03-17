using Abel.Dynamics.PluginTools.Attributes;
using Abel.Dynamics.PluginTools.Extensions;
using Abel.Dynamics.PluginTools.Tests.Models;
using Microsoft.Xrm.Sdk;

namespace Abel.Dynamics.PluginTools.Tests.Plugins
{
	[PluginStep("woop", "account")]
	public class EntityReferencePlugin : Plugin<EntityReference>
	{
		public override void Execute(PluginContext<EntityReference> context)
		{
			var accountRef = context.Target;

			var account = context.OrganizationService.Retrieve<Account>(accountRef);

			account.Name = "foo";

			context.OrganizationService.Update(account);
		}
	}
}