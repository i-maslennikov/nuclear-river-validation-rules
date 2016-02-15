using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.River.Common.Metadata.Elements;

namespace NuClear.River.Common.Metadata.Features
{
    public sealed class EntityIdentityFeature : IUniqueMetadataFeature
    {
        private readonly IReadOnlyCollection<EntityPropertyElement> _identifyingProperties;

        public EntityIdentityFeature(IReadOnlyCollection<EntityPropertyElement> identifyingProperties)
        {
            if (identifyingProperties == null)
            {
                throw new ArgumentNullException("identifyingProperties");
            }
            if (identifyingProperties.Count == 0)
            {
                throw new ArgumentException("The properties should be provided.", "identifyingProperties");
            }

            _identifyingProperties = identifyingProperties;
        }

        public IReadOnlyCollection<EntityPropertyElement> IdentifyingProperties
        {
            get
            {
                return _identifyingProperties;
            }
        }
    }
}