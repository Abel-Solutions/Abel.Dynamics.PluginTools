using System;
using Microsoft.Xrm.Sdk;

namespace Abel.Dynamics.PluginTools.Extensions
{
	public static class EntityExtensions
	{
		public static TEntity ToEntity<TEntity>(this Entity entity)
		{
			var copy = Activator.CreateInstance<TEntity>();
			entity.InvokeMethod("ShallowCopyTo", copy);
			return copy;
		}
	}
}
