using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Newtonsoft.Json.Linq;

namespace ValidationRules.Querying.Host.Controllers
{
    public class ReleaseController : ApiController
    {
        // POST: api/Release
        public int Post([FromBody]ApiRequest request)
        {
            //var request = jsonBody.ToObject<Request>();
            return 5;
        }

        public class ApiRequest
        {
            public IReadOnlyCollection<long> OrderIds { get; set; }
            public long ProjectId { get; set; }
            public DateTime ReleaseDate { get; set; }
            public Guid? State { get; set; }
        }
    }
}
