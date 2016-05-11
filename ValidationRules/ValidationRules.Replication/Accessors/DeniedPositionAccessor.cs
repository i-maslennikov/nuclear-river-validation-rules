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

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class DeniedPositionAccessor : IStorageBasedDataObjectAccessor<DeniedPosition>, IDataChangesHandler<DeniedPosition>
    {
        private readonly IQuery _query;

        public DeniedPositionAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<DeniedPosition> GetSource() => Specs.Map.Erm.ToFacts.DeniedPosition.Map(_query);

        public FindSpecification<DeniedPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<DeniedPosition>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<DeniedPosition> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<DeniedPosition> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<DeniedPosition> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<DeniedPosition> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<DeniedPosition>(x => ids.Contains(x.Id));

            var priceIds = (from deniedPosition in _query.For(specification)
                            select deniedPosition.PriceId)
                            .Distinct()
                            .ToArray();

            return priceIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Price), x))
                          .ToArray();
        }
    }
}