using Dynamocs.DevTools.Attributes;
using Dynamocs.DevTools.Extensions;
using Dynamocs.DevTools.Tests.Models;

namespace Dynamocs.DevTools.Tests.Plugins
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