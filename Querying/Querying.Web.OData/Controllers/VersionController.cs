using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace NuClear.Querying.Web.OData.Controllers
{
    public sealed class VersionController : ApiController
    {
        public string GetVersion()
        {
            var assemblyFileVersion = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute))
                .Cast<AssemblyInformationalVersionAttribute>()
                .FirstOrDefault();

            return assemblyFileVersion?.InformationalVersion;
        }
    }
}
