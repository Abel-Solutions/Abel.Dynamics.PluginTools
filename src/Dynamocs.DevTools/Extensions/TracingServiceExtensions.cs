using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using NSubstitute;

namespace Dynamocs.DevTools
{
	public static class TracingServiceExtensions
	{
		public static IEnumerable<string> GetTraces(this ITracingService tracingService) =>
			tracingService.ReceivedCalls().Select(c => c.GetArguments().First().ToString());
	}
}
