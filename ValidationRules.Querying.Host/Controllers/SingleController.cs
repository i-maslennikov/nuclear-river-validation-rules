using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Serialization;

namespace NuClear.ValidationRules.Querying.Host.Controllers
{
    public class SingleController : ApiController
    {
        private readonly MessageRepositiory _repositiory;
        private readonly MessageSerializer _serializer;

        public SingleController(MessageRepositiory repositiory, MessageSerializer serializer)
        {
            _repositiory = repositiory;
            _serializer = serializer;
        }

        // POST: api/Single
        public IReadOnlyCollection<Model.ValidationResult> Post([FromBody]ApiRequest request)
        {
            long versionId;
            if (!_repositiory.TryGetVersion(request.State, out versionId))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var messages = _repositiory.GetMessages(versionId, new [] { request.OrderId }, null, DateTime.MinValue, DateTime.MaxValue, ResultExtensions.SingleMask);
            var result = _serializer.Serialize(messages, ResultExtensions.WhenSingle);
            return result;
        }

        public class ApiRequest
        {
            public long OrderId { get; set; }
            public Guid? State { get; set; }
        }
    }
}
