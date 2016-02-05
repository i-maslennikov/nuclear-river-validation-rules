using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.OData.Batch;
using System.Web.OData.Extensions;
using System.Web.OData.Routing;

using Microsoft.OData.Edm;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.AdvancedSearch.Common.Metadata.Identities;
using NuClear.Metamodeling.Provider;
using NuClear.Querying.Web.OData.DI;
using NuClear.Querying.Web.OData.DynamicControllers;

namespace NuClear.Querying.Web.OData
{
    internal sealed class ODataModelRegistrator
    {
        private static readonly ConfigureHttpRequest ConfigureHttpRequest = Bootstrapper.ConfigureHttpRequest;

        private readonly IMetadataProvider _metadataProvider;
        private readonly DynamicControllersRegistrar _dynamicControllersRegistrar;
        private readonly EdmModelWithClrTypesBuilder _edmModelWithClrTypesBuilder;

        public ODataModelRegistrator(IMetadataProvider metadataProvider, DynamicControllersRegistrar dynamicControllersRegistrar, EdmModelWithClrTypesBuilder edmModelWithClrTypesBuilder)
        {
            _metadataProvider = metadataProvider;
            _dynamicControllersRegistrar = dynamicControllersRegistrar;
            _edmModelWithClrTypesBuilder = edmModelWithClrTypesBuilder;
        }

        public void RegisterModels(HttpServer httpServer)
        {
            MetadataSet metadataSet;
            if (!_metadataProvider.TryGetMetadata<QueryingMetadataIdentity>(out metadataSet))
            {
                return;
            }

            var contexts = metadataSet.Metadata.Values.OfType<BoundedContextElement>();
            foreach (var context in contexts)
            {
                var contextId = context.Identity.Id;
                var edmModel = _edmModelWithClrTypesBuilder.Build(contextId);

                var routePrefix = contextId.Segments.Last();
                MapRoute(routePrefix, edmModel, httpServer, ConfigureHttpRequest);

                _dynamicControllersRegistrar.RegisterDynamicControllers(contextId);
            }
        }

        private static void MapRoute(string routePrefix, IEdmModel edmModel, HttpServer httpServer, ConfigureHttpRequest configureHttpRequest)
        {
            // batch handler should be mapped first
            var batchHandler = new DefaultODataBatchHandler(httpServer) { ODataRouteName = routePrefix };
            var routeTemplate = string.IsNullOrEmpty(routePrefix) ? ODataRouteConstants.Batch : (routePrefix + '/' + ODataRouteConstants.Batch);

            var config = httpServer.Configuration;
            config.Routes.MapHttpBatchRoute(routePrefix + "Batch", routeTemplate, batchHandler);

            var additionalHandlers = new[]
            {
                new UnityResolver.PerRequestResolver(configureHttpRequest)
            };
            var handler = HttpClientFactory.CreatePipeline(new HttpControllerDispatcher(config), additionalHandlers);
            config.MapODataServiceRoute(routePrefix, routePrefix, edmModel, handler);
        }
    }
}