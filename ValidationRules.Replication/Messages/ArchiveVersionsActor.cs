using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.Settings;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Messages
{
    public sealed class ArchiveVersionsActor : IActor
    {
        private readonly IArchiveVersionsSettings _settings;
        private readonly IQuery _query;
        private readonly IBulkRepository<Version> _versionDeleteRepository;
        private readonly IBulkRepository<Version.ValidationResult> _validationResultRepository;
        private readonly IBulkRepository<Version.ErmStateBulkDelete> _ermStateDeleteRepository;
        private readonly IBulkRepository<Version.ValidationResultBulkDelete> _validationResultDeleteRepository;

        public ArchiveVersionsActor(
            IArchiveVersionsSettings settings,
            IQuery query,
            IBulkRepository<Version> versionDeleteRepository,
            IBulkRepository<Version.ValidationResult> validationResultRepository,
            IBulkRepository<Version.ErmStateBulkDelete> ermStateDeleteRepository,
            IBulkRepository<Version.ValidationResultBulkDelete> validationResultDeleteRepository)
        {
            _settings = settings;
            _query = query;
            _versionDeleteRepository = versionDeleteRepository;
            _validationResultRepository = validationResultRepository;
            _ermStateDeleteRepository = ermStateDeleteRepository;
            _validationResultDeleteRepository = validationResultDeleteRepository;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var archiveDate = DateTime.UtcNow - _settings.ArchiveVersionsInterval;

            var versions = _query.For<Version>().Where(x => x.Date < archiveDate).OrderByDescending(x => x.Id).ToList();
            if (versions.Count > 1)
            {
                var keepVersion = versions.First();
                var keepValidationResults = _query.For<Version.ValidationResult>().GetValidationResults(keepVersion.Id).ApplyVersionId(keepVersion.Id).ToList();

                _validationResultDeleteRepository.Delete(versions.Select(x => new Version.ValidationResultBulkDelete { VersionId = x.Id }));
                _ermStateDeleteRepository.Delete(versions.Select(x => new Version.ErmStateBulkDelete { VersionId = x.Id }));
                _versionDeleteRepository.Delete(versions.Skip(1));

                _validationResultRepository.Create(keepValidationResults);
            }

            return Array.Empty<IEvent>();
        }
    }
}
