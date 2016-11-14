using System;
using System.IO;
using System.Linq;
using System.Web.Http;

namespace NuClear.ValidationRules.Querying.Host.Controllers
{
    public sealed class VersionController : ApiController
    {
        public string GetVersion()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".version.txt");
            if (!File.Exists(path))
            {
                return null;
            }

            var lines = File.ReadLines(path).Take(2).ToArray();
            if (lines.Length < 2)
            {
                return null;
            }

            var versionLine = lines[1];
            return versionLine.Substring(versionLine.IndexOf(':')).Trim(':', ' ');
        }
    }
}