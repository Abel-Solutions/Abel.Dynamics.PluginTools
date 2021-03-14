using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Dynamocs.DevTools.Extensions
{
	public static class OrganizationServiceExtensions
	{
		public static TEntity RetrieveByAttribute<TEntity>(this IOrganizationService orgService, string key, object value, ColumnSet columnSet = null)
			where TEntity : Entity =>
			orgService.RetrieveMultipleByAttribute<TEntity>(key, value, columnSet)
				.FirstOrDefault();

		public static TEntity RetrieveByAttributes<TEntity>(this IOrganizationService orgService, Dictionary<string, object> attributes, ColumnSet columnSet = null)
			where TEntity : Entity =>
			orgService.RetrieveMultipleByAttributes<TEntity>(attributes, columnSet)
				.FirstOrDefault();

		public static IEnumerable<TEntity> RetrieveMultipleByAttribute<TEntity>(this IOrganizationService orgService, string key, object value, ColumnSet columnSet = null)
			where TEntity : Entity =>
			orgService.RetrieveMultipleByAttributes<TEntity>(new Dictionary<string, object> { { key, value } }, columnSet);

		public static IEnumerable<TEntity> RetrieveMultipleByAttributes<TEntity>(this IOrganizationService orgService, Dictionary<string, object> attributes, ColumnSet columnSet = null)
			where TEntity : Entity =>
			orgService.RetrieveMultipleByAttributes(Activator.CreateInstance<TEntity>().LogicalName, attributes, columnSet)
				.Select(e => e.ToEntity<TEntity>());

		public static Entity RetrieveByAttribute(this IOrganizationService orgService, string entityName, string key, object value, ColumnSet columnSet = null) =>
			orgService.RetrieveMultipleByAttribute(entityName, key, value, columnSet)
				.FirstOrDefault();

		public static Entity RetrieveByAttributes(this IOrganizationService orgService, string entityName, Dictionary<string, object> attributes, ColumnSet columnSet = null) =>
			orgService.RetrieveMultipleByAttributes(entityName, attributes, columnSet)
				.FirstOrDefault();

		public static IEnumerable<Entity> RetrieveMultipleByAttribute(this IOrganizationService orgService, string entityName, string key, object value, ColumnSet columnSet = null) =>
			orgService.RetrieveMultipleByAttributes(entityName, new Dictionary<string, object> { { key, value } }, columnSet);

		public static IEnumerable<Entity> RetrieveMultipleByAttributes(this IOrganizationService orgService, string entityName, Dictionary<string, object> attributes, ColumnSet columnSet = null)
		{
			var query = new QueryByAttribute(entityName);
			foreach (var a in attributes)
			{
				query.AddAttributeValue(a.Key, a.Value);
			}

			query.ColumnSet = columnSet ?? new ColumnSet(true);
			return orgService.RetrieveMultiple(query).Entities;
		}

		public static TEntity Retrieve<TEntity>(this IOrganizationService orgService, Guid id, ColumnSet columnSet = null)
			where TEntity : Entity =>
			orgService.Retrieve(Activator.CreateInstance<TEntity>().LogicalName, id, columnSet ?? new ColumnSet(true))
				.ToEntity<TEntity>();

		public static Entity Retrieve<TEntity>(this IOrganizationService orgService, EntityReference entityReference, ColumnSet columnSet = null)
			where TEntity : Entity =>
			Retrieve(orgService, entityReference, columnSet).ToEntity<TEntity>();

		public static Entity Retrieve(this IOrganizationService orgService, EntityReference entityReference, ColumnSet columnSet = null) =>
			orgService.Retrieve(entityReference.LogicalName, entityReference.Id, columnSet ?? new ColumnSet(true));

		public static Entity Retrieve(this IOrganizationService orgService, string entityName, Guid id) =>
			orgService.Retrieve(entityName, id, new ColumnSet(true));
	}
}