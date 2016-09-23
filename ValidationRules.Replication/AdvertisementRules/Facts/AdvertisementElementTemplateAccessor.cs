using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Facts
{
    public sealed class AdvertisementElementTemplateAccessor : IStorageBasedDataObjectAccessor<AdvertisementElementTemplate>, IDataChangesHandler<AdvertisementElementTemplate>
    {
        private readonly IQuery _query;

        public AdvertisementElementTemplateAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<AdvertisementElementTemplate> GetSource() => _query
            .For(Specs.Find.Erm.AdvertisementElementTemplates())
            .Select(x => new AdvertisementElementTemplate
            {
                Id = x.Id,
                Name = x.Name,
                IsRequired = x.IsRequired,
                NeedsValidation = x.NeedsValidation,
            });

        public FindSpecification<AdvertisementElementTemplate> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<AdvertisementElementTemplate>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<AdvertisementElementTemplate> dataObjects) => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(AdvertisementElementTemplate), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<AdvertisementElementTemplate> dataObjects) => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(AdvertisementElementTemplate), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<AdvertisementElementTemplate> dataObjects) => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(AdvertisementElementTemplate), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<AdvertisementElementTemplate> dataObjects)
        {
            var dataObjectIds = dataObjects.Select(x => x.Id).ToArray();

            var advertisementIds =
                from element in _query.For<AdvertisementElement>().Where(x => dataObjectIds.Contains(x.AdvertisementElementTemplateId))
                select element.AdvertisementId;

            return new EventCollectionHelper { { typeof(Advertisement), advertisementIds.Distinct() } }.ToArray();
        }
    }
}