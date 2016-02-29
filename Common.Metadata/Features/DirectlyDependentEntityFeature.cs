using System;

using NuClear.River.Common.Metadata.Model;

namespace NuClear.River.Common.Metadata.Features
{
    public class DirectlyDependentEntityFeature<TFact> : IFactDependencyFeature
        where TFact : class, IIdentifiable<long>
    {
        public DirectlyDependentEntityFeature(Type entityType)
        {
            EntityType = entityType;
            FactType = typeof(TFact);
        }

        public Type FactType { get; }

        public Type EntityType { get; }

        public DependencyType DependencyType => DependencyType.Direct;
    }
}