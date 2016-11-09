using System.Web.Http;

using ValidationRules.Querying.Host.DI;

namespace ValidationRules.Querying.Host
{
    public static class UnityConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var container = Bootstrapper.ConfigureUnity();
            config.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}