using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.CustomerIntelligence.Replication.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Replication.Accessors
{
    public sealed class ActivityAccessor : IStorageBasedDataObjectAccessor<Activity>, IDataChangesHandler<Activity>
    {
        private readonly IQuery _query;

        public ActivityAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Activity> GetSource() => Specs.Map.Erm.ToFacts.Activities.Map(_query);

        public FindSpecification<Activity> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Activity>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Activity> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Activity> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Activity> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Activity> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<Activity>(x => ids.Contains(x.Id));

            var firmIds = (from activity in _query.For(specification)
                           where activity.FirmId.HasValue
                           select activity.FirmId.Value)
                .Concat(from activity in _query.For(specification)
                        join firm in _query.For<Firm>() on activity.ClientId equals firm.ClientId
                        where activity.ClientId.HasValue
                        select firm.Id)
                .ToArray();

            return firmIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Firm), x))
                          .ToArray();
        }
    }
}