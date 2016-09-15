using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Facts
{
    public sealed class BargainScanFileAccessor : IStorageBasedDataObjectAccessor<BargainScanFile>, IDataChangesHandler<BargainScanFile>
    {
        private const int BargainScanFileKind = 12;

        private readonly IQuery _query;

        public BargainScanFileAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<BargainScanFile> GetSource()
            => _query.For<Erm::BargainFile>()
                     .Where(x => x.IsActive && !x.IsDeleted && x.FileKind == BargainScanFileKind)
                     .Select(x => new BargainScanFile
                         {
                             Id = x.Id,
                             BargainId = x.BargainId,
                         });

        public FindSpecification<BargainScanFile> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<BargainScanFile>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<BargainScanFile> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<BargainScanFile> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<BargainScanFile> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<BargainScanFile> dataObjects)
        {
            var bargainIds = dataObjects.Select(x => x.BargainId).ToArray();

            var orderIds =
                from order in _query.For<Order>().Where(x => x.BargainId.HasValue && bargainIds.Contains(x.BargainId.Value))
                select order.Id;

            return new EventCollectionHelper { { typeof(Order), orderIds.Distinct() } }.ToArray();
        }
    }
}