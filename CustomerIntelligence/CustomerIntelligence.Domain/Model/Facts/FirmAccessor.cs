using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Events;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API;
using NuClear.River.Common.Metadata;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class FirmAccessor : IStorageBasedDataObjectAccessor<Firm>, IDataChangesHandler<Firm>
    {
        public IEqualityComparer<Firm> EqualityComparer => null;

        public IQueryable<Firm> GetSource(IQuery query) => Specs.Map.Erm.ToFacts.Firms.Map(query);

        public FindSpecification<Firm> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            => new FindSpecification<Firm>(x => commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).Contains(x.Id));

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Firm> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(CI.Firm), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleReferences(IQuery query, IReadOnlyCollection<Firm> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();

            var commands = new List<IEvent>();
            commands.AddRange(Specs.Map.Facts.ToStatistics.ByFirm(new FindSpecification<Firm>(x => ids.Contains(x.Id)))
                                   .Map(query)
                                   .Select(x => new ReferencedDataObjectUpdatedEvent<StatisticsKey>(typeof(Statistics.FirmForecast), x))
                                   .ToArray());
            commands.AddRange(Specs.Map.Facts.ToClientAggregate.ByFirm(new FindSpecification<Firm>(x => ids.Contains(x.Id)))
                                   .Map(query)
                                   .Select(x => new ReferencedDataObjectUpdatedEvent<long>(typeof(CI.Client), x))
                                   .ToArray());
            return commands;
        }

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Firm> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(CI.Firm), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Firm> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(CI.Firm), x.Id)).ToArray();
    }
}