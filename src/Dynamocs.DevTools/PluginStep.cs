using System;

namespace Dynamocs.DevTools
{
	public class PluginStepAttribute : Attribute
	{
		public string EntityName { get; }

		public string MessageName { get; }

		public PluginStepAttribute(string entityName, string messageName)
		{
			EntityName = entityName;
			MessageName = messageName;
		}
	}
}
