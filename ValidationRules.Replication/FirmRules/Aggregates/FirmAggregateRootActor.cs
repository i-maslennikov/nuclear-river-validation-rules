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
using NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.FirmRules.Facts;

namespace NuClear.ValidationRules.Replication.FirmRules.Aggregates
{
    public sealed class FirmAggregateRootActor : EntityActorBase<Firm>, IAggregateRootActor
    {
        public FirmAggregateRootActor(
            IQuery query,
            IBulkRepository<Firm> bulkRepository,
            IEqualityComparerFactory equalityComparerFactory)
            : base(query, bulkRepository, equalityComparerFactory, new FirmAccessor(query))
        {
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors() => new IEntityActor[0];

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => Array.Empty<IActor>();

        public sealed class FirmAccessor : IStorageBasedDataObjectAccessor<Firm>
        {
            private const long SpecialCategoryId = 18599; // Выгодные покупки с 2ГИС.

            private readonly IQuery _query;

            public FirmAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Firm> GetSource()
                => from firm in _query.For<Facts::Firm>()
                   let needsSpecialPosition = (from firmAddress in _query.For<Facts::FirmAddress>().Where(x => x.FirmId == firm.Id)
                                               from firmAddressCategory in _query.For<Facts::FirmAddressCategory>().Where(x => x.FirmAddressId == firmAddress.Id)
                                               where firmAddressCategory.CategoryId == SpecialCategoryId
                                               select firmAddressCategory.CategoryId).Any()
                   select new Firm
                       {
                           Id = firm.Id,
                           Name = firm.Name,
                           NeedsSpecialPosition = needsSpecialPosition
                       };

            public FindSpecification<Firm> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Firm>(x => aggregateIds.Contains(x.Id));
            }
        }
    }
}

