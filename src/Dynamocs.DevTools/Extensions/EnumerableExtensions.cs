using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace Abel.Dynamics.PluginTools.Extensions
{
	public static class EnumerableExtensions
	{
		public static EntityCollection ToEntityCollection(this IEnumerable<Entity> enumerable) =>
			new EntityCollection(enumerable.ToList());

		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) =>
			enumerable.ToList().ForEach(action);
	}
}
