﻿using Abel.Dynamics.PluginTools.Attributes;
using Abel.Dynamics.PluginTools.Tests.Models;

namespace Abel.Dynamics.PluginTools.Tests.Plugins
{
	[PluginStep("create", "account")]
	[PluginStep("update", "account")]
	public class GenericPlugin : Plugin<Account>
	{
		public override void Execute(PluginContext<Account> context)
		{
			var account = context.Target;

			account.Name = "foo";

			context.OrganizationService.Update(account);
		}
	}
}