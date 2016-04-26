using System;

using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.Querying.Metadata.Features
{
    public sealed class EntitySetFeature : IUniqueMetadataFeature
    {
        public EntitySetFeature(string entitySetName)
        {
            if (string.IsNullOrWhiteSpace(entitySetName))
            {
                throw new ArgumentException("The entity set name should be specified.", "entitySetName");
            }
            EntitySetName = entitySetName;
        }

        public string EntitySetName { get; private set; }
    }
}