using System.Web.Http;

using NuClear.Querying.Edm.EF;
using NuClear.Querying.Web.OData.DynamicControllers;

namespace NuClear.Querying.Web.OData
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
    }
}