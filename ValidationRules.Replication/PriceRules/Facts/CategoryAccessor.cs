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

        public IQueryable<Category> GetSource() => Specs.Map.Erm.ToFacts.Category.Map(_query);

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
            // todo: А нужно ли пересчитывать заказы, которые имею продажи в одну из родительских рубрик?
            // т.е. связаны не с Category.Id, а с Category.ParentId (по факту не бывает) или Category.ParentId.ParentId (очень даже бывает)
            // нет, не требуется - это должно быть поддержано на уровне вычисления сообщений, а для пересчёта заказа не требуется.

            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<Category>(x => ids.Contains(x.Id));

            var orderIds = (from category in _query.For(specification)
                            join opa in _query.For<OrderPositionAdvertisement>() on category.Id equals opa.CategoryId
                            join orderPosition in _query.For<OrderPosition>() on opa.OrderPositionId equals orderPosition.Id
                            select orderPosition.OrderId)
                            .Distinct()
                            .ToArray();

            return orderIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Order), x))
                          .ToArray();
        }
    }
}