using System.Web.Http;

using NuClear.ValidationRules.Querying.Host.Properties;

namespace NuClear.ValidationRules.Querying.Host
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new Int64ToStringConverter());
            config.MessageHandlers.Add(new LocalizationMessageHandler(typeof(Resources)));
        }
    }
}
