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
    public sealed class FirmAccessor : IStorageBasedDataObjectAccessor<Firm>, IDataChangesHandler<Firm>
    {
        private readonly IQuery _query;

        public FirmAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Firm> GetSource() => Specs.Map.Erm.ToFacts.Firms.Map(_query);

        public FindSpecification<Firm> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Firm>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Firm> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Firm), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Firm> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Firm), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Firm> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Firm), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Firm> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<Firm>(x => ids.Contains(x.Id));

            IEnumerable<IEvent> events = Specs.Map.Facts.ToStatistics.ByFirm(specification)
                                              .Map(_query)
                                              .Select(x => new RelatedDataObjectOutdatedEvent<StatisticsKey>(typeof(ProjectCategoryStatistics), x));

            events = events.Concat(Specs.Map.Facts.ToClientAggregate.ByFirm(specification)
                                        .Map(_query)
                                        .Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Client), x)));
            return events.ToArray();
        }
    }
}