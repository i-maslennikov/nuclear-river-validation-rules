using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

using NuClear.ValidationRules.Querying.Host.Composition;
using NuClear.ValidationRules.Querying.Host.DataAccess;

namespace NuClear.ValidationRules.Querying.Host.Controllers
{
    public class ManualController : ApiController
    {
        private readonly MessageRepositiory _repositiory;
        private readonly ValidationResultFactory _factory;

        public ManualController(MessageRepositiory repositiory, ValidationResultFactory factory)
        {
            _repositiory = repositiory;
            _factory = factory;
        }

        // POST: api/Manual
        public IReadOnlyCollection<Model.ValidationResult> Post([FromBody]ApiRequest request)
        {
            long versionId;
            if (!_repositiory.TryGetVersion(request.State, out versionId))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var messages = _repositiory.GetMessages(versionId, request.OrderIds, request.ProjectId, request.ReleaseDate, request.ReleaseDate.AddMonths(1), ResultExtensions.ManualMask);
            var result = _factory.ComposeAll(messages, ResultExtensions.WhenManual);
            return result;
        }

        public class ApiRequest
        {
            public IReadOnlyCollection<long> OrderIds { get; set; }
            public long? ProjectId { get; set; }
            public DateTime ReleaseDate { get; set; }
            public Guid? State { get; set; }
        }
    }
}
