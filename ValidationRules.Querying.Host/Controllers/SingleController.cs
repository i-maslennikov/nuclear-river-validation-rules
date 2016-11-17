using System;
using System.Web.Http;

using NuClear.ValidationRules.Querying.Host.Composition;
using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Controllers
{
    [RoutePrefix("api/Single")]
    public class SingleController : ApiController
    {
        private readonly MessageRepositiory _repositiory;
        private readonly ValidationResultFactory _factory;

        public SingleController(MessageRepositiory repositiory, ValidationResultFactory factory)
        {
            _repositiory = repositiory;
            _factory = factory;
        }

        [Route("{stateToken:guid}"), HttpPost]
        public IHttpActionResult Post([FromBody]ApiRequest request, [FromUri]Guid stateToken)
        {
            long versionId;
            if (!_repositiory.TryGetVersion(stateToken, out versionId))
            {
                return NotFound();
            }

            var messages = _repositiory.GetMessages(versionId, new [] { request.OrderId }, null, DateTime.MinValue, DateTime.MaxValue, CombinedResult.SingleMask);
            var result = _factory.ComposeAll(messages, x => x.ForSingle);
            return Ok(result);
        }

        [Route(""), HttpPost]
        public IHttpActionResult Post([FromBody]ApiRequest request)
        {
            var versionId = _repositiory.GetLatestVersion();
            var messages = _repositiory.GetMessages(versionId, new[] { request.OrderId }, null, DateTime.MinValue, DateTime.MaxValue, CombinedResult.SingleMask);
            var result = _factory.ComposeAll(messages, x => x.ForSingle);
            return Ok(result);
        }

        public class ApiRequest
        {
            public long OrderId { get; set; }
        }
    }
}
