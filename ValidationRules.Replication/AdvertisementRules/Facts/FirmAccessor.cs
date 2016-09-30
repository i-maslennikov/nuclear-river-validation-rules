using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Facts
{
    public sealed class FirmAccessor : IStorageBasedDataObjectAccessor<Firm>, IDataChangesHandler<Firm>
    {
        private readonly IQuery _query;

        public FirmAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Firm> GetSource() => _query
            .For<Storage.Model.Erm.Firm>()
            .Select(x => new Firm
            {
                Id = x.Id,
                Name = x.Name,
            });

        public FindSpecification<Firm> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Firm>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Firm> dataObjects) => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Firm), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Firm> dataObjects) => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Firm), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Firm> dataObjects) => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Firm), x.Id)).ToArray();

        // пересчитывать агрегат Order не нужно, т.к. нельзя перепривязать заказ к другой фирме
        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Firm> dataObjects) => Array.Empty<IEvent>();
    }
}