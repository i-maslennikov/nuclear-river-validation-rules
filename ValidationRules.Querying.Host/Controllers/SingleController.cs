using System.Web.Http;

using NuClear.ValidationRules.Querying.Host.Composition;
using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.SingleCheck;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Controllers
{
    [RoutePrefix("api/Single")]
    public class SingleController : ApiController
    {
        private readonly ValidationResultFactory _factory;
        private readonly ValidatorFactory _validatorFactory;

        public SingleController(ValidationResultFactory factory, ValidatorFactory validatorFactory)
        {
            _factory = factory;
            _validatorFactory = validatorFactory;
        }

        [Route(""), HttpPost]
        public IHttpActionResult Post([FromBody] ApiRequest request)
        {
            var validator = _validatorFactory.Create();
            var query = validator.Execute(request.OrderId);

            var messages = query.ToMessages(ResultType.Single);
            var result = _factory.GetValidationResult(messages);
            return Ok(result);
        }

        public class ApiRequest
        {
            public long OrderId { get; set; }
        }
    }
}
