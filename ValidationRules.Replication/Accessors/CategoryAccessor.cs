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
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class CategoryAccessor : IStorageBasedDataObjectAccessor<Category>, IDataChangesHandler<Category>
    {
        private readonly IQuery _query;

        public CategoryAccessor(IQuery query)
        {
            _query = query;
        }

        // Тут мы ещё раз столкнулись с https://github.com/linq2db/linq2db/issues/395
        public IQueryable<Category> GetSource()
        {
            var x = CategoriesLevel1.Union(CategoriesLevel2).Union(CategoriesLevel3);
            return x.ToList().AsQueryable();
        }

        private IQueryable<Category> CategoriesLevel3
            => from c3 in _query.For(Specs.Find.Erm.Category).Where(x => x.Level == 3)
               from c2 in _query.For(Specs.Find.Erm.Category).Where(x => x.Level == 2 && x.Id == c3.ParentId)
               from c1 in _query.For(Specs.Find.Erm.Category).Where(x => x.Level == 1 && x.Id == c2.ParentId)
               select new Category
                {
                    Id = c3.Id,
                    L3Id = c3.Id,
                    L2Id = c2.Id,
                    L1Id = c1.Id,
                    IsActiveNotDeleted = c3.IsActive && !c3.IsDeleted
               };

        private IQueryable<Category> CategoriesLevel2
            => from c2 in _query.For(Specs.Find.Erm.Category).Where(x => x.Level == 2)
               from c1 in _query.For(Specs.Find.Erm.Category).Where(x => x.Level == 1 && x.Id == c2.ParentId)
               select new Category
                {
                    Id = c2.Id,
                    L3Id = null,
                    L2Id = c2.Id,
                    L1Id = c1.Id,
                    IsActiveNotDeleted = c2.IsActive && !c2.IsDeleted
               };

        private IQueryable<Category> CategoriesLevel1
            => from c1 in _query.For(Specs.Find.Erm.Category).Where(x => x.Level == 1)
               select new Category
                {
                    Id = c1.Id,
                    L3Id = null,
                    L2Id = null,
                    L1Id = c1.Id,
                    IsActiveNotDeleted = c1.IsActive && !c1.IsDeleted
               };

        public FindSpecification<Category> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToList();
            return new FindSpecification<Category>(x => x.L1Id.HasValue && ids.Contains(x.L1Id.Value) || x.L2Id.HasValue && ids.Contains(x.L2Id.Value) || x.L3Id.HasValue && ids.Contains(x.L3Id.Value));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Category> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Category> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Category> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Category> dataObjects)
        {
            var categoryIds = dataObjects.Select(x => x.Id).ToList();

            var firmIds =
                from opa in _query.For<OrderPositionAdvertisement>().Where(x => x.CategoryId.HasValue && categoryIds.Contains(x.CategoryId.Value))
                from orderPosition in _query.For<OrderPosition>().Where(x => x.Id == opa.OrderPositionId)
                from order in _query.For<Order>().Where(x => x.Id == orderPosition.OrderId)
                select order.FirmId;

            var themeIds =
                from themeCategory in _query.For<ThemeCategory>().Where(x => categoryIds.Contains(x.CategoryId))
                select themeCategory.ThemeId;

            return new EventCollectionHelper<Category> { { typeof(Theme), themeIds }, { typeof(Firm), firmIds } };
        }
    }
}