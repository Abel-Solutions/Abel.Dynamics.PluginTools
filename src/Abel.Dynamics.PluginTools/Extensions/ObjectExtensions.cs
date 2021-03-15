using System;
using NSubstitute;
using NSubstitute.Core;

namespace Abel.Dynamics.PluginTools.Extensions
{
	public static class ObjectExtensions
	{
		public static ConfiguredCall Returns<TArg, TReturn>(this object obj, Func<TArg, TReturn> func) =>
			obj.Returns(args => func(args.Arg<TArg>()));
	}
}
