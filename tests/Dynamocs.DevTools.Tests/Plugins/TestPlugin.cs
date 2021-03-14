using Dynamocs.DevTools.Attributes;
using Dynamocs.DevTools.Extensions;
using Dynamocs.DevTools.Tests.Models;
using Microsoft.Xrm.Sdk;

namespace Dynamocs.DevTools.Tests.Plugins
{
	[PluginStep("update", "account")]
	public class TestPlugin : Plugin
	{
		public override void Execute(PluginContext<Entity> context)
		{
			context.Trace("looool");

			var target = context.Target.ToEntity<Account>();
			var service = context.OrgService;

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