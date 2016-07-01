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
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.PriceRules.Facts
{
    public sealed class AssociatedPositionAccessor : IStorageBasedDataObjectAccessor<AssociatedPosition>, IDataChangesHandler<AssociatedPosition>
    {
        private readonly IQuery _query;

        public AssociatedPositionAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<AssociatedPosition> GetSource() => Specs.Map.Erm.ToFacts.AssociatedPosition.Map(_query);

        public FindSpecification<AssociatedPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<AssociatedPosition>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<AssociatedPosition> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<AssociatedPosition> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<AssociatedPosition> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<AssociatedPosition> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<AssociatedPosition>(x => ids.Contains(x.Id));

            var priceIds = (from associatedPosition in _query.For(specification)
                            join associatedPositionsGroup in _query.For<AssociatedPositionsGroup>() on associatedPosition.AssociatedPositionsGroupId equals
                                associatedPositionsGroup.Id
                            join pricePosition in _query.For<PricePosition>() on associatedPositionsGroup.PricePositionId equals pricePosition.Id
                            select pricePosition.PriceId)
                            .Distinct()
                            .ToArray();

            return priceIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Price), x))
                          .ToArray();
        }
    }
}