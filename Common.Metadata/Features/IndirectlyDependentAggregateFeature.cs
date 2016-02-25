using System;

using NuClear.River.Common.Metadata.Model;

namespace NuClear.River.Common.Metadata.Features
{
    public class IndirectlyDependentAggregateFeature<TFact> : IIndirectFactDependencyFeature, IFactDependencyFeature<TFact>
        where TFact : IIdentifiable<long>
    {
        public IndirectlyDependentAggregateFeature(MapToObjectsSpecProvider<TFact, IOperation> mapSpecificationProvider)
        {
            MapSpecificationProviderOnCreate
                = MapSpecificationProviderOnUpdate
                  = MapSpecificationProviderOnDelete
                    = mapSpecificationProvider;
        }

        public Type DependencyType => typeof(TFact);

        public MapToObjectsSpecProvider<TFact, IOperation> MapSpecificationProviderOnCreate { get; }
        public MapToObjectsSpecProvider<TFact, IOperation> MapSpecificationProviderOnUpdate { get; }
        public MapToObjectsSpecProvider<TFact, IOperation> MapSpecificationProviderOnDelete { get; }
    }
}