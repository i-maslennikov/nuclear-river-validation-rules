using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Filters;
using System.Web.OData;
using System.Web.OData.Query;

using NuClear.Metamodeling.Provider;
using NuClear.Querying.Metadata.Elements;

namespace NuClear.Querying.Http.Controllers
{
    public sealed class DynamicEnableQueryAttribute : EnableQueryAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var entityElementIdAttribute = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<EntityElementIdAttribute>().SingleOrDefault();
            if (entityElementIdAttribute == null)
            {
                throw new ArgumentException();
            }

            var dependencyResolver = actionExecutedContext.Request.GetConfiguration().DependencyResolver;
            var metadataProvider = (IMetadataProvider)dependencyResolver.GetService(typeof(IMetadataProvider));

            CustomizeEntity(metadataProvider, entityElementIdAttribute.Uri);
            Ieee754Compatible(actionExecutedContext);
            CachingHeader(actionExecutedContext);

            base.OnActionExecuted(actionExecutedContext);
        }

        private void CustomizeEntity(IMetadataProvider metadataProvider, Uri entityElementId)
        {
            EntityElement entityElement;
            if (!metadataProvider.TryGetMetadata(entityElementId, out entityElement))
            {
                throw new ArgumentException();
            }

            // ограничение для серверного paging
            PageSize = 100;

            // убрали ограничение, вернуть когда в ODATA появится $filter=Id in [1, 2, 3]
            MaxNodeCount = int.MaxValue;

            // запреты
            AllowedArithmeticOperators = AllowedArithmeticOperators.None;
            AllowedFunctions = AllowedFunctions.All | AllowedFunctions.Any;
        }

        // workaround for https://github.com/OData/WebApi/issues/53
        private static void Ieee754Compatible(HttpActionExecutedContext actionExecutedContext)
        {
            var compatibleParameter = actionExecutedContext.Request.Headers.Accept.Where(x => string.Equals(x.MediaType, "application/json", StringComparison.OrdinalIgnoreCase))
                   .SelectMany(x => x.Parameters)
                   .FirstOrDefault(x => string.Equals(x.Name, "IEEE754Compatible", StringComparison.OrdinalIgnoreCase));

            if (compatibleParameter != null)
            {
                actionExecutedContext.Response.Content.Headers.ContentType.Parameters.Add(compatibleParameter);
            }
        }

        // explicitly set Cache-Control: no-store
        private static void CachingHeader(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Request.Method == HttpMethod.Get)
            {
                actionExecutedContext.Response.Headers.CacheControl = new CacheControlHeaderValue
                {
                    NoStore = true
                };
            }
        }
    }
}