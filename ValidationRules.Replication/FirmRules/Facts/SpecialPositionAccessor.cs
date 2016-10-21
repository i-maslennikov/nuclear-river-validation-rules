using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.FirmRules.Facts;

namespace NuClear.ValidationRules.Replication.FirmRules.Facts
{
    public sealed class SpecialPositionAccessor : IStorageBasedDataObjectAccessor<SpecialPosition>, IDataChangesHandler<SpecialPosition>
    {
        public const long SelfAdvertisementOnlyOnPc = 287; // Самореклама только для ПК
        public const long AdvantageousPurchaseWith2Gis = 14; // Выгодные покупки с 2ГИС
        public const long PlatformIndependent = 0;
        public const long PlatformDesktop = 1;

        private readonly IQuery _query;

        public SpecialPositionAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<SpecialPosition> GetSource()
            => from position in _query.For(Specs.Find.Erm.Positions())
               select new SpecialPosition
                   {
                       Id = position.Id,
                       IsSelfAdvertisementOnPc = position.CategoryCode == SelfAdvertisementOnlyOnPc,
                       IsAdvantageousPurchaseOnPc = position.CategoryCode == AdvantageousPurchaseWith2Gis && position.Platform == PlatformDesktop,
                       IsApplicapleForPc = position.Platform == PlatformDesktop || position.Platform == PlatformIndependent,
                   };

        public FindSpecification<SpecialPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<SpecialPosition>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<SpecialPosition> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<SpecialPosition> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<SpecialPosition> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<SpecialPosition> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();

            var orderIds =
                from pricePosition in _query.For<OrderPositionAdvertisement>().Where(x => ids.Contains(x.PositionId))
                from orderPosition in _query.For<OrderPosition>().Where(x => x.Id == pricePosition.OrderPositionId)
                select orderPosition.OrderId;

            var firmIds =
                from order in _query.For<Order>().Where(x => orderIds.Contains(x.Id))
                select order.FirmId;

            return new EventCollectionHelper { { typeof(Order), orderIds.Distinct() }, { typeof(Firm), firmIds.Distinct() } };
        }
    }
}