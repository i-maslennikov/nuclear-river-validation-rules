using System;

using NuClear.River.Common.Metadata.Model;

namespace NuClear.River.Common.Metadata.Features
{
    public class DirectlyDependentAggregateFeature<TFact> : IFactDependencyFeature<TFact>
        where TFact : class, IIdentifiable<long>
    {
        public DirectlyDependentAggregateFeature(
            MapToObjectsSpecProvider<TFact, IOperation> mapSpecificationProviderOnCreate,
            MapToObjectsSpecProvider<TFact, IOperation> mapSpecificationProviderOnUpdate,
            MapToObjectsSpecProvider<TFact, IOperation> mapSpecificationProviderOnDelete)

        {
            MapSpecificationProviderOnCreate = mapSpecificationProviderOnCreate;
            MapSpecificationProviderOnUpdate = mapSpecificationProviderOnUpdate;
            MapSpecificationProviderOnDelete = mapSpecificationProviderOnDelete;
        }

        public Type DependencyType => typeof(TFact);

        public MapToObjectsSpecProvider<TFact, IOperation> MapSpecificationProviderOnCreate { get; }
        public MapToObjectsSpecProvider<TFact, IOperation> MapSpecificationProviderOnUpdate { get; }
        public MapToObjectsSpecProvider<TFact, IOperation> MapSpecificationProviderOnDelete { get; }
    }
}