using System;

namespace Dynamocs.DevTools
{
	public class PluginStepAttribute : Attribute
	{
		public string MessageName { get; }

		public string EntityName { get; }

		public PluginStepAttribute(string messageName, string entityName)
		{
			MessageName = messageName;
			EntityName = entityName;
		}
	}
}
