using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.OData.Batch;
using System.Web.OData.Extensions;
using System.Web.OData.Routing;

using Microsoft.OData.Edm;

using NuClear.Querying.Edm;

namespace NuClear.Querying.Web.OData
{
    public static class HttpServerExtensions
    {
        public static HttpServer MapODataServiceRoutes(this HttpServer httpServer)
        {
            var edmModelBuilder = (IEdmModelBuilder)httpServer.Configuration.DependencyResolver.GetService(typeof(IEdmModelBuilder));
            var edmModels = edmModelBuilder.Build();
            foreach (var pair in edmModels)
            {
                var routePrefix = pair.Key.Segments.Last();
                MapRoute(routePrefix, pair.Value, httpServer);
            }

            return httpServer;
        }

        private static void MapRoute(string routePrefix, IEdmModel edmModel, HttpServer httpServer)
        {
            // batch handler should be mapped first
            var batchHandler = new DefaultODataBatchHandler(httpServer) { ODataRouteName = routePrefix };
            var routeTemplate = string.IsNullOrEmpty(routePrefix) ? ODataRouteConstants.Batch : routePrefix + '/' + ODataRouteConstants.Batch;

            var config = httpServer.Configuration;
            config.Routes.MapHttpBatchRoute(routePrefix + "Batch", routeTemplate, batchHandler);

            var handler = HttpClientFactory.CreatePipeline(new HttpControllerDispatcher(config), new DelegatingHandler[0]);
            config.MapODataServiceRoute(routePrefix, routePrefix, edmModel, handler);
        }
    }
}