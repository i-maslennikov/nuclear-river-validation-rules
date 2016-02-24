using System.Collections.Generic;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.River.Common.Metadata.Builders;

namespace NuClear.River.Common.Metadata.Elements
{
    public sealed class BoundedContextElement : BaseMetadataElement<BoundedContextElement, BoundedContextElementBuilder>
    {
        private readonly StructuralModelElement _conceptualModel;
        private readonly StructuralModelElement _storeModel;

        internal BoundedContextElement(
            IMetadataElementIdentity contextIdentity,
            StructuralModelElement conceptualModel,
            StructuralModelElement storeModel,
            IEnumerable<IMetadataFeature> features)
            : base(contextIdentity, features)
        {
            _conceptualModel = conceptualModel;
            _storeModel = storeModel;
        }

        public StructuralModelElement ConceptualModel
        {
            get
            {
                return _conceptualModel;
            }
        }

        public StructuralModelElement StoreModel
        {
            get
            {
                return _storeModel;
            }
        }
    }
}