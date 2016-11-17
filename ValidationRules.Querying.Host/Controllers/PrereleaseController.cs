using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

using NuClear.ValidationRules.Querying.Host.Composition;
using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Controllers
{
    [RoutePrefix("api/Prerelease")]
    public class PrereleaseController : ApiController
    {
        private readonly MessageRepositiory _repositiory;
        private readonly ValidationResultFactory _factory;

        public PrereleaseController(MessageRepositiory repositiory, ValidationResultFactory factory)
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

            var messages = _repositiory.GetMessages(versionId, request.OrderIds, request.ProjectId, request.ReleaseDate, request.ReleaseDate.AddMonths(1), CombinedResult.PrereleaseMask);
            var result = _factory.ComposeAll(messages, x => x.ForPrerelease);
            return Ok(result);
        }

        public class ApiRequest
        {
            public IReadOnlyCollection<long> OrderIds { get; set; }
            public long ProjectId { get; set; }
            public DateTime ReleaseDate { get; set; }
        }
    }
}
