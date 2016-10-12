using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.ProjectRules.Facts;

namespace NuClear.ValidationRules.Replication.ProjectRules.Aggregates
{
    public sealed class FirmAddressAggregateRootActor : EntityActorBase<FirmAddress>, IAggregateRootActor
    {
        public FirmAddressAggregateRootActor(
            IQuery query,
            IBulkRepository<FirmAddress> bulkRepository,
            IEqualityComparerFactory equalityComparerFactory)
            : base(query, bulkRepository, equalityComparerFactory, new FirmAddressAccessor(query))
        {
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => Array.Empty<IActor>();

        public sealed class FirmAddressAccessor : IStorageBasedDataObjectAccessor<FirmAddress>
        {
            private readonly IQuery _query;

            public FirmAddressAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<FirmAddress> GetSource()
                => from address in _query.For<Facts::FirmAddress>()
                   select new FirmAddress
                   {
                       Id = address.Id,
                       Name = address.Name,
                       IsLocatedOnTheMap = address.IsLocatedOnTheMap,
                   };

            public FindSpecification<FirmAddress> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<FirmAddress>(x => aggregateIds.Contains(x.Id));
            }
        }
    }
}

