﻿using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Dto;
using NuClear.ValidationRules.Replication.Events;
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
            var now = DateTime.UtcNow;
            return dtos.Select(x => new Ruleset
                           {
                               Id = x.Id,
                               BeginDate = x.BeginDate,
                               EndDate = x.EndDate?.Add(TimeSpan.FromSeconds(1)) ?? DateTime.MaxValue,
                               IsDeleted = x.IsDeleted,
                               Version = x.Version,
                               ImportedOn = now
                           })
                       .ToList();
        }

        public FindSpecification<Ruleset> GetFindSpecification(ICommand command)
        {
            var dtos = ((ReplaceDataObjectCommand)command).Dtos.Cast<RulesetDto>();
            var ids = dtos.Select(x => x.Id);

            return new FindSpecification<Ruleset>(x => ids.Contains(x.Id));
        }
        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Ruleset> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Ruleset), x.Id)).ToList();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Ruleset> dataObjects) => Array.Empty<IEvent>();
        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Ruleset> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Ruleset> dataObjects)
        {
            if (!dataObjects.Any())
            {
                return Array.Empty<IEvent>();
            }

            var rulesetsIds = dataObjects.Select(x => x.Id);

            var firmIds = from ruleset in _query.For<Ruleset>().Where(x => rulesetsIds.Contains(x.Id))
                          from rulesetProject in _query.For<Ruleset.RulesetProject>().Where(x => x.RulesetId == ruleset.Id)
                          from project in _query.For<Project>().Where(x => x.Id == rulesetProject.ProjectId)
                          from order in _query.For<Order>()
                                              .Where(x => ruleset.BeginDate <= x.BeginDistribution
                                                          && x.BeginDistribution < ruleset.EndDate
                                                          && x.DestOrganizationUnitId == project.OrganizationUnitId)
                          select order.FirmId;

            return new EventCollectionHelper<Ruleset>
                {
                    { typeof(Firm), firmIds.Distinct() },
                    { typeof(Ruleset), rulesetsIds }
                };
        }
    }
}
