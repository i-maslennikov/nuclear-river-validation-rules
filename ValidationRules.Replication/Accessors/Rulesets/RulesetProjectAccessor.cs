using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Dto;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.Accessors.Rulesets
{
    public sealed class RulesetProjectAccessor : IMemoryBasedDataObjectAccessor<Ruleset.RulesetProject>, IDataChangesHandler<Ruleset.RulesetProject>
    {
        private readonly IQuery _query;

        public RulesetProjectAccessor(IQuery query)
        {
            _query = query;
        }

        public IReadOnlyCollection<Ruleset.RulesetProject> GetDataObjects(ICommand command)
        {
            var dtos = ((ReplaceDataObjectCommand)command).Dtos.Cast<RulesetDto>();

            return dtos.SelectMany(ruleset => ruleset.Projects
                                                     .Select(projectId => new Ruleset.RulesetProject
                                                         {
                                                             RulesetId = ruleset.Id,
                                                             ProjectId = projectId
                                                         }))
                       .ToList();
        }

        public FindSpecification<Ruleset.RulesetProject> GetFindSpecification(ICommand command)
        {
            var dtos = ((ReplaceDataObjectCommand)command).Dtos.Cast<RulesetDto>();
            var ids = dtos.Select(x => x.Id);

            return new FindSpecification<Ruleset.RulesetProject>(x => ids.Contains(x.RulesetId));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Ruleset.RulesetProject> dataObjects) => Array.Empty<IEvent>();
        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Ruleset.RulesetProject> dataObjects) => Array.Empty<IEvent>();
        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Ruleset.RulesetProject> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Ruleset.RulesetProject> dataObjects)
        {
            var projectIds = dataObjects.Select(x => x.ProjectId);

            var firmIds =
                from project in _query.For<Project>().Where(x => projectIds.Contains(x.Id))
                from order in _query.For<Order>().Where(x => x.DestOrganizationUnitId == project.OrganizationUnitId)
                select order.FirmId;

            return new EventCollectionHelper<Ruleset.RulesetProject> { { typeof(Firm), firmIds.Distinct() } };
        }
    }
}
