using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.River.Common.Metadata.Builders;
using NuClear.River.Common.Metadata.Features;

namespace NuClear.River.Common.Metadata.Elements
{
    public sealed class EntityRelationElement : BaseMetadataElement<EntityRelationElement, EntityRelationElementBuilder>
    {
        internal EntityRelationElement(IMetadataElementIdentity identity, IEnumerable<IMetadataFeature> features)
            : base(identity, features)
        {
        }

        public EntityRelationCardinality Cardinality
        {
            get
            {
                return ResolveFeature<EntityRelationCardinalityFeature, EntityRelationCardinality>(
                    f => f.Cardinality,
                    () => { throw new InvalidOperationException("The cardinality was not specified."); });
            }
        }

        public EntityElement Target
        {
            get
            {
                return ResolveFeature<EntityRelationCardinalityFeature, EntityElement>(
                    f => f.Target,
                    () => { throw new InvalidOperationException("The cardinality was not specified."); });
            }
        }

        public bool ContainsTarget
        {
            get { return ResolveFeature<EntityRelationContainmentFeature, bool>(_ => true, false); }
        }
    }
}