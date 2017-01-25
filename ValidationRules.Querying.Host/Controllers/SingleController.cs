using System;
using System.Linq;
using System.Web.Http;

using NuClear.ValidationRules.Querying.Host.Composition;
using NuClear.ValidationRules.SingleCheck;
using NuClear.ValidationRules.SingleCheck.Store;

namespace NuClear.ValidationRules.Querying.Host.Controllers
{
    [RoutePrefix("api/Single")]
    public class SingleController : ApiController
    {
        private readonly ValidationResultFactory _factory;
        private readonly PipelineFactory _pipelineFactory;

        public SingleController(ValidationResultFactory factory, PipelineFactory pipelineFactory)
        {
            _factory = factory;
            _pipelineFactory = pipelineFactory;
        }

        [Route(""), HttpPost]
        public IHttpActionResult Post([FromBody]ApiRequest request)
        {
            using (var validator = new Validator(_pipelineFactory.CreatePipeline(), new ErmStoreFactory("Erm", request.OrderId), new PersistentTableStoreFactory("Messages"), new HashSetStoreFactory()))
            {
                var messages = validator.Execute().Where(x => x.OrderId == request.OrderId).ToArray();
                var result = _factory.ComposeAll(messages, x => x.ForSingle);
                return Ok(result);
            }
        }

        public class ApiRequest
        {
            public long OrderId { get; set; }
        }
    }
}
