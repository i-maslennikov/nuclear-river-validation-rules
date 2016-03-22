using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Batch;
using System.Web.Http.Dispatcher;
using System.Web.OData.Extensions;
using System.Web.OData.Routing;

using Microsoft.OData.Edm;

using NuClear.Querying.Edm;
using NuClear.Querying.Edm.EF;
using NuClear.Querying.Http.Emit;

namespace NuClear.Querying.Http
{
    public static class HttpConfigurationExtensions
    {
        public static HttpConfiguration SetupClrTypes(this HttpConfiguration httpConfiguration)
        {
            var clrTypeBuilder = (IClrTypeBuilder)httpConfiguration.DependencyResolver.GetService(typeof(IClrTypeBuilder));
            clrTypeBuilder.Build();

            return httpConfiguration;
        }

        public static HttpConfiguration RegisterODataControllers(this HttpConfiguration httpConfiguration)
        {
            var dynamicControllersRegistrar = (DynamicControllersRegistrar)httpConfiguration.DependencyResolver.GetService(typeof(DynamicControllersRegistrar));
            dynamicControllersRegistrar.Register();

            return httpConfiguration;
        }

        public static HttpConfiguration MapODataServiceRoutes(this HttpConfiguration httpConfiguration, HttpBatchHandler batchHandler)
        {
            var edmModelBuilder = (IEdmModelBuilder)httpConfiguration.DependencyResolver.GetService(typeof(IEdmModelBuilder));
            var edmModels = edmModelBuilder.Build();
            foreach (var pair in edmModels)
            {
                var routePrefix = pair.Key.Segments.Last();
                MapRoute(httpConfiguration, pair.Value, routePrefix, batchHandler);
            }

            return httpConfiguration;
        }

        private static void MapRoute(HttpConfiguration httpConfiguration, IEdmModel edmModel, string routePrefix, HttpBatchHandler batchHandler)
        {
            // batch handler should be mapped first
            var routeTemplate = string.IsNullOrEmpty(routePrefix) ? ODataRouteConstants.Batch : routePrefix + '/' + ODataRouteConstants.Batch;
            httpConfiguration.Routes.MapHttpBatchRoute(routePrefix + "Batch", routeTemplate, batchHandler);

            var httpMessageHandler = HttpClientFactory.CreatePipeline(new HttpControllerDispatcher(httpConfiguration), new DelegatingHandler[0]);
            httpConfiguration.MapODataServiceRoute(routePrefix, routePrefix, edmModel, httpMessageHandler);
        }
    }
}