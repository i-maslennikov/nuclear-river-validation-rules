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
    public sealed class RulesetQuantitativeRuleAccessor : IMemoryBasedDataObjectAccessor<Ruleset.QuantitativeRule>, IDataChangesHandler<Ruleset.QuantitativeRule>
    {
        private readonly IQuery _query;

        public RulesetQuantitativeRuleAccessor(IQuery query)
        {
            _query = query;
        }

        public IReadOnlyCollection<Ruleset.QuantitativeRule> GetDataObjects(ICommand command)
        {
            var dtos = ((ReplaceDataObjectCommand)command).Dtos.Cast<RulesetDto>();

            return dtos.SelectMany(ruleset => ruleset.QuantitativeRules
                                                     .Select(rule => new Ruleset.QuantitativeRule
                                                         {
                                                             RulesetId = ruleset.Id,
                                                             NomenclatureCategoryCode = rule.NomenclatureCategoryCode,
                                                             Min = rule.Min,
                                                             Max = rule.Max
                                                         }))
                       .ToList();
        }

        public FindSpecification<Ruleset.QuantitativeRule> GetFindSpecification(ICommand command)
        {
            var dtos = ((ReplaceDataObjectCommand)command).Dtos.Cast<RulesetDto>();
            var ids = dtos.Select(x => x.Id);

            return new FindSpecification<Ruleset.QuantitativeRule>(x => ids.Contains(x.RulesetId));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Ruleset.QuantitativeRule> dataObjects) => Array.Empty<IEvent>();
        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Ruleset.QuantitativeRule> dataObjects) => Array.Empty<IEvent>();
        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Ruleset.QuantitativeRule> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Ruleset.QuantitativeRule> dataObjects)
        {
            var categoryIds = dataObjects.Select(x => x.NomenclatureCategoryCode);

            // Для пакетов и простых позиций
            var firmIdsFromPricePostion =
                from position in _query.For<Position>().Where(x => categoryIds.Contains(x.CategoryCode))
                from pricePosition in _query.For<PricePosition>().Where(x => x.PositionId == position.Id)
                from orderPosition in _query.For<OrderPosition>().Where(x => x.PricePositionId == pricePosition.Id)
                from order in _query.For<Order>().Where(x => x.Id == orderPosition.OrderId)
                select order.FirmId;

            // Для элементов пакетов и простых позиций
            var firmIdsFromOpa =
                from position in _query.For<Position>().Where(x => categoryIds.Contains(x.CategoryCode))
                from opa in _query.For<OrderPositionAdvertisement>().Where(x => x.PositionId == position.Id)
                from orderPosition in _query.For<OrderPosition>().Where(x => x.Id == opa.OrderPositionId)
                from order in _query.For<Order>().Where(x => x.Id == orderPosition.OrderId)
                select order.FirmId;

            return new EventCollectionHelper<Ruleset.QuantitativeRule> { { typeof(Firm), firmIdsFromPricePostion.Distinct()
                                                                                                                .Concat(firmIdsFromOpa.Distinct()) } };
        }
    }
}
