using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

using NuClear.ValidationRules.Querying.Host.CheckModes;
using NuClear.ValidationRules.Querying.Host.Composition;
using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Controllers
{
    [RoutePrefix("api/Release")]
    public class ReleaseController : ApiController
    {
        private readonly VersioningService _versioningService;
        private readonly ValidationResultRepositiory _repositiory;
        private readonly ValidationResultFactory _factory;
        private readonly ICheckModeDescriptor _checkModeDescriptor;

        public ReleaseController(VersioningService versioningService, ValidationResultRepositiory repositiory, ValidationResultFactory factory, CheckModeDescriptorFactory descriptorFactory)
        {
            _versioningService = versioningService;
            _repositiory = repositiory;
            _factory = factory;
            _checkModeDescriptor = descriptorFactory.GetDescriptorFor(CheckMode.Release);
        }

        [Route("{stateToken:guid}"), HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]ApiRequest request, [FromUri]Guid stateToken)
        {
            var versionId = await _versioningService.WaitForVersion(stateToken);

            var validationResults = _repositiory.GetResults(versionId, request.OrderIds, request.ProjectId, request.ReleaseDate, request.ReleaseDate.AddMonths(1), _checkModeDescriptor);
            var result = _factory.GetValidationResult(validationResults, _checkModeDescriptor);
            return Ok(result);
        }

        [Route(""), HttpPost]
        public IHttpActionResult Post([FromBody]ApiRequest request)
        {
            var versionId = _versioningService.GetLatestVersion();
            var validationResults = _repositiory.GetResults(versionId, request.OrderIds, request.ProjectId, request.ReleaseDate, request.ReleaseDate.AddMonths(1), _checkModeDescriptor);
            var result = _factory.GetValidationResult(validationResults, _checkModeDescriptor);
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
