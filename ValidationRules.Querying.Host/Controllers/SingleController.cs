using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Newtonsoft.Json.Linq;

namespace ValidationRules.Querying.Host.Controllers
{
    public class SingleController : ApiController
    {
        // POST: api/Single
        public int Post([FromBody]ApiRequest request)
        {
            //var request = jsonBody.ToObject<Request>();
            return 5;
        }

        public class ApiRequest
        {
            public long OrderId { get; set; }
            public Guid? State { get; set; }
        }
    }
}
