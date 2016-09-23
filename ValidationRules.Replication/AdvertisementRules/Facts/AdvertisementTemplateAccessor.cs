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
    public sealed class AdvertisementTemplateAccessor : IStorageBasedDataObjectAccessor<AdvertisementTemplate>, IDataChangesHandler<AdvertisementTemplate>
    {
        private readonly IQuery _query;

        public AdvertisementTemplateAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<AdvertisementTemplate> GetSource() => _query
            .For(Specs.Find.Erm.AdvertisementTemplates())
            .Select(x => new AdvertisementTemplate
            {
                Id = x.Id,
                DummyAdvertisementId = x.DummyAdvertisementId.Value,
                IsAdvertisementRequired = x.IsAdvertisementRequired,
                IsAllowedToWhiteList = x.IsAllowedToWhiteList,
            });

        public FindSpecification<AdvertisementTemplate> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<AdvertisementTemplate>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<AdvertisementTemplate> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<AdvertisementTemplate> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<AdvertisementTemplate> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<AdvertisementTemplate> dataObjects)
        {
            var dataObjectIds = dataObjects.Select(x => x.Id).ToArray();

            var advertisementIds = from advertisement in _query.For<Advertisement>().Where(x => dataObjectIds.Contains(x.AdvertisementTemplateId))
                                   select advertisement.Id;

            // позиция номенклатуры и шаблон РМ является константой для заказа, агрегат Order не пересчитываем

            return new EventCollectionHelper { { typeof(Advertisement), advertisementIds } }.ToArray();
        }
    }
}