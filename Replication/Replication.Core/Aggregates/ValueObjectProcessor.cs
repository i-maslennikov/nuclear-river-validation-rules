using System.Collections.Generic;

using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Equality;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class ValueObjectProcessor<T> : IValueObjectProcessor 
        where T : class, IObject
    {
        private readonly IBulkRepository<T> _repository;
        private readonly ValueObjectMetadataElement<T> _metadata;
        private readonly DataChangesDetector<T, T> _changesDetector;
        private readonly IEqualityComparerFactory _equalityComparerFactory;

        // TODO {all, 15.09.2015}: Имеет смысл избавить *Processor от зависимостей IQuery, I*Info, заменить на DataChangesDetector
        public ValueObjectProcessor(ValueObjectMetadataElement<T> metadata, IQuery query, IBulkRepository<T> repository, IEqualityComparerFactory equalityComparerFactory)
        {
            _metadata = metadata;
            _repository = repository;
            _equalityComparerFactory = equalityComparerFactory;
            _changesDetector = new DataChangesDetector<T, T>(_metadata.MapSpecificationProviderForSource, _metadata.MapSpecificationProviderForTarget, query);
        }

        public void ApplyChanges(IReadOnlyCollection<long> ids)
        {
            var mergeResult = _changesDetector.DetectChanges(Specs.Map.ZeroMapping<T>(), _metadata.FindSpecificationProvider.Invoke(ids), _equalityComparerFactory.CreateIdentityComparer<T>());

            _repository.Delete(mergeResult.Complement);
            _repository.Create(mergeResult.Difference);
            _repository.Update(mergeResult.Intersection);
        }
    }
}