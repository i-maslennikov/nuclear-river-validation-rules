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
    public sealed class RulesetAssociatedRuleAccessor : IMemoryBasedDataObjectAccessor<Ruleset.AssociatedRule>, IDataChangesHandler<Ruleset.AssociatedRule>
    {
        private readonly IQuery _query;

        public RulesetAssociatedRuleAccessor(IQuery query)
        {
            _query = query;
        }

        public IReadOnlyCollection<Ruleset.AssociatedRule> GetDataObjects(ICommand command)
        {
            var dtos = ((ReplaceDataObjectCommand)command).Dtos.Cast<RulesetDto>();
            return dtos.SelectMany(ruleset => ruleset.AssociatedRules
                                                     .Select(rule => new Ruleset.AssociatedRule
                                                         {
                                                             RulesetId = ruleset.Id,
                                                             PrincipalNomenclatureId = rule.NomeclatureId,
                                                             AssociatedNomenclatureId = rule.AssociatedNomenclatureId,
                                                             ConsideringBindingObject = rule.ConsideringBindingObject
                                                         }))
                       .ToList();
        }

        public FindSpecification<Ruleset.AssociatedRule> GetFindSpecification(ICommand command)
        {
            var dtos = ((ReplaceDataObjectCommand)command).Dtos.Cast<RulesetDto>();
            var ids = dtos.Select(x => x.Id);

            return new FindSpecification<Ruleset.AssociatedRule>(x => ids.Contains(x.RulesetId));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Ruleset.AssociatedRule> dataObjects) => Array.Empty<IEvent>();
        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Ruleset.AssociatedRule> dataObjects) => Array.Empty<IEvent>();
        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Ruleset.AssociatedRule> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Ruleset.AssociatedRule> dataObjects)
        {
            var positionIds = dataObjects.Select(x => x.AssociatedNomenclatureId)
                                         .Union(dataObjects.Select(x => x.PrincipalNomenclatureId));

            // Для пакетов и простых позиций
            var firmIdsFromPricePostion =
                from pricePosition in _query.For<PricePosition>().Where(x => positionIds.Contains(x.PositionId))
                from orderPosition in _query.For<OrderPosition>().Where(x => x.PricePositionId == pricePosition.Id)
                from order in _query.For<Order>().Where(x => x.Id == orderPosition.OrderId)
                select order.FirmId;

            // Для элементов пакетов и простых позиций
            var firmIdsFromOpa =
                from opa in _query.For<OrderPositionAdvertisement>().Where(x => positionIds.Contains(x.PositionId))
                from orderPosition in _query.For<OrderPosition>().Where(x => x.Id == opa.OrderPositionId)
                from order in _query.For<Order>().Where(x => x.Id == orderPosition.OrderId)
                select order.FirmId;

            return new EventCollectionHelper<Ruleset.AssociatedRule> { { typeof(Firm), firmIdsFromPricePostion.Concat(firmIdsFromOpa) } };
        }
    }
}
