using Dynamocs.DevTools.Extensions;
using Dynamocs.DevTools.Tests.Models;
using Microsoft.Xrm.Sdk;

namespace Dynamocs.DevTools.Tests.Plugins
{
	public class NoStepPlugin : Plugin
	{
		public override void Execute(PluginContext<Entity> context)
		{
			context.Trace("looool");

			var target = context.Target.ToEntity<Account>();
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