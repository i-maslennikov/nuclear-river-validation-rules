using System;
using System.Linq;
using System.Web.Http;

using NuClear.ValidationRules.Querying.Host.Composition;
using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.SingleCheck;
using NuClear.ValidationRules.SingleCheck.Store;

namespace NuClear.ValidationRules.Querying.Host.Controllers
{
    [RoutePrefix("api/Single")]
    public class SingleController : ApiController
    {
        private readonly MessageRepositiory _repositiory;
        private readonly ValidationResultFactory _factory;
        private readonly PipelineFactory _pipelineFactory;

        public SingleController(MessageRepositiory repositiory, ValidationResultFactory factory, PipelineFactory pipelineFactory)
        {
            _repositiory = repositiory;
            _factory = factory;
            _pipelineFactory = pipelineFactory;
        }

        [Route("{stateToken:guid}"), HttpPost]
        public IHttpActionResult Post([FromBody]ApiRequest request, [FromUri]Guid stateToken)
        {
            long versionId;
            if (!_repositiory.TryGetVersion(stateToken, out versionId))
            {
                return NotFound();
            }

            using (var validator = new Validator(_pipelineFactory.CreatePipeline(), new ErmStoreFactory("Erm", request.OrderId), new NMemoryStoreFactory(), new HashSetStoreFactory()))
            {
                var messages = validator.Execute().Where(x => x.OrderId == request.OrderId).ToArray();
                var result = _factory.ComposeAll(messages, x => x.ForSingle);
                return Ok(result);
            }
        }

        [Route(""), HttpPost]
        public IHttpActionResult Post([FromBody]ApiRequest request)
        {
            using (var validator = new Validator(_pipelineFactory.CreatePipeline(), new ErmStoreFactory("Erm", request.OrderId), new NMemoryStoreFactory(), new HashSetStoreFactory()))
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
