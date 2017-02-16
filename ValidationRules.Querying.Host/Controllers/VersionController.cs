using System;
using System.IO;
using System.Linq;
using System.Web.Http;

namespace NuClear.ValidationRules.Querying.Host.Controllers
{
    [RoutePrefix("api/version")]
    public sealed class VersionController : ApiController
    {
        [Route(""), HttpGet]
        public IHttpActionResult Get()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".version.txt");
            if (!File.Exists(path))
            {
                return Ok();
            }

            var lines = File.ReadLines(path).Take(2).ToArray();
            if (lines.Length < 2)
            {
                return Ok();
            }

            var versionLine = lines[1];
            var version = versionLine.Substring(versionLine.IndexOf(':')).Trim(':', ' ');

            return Ok(version);
        }
    }
}