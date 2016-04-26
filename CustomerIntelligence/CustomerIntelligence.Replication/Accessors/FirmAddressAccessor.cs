using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.CustomerIntelligence.Replication.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.Bit;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Replication.Accessors
{
    public sealed class FirmAddressAccessor : IStorageBasedDataObjectAccessor<FirmAddress>, IDataChangesHandler<FirmAddress>
    {
        private readonly IQuery _query;

        public FirmAddressAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<FirmAddress> GetSource() => Specs.Map.Erm.ToFacts.FirmAddresses.Map(_query);

        public FindSpecification<FirmAddress> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<FirmAddress>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<FirmAddress> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmAddress> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmAddress> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<FirmAddress> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<FirmAddress>(x => ids.Contains(x.Id));

            IEnumerable<IEvent> events = Specs.Map.Facts.ToStatistics.ByFirmAddress(specification)
                                              .Map(_query)
                                              .Select(x => new RelatedDataObjectOutdatedEvent<StatisticsKey>(typeof(ProjectCategoryStatistics), x));

            events = events.Concat(Specs.Map.Facts.ToFirmAggregate.ByFirmAddress(specification)
                                        .Map(_query)
                                        .Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Firm), x)));

            events = events.Concat(Specs.Map.Facts.ToClientAggregate.ByFirmAddress(specification)
                                        .Map(_query)
                                        .Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Client), x)));
            return events.ToArray();
        }
    }
}