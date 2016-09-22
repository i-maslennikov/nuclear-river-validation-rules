using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.PriceRules.Facts;

namespace NuClear.ValidationRules.Replication.PriceRules.Facts
{
    public sealed class RulesetRuleAccessor : IStorageBasedDataObjectAccessor<RulesetRule>, IDataChangesHandler<RulesetRule>
    {
        private readonly IQuery _query;

        public RulesetRuleAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<RulesetRule> GetSource() => Specs.Map.Erm.ToFacts.RulesetRule.Map(_query);

        public FindSpecification<RulesetRule> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<RulesetRule>(x => ids.Contains(x.Id));
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