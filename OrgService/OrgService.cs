using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace OrgServiz
{
	public class OrgService : IOrgService
	{
		private readonly IOrganizationService _orgService;

		public OrgService(IOrganizationService orgService) => _orgService = orgService;

		public TEntity RetrieveByAttribute<TEntity>(string key, object value, ColumnSet columnSet = null)
			where TEntity : Entity =>
			RetrieveMultipleByAttribute<TEntity>(key, value, columnSet)
				.FirstOrDefault();

		public TEntity RetrieveByAttributes<TEntity>(Dictionary<string, object> attributes, ColumnSet columnSet = null)
			where TEntity : Entity =>
			RetrieveMultipleByAttributes<TEntity>(attributes, columnSet)
				.FirstOrDefault();

		public IEnumerable<TEntity> RetrieveMultipleByAttribute<TEntity>(string key, object value, ColumnSet columnSet = null)
			where TEntity : Entity =>
			RetrieveMultipleByAttributes<TEntity>(new Dictionary<string, object> { { key, value } }, columnSet);

		public IEnumerable<TEntity> RetrieveMultipleByAttributes<TEntity>(Dictionary<string, object> attributes, ColumnSet columnSet = null)
			where TEntity : Entity =>
			RetrieveMultipleByAttributes(Activator.CreateInstance<TEntity>().LogicalName, attributes, columnSet)
				.Select(e => e.ToEntity<TEntity>());

		public Entity RetrieveByAttribute(string entityName, string key, object value, ColumnSet columnSet = null) =>
			RetrieveMultipleByAttribute(entityName, key, value, columnSet)
				.FirstOrDefault();

		public Entity RetrieveByAttributes(string entityName, Dictionary<string, object> attributes, ColumnSet columnSet = null) =>
			RetrieveMultipleByAttributes(entityName, attributes, columnSet)
				.FirstOrDefault();

		public IEnumerable<Entity> RetrieveMultipleByAttribute(string entityName, string key, object value, ColumnSet columnSet = null) =>
			RetrieveMultipleByAttributes(entityName, new Dictionary<string, object> { { key, value } }, columnSet);

		public IEnumerable<Entity> RetrieveMultipleByAttributes(string entityName, Dictionary<string, object> attributes, ColumnSet columnSet = null)
		{
			var query = new QueryByAttribute(entityName);
			foreach (var a in attributes)
			{
				query.AddAttributeValue(a.Key, a.Value);
			}

			query.ColumnSet = columnSet;
			return _orgService.RetrieveMultiple(query).Entities;
		}

		public TEntity Retrieve<TEntity>(Guid id, ColumnSet columnSet = null)
			where TEntity : Entity =>
			Retrieve(Activator.CreateInstance<TEntity>().LogicalName, id, columnSet)
				.ToEntity<TEntity>();

		public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet = null) =>
			_orgService.Retrieve(entityName, id, columnSet);

		public EntityCollection RetrieveMultiple(QueryBase query) => _orgService.RetrieveMultiple(query);

		public Guid Create(Entity entity) => _orgService.Create(entity);

		public void Update(Entity entity) => _orgService.Update(entity);

		public void Delete(string entityName, Guid id) => _orgService.Delete(entityName, id);

		public OrganizationResponse Execute(OrganizationRequest request) => _orgService.Execute(request);

		public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities) =>
			_orgService.Associate(entityName, entityId, relationship, relatedEntities);

		public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities) =>
			_orgService.Disassociate(entityName, entityId, relationship, relatedEntities);
	}
}