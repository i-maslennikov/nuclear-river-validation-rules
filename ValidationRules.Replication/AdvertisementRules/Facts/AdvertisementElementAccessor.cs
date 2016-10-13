using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Facts
{
    public sealed class AdvertisementElementAccessor : IStorageBasedDataObjectAccessor<AdvertisementElement>, IDataChangesHandler<AdvertisementElement>
    {
        private readonly IQuery _query;

        public AdvertisementElementAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<AdvertisementElement> GetSource() => _query
            .For(Specs.Find.Erm.AdvertisementElements())
            .Join(_query.For<Storage.Model.Erm.AdvertisementElementStatus>(), x => x.Id, x => x.Id, (x, y) => new AdvertisementElement
            {
                Id = x.Id,
                AdvertisementId = x.AdvertisementId,
                AdvertisementElementTemplateId = x.AdvertisementElementTemplateId,
                IsEmpty = (x.BeginDate == null || x.EndDate == null) && x.FileId == null && string.IsNullOrEmpty(x.Text),
                Text = x.Text,
                BeginDate = x.BeginDate,
                EndDate = x.EndDate,
                Status = y.Status,
            });

        public FindSpecification<AdvertisementElement> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<AdvertisementElement>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<AdvertisementElement> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<AdvertisementElement> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<AdvertisementElement> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<AdvertisementElement> dataObjects)
        {
            var dataObjectIds = dataObjects.Select(x => x.Id).ToArray();

            var advertisementIds =
                from element in _query.For<AdvertisementElement>().Where(x => dataObjectIds.Contains(x.Id))
                select element.AdvertisementId;

            return new EventCollectionHelper { { typeof(Advertisement), advertisementIds.Distinct() } }.ToArray();
        }
    }
}