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
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.PriceRules.Facts;

namespace NuClear.ValidationRules.Replication.PriceRules.Aggregates
{
    public sealed class ProjectAggregateRootActor : EntityActorBase<Project>, IAggregateRootActor
    {
        public ProjectAggregateRootActor(
            IQuery query,
            IBulkRepository<Project> bulkRepository,
            IEqualityComparerFactory equalityComparerFactory)
            : base(query, bulkRepository, equalityComparerFactory, new ProjectAccessor(query))
        {
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => Array.Empty<IActor>();

        public sealed class ProjectAccessor : AggregateDataChangesHandler<Project>, IStorageBasedDataObjectAccessor<Project>
        {
            private readonly IQuery _query;

            public ProjectAccessor(IQuery query)
            {
                Invalidate(MessageTypeCode.AssociatedPositionsGroupCount);
                Invalidate(MessageTypeCode.MinimalAdvertisementRestrictionShouldBeSpecified);

                _query = query;
            }

            public IQueryable<Project> GetSource()
                => _query.For<Facts::Project>().Select(x => new Project { Id = x.Id, Name = x.Name });

            public FindSpecification<Project> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Project>(x => aggregateIds.Contains(x.Id));
            }
        }
    }
}