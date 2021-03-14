using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dynamocs.DevTools.Extensions
{
	public static class TypeExtensions
	{
		public static TAttribute GetAttribute<TAttribute>(this Type type) =>
			GetAttributes<TAttribute>(type).FirstOrDefault();

		public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this Type type) =>
			type.GetCustomAttributes().Where(a => a.GetType() == typeof(TAttribute)).Cast<TAttribute>();
	}
}
