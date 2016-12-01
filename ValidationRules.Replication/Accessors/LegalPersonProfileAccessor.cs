using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class LegalPersonProfileAccessor : IStorageBasedDataObjectAccessor<LegalPersonProfile>, IDataChangesHandler<LegalPersonProfile>
    {
        private readonly IQuery _query;

        public LegalPersonProfileAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<LegalPersonProfile> GetSource() => _query
            .For<Erm::LegalPersonProfile>()
            .Where(x => x.IsActive && !x.IsDeleted)
            .Select(x => new LegalPersonProfile
                {
                    Id = x.Id,
                    LegalPersonId = x.LegalPersonId,
                    BargainEndDate = x.BargainEndDate,
                    WarrantyEndDate = x.WarrantyEndDate,
                    Name = x.Name,
                });

        public FindSpecification<LegalPersonProfile> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return Specification<LegalPersonProfile>.Create(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<LegalPersonProfile> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<LegalPersonProfile> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<LegalPersonProfile> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<LegalPersonProfile> dataObjects)
        {
            var legalPersonProfileIds = dataObjects.Select(x => x.Id).ToArray();
            var legalPersonIds = dataObjects.Select(x => x.LegalPersonId).Distinct().ToArray();

            var orderIds =
                from order in _query.For<Order>()
                where order.LegalPersonProfileId.HasValue && legalPersonProfileIds.Contains(order.LegalPersonProfileId.Value)
                      || order.LegalPersonId.HasValue && legalPersonIds.Contains(order.LegalPersonId.Value)
                select order.Id;

            return new EventCollectionHelper { { typeof(Order), orderIds } };
        }
    }
}