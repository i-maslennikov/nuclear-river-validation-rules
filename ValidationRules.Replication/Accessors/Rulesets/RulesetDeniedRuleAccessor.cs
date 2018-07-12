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
    public sealed class RulesetDeniedRuleAccessor : IMemoryBasedDataObjectAccessor<Ruleset.DeniedRule>, IDataChangesHandler<Ruleset.DeniedRule>
    {
        private readonly IQuery _query;

        public RulesetDeniedRuleAccessor(IQuery query)
        {
            _query = query;
        }

        public IReadOnlyCollection<Ruleset.DeniedRule> GetDataObjects(ICommand command)
        {
            var dtos = ((ReplaceDataObjectCommand)command).Dtos.Cast<RulesetDto>();

            var targetRules = dtos.SelectMany(ruleset => ruleset.DeniedRules
                                                                .Select(rule => new Ruleset.DeniedRule
                                                                    {
                                                                        RulesetId = ruleset.Id,
                                                                        NomenclatureId = rule.NomeclatureId,
                                                                        DeniedNomenclatureId = rule.DeniedNomenclatureId,
                                                                        BindingObjectStrategy = rule.BindingObjectStrategy
                                                                    }))
                                  .ToList();
            var symmetricRules = targetRules.Where(rule => rule.NomenclatureId != rule.DeniedNomenclatureId)
                                            .Select(rule => new Ruleset.DeniedRule
                                                {
                                                    RulesetId = rule.RulesetId,
                                                    NomenclatureId = rule.DeniedNomenclatureId,
                                                    DeniedNomenclatureId = rule.NomenclatureId,
                                                    BindingObjectStrategy = rule.BindingObjectStrategy
                                                });

            return targetRules.Concat(symmetricRules)
                              .ToList();
        }

        public FindSpecification<Ruleset.DeniedRule> GetFindSpecification(ICommand command)
        {
            var dtos = ((ReplaceDataObjectCommand)command).Dtos.Cast<RulesetDto>();
            var ids = dtos.Select(x => x.Id);

            return new FindSpecification<Ruleset.DeniedRule>(x => ids.Contains(x.RulesetId));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Ruleset.DeniedRule> dataObjects) => Array.Empty<IEvent>();
        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Ruleset.DeniedRule> dataObjects) => Array.Empty<IEvent>();
        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Ruleset.DeniedRule> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Ruleset.DeniedRule> dataObjects)
        {
            var positionIds = dataObjects.Select(x => x.NomenclatureId)
                                         .Union(dataObjects.Select(x => x.DeniedNomenclatureId));

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

            return new EventCollectionHelper<Ruleset.DeniedRule> { { typeof(Firm), firmIdsFromPricePostion.Distinct().Concat(firmIdsFromOpa.Distinct()) } };
        }
    }
}
