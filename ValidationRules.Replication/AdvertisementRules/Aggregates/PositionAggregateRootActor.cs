using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Facts = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Aggregates
{
    public sealed class PositionAggregateRootActor : AggregateRootActor<Position>
    {
        public PositionAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Position> bulkRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new PositionAccessor(query), bulkRepository);
        }

        public sealed class PositionAccessor : DataChangesHandler<Position>, IStorageBasedDataObjectAccessor<Position>
        {
            private readonly IQuery _query;

            public PositionAccessor(IQuery query)
            {
                Invalidate(MessageTypeCode.AdvertisementMustBelongToFirm);
                Invalidate(MessageTypeCode.AdvertisementWebsiteShouldNotBeFirmWebsite);
                Invalidate(MessageTypeCode.CouponMustBeSoldOnceAtTime);
                Invalidate(MessageTypeCode.OrderMustNotContainDummyAdvertisement);
                Invalidate(MessageTypeCode.OrderPeriodMustContainAdvertisementPeriod);
                Invalidate(MessageTypeCode.OrderPositionAdvertisementMustBeCreated);
                Invalidate(MessageTypeCode.OrderPositionAdvertisementMustHaveAdvertisement);
                Invalidate(MessageTypeCode.OrderPositionMustNotReferenceDeletedAdvertisement);

                _query = query;
            }

            public IQueryable<Position> GetSource()
                => (from position in _query.For<Facts::Position>()
                   select new Position
                   {
                       Id = position.Id,
                       Name = position.Name,
                   }).Distinct();

            public FindSpecification<Position> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Position>(x => aggregateIds.Contains(x.Id));
            }
        }
    }
}
