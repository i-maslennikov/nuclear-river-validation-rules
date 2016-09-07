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
    public sealed class CategoryAccessor : IStorageBasedDataObjectAccessor<Category>, IDataChangesHandler<Category>
    {
        private readonly IQuery _query;

        public CategoryAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Category> GetSource()
            => from category in _query.For<Erm::Category>()
               select new Category
                   {
                       Id = category.Id,
                       Name = category.Name,
                       IsActiveNotDeleted = category.IsActive && !category.IsDeleted
                   };

        public FindSpecification<Category> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Category>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Category> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Category), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Category> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Category), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Category> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Category), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Category> dataObjects)
            => Array.Empty<IEvent>();
    }
}
