using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Specs;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class AdvertisementElementTemplateAccessor : IStorageBasedDataObjectAccessor<AdvertisementElementTemplate>, IDataChangesHandler<AdvertisementElementTemplate>
    {
        private readonly IQuery _query;

        public AdvertisementElementTemplateAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<AdvertisementElementTemplate> GetSource() => _query
            .For(Specs.Find.Erm.AdvertisementElementTemplate)
            .Select(x => new AdvertisementElementTemplate
            {
                Id = x.Id,
                IsRequired = x.IsRequired,
                NeedsValidation = x.NeedsValidation,
                IsAdvertisementLink = x.IsAdvertisementLink,
            });

        public FindSpecification<AdvertisementElementTemplate> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToList();
            return SpecificationFactory<AdvertisementElementTemplate>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<AdvertisementElementTemplate> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<AdvertisementElementTemplate> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<AdvertisementElementTemplate> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<AdvertisementElementTemplate> dataObjects)
        {
            var dataObjectIds = dataObjects.Select(x => x.Id).ToList();

            var advertisementIds =
                from element in _query.For<AdvertisementElement>().Where(x => dataObjectIds.Contains(x.AdvertisementElementTemplateId))
                select element.AdvertisementId;

            return new EventCollectionHelper<AdvertisementElementTemplate> { { typeof(Advertisement), advertisementIds } };
        }
    }
}