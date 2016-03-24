using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.OData.Batch;

using Microsoft.Owin;

using NuClear.CustomerIntelligence.Querying.Host;
using NuClear.CustomerIntelligence.Querying.Host.DI;
using NuClear.CustomerIntelligence.Querying.Host.Settings;
using NuClear.Querying.Http;

using Owin;

using Swashbuckle.Application;
using Swashbuckle.OData;

[assembly: OwinStartup(typeof(Startup))]

namespace NuClear.CustomerIntelligence.Querying.Host
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration
                             {
                                 DependencyResolver = new UnityDependencyResolver(Bootstrapper.ConfigureUnity(new WebApplicationSettings()))
                             };

            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            config.MapHttpAttributeRoutes();

            config.SetupClrTypes();
            config.RegisterODataControllers();

            config.EnableSwagger(c =>
                                     {
                                         c.SingleApiVersion("v1", "NuClear River API reference");
                                         c.CustomProvider(defaultProvider => new ODataSwaggerProvider(defaultProvider, c, config));
                                     })
                  .EnableSwaggerUi();


            var httpServer = new HttpServer(config);
            var batchHandler = new DefaultODataBatchHandler(httpServer);

            config.MapODataServiceRoutes(batchHandler);

            appBuilder.UseWebApi(httpServer);
        }
    }
}