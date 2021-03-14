namespace Dynamocs.DevTools.Tests
{
	public class TestPlugin : PluginBase
	{
		public override void Execute(PluginContext context)
		{
			context.Trace("looool");

			var target = context.GetTarget<Account>();
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