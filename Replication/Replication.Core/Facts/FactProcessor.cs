using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Facts;
using NuClear.River.Common.Metadata;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Features;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.Facts
{
    public class FactProcessor<TFact> : IFactProcessor 
        where TFact : class, IIdentifiable
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<TFact> _repository;

        private readonly FactMetadata<TFact> _factMetadata;
        private readonly IReadOnlyCollection<IFactDependencyProcessor> _depencencyProcessors;
        private readonly IReadOnlyCollection<IFactDependencyProcessor> _indirectDepencencyProcessors;
        private readonly DataChangesDetector<TFact, TFact> _changesDetector;

        public FactProcessor(FactMetadata<TFact> factMetadata, IFactDependencyProcessorFactory dependencyProcessorFactory, IQuery query, IBulkRepository<TFact> repository)
        {
            _query = query;
            _repository = repository;
            _factMetadata = factMetadata;
            _depencencyProcessors = _factMetadata.Features.OfType<IFactDependencyFeature>().Select(dependencyProcessorFactory.Create).ToArray();
            _indirectDepencencyProcessors = _factMetadata.Features.OfType<IIndirectFactDependencyFeature>().Select(dependencyProcessorFactory.Create).ToArray();
            _changesDetector = new DataChangesDetector<TFact, TFact>(_factMetadata.MapSpecificationProviderForSource, _factMetadata.MapSpecificationProviderForTarget, _query);
        }

        public IReadOnlyCollection<IOperation> ApplyChanges(IReadOnlyCollection<long> ids)
        {
            var changes = _changesDetector.DetectChanges(Specs.Map.ToIds<TFact>(), _factMetadata.FindSpecificationProvider.Invoke(ids), EqualityComparer<long>.Default);
            var result = new List<IOperation>();

            var idsToCreate = changes.Difference.ToArray();
            var idsToUpdate = changes.Intersection.ToArray();
            var idsToDelete = changes.Complement.ToArray();

            // Create
            CreateFact(idsToCreate);
            result.AddRange(_depencencyProcessors.SelectMany(x => x.ProcessCreation(idsToCreate)));

            // Update
            result.AddRange(_indirectDepencencyProcessors.SelectMany(x => x.ProcessUpdating(idsToUpdate)));
            UpdateFact(idsToUpdate);
            result.AddRange(_depencencyProcessors.SelectMany(x => x.ProcessUpdating(idsToUpdate)));

            // Delete
            result.AddRange(_depencencyProcessors.SelectMany(x => x.ProcessDeletion(idsToDelete)));
            DeleteFact(idsToDelete);

            return result;
        }

        private void CreateFact(IReadOnlyCollection<long> ids)
        {
            var spec = _factMetadata.FindSpecificationProvider.Invoke(ids);
            var sourceQueryable = _factMetadata.MapSpecificationProviderForSource.Invoke(spec).Map(_query);
            _repository.Create(sourceQueryable);
        }

        private void UpdateFact(IReadOnlyCollection<long> ids)
        {
            var spec = _factMetadata.FindSpecificationProvider.Invoke(ids);
            var sourceQueryable = _factMetadata.MapSpecificationProviderForSource(spec).Map(_query);
            _repository.Update(sourceQueryable);
        }

        private void DeleteFact(IReadOnlyCollection<long> ids)
        {
            var spec = _factMetadata.FindSpecificationProvider.Invoke(ids);
            var targetQueryable = _factMetadata.MapSpecificationProviderForTarget(spec).Map(_query);
            _repository.Delete(targetQueryable);
        }
    }
}