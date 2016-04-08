using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API.Facts;
using NuClear.River.Common.Metadata;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class FirmActor : IStorageBasedFactActor<Firm>
    {
        public IEqualityComparer<Firm> DataObjectEqualityComparer => null;

        public IQueryable<Firm> GetDataObjectsSource(IQuery query)
        {
            return Specs.Map.Erm.ToFacts.Firms.Map(query);
        }

        public FindSpecification<Firm> GetDataObjectsFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            return new FindSpecification<Firm>(x => commands.Cast<SyncFactCommand>().Select(c => c.FactId).Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Firm> dataObjects)
        {
            return dataObjects.Select(x => new InitializeAggregate(typeof(CI.Firm), x.Id)).ToArray();
        }

        public IReadOnlyCollection<IEvent> HandleReferences(IQuery query, IReadOnlyCollection<Firm> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();

            var commands = new List<IEvent>();
            commands.AddRange(Specs.Map.Facts.ToStatistics.ByFirm(new FindSpecification<Firm>(x => ids.Contains(x.Id)))
                                   .Map(query)
                                   .Select(x => new RecalculateStatisticsOperation(x))
                                   .ToArray());
            commands.AddRange(Specs.Map.Facts.ToClientAggregate.ByFirm(new FindSpecification<Firm>(x => ids.Contains(x.Id)))
                                   .Map(query)
                                   .Select(x => new RecalculateAggregate(typeof(CI.Client), x))
                                   .ToArray());

            return commands;
        }

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Firm> dataObjects)
        {
            return dataObjects.Select(x => new RecalculateAggregate(typeof(CI.Firm), x.Id)).ToArray();
        }

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Firm> dataObjects)
        {
            return dataObjects.Select(x => new DestroyAggregate(typeof(CI.Firm), x.Id)).ToArray();
        }
    }
}