using System;
using NuClear.River.Common.Metadata.Model;

namespace NuClear.River.Common.Metadata.Features
{
    public class IndirectlyDependentEntityFeature<TFact, TEntityKey> : IFactDependencyFeature
        where TFact : IIdentifiable<long>
    {
        public IndirectlyDependentEntityFeature(Type entityType, MapToObjectsSpecProvider<TFact, TEntityKey> dependentEntitySpecProvider)
        {
            EntityType = entityType;
            FactType = typeof(TFact);
            DependentEntitySpecProvider = dependentEntitySpecProvider;
        }

        public Type FactType { get; }

        public Type EntityType { get; }

        public MapToObjectsSpecProvider<TFact, TEntityKey> DependentEntitySpecProvider { get; }
    }
}