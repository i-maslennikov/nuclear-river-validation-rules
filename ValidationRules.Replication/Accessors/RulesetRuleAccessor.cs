using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class RulesetRuleAccessor : IStorageBasedDataObjectAccessor<RulesetRule>, IDataChangesHandler<RulesetRule>
    {
        private readonly IQuery _query;

        public RulesetRuleAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<RulesetRule> GetSource()
            => from ruleset in _query.For<Erm::Ruleset>().Where(x => !x.IsDeleted)
               from rulesetRule in _query.For<Erm::RulesetRule>().Where(x => x.RulesetId == ruleset.Id)
               where ruleset.Priority == _query.For<Erm::Ruleset>().Where(x => !x.IsDeleted).Select(x => x.Priority).Max()
               select new RulesetRule
                   {
                       RuleType = rulesetRule.RuleType,
                       DependentPositionId = rulesetRule.DependentPositionId,
                       PrincipalPositionId = rulesetRule.PrincipalPositionId,
                       ObjectBindingType = rulesetRule.ObjectBindingType
                   };

        public FindSpecification<RulesetRule> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            // Исходя из предположения, что в ERM происходит только опреации публикации нового набора правил,
            // а по старому не происходит никаких событий, я не смотрю наидентификар правила,
            // поскольку все старые стали неактуальными, а актуальные - всегда новые.
            return new FindSpecification<RulesetRule>(x => true);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<RulesetRule> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<RulesetRule> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<RulesetRule> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<RulesetRule> dataObjects)
        {
            var positionIds = dataObjects.Select(x => x.DependentPositionId)
                .Union(dataObjects.Select(x => x.PrincipalPositionId));

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

            return new EventCollectionHelper<RulesetRule> { { typeof(Firm), firmIdsFromPricePostion.Concat(firmIdsFromOpa) } };
        }
    }
}