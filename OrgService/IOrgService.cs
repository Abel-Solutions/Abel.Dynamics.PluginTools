using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace OrgServiz
{
	public interface IOrgService : IOrganizationService
	{
		TEntity RetrieveByAttribute<TEntity>(string key, object value, ColumnSet columnSet = null)
			where TEntity : Entity;

		TEntity RetrieveByAttributes<TEntity>(Dictionary<string, object> attributes, ColumnSet columnSet = null)
			where TEntity : Entity;

		IEnumerable<TEntity> RetrieveMultipleByAttribute<TEntity>(string key, object value, ColumnSet columnSet = null)
			where TEntity : Entity;

		IEnumerable<TEntity> RetrieveMultipleByAttributes<TEntity>(Dictionary<string, object> attributes, ColumnSet columnSet = null)
			where TEntity : Entity;

		Entity RetrieveByAttribute(string entityName, string key, object value, ColumnSet columnSet = null);

		Entity RetrieveByAttributes(string entityName, Dictionary<string, object> attributes, ColumnSet columnSet = null);

		IEnumerable<Entity> RetrieveMultipleByAttribute(string entityName, string key, object value, ColumnSet columnSet = null);

		IEnumerable<Entity> RetrieveMultipleByAttributes(string entityName, Dictionary<string, object> attributes, ColumnSet columnSet = null);

		TEntity Retrieve<TEntity>(Guid id, ColumnSet columnSet = null)
			where TEntity : Entity;
	}
}