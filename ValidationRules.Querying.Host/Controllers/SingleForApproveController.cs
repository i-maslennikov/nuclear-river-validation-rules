using System.Linq;
using System.Web.Http;

using NuClear.ValidationRules.Querying.Host.Composition;
using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.SingleCheck;
using NuClear.ValidationRules.SingleCheck.Store;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Controllers
{
    [RoutePrefix("api/SingleForCancel")]
    public class SingleForCancelController : ApiController
    {
        private readonly ValidationResultFactory _factory;
        private readonly PipelineFactory _pipelineFactory;

        public SingleForCancelController(ValidationResultFactory factory, PipelineFactory pipelineFactory)
        {
            _factory = factory;
            _pipelineFactory = pipelineFactory;
        }

        [Route(""), HttpPost]
        public IHttpActionResult Post([FromBody]ApiRequest request)
        {
            using (var validator = new Validator(_pipelineFactory.CreatePipeline(), new ErmStoreFactory("Erm", request.OrderId), new PersistentTableStoreFactory("Messages"), new HashSetStoreFactory()))
            {
                var sqlBitwiseFilter = ResultType.SingleForCancel.ToBitMask();

                var query = validator.Execute()
                    .Where(x => x.OrderId == request.OrderId)
                    .Where(x => (x.Result & sqlBitwiseFilter) != 0);

                var messages = query.ToMessages(ResultType.SingleForCancel).ToList();
                var result = _factory.GetValidationResult(messages);
                return Ok(result);
            }
        }

        public class ApiRequest
        {
            public long OrderId { get; set; }
        }
    }
}
