using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB;

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
        private const long ArchiveVersionId = 0;

        private readonly IArchiveVersionsSettings _settings;
        private readonly IQuery _query;
        private readonly IBulkRepository<Version> _versionRepository;
        private readonly IBulkRepository<Version.ValidationResult> _validationResultRepository;

        public ArchiveVersionsActor(IArchiveVersionsSettings settings, IQuery query, IBulkRepository<Version> versionRepository, IBulkRepository<Version.ValidationResult> validationResultRepository)
        {
            _settings = settings;
            _query = query;
            _versionRepository = versionRepository;
            _validationResultRepository = validationResultRepository;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var archiveDate = DateTime.UtcNow - _settings.ArchiveVersionPeriod;

            var versions = _query.For<Version>().Where(x => x.Date < archiveDate).OrderByDescending(x => x.Id).ToList();
            if (versions.Count > 1)
            {
                var newestVersion = versions.First();
                var archiveVersion = versions.First(x => x.Id == ArchiveVersionId);

                var createObjects = _query.For<Version.ValidationResult>().GetValidationResults(newestVersion.Id).ToList();
                _query.For<Version.ValidationResult>().Where(x => x.VersionId <= newestVersion.Id).Delete();
                _validationResultRepository.Create(createObjects.ApplyVersionId(archiveVersion.Id));

                var reconcileErmStates = _query.For<Version.ErmState>().Where(x => x.VersionId != archiveVersion.Id && x.VersionId <= newestVersion.Id);
                reconcileErmStates.Set(x => x.VersionId, archiveVersion.Id).Update();

                archiveVersion.Date = newestVersion.Date;
                _versionRepository.Update(new[] { archiveVersion });
                _versionRepository.Delete(versions.Where(x => x != archiveVersion));
            }

            return Array.Empty<IEvent>();
        }
    }
}
