using System;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;

namespace Dynamocs.DevTools
{
	public static class EnumerableExtensions
	{
		public static EntityCollection ToEntityCollection(this IEnumerable<Entity> enumerable) =>
			new EntityCollection(enumerable.ToList());

		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) =>
			enumerable.ToList().ForEach(action);
	}
}
