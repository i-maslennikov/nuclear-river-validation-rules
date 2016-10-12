using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Facts;

namespace NuClear.ValidationRules.Replication.ProjectRules.Facts
{
    public sealed class PricePositionAccessor : IStorageBasedDataObjectAccessor<PricePosition>, IDataChangesHandler<PricePosition>
    {
        private readonly IQuery _query;

        public PricePositionAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<PricePosition> GetSource()
            => from x in _query.For(Specs.Find.Erm.PricePositions())
               select new PricePosition
                   {
                       Id = x.Id,
                       PositionId = x.PositionId,
                   };

        public FindSpecification<PricePosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<PricePosition>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<PricePosition> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<PricePosition> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<PricePosition> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<PricePosition> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id);

            var orderIds =
                from orderPosition in _query.For<OrderPosition>().Where(x => ids.Contains(x.PricePositionId))
                select orderPosition.OrderId;

            return new EventCollectionHelper { { typeof(Order), orderIds.Distinct() } };
        }
    }
}