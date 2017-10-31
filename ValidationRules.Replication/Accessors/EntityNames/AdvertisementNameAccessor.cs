using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.Accessors.EntityNames
{
    public sealed class AdvertisementNameAccessor : IMemoryBasedDataObjectAccessor<EntityName>, IDataChangesHandler<EntityName>
    {
        // ReSharper disable once UnusedParameter.Local
        public AdvertisementNameAccessor(IQuery query) { }

        public IReadOnlyCollection<EntityName> GetDataObjects(ICommand command)
        {
            switch (command)
            {
                case ReplaceDataObjectCommand<EntityName> replaceCommand:
                    return replaceCommand.DataObjects;
                default:
                    throw new ArgumentException($"Expected only command of type {typeof(ReplaceDataObjectCommand<EntityName>)}, but received {command.GetType()}", nameof(command));
            }
        }

        public FindSpecification<EntityName> GetFindSpecification(ICommand command)
        {
            switch (command)
            {
                case ReplaceDataObjectCommand<EntityName> replaceCommand:
                    var ids = replaceCommand.DataObjects.Select(x => x.Id);
                    return new FindSpecification<EntityName>(x => x.EntityType == EntityTypeIds.Advertisement && ids.Contains(x.Id));
                default:
                    throw new ArgumentException($"Expected only command of type {typeof(ReplaceDataObjectCommand<EntityName>)}, but received {command.GetType()}", nameof(command));
            }
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<EntityName> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<EntityName> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<EntityName> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<EntityName> dataObjects)
            => Array.Empty<IEvent>();
    }
}