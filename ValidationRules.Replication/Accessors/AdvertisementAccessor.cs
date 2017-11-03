using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class AdvertisementAccessor : IMemoryBasedDataObjectAccessor<Advertisement>, IDataChangesHandler<Advertisement>
    {
        private readonly IQuery _query;

        public AdvertisementAccessor(IQuery query)
        {
            _query = query;
        }

        public IReadOnlyCollection<Advertisement> GetDataObjects(ICommand command)
        {
            switch (command)
            {
                case ReplaceDataObjectCommand<Advertisement> replaceCommand:
                    return replaceCommand.DataObjects;
                default:
                    return Array.Empty<Advertisement>();
            }
        }

        public FindSpecification<Advertisement> GetFindSpecification(ICommand command)
        {
            switch (command)
            {
                case ReplaceDataObjectCommand<Advertisement> replaceCommand:
                    var ids = replaceCommand.DataObjects.Select(x => x.Id);
                    return new FindSpecification<Advertisement>(x => ids.Contains(x.Id));
                default:
                    throw new ArgumentException($"Expected only command of type {typeof(ReplaceDataObjectCommand<Advertisement>)}, but received {command.GetType()}", nameof(command));
            }
        }

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Advertisement> dataObjects)
        {
            var advertisementIds = dataObjects.Select(x => x.Id);

            var orderIds =
                from pricePosition in _query.For<OrderPositionAdvertisement>().Where(x => x.AdvertisementId != null && advertisementIds.Contains(x.AdvertisementId.Value))
                from orderPosition in _query.For<OrderPosition>().Where(x => x.Id == pricePosition.OrderPositionId)
                select orderPosition.OrderId;

            return new EventCollectionHelper<Advertisement> { { typeof(Order), orderIds } };
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Advertisement> dataObjects) => Array.Empty<IEvent>();
        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Advertisement> dataObjects) => throw new NotSupportedException();
        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Advertisement> dataObjects) => throw new NotSupportedException();
    }
}
