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
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Aggregates
{
    public sealed class AdvertisementElementTemplateAggregateRootActor : EntityActorBase<AdvertisementElementTemplate>, IAggregateRootActor
    {
        public AdvertisementElementTemplateAggregateRootActor(
            IQuery query,
            IBulkRepository<AdvertisementElementTemplate> bulkRepository,
            IEqualityComparerFactory equalityComparerFactory)
            : base(query, bulkRepository, equalityComparerFactory, new AdvertisementElementTemplateAccessor(query))
        {
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => Array.Empty<IActor>();

        public sealed class AdvertisementElementTemplateAccessor : IStorageBasedDataObjectAccessor<AdvertisementElementTemplate>
        {
            private readonly IQuery _query;

            public AdvertisementElementTemplateAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<AdvertisementElementTemplate> GetSource()
                => from template in _query.For<Facts::AdvertisementElementTemplate>()
                   select new AdvertisementElementTemplate
                   {
                       Id = template.Id,
                       Name = template.Name,
                   };

            public FindSpecification<AdvertisementElementTemplate> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<AdvertisementElementTemplate>(x => aggregateIds.Contains(x.Id));
            }
        }
    }
}
