using System.Collections.Generic;

using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class ValueObjectProcessor<TValueObject> : IValueObjectProcessor
        where TValueObject : class, IObject
    {
        private readonly IBulkRepository<TValueObject> _repository;
        private readonly DataChangesDetector<TValueObject> _changesDetector;
        private readonly IFindSpecificationProvider<TValueObject> _findSpecificationProvider;

        public ValueObjectProcessor(DataChangesDetector<TValueObject> changesDetector, IBulkRepository<TValueObject> repository, IFindSpecificationProvider<TValueObject> findSpecificationProvider)
        {
            _repository = repository;
            _changesDetector = changesDetector;
            _findSpecificationProvider = findSpecificationProvider;
        }

        public void ApplyChanges(IReadOnlyCollection<AggregateOperation> commands)
        {
            var spec = _findSpecificationProvider.Create(commands);
            var mergeResult = _changesDetector.DetectChanges(spec);

            _repository.Delete(mergeResult.Complement);
            _repository.Create(mergeResult.Difference);
        }
    }
}