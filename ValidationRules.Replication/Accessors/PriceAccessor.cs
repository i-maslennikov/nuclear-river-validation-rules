using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Specs;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class PriceAccessor : IStorageBasedDataObjectAccessor<Price>, IDataChangesHandler<Price>
    {
        private readonly IQuery _query;

        public PriceAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Price> GetSource() => _query
            .For<Erm::Price>()
            .Where(x => !x.IsDeleted && x.IsPublished)
            .Select(x => new Price
                {
                    Id = x.Id,
                    BeginDate = x.BeginDate,
                    ProjectId = x.ProjectId,
                });

        public FindSpecification<Price> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToList();
            return SpecificationFactory<Price>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Price> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Price), x.Id)).ToList();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Price> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Price), x.Id)).ToList();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Price> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Price), x.Id)).ToList();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Price> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToList();

            // Публикация нового прайс-листа неявно меняет предыдущий (его дату окончания действия)
            var previousPrices =
                from price in _query.For<Price>().Where(x => ids.Contains(x.Id))
                let previous = _query.For<Price>().Where(x => x.BeginDate < price.BeginDate && x.ProjectId == price.ProjectId).OrderByDescending(x => x.BeginDate).FirstOrDefault()
                where previous != null
                select previous.Id;

            var periods =
                from price in _query.For<Price>().Where(x => ids.Contains(x.Id))
                select new PeriodKey { Date = price.BeginDate };

            var orderIds =
                from price in _query.For<Price>().Where(x => ids.Contains(x.Id))
                from project in _query.For<Project>().Where(x => x.Id == price.ProjectId)
                from order in _query.For<Order>().Where(x => x.BeginDistribution >= price.BeginDate && x.DestOrganizationUnitId == project.OrganizationUnitId)
                select order.Id;

            return new EventCollectionHelper<Price> { { typeof(Price), previousPrices }, { typeof(object), periods }, { typeof(Order), orderIds } };
        }
    }
}