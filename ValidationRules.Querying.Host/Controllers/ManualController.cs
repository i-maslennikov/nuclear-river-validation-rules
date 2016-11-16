using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

using NuClear.ValidationRules.Querying.Host.Composition;
using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Storage.Model.Messages;

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

        [Route("api/Manual/{stateToken}")]
        public IReadOnlyCollection<Model.ValidationResult> Post([FromBody]ApiRequest request, [FromUri]Guid stateToken)
        {
            long versionId;
            if (!_repositiory.TryGetVersion(stateToken, out versionId))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var messages = _repositiory.GetMessages(versionId, request.OrderIds, request.ProjectId, request.ReleaseDate, request.ReleaseDate.AddMonths(1), CombinedResult.ManualMask);
            var result = _factory.ComposeAll(messages, x => x.ForManual);
            return result;
        }

        [Route("api/Manual")]
        public IReadOnlyCollection<Model.ValidationResult> Post([FromBody]ApiRequest request)
        {
            var versionId = _repositiory.GetLatestVersion();
            var messages = _repositiory.GetMessages(versionId, request.OrderIds, request.ProjectId, request.ReleaseDate, request.ReleaseDate.AddMonths(1), CombinedResult.ManualMask);
            var result = _factory.ComposeAll(messages, x => x.ForManual);
            return result;
        }

        public class ApiRequest
        {
            public IReadOnlyCollection<long> OrderIds { get; set; }
            public long? ProjectId { get; set; }
            public DateTime ReleaseDate { get; set; }
        }
    }
}
