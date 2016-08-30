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
    public sealed class ThemeAccessor : IStorageBasedDataObjectAccessor<Theme>, IDataChangesHandler<Theme>
    {
        private readonly IQuery _query;

        public ThemeAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Theme> GetSource() => Specs.Map.Erm.ToFacts.Theme.Map(_query);

        public FindSpecification<Theme> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Theme>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Theme> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Theme), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Theme> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Theme), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Theme> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Theme), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Theme> dataObjects)
            => Array.Empty<IEvent>();
    }
}