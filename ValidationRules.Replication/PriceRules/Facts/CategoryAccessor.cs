using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.PriceRules.Facts;

namespace NuClear.ValidationRules.Replication.PriceRules.Facts
{
    public sealed class CategoryAccessor : IStorageBasedDataObjectAccessor<Category>, IDataChangesHandler<Category>
    {
        private readonly IQuery _query;

        public CategoryAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Category> GetSource()
            => CategoriesLevel1.Union(CategoriesLevel2).Union(CategoriesLevel3);

        private IQueryable<Category> CategoriesLevel3
            => from c3 in _query.For(Specs.Find.Erm.Categories()).Where(x => x.Level == 3)
               from c2 in _query.For(Specs.Find.Erm.Categories()).Where(x => x.Level == 2 && x.Id == c3.ParentId)
               from c1 in _query.For(Specs.Find.Erm.Categories()).Where(x => x.Level == 1 && x.Id == c2.ParentId)
               select new Category { Id = c3.Id, Name = c3.Name, L3Id = c3.Id, L2Id = c2.Id, L1Id = c1.Id };

        private IQueryable<Category> CategoriesLevel2
            => from c2 in _query.For(Specs.Find.Erm.Categories()).Where(x => x.Level == 2)
               from c1 in _query.For(Specs.Find.Erm.Categories()).Where(x => x.Level == 1 && x.Id == c2.ParentId)
               select new Category { Id = c2.Id, Name = c2.Name, L3Id = null, L2Id = c2.Id, L1Id = c1.Id };

        private IQueryable<Category> CategoriesLevel1
            => from c1 in _query.For(Specs.Find.Erm.Categories()).Where(x => x.Level == 1)
               select new Category { Id = c1.Id, Name = c1.Name, L3Id = null, L2Id = null, L1Id = c1.Id };

        public FindSpecification<Category> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Category>(x => ids.Contains(x.L1Id.Value) || ids.Contains(x.L2Id.Value) || ids.Contains(x.L3Id.Value));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Category> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Category), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Category> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Category), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Category> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Category), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Category> dataObjects)
        {
            var categoryIds = dataObjects.Select(x => x.Id).ToArray();

            var orderIds = from opa in _query.For<OrderPositionAdvertisement>().Where(x => x.CategoryId.HasValue && categoryIds.Contains(x.CategoryId.Value))
                           from orderPosition in _query.For<OrderPosition>().Where(x => x.Id == opa.OrderPositionId)
                           select orderPosition.OrderId;

            return new EventCollectionHelper { { typeof(Order), orderIds.Distinct() } };
        }
    }
}