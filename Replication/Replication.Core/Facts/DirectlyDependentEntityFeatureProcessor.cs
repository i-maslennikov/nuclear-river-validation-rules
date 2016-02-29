using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.API.Facts;
using NuClear.River.Common.Metadata.Features;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.Replication.Core.Facts
{
    /// <summary>
    /// Выполняет обработку прямой зависимости между фактом и сущностью.
    /// Приямая зависимость означет, что идентификатор факта является идентиифкатором сущности.
    /// </summary>
    public class DirectlyDependentEntityFeatureProcessor<TFact> : IFactDependencyProcessor
        where TFact : class, IIdentifiable<long>
    {
        private readonly DirectlyDependentEntityFeature<TFact> _metadata;

        public DirectlyDependentEntityFeatureProcessor(DirectlyDependentEntityFeature<TFact> metadata)
        {
            _metadata = metadata;
        }

        public IEnumerable<IOperation> ProcessCreation(IReadOnlyCollection<long> factIds)
        {
            return factIds.Select(id => new InitializeAggregate(_metadata.EntityType, id));
        }

        public IEnumerable<IOperation> ProcessUpdating(IReadOnlyCollection<long> factIds)
        {
            return factIds.Select(id => new RecalculateAggregate(_metadata.EntityType, id));
        }

        public IEnumerable<IOperation> ProcessDeletion(IReadOnlyCollection<long> factIds)
        {
            return factIds.Select(id => new DestroyAggregate(_metadata.EntityType, id));
        }
    }
}