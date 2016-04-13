using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.Replication.Core.API.Facts;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Model.CI
{
    public class FirmAggregateActorsFactory : IAggregateActorsFactory
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<Firm> _firmBulkRepository;
        private readonly IBulkRepository<FirmActivity> _firmActivityBulkRepository;
        private readonly IBulkRepository<FirmBalance> _firmBalanceBulkRepository;
        private readonly IBulkRepository<FirmCategory1> _firmCategory1BulkRepository;
        private readonly IBulkRepository<FirmCategory2> _firmCategory2BulkRepository;
        private readonly IBulkRepository<FirmTerritory> _firmFirmTerritoryBulkRepository;

        public FirmAggregateActorsFactory(
            IQuery query,
            IBulkRepository<Firm> firmBulkRepository,
            IBulkRepository<FirmActivity> firmActivityBulkRepository,
            IBulkRepository<FirmBalance> firmBalanceBulkRepository,
            IBulkRepository<FirmCategory1> firmCategory1BulkRepository,
            IBulkRepository<FirmCategory2> firmCategory2BulkRepository,
            IBulkRepository<FirmTerritory> firmFirmTerritoryBulkRepository)
        {
            _query = query;
            _firmBulkRepository = firmBulkRepository;
            _firmActivityBulkRepository = firmActivityBulkRepository;
            _firmBalanceBulkRepository = firmBalanceBulkRepository;
            _firmCategory1BulkRepository = firmCategory1BulkRepository;
            _firmCategory2BulkRepository = firmCategory2BulkRepository;
            _firmFirmTerritoryBulkRepository = firmFirmTerritoryBulkRepository;
        }

        public ICreateDataObjectsActor CreateRootInitializationActor()
            => new CreateDataObjectsActor<Firm>(_query, _firmBulkRepository, new FirmActor());

        public ISyncDataObjectsActor CreateRootSyncActor()
            => new SyncDataObjectsActor<Firm>(_query, _firmBulkRepository, new FirmActor());

        public IDeleteDataObjectsActor CreateRootDestructionActor()
            => new DeleteDataObjectsActor<Firm>(_query, _firmBulkRepository, new FirmActor());

        public IReadOnlyCollection<IReplaceDataObjectsActor> CreateValueObjectsActors()
        {
            return new IReplaceDataObjectsActor[]
                       {
                           new ReplaceDataObjectsActor<FirmActivity>(_query, _firmActivityBulkRepository, new FirmActivityActor()),
                           new ReplaceDataObjectsActor<FirmBalance>(_query, _firmBalanceBulkRepository, new FirmBalanceActor()),
                           new ReplaceDataObjectsActor<FirmCategory1>(_query, _firmCategory1BulkRepository, new FirmCategory1Actor()),
                           new ReplaceDataObjectsActor<FirmCategory2>(_query, _firmCategory2BulkRepository, new FirmCategory2Actor()),
                           new ReplaceDataObjectsActor<FirmTerritory>(_query, _firmFirmTerritoryBulkRepository, new FirmTerritoryActor())
                       };
        }

        private class FirmActor : IStorageBasedFactActor<Firm>
        {
            public IEqualityComparer<Firm> DataObjectEqualityComparer => null;

            public IQueryable<Firm> GetDataObjectsSource(IQuery query) => Specs.Map.Facts.ToCI.Firms.Map(query);

            public FindSpecification<Firm> GetDataObjectsFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                return new FindSpecification<Firm>(x => commands.Cast<IAggregateCommand>().Select(c => c.AggregateId).Distinct().Contains(x.Id));
            }

            public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Firm> dataObjects) => Array.Empty<IEvent>();

            public IReadOnlyCollection<IEvent> HandleReferences(IQuery query, IReadOnlyCollection<Firm> dataObjects) => Array.Empty<IEvent>();

            public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Firm> dataObjects) => Array.Empty<IEvent>();

            public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Firm> dataObjects) => Array.Empty<IEvent>();
        }

        private class FirmActivityActor : IStorageBasedFactActor<FirmActivity>
        {
            public IEqualityComparer<FirmActivity> DataObjectEqualityComparer => null;

            public IQueryable<FirmActivity> GetDataObjectsSource(IQuery query) => Specs.Map.Facts.ToCI.FirmActivities.Map(query);

            public FindSpecification<FirmActivity> GetDataObjectsFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                return Specs.Find.CI.FirmActivities(commands.Cast<IAggregateCommand>().Select(c => c.AggregateId).Distinct().ToArray());
            }

            public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<FirmActivity> dataObjects) => Array.Empty<IEvent>();

            public IReadOnlyCollection<IEvent> HandleReferences(IQuery query, IReadOnlyCollection<FirmActivity> dataObjects) => Array.Empty<IEvent>();

            public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmActivity> dataObjects) => Array.Empty<IEvent>();

            public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmActivity> dataObjects) => Array.Empty<IEvent>();
        }

        private class FirmBalanceActor : IStorageBasedFactActor<FirmBalance>
        {
            public IEqualityComparer<FirmBalance> DataObjectEqualityComparer => null;

            public IQueryable<FirmBalance> GetDataObjectsSource(IQuery query) => Specs.Map.Facts.ToCI.FirmBalances.Map(query);

            public FindSpecification<FirmBalance> GetDataObjectsFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                return Specs.Find.CI.FirmBalances(commands.Cast<IAggregateCommand>().Select(c => c.AggregateId).Distinct().ToArray());
            }

            public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<FirmBalance> dataObjects) => Array.Empty<IEvent>();

            public IReadOnlyCollection<IEvent> HandleReferences(IQuery query, IReadOnlyCollection<FirmBalance> dataObjects) => Array.Empty<IEvent>();

            public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmBalance> dataObjects) => Array.Empty<IEvent>();

            public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmBalance> dataObjects) => Array.Empty<IEvent>();
        }

        private class FirmCategory1Actor : IStorageBasedFactActor<FirmCategory1>
        {
            public IEqualityComparer<FirmCategory1> DataObjectEqualityComparer => null;

            public IQueryable<FirmCategory1> GetDataObjectsSource(IQuery query) => Specs.Map.Facts.ToCI.FirmCategories1.Map(query);

            public FindSpecification<FirmCategory1> GetDataObjectsFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                return Specs.Find.CI.FirmCategories1(commands.Cast<IAggregateCommand>().Select(c => c.AggregateId).Distinct().ToArray());
            }

            public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<FirmCategory1> dataObjects) => Array.Empty<IEvent>();

            public IReadOnlyCollection<IEvent> HandleReferences(IQuery query, IReadOnlyCollection<FirmCategory1> dataObjects) => Array.Empty<IEvent>();

            public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmCategory1> dataObjects) => Array.Empty<IEvent>();

            public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmCategory1> dataObjects) => Array.Empty<IEvent>();
        }

        private class FirmCategory2Actor : IStorageBasedFactActor<FirmCategory2>
        {
            public IEqualityComparer<FirmCategory2> DataObjectEqualityComparer => null;

            public IQueryable<FirmCategory2> GetDataObjectsSource(IQuery query) => Specs.Map.Facts.ToCI.FirmCategories2.Map(query);

            public FindSpecification<FirmCategory2> GetDataObjectsFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                return Specs.Find.CI.FirmCategories2(commands.Cast<IAggregateCommand>().Select(c => c.AggregateId).Distinct().ToArray());
            }

            public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<FirmCategory2> dataObjects) => Array.Empty<IEvent>();

            public IReadOnlyCollection<IEvent> HandleReferences(IQuery query, IReadOnlyCollection<FirmCategory2> dataObjects) => Array.Empty<IEvent>();

            public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmCategory2> dataObjects) => Array.Empty<IEvent>();

            public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmCategory2> dataObjects) => Array.Empty<IEvent>();
        }

        private class FirmTerritoryActor : IStorageBasedFactActor<FirmTerritory>
        {
            public IEqualityComparer<FirmTerritory> DataObjectEqualityComparer => null;

            public IQueryable<FirmTerritory> GetDataObjectsSource(IQuery query) => Specs.Map.Facts.ToCI.FirmTerritories.Map(query);

            public FindSpecification<FirmTerritory> GetDataObjectsFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                return Specs.Find.CI.FirmTerritories(commands.Cast<IAggregateCommand>().Select(c => c.AggregateId).Distinct().ToArray());
            }

            public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<FirmTerritory> dataObjects) => Array.Empty<IEvent>();

            public IReadOnlyCollection<IEvent> HandleReferences(IQuery query, IReadOnlyCollection<FirmTerritory> dataObjects) => Array.Empty<IEvent>();

            public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<FirmTerritory> dataObjects) => Array.Empty<IEvent>();

            public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<FirmTerritory> dataObjects) => Array.Empty<IEvent>();
        }
    }
}