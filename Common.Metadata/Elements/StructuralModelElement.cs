using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Querying.Metadata.Builders;

namespace NuClear.Querying.Metadata.Elements
{
    public sealed class StructuralModelElement : BaseMetadataElement<StructuralModelElement, StructuralModelElementBuilder>
    {
        internal StructuralModelElement(IMetadataElementIdentity identity, IEnumerable<EntityElement> rootEntities, IEnumerable<IMetadataFeature> features)
            : base(identity, features)
        {
            if (rootEntities == null)
            {
                throw new ArgumentNullException(nameof(rootEntities));
            }
            RootEntities = rootEntities;
        }

        public IEnumerable<EntityElement> RootEntities { get; }

        public IEnumerable<EntityElement> Entities => Elements.OfType<EntityElement>();
    }
}