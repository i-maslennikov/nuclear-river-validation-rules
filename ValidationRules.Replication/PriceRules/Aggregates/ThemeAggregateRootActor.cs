using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
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
    public sealed class ThemeAggregateRootActor : AggregateRootActor<Theme>
    {
        public ThemeAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Theme> bulkRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new ThemeAccessor(query), bulkRepository);
        }

        public sealed class ThemeAccessor : AggregateDataChangesHandler<Theme>, IStorageBasedDataObjectAccessor<Theme>
        {
            private readonly IQuery _query;

            public ThemeAccessor(IQuery query)
            {
                Invalidate(MessageTypeCode.AdvertisementCountPerThemeShouldBeLimited);

                _query = query;
            }

            public IQueryable<Theme> GetSource()
                => _query.For<Facts::Theme>().Select(x => new Theme { Id = x.Id, Name = x.Name });

            public FindSpecification<Theme> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Theme>(x => aggregateIds.Contains(x.Id));
            }
        }
    }
}