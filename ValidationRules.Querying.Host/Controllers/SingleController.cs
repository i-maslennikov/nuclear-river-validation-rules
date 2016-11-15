using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

using NuClear.ValidationRules.Querying.Host.Composition;
using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Controllers
{
    public class SingleController : ApiController
    {
        private readonly MessageRepositiory _repositiory;
        private readonly ValidationResultFactory _factory;

        public SingleController(MessageRepositiory repositiory, ValidationResultFactory factory)
        {
            _repositiory = repositiory;
            _factory = factory;
        }

        // POST: api/Single
        public IReadOnlyCollection<Model.ValidationResult> Post([FromBody]ApiRequest request)
        {
            long versionId;
            if (!_repositiory.TryGetVersion(request.State, out versionId))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var messages = _repositiory.GetMessages(versionId, new [] { request.OrderId }, null, DateTime.MinValue, DateTime.MaxValue, CombinedResult.SingleMask);
            var result = _factory.ComposeAll(messages, x => x.ForSingle);
            return result;
        }

        public class ApiRequest
        {
            public long OrderId { get; set; }
            public Guid? State { get; set; }
        }
    }
}
