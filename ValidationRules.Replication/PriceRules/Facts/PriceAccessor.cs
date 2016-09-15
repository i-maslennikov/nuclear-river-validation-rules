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
    public sealed class PriceAccessor : IStorageBasedDataObjectAccessor<Price>, IDataChangesHandler<Price>
    {
        private readonly IQuery _query;

        public PriceAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Price> GetSource() => Specs.Map.Erm.ToFacts.Price.Map(_query);

        public FindSpecification<Price> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Price>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Price> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Price), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Price> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Price), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Price> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Price), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Price> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();

            var periodIds =
                from price in _query.For<Price>().Where(x => ids.Contains(x.Id))
                group price by price.OrganizationUnitId into prices
                select new PeriodKey { OrganizationUnitId = prices.Key, Start = prices.Min(y => y.BeginDate), End = DateTime.MaxValue };

            // И какой тип я должен тут указать?
            // Тип outdated-сущности - это период. Нет периода в фактах, а агрегатный тип тут указывать некорректно.
            return periodIds.Select(x => new RelatedDataObjectOutdatedEvent<PeriodKey>(null, x)).ToArray();
        }
    }
}