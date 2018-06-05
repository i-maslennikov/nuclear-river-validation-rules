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
    public sealed class RulesetAccessor : IMemoryBasedDataObjectAccessor<Ruleset>, IDataChangesHandler<Ruleset>
    {
        private readonly IQuery _query;

        public RulesetAccessor(IQuery query)
        {
            _query = query;
        }

        public IReadOnlyCollection<Ruleset> GetDataObjects(ICommand command)
        {
            var dtos = ((ReplaceDataObjectCommand)command).Dtos.Cast<RulesetDto>();
            return dtos.Select(x => new Ruleset
                           {
                               Id = x.Id,
                               BeginDate = x.BeginDate,
                               EndDate = x.EndDate?.Add(TimeSpan.FromSeconds(1)) ?? DateTime.MaxValue
                           })
                       .ToList();
        }

        public FindSpecification<Ruleset> GetFindSpecification(ICommand command)
        {
            var dtos = ((ReplaceDataObjectCommand)command).Dtos.Cast<RulesetDto>();
            var ids = dtos.Select(x => x.Id);

            return new FindSpecification<Ruleset>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Ruleset> dataObjects) => Array.Empty<IEvent>();
        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Ruleset> dataObjects) => Array.Empty<IEvent>();
        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Ruleset> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Ruleset> dataObjects)
        {
            var rulesetsIds = dataObjects.Select(x => x.Id);
            var projectIds = _query.For<Ruleset.RulesetProject>().Where(x => rulesetsIds.Contains(x.RulesetId))
                                   .Select(x => x.ProjectId)
                                   .Distinct()
                                   .ToList();
            var begin = Min(dataObjects.Min(x => x.BeginDate), _query.For<Ruleset>().Where(x => rulesetsIds.Contains(x.Id)).Min(x => x.BeginDate));
            var end = Max(dataObjects.Max(x => x.EndDate), _query.For<Ruleset>().Where(x => rulesetsIds.Contains(x.Id)).Max(x => x.EndDate));

            var firmIds = from project in _query.For<Project>()
                                                .Where(x => projectIds.Contains(x.Id))
                          from order in _query.For<Order>()
                                              .Where(x => begin < x.EndDistributionPlan && x.BeginDistribution < end)
                                              .Where(x => x.DestOrganizationUnitId == project.OrganizationUnitId)
                          select order.FirmId;

            return new EventCollectionHelper<Ruleset> { { typeof(Firm), firmIds } };
        }

        private static DateTime Min(DateTime a, DateTime b)
            => a < b ? a : b;

        private static DateTime Max(DateTime a, DateTime b)
            => a > b ? a : b;
    }
}
