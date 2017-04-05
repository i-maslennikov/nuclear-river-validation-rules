using System.Web.Http;

using NuClear.ValidationRules.Querying.Host.Composition;
using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.SingleCheck;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Controllers
{
    [RoutePrefix("api/SingleForApprove")]
    public class SingleForApproveController : ApiController
    {
        private readonly ValidationResultFactory _factory;
        private readonly PipelineFactory _pipelineFactory;

        public SingleForApproveController(ValidationResultFactory factory, PipelineFactory pipelineFactory)
        {
            _factory = factory;
            _pipelineFactory = pipelineFactory;
        }

        [Route(""), HttpPost]
        public IHttpActionResult Post([FromBody] ApiRequest request)
        {
            var pipeline = _pipelineFactory.Create();
            var query = pipeline.Execute(request.OrderId);

            var messages = query.ToMessages(ResultType.SingleForApprove);
            var result = _factory.GetValidationResult(messages);
            return Ok(result);
        }

        public class ApiRequest
        {
            public long OrderId { get; set; }
        }
    }
}
