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
using NuClear.ValidationRules.Storage.Model.ThemeRules.Facts;

namespace NuClear.ValidationRules.Replication.ThemeRules.Facts
{
    public sealed class ThemeAccessor : IStorageBasedDataObjectAccessor<Theme>, IDataChangesHandler<Theme>
    {
        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        private readonly IQuery _query;

        public ThemeAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Theme> GetSource()
            => _query.For(Specs.Find.Erm.Theme()).Select(x => new Theme
            {
                Id = x.Id,
                Name = x.Name,
                BeginDistribution = x.BeginDistribution,
                EndDistribution = x.EndDistribution + OneSecond,
                IsDefault = x.IsDefault,
            });

        public FindSpecification<Theme> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Theme>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Theme> dataObjects) => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Theme), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Theme> dataObjects) => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Theme), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Theme> dataObjects) => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Theme), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Theme> dataObjects)
        {
            var dataObjectIds = dataObjects.Select(x => x.Id).ToArray();

            var orderIds =
                from opa in _query.For<OrderPositionAdvertisement>().Where(x => dataObjectIds.Contains(x.ThemeId))
                from op in _query.For<OrderPosition>().Where(x => x.Id == opa.OrderPositionId)
                select op.OrderId;

            return new EventCollectionHelper { { typeof(Order), orderIds.Distinct() } }.ToArray();
        }
    }
}