using System;
using System.Linq;

using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Specifications;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Messages
{
    public sealed class ArchiveVersionsService
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<Version> _versionDeleteRepository;
        private readonly IBulkRepository<Version.ValidationResult> _validationResultRepository;
        private readonly IBulkRepository<Version.ErmStateBulkDelete> _ermStateDeleteRepository;
        private readonly IBulkRepository<Version.AmsStateBulkDelete> _amsStateDeleteRepository;
        private readonly IBulkRepository<Version.ValidationResultBulkDelete> _validationResultDeleteRepository;

        public ArchiveVersionsService(
            IQuery query,
            IBulkRepository<Version> versionDeleteRepository,
            IBulkRepository<Version.ValidationResult> validationResultRepository,
            IBulkRepository<Version.ErmStateBulkDelete> ermStateDeleteRepository,
            IBulkRepository<Version.AmsStateBulkDelete> amsStateDeleteRepository,
            IBulkRepository<Version.ValidationResultBulkDelete> validationResultDeleteRepository)
        {
            _query = query;
            _versionDeleteRepository = versionDeleteRepository;
            _validationResultRepository = validationResultRepository;
            _ermStateDeleteRepository = ermStateDeleteRepository;
            _amsStateDeleteRepository = amsStateDeleteRepository;
            _validationResultDeleteRepository = validationResultDeleteRepository;
        }

        public void Execute(DateTime archiveDate)
        {
            var versions = _query.For<Version>().Where(x => x.UtcDateTime < archiveDate).OrderByDescending(x => x.Id).ToList();
            if (versions.Count > 1)
            {
                var keepVersion = versions.First();
                var keepValidationResults = _query.For<Version.ValidationResult>().ForVersion(keepVersion.Id).ApplyVersionId(keepVersion.Id).ToList();

                _validationResultDeleteRepository.Delete(versions.Select(x => new Version.ValidationResultBulkDelete { VersionId = x.Id }));
                _ermStateDeleteRepository.Delete(versions.Select(x => new Version.ErmStateBulkDelete { VersionId = x.Id }));
                _amsStateDeleteRepository.Delete(versions.Select(x => new Version.AmsStateBulkDelete { VersionId = x.Id }));
                _versionDeleteRepository.Delete(versions.Skip(1));

                _validationResultRepository.Create(keepValidationResults);
            }
        }
    }
}
