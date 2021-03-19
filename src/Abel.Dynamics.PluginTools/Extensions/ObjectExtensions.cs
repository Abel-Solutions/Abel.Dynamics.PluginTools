using System;
using System.Reflection;
using NSubstitute;
using NSubstitute.Core;

namespace Abel.Dynamics.PluginTools.Extensions
{
	public static class ObjectExtensions
	{
		public static ConfiguredCall Returns<TArg, TResult>(this object obj, Func<TArg, TResult> func) =>
			obj.Returns(args => func(args.Arg<TArg>()));

		public static TResult InvokeMethod<TResult>(this object obj, string methodName, params object[] parameters) =>
			(TResult)obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(obj, parameters);

		public static void InvokeMethod(this object obj, string methodName, params object[] parameters) =>
			InvokeMethod<object>(obj, methodName, parameters);

		public static TResult GetValue<TResult>(this object obj, string name)
		{
			if (obj.GetType().GetField(name) is var field && field != null)
			{
				return (TResult)field.GetValue(obj);
			}

			if (obj.GetType().GetProperty(name) is var prop && prop != null)
			{
				return (TResult)prop.GetValue(obj);
			}

			return default;
		}

		public static void SetValue(this object obj, string name, object value)
		{
			if (obj.GetType().GetField(name) is var field && field != null)
			{
				field.SetValue(obj, value);
			}

			if (obj.GetType().GetProperty(name) is var prop && prop != null)
			{
				prop.SetValue(obj, value, null);
			}
		}
	}
}
