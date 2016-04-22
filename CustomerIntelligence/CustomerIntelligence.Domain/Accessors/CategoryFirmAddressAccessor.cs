using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Events;
using NuClear.CustomerIntelligence.Domain.Model.Bit;
using NuClear.CustomerIntelligence.Domain.Model.Facts;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API.DataObjects;
using NuClear.River.Common.Metadata;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Accessors
{
    public sealed class CategoryFirmAddressAccessor : IStorageBasedDataObjectAccessor<CategoryFirmAddress>, IDataChangesHandler<CategoryFirmAddress>
    {
        private readonly IQuery _query;

        public CategoryFirmAddressAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<CategoryFirmAddress> GetSource() => Specs.Map.Erm.ToFacts.CategoryFirmAddresses.Map(_query);

        public FindSpecification<CategoryFirmAddress> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            => new FindSpecification<CategoryFirmAddress>(x => commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).Contains(x.Id));

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<CategoryFirmAddress> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<CategoryFirmAddress> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<CategoryFirmAddress> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<CategoryFirmAddress> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<CategoryFirmAddress>(x => ids.Contains(x.Id));

            IEnumerable<IEvent> events = Specs.Map.Facts.ToStatistics.ByFirmAddressCategory(specification)
                                              .Map(_query)
                                              .Select(x => new RelatedDataObjectOutdatedEvent<StatisticsKey>(typeof(ProjectCategoryStatistics), x));

            events = events.Concat(Specs.Map.Facts.ToFirmAggregate.ByCategoryFirmAddress(specification)
                                        .Map(_query)
                                        .Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Firm), x)));

            events = events.Concat(Specs.Map.Facts.ToClientAggregate.ByCategoryFirmAddress(specification)
                                        .Map(_query)
                                        .Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Client), x)));
            return events.ToArray();
        }
    }
}