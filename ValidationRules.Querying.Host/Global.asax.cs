using System.Web.Http;

namespace NuClear.ValidationRules.Querying.Host
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(UnityConfig.Register);
            LibRdKafkaLoader.LoadLibrary();
        }

        protected void Application_End()
        {
            LibRdKafkaLoader.FreeLibrary();
        }
    }
}