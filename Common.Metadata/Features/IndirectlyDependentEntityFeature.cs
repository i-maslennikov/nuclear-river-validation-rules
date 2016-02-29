using System;
using NuClear.River.Common.Metadata.Model;

namespace NuClear.River.Common.Metadata.Features
{
    public class IndirectlyDependentEntityFeature<TFact, TEntityKey> : IFactDependencyFeature
        where TFact : IIdentifiable<long>
    {
        public IndirectlyDependentEntityFeature(Type entityType, MapToObjectsSpecProvider<TFact, TEntityKey> dependentAggregateSpecProvider)
        {
            EntityType = entityType;
            FactType = typeof(TFact);
            DependentAggregateSpecProvider = dependentAggregateSpecProvider;
        }

        public Type FactType { get; }

        public Type EntityType { get; }

        public MapToObjectsSpecProvider<TFact, TEntityKey> DependentAggregateSpecProvider { get; }

        public DependencyType DependencyType => DependencyType.Indirect;
    }
}