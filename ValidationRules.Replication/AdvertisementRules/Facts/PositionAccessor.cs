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
    public sealed class PositionAccessor : IStorageBasedDataObjectAccessor<Position>, IDataChangesHandler<Position>
    {
        private readonly IQuery _query;

        public PositionAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Position> GetSource() =>
            from position in _query.For(Specs.Find.Erm.Positions())
            from child in _query.For<Storage.Model.Erm.PositionChild>().Where(x => x.MasterPositionId == position.Id).DefaultIfEmpty()
            select new Position
            {
                Id = position.Id,
                AdvertisementTemplateId = position.AdvertisementTemplateId,
                IsCompositionOptional = position.IsCompositionOptional,
                Name = position.Name,

                ChildPositionId = child.ChildPositionId,
            };

        public FindSpecification<Position> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Position>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Position> dataObjects) => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Position), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Position> dataObjects) => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Position), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Position> dataObjects) => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Position), x.Id)).ToArray();

        // позиция номенклатуры и шаблон РМ является константой для заказа, агрегат Order не пересчитываем
        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Position> dataObjects) => Array.Empty<IEvent>();
    }
}