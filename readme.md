# Dynamocs

Dynamocs is a suite of tools for developing and testing Dynamics plug-ins.

## Base class for plug-ins

The base class removes boilerplate code. Example plugin:

~~~
[PluginStep("update", "account")]
public class GenericPlugin : Plugin<Account>
{
	public override void Execute(PluginContext<Account> context)
	{
		context.Target.Name = "foo";
		context.OrganizationService.Update(target);
	}
}
~~~

The optional PluginStep attribute validates the trigger when running the plugin.

The optional generic type of the Plugin class makes sure the Target is of the correct Entity type.

OrganizationService and other useful tools are available on the context object.

The base class logs (traces) useful information and stops plug-ins from calling themselves in infinite loops. 

## Extension methods for IOrganizationService

Lots of generic overloads for querying Dynamics. Examples:

~~~
var account = orgService.RetrieveByAttribute<Account>("name", "Lol");

var accounts = orgService.RetrieveMultipleByAttributes<Account>(new Dictionary<string, object>
{
	{ "firstname", "foo" },
	{ "lastname", "bar" }
});
~~~

All overloads have an optional ColumnSet parameter. If it is skipped like above, all attributes are loaded. 

## Test plugins with a fake version of Dynamics

The Dynamocs class works like a fake in-memory version of Dynamics, and aids in executing plug-ins. Changes done to Dynamics via OrganizationService persist throughout the test.

Execute a plug-in and verify state and method calls:

~~~
var dynamocs = new Dynamocs();
dynamocs.Initialize(new Account
{
	Name = "foo"
});

dynamocs.ExecutePlugin<ChangeNameToBarPlugin>(account);

Assert.Equal("bar", dynamocs.GetRecord<Account>().Name);
dynamocs.OrganizationService.Received().Update(Arg.Is<Account>(a => a.Name == "bar"));
~~~

The fake Dynamics is built with NSubstitute, hence ```Received()``` and other NSubstitute extensions can be used to verify method calls.

## Trigger plug-ins by changes in Dynamics

Instead of executing plug-ins directly, they can be triggered by changes in (fake) Dynamics:

~~~
dynamocs.RegisterPlugin<TestPlugin>("update", "account");
dynamocs.OrganizationService.Update(account);
~~~

If the plugin has PluginStep attributes, they can be registered automatically:

~~~
dynamocs.RegisterPlugin<TestPlugin>();
dynamocs.OrganizationService.Update(account);
~~~