using Microsoft.Xrm.Sdk.Query;

namespace Dynamocs.DevTools
{
	public static class QueryBaseExtensions
	{
		public static string EntityName(this QueryBase query) =>
			query.GetValue<string>("EntityName");
	}
}
