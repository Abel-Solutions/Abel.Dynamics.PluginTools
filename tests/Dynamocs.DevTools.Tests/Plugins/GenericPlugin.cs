using Abel.Dynamics.PluginTools.Attributes;
using Abel.Dynamics.PluginTools.Extensions;
using Abel.Dynamics.PluginTools.Tests.Models;

namespace Abel.Dynamics.PluginTools.Tests.Plugins
{
	[PluginStep("update", "account")]
	public class GenericPlugin : Plugin<Account>
	{
		public override void Execute(PluginContext<Account> context)
		{
			context.Trace("looool");

			var target = context.Target;
			var service = context.OrganizationService;

			target.Name = "foo";

			var lol = new Account
			{
				Name = "bar"
			};
			var id = service.Create(lol);

			lol = service.RetrieveByAttribute<Account>("name", "bar");

			lol.Id = id;
			lol.Name = "bar2";
			service.Update(lol);
		}
	}
}