using System;

using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata.Model;

namespace NuClear.River.Common.Metadata.Features
{
    public class DirectlyDependentEntityFeature<TFact> : IFactDependencyFeature
        where TFact : class, IIdentifiable<long>
    {
        public DirectlyDependentEntityFeature(IEntityType entityType)
        {
            EntityType = entityType;
            FactType = typeof(TFact);
        }

        public Type FactType { get; }

        public IEntityType EntityType { get; }
    }
}