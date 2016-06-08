using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Writings;
using NuClear.ValidationRules.Replication.Commands;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Actors
{
    public class CreateNewVersionActor : IActor
    {
        private readonly IQuery _query;
        private readonly IRepository<Version> _versionRepository;
        private readonly IRepository<Version.ErmState> _tokenRepository;
        private readonly IRepository<Version.ValidationResult> _validationResultRepository;

        public CreateNewVersionActor(IQuery query, IRepository<Version.ErmState> tokenRepository, IRepository<Version> versionRepository, IRepository<Version.ValidationResult> validationResultRepository)
        {
            _query = query;
            _tokenRepository = tokenRepository;
            _versionRepository = versionRepository;
            _validationResultRepository = validationResultRepository;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var versionStates = commands.OfType<CreateNewVersionCommand>().SelectMany(x => x.States);

            var lastVersion = _query.For<Version>().OrderByDescending(x => x.Id).FirstOrDefault();
            if (lastVersion == null)
            {
                // TODO: Синхронизировать с инициализацией состояния.
                // Если она будет порождать версию - тогда можно считать, что версия есть всегда.
                // Если только результаты, не привязанные к версии, тогда их нужно будет создавать новую и привязывать результаты к ней.
                lastVersion = new Version { Id = 0 };
                _versionRepository.Add(lastVersion);
                _versionRepository.Save();
            }

            _tokenRepository.AddRange(versionStates.Select(x => new Version.ErmState { Token = x, VersionId = lastVersion.Id }));
            _tokenRepository.Save();

            var nextVersion = new Version { Id = lastVersion.Id + 1 };
            _versionRepository.Add(nextVersion);
            _versionRepository.Save();

            // TODO: Возможно, не лучшее решение с точки зрения производительности и администратора БД, но трюками займёмся позже.
            var lastVersionResults = _query.For<Version.ValidationResult>().Where(x => x.VersionId == lastVersion.Id);
            var nextVersionResults = lastVersionResults.Select(x =>
                                                               new Version.ValidationResult
                                                                   {
                                                                       VersionId = nextVersion.Id,
                                                                       MessageParams = x.MessageParams,
                                                                       MessageType = x.MessageType,
                                                                       PeriodEnd = x.PeriodEnd,
                                                                       PeriodStart = x.PeriodStart,
                                                                       ProjectId = x.ProjectId,
                                                                       ReferenceType = x.ReferenceType,
                                                                       ReferenceId = x.ReferenceId,
                                                                   });
            _validationResultRepository.AddRange(nextVersionResults);
            _validationResultRepository.Save();

            return Array.Empty<IEvent>();
        }
    }
}