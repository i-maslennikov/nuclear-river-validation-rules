using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;

namespace NuClear.River.Common.Metadata.Builders
{
    public sealed class OperationRegistryMetadataElementBuilder : MetadataElementBuilder<OperationRegistryMetadataElementBuilder, OperationRegistryMetadataElement>
    {
        private string[] _segments;

        protected override OperationRegistryMetadataElement Create()
        {
            var identity = Metamodeling.Elements.Identities.Builder.Metadata.Id.For<OperationRegistryMetadataIdentity>(_segments).Build().AsIdentity();

            return new OperationRegistryMetadataElement(identity, Features);
        }

        public OperationRegistryMetadataElementBuilder For<TSubDomain>()
            where TSubDomain : ISubDomain
        {
            _segments = new[] { typeof(TSubDomain).Name };
            return this;
        }

        public OperationRegistryMetadataElementBuilder AllowedOperationIdentities(IEnumerable<StrictOperationIdentity> allowedOperationIdentities)
        {
            AddFeatures(new OperationRegistryMetadataElement.AllowedOperationIdentitiesFeature(allowedOperationIdentities));
            return this;
        }


        public OperationRegistryMetadataElementBuilder DisallowedOperationIdentities(IEnumerable<StrictOperationIdentity> disallowedOperationIdentities)
        {
            AddFeatures(new OperationRegistryMetadataElement.DisallowedOperationIdentitiesFeature(disallowedOperationIdentities));
            return this;
        }

        public OperationRegistryMetadataElementBuilder ExplicitEntityTypesMap(IReadOnlyDictionary<IEntityType, IEntityType> dictionary)
        {
            AddFeatures(new OperationRegistryMetadataElement.ExplicitEntityTypesMapFeature(dictionary));
            return this;
        }
    }
}