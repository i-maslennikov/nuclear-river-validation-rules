using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.CustomerIntelligence.Replication.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Replication.Accessors
{
    public sealed class CategoryAccessor : IStorageBasedDataObjectAccessor<Category>, IDataChangesHandler<Category>
    {
        private readonly IQuery _query;

        public CategoryAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Category> GetSource() => Specs.Map.Erm.ToFacts.Categories.Map(_query);

        public FindSpecification<Category> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Category>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Category> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Category> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Category> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Category> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<Category>(x => ids.Contains(x.Id));

            var categories1 = _query.For(new FindSpecification<Category>(x => x.Level == 1));
            var categories2 = _query.For(new FindSpecification<Category>(x => x.Level == 2));
            var categories3 = _query.For(new FindSpecification<Category>(x => x.Level == 3));

            var level3 = from firmAddress in _query.For<FirmAddress>()
                         join categoryFirmAddress in _query.For<CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                         join category3 in categories3.Where(specification) on categoryFirmAddress.CategoryId equals category3.Id
                         select firmAddress.FirmId;

            var level2 = from firmAddress in _query.For<FirmAddress>()
                         join categoryFirmAddress in _query.For<CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                         join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                         join category2 in categories2.Where(specification) on category3.ParentId equals category2.Id
                         select firmAddress.FirmId;

            var level1 = from firmAddress in _query.For<FirmAddress>()
                         join categoryFirmAddress in _query.For<CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                         join category3 in categories3 on categoryFirmAddress.CategoryId equals category3.Id
                         join category2 in categories2 on category3.ParentId equals category2.Id
                         join category1 in categories1.Where(specification) on category2.ParentId equals category1.Id
                         select firmAddress.FirmId;

            var firmIds = level3.Union(level2).Union(level1).ToArray();

            return firmIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Firm), x))
                          .ToArray();
        }
    }
}