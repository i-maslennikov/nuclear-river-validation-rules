using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Facts;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.Replication.Core.Facts
{
    public class FactProcessor<TFact> : IFactProcessor
        where TFact : class, IIdentifiable<long>
    {
        private readonly IBulkRepository<TFact> _repository;

        private readonly IReadOnlyCollection<IFactDependencyProcessor> _depencencyProcessors;
        private readonly IReadOnlyCollection<IFactDependencyProcessor> _indirectDepencencyProcessors;
        private readonly FactChangesDetector<TFact> _changesDetector;
        private readonly FindSpecificationProvider<TFact, long> _findSpecificationProvider;
        private readonly Func<TFact, long> _identityProvider;

        public FactProcessor(
            FactChangesDetector<TFact> changesDetector,
            IBulkRepository<TFact> repository,
            IReadOnlyCollection<IFactDependencyProcessor> depencencyProcessors,
            IIdentityProvider<long> factIdentityProvider)
        {
            _repository = repository;
            _depencencyProcessors = depencencyProcessors;
            _indirectDepencencyProcessors = depencencyProcessors.Where(p => p.DependencyType == DependencyType.Indirect).ToArray();
            _changesDetector = changesDetector;

            _findSpecificationProvider = new FindSpecificationProvider<TFact, long>(factIdentityProvider);
            _identityProvider = factIdentityProvider.Get<TFact>().Compile();
        }

        public IReadOnlyCollection<IOperation> Execute(IReadOnlyCollection<FactOperation> commands)
        {
            var spec = _findSpecificationProvider.Create(commands.Select(x => x.FactId).ToArray());
            var changes = _changesDetector.DetectChanges(spec);
            var result = new List<IOperation>();

            var factsToCreate = changes.Difference.ToArray();
            var factsToUpdate = changes.Intersection.ToArray();
            var factsToDelete = changes.Complement.ToArray();

            // Create
            _repository.Create(factsToCreate);
            result.AddRange(_depencencyProcessors.SelectMany(x => x.ProcessCreation(factsToCreate.Select(_identityProvider).ToArray())));

            // Update
            result.AddRange(_indirectDepencencyProcessors.SelectMany(x => x.ProcessUpdating(factsToUpdate.Select(_identityProvider).ToArray())));
            _repository.Update(factsToUpdate);
            result.AddRange(_depencencyProcessors.SelectMany(x => x.ProcessUpdating(factsToUpdate.Select(_identityProvider).ToArray())));

            // Delete
            result.AddRange(_depencencyProcessors.SelectMany(x => x.ProcessDeletion(factsToDelete.Select(_identityProvider).ToArray())));
            _repository.Delete(factsToDelete);

            return result;
        }
    }
}