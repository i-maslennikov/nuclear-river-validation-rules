using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;

using NuClear.Querying.Web.OData.DataAccess;
using NuClear.Storage.API.Readings;

namespace NuClear.Querying.Web.OData.Controllers
{

    public abstract class GenericODataController<TEntity> : ODataController where TEntity : class
    {
        private readonly IQuery _query;

        protected GenericODataController(IQuery query)
        {
            _query = query;
        }

        [DynamicEnableQuery]
        public IHttpActionResult Get(ODataQueryOptions<TEntity> queryOptions)
        {
            var entities = _query.For<TEntity>();
            return Ok(entities);
        }

        [DynamicEnableQuery]
        public IHttpActionResult Get([FromODataUri] long key)
        {
            var entities = _query.For(Specs.Find.ById<TEntity>(key));
            return Ok(SingleResult.Create(entities));
        }

        protected IHttpActionResult GetContainedEntity<TContainedEntity>(long key, string propertyName) where TContainedEntity : class
        {
            var entities = _query.For(Specs.Find.ById<TEntity>(key)).SelectManyProperties<TEntity, TContainedEntity>(propertyName);
            return Ok(entities);
        }
    }
}