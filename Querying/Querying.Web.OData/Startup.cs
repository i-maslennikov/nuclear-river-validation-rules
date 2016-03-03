using System.Web.Http;
using System.Web.Http.Cors;

using Microsoft.Owin;

using NuClear.Querying.Web.OData;
using NuClear.Querying.Web.OData.DI;
using NuClear.Querying.Web.OData.Settings;

using Owin;

using Swashbuckle.Application;
using Swashbuckle.OData;

[assembly: OwinStartup(typeof(Startup))]

namespace NuClear.Querying.Web.OData
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
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            config.SetupClrTypes();
            config.RegisterODataControllers();

            config.EnableSwagger(c =>
                                     {
                                         c.SingleApiVersion("v1", "NuClear River API reference");
                                         c.CustomProvider(defaultProvider => new ODataSwaggerProvider(defaultProvider, c, config));
                                     })
                  .EnableSwaggerUi();


            var httpServer = new HttpServer(config);
            httpServer.MapODataServiceRoutes();

            appBuilder.UseWebApi(httpServer);
        }
    }
}