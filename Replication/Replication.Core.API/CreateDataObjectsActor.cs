using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.API.Facts;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.API
{
    public sealed class CreateDataObjectsActor<TDataObject> : DataObjectsActor<TDataObject>, ICreateDataObjectsActor
        where TDataObject : class
    {
        private readonly IBulkRepository<TDataObject> _bulkRepository;
        private readonly IStorageBasedFactActor<TDataObject> _storageBasedFactActor;

        public CreateDataObjectsActor(IQuery query, IBulkRepository<TDataObject> bulkRepository, IStorageBasedFactActor<TDataObject> storageBasedFactActor)
            : base(query, storageBasedFactActor)
        {
            _bulkRepository = bulkRepository;
            _storageBasedFactActor = storageBasedFactActor;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var changes = DetectChanges(commands);

            var toCreate = changes.Difference.ToArray();

            _bulkRepository.Create(toCreate);
            return _storageBasedFactActor.HandleCreates(toCreate);
        }
    }
}