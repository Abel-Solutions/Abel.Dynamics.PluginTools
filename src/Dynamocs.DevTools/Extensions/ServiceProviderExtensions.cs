using System;

namespace Dynamocs.DevTools.Extensions
{
	public static class ServiceProviderExtensions
	{
		public static TService GetService<TService>(this IServiceProvider serviceProvider) =>
			(TService)serviceProvider.GetService(typeof(TService));
	}
}
