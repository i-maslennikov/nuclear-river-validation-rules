using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class RulesetRuleAccessor : IStorageBasedDataObjectAccessor<RulesetRule>, IDataChangesHandler<RulesetRule>
    {
        private const int RulesetDraftPriority = 0;

        private readonly IQuery _query;

        public RulesetRuleAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<RulesetRule> GetSource() =>
            from ruleset in _query.For<Erm::Ruleset>().Where(x => x.Priority != RulesetDraftPriority && !x.IsDeleted)
            from rulesetRule in _query.For<Erm::RulesetRule>().Where(x => x.RulesetId == ruleset.Id)
            select new RulesetRule
                {
                    Id = rulesetRule.RulesetId,
                    RuleType = rulesetRule.RuleType,
                    DependentPositionId = rulesetRule.DependentPositionId,
                    PrincipalPositionId = rulesetRule.PrincipalPositionId,
                    Priority = ruleset.Priority,
                    ObjectBindingType = rulesetRule.ObjectBindingType
                };

        public FindSpecification<RulesetRule> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return Specification<RulesetRule>.Create(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<RulesetRule> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<RulesetRule> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<RulesetRule> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<RulesetRule> dataObjects)
        {
            var positionIds = dataObjects.Select(x => x.DependentPositionId);

            // Для пакетов и простых позиций
            var orderIdsFromPricePostion =
                from pricePosition in _query.For<PricePosition>().Where(x => positionIds.Contains(x.PositionId))
                from orderPosition in _query.For<OrderPosition>().Where(x => x.PricePositionId == pricePosition.Id)
                select orderPosition.OrderId;

            // Для элементов пакетов и простых позиций
            var orderIdsFromOpa =
                from opa in _query.For<OrderPositionAdvertisement>().Where(x => positionIds.Contains(x.PositionId))
                from orderPosition in _query.For<OrderPosition>().Where(x => x.Id == opa.OrderPositionId)
                select orderPosition.OrderId;

            return new EventCollectionHelper { { typeof(Order), orderIdsFromPricePostion.Union(orderIdsFromOpa) } };
        }
    }
}