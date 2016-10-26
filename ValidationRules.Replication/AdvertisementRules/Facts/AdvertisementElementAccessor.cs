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

        public IQueryable<AdvertisementElement> GetSource() =>
            from element in _query.For(Specs.Find.Erm.AdvertisementElements())
            from status in _query.For<Storage.Model.Erm.AdvertisementElementStatus>().Where(x => x.Id == element.Id)
            select new AdvertisementElement
                {
                    Id = element.Id,
                    AdvertisementId = element.AdvertisementId,
                    AdvertisementElementTemplateId = element.AdvertisementElementTemplateId,
                    IsEmpty = (element.BeginDate == null || element.EndDate == null) && element.FileId == null && string.IsNullOrEmpty(element.Text),
                    Text = element.Text,
                    BeginDate = element.BeginDate,
                    EndDate = element.EndDate,
                    Status = status.Status,
                };

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