using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.River.Common.Metadata.Builders;

namespace NuClear.River.Common.Metadata.Elements
{
    public sealed class OperationRegistryMetadataElement : BaseMetadataElement<OperationRegistryMetadataElement, OperationRegistryMetadataElementBuilder>
    {
        public OperationRegistryMetadataElement(IMetadataElementIdentity identity, IEnumerable<IMetadataFeature> features) : base(identity, features)
        {
            AllowedOperationIdentities = ResolveFeature<AllowedOperationIdentitiesFeature, IEnumerable<StrictOperationIdentity>>(x => x.AllowedOperationIdentities, Enumerable.Empty<StrictOperationIdentity>());
            DisallowedOperationIdentities = ResolveFeature<DisallowedOperationIdentitiesFeature, IEnumerable<StrictOperationIdentity>>(x => x.DisallowedOperationIdentities, Enumerable.Empty<StrictOperationIdentity>());
            ExplicitEntityTypesMap = ResolveFeature<ExplicitEntityTypesMapFeature, IReadOnlyDictionary<IEntityType, IEntityType>>(x => x.ExplicitEntityTypesMap, new Dictionary<IEntityType, IEntityType>());
        }

        public IEnumerable<StrictOperationIdentity> AllowedOperationIdentities { get; }

        public IEnumerable<StrictOperationIdentity> DisallowedOperationIdentities { get; }

        public IReadOnlyDictionary<IEntityType, IEntityType> ExplicitEntityTypesMap { get; }

        public sealed class AllowedOperationIdentitiesFeature : IUniqueMetadataFeature
        {
            public AllowedOperationIdentitiesFeature(IEnumerable<StrictOperationIdentity> allowedOperationIdentities)
            {
                AllowedOperationIdentities = allowedOperationIdentities;
            }

            public IEnumerable<StrictOperationIdentity> AllowedOperationIdentities { get; }
        }

        public sealed class DisallowedOperationIdentitiesFeature : IUniqueMetadataFeature
        {
            public DisallowedOperationIdentitiesFeature(IEnumerable<StrictOperationIdentity> disallowedOperationIdentities)
            {
                DisallowedOperationIdentities = disallowedOperationIdentities;
            }

            public IEnumerable<StrictOperationIdentity> DisallowedOperationIdentities { get; }
        }

        public sealed class ExplicitEntityTypesMapFeature : IUniqueMetadataFeature
        {
            public ExplicitEntityTypesMapFeature(IReadOnlyDictionary<IEntityType, IEntityType> dictionary)
            {
                ExplicitEntityTypesMap = dictionary;
            }

            public IReadOnlyDictionary<IEntityType, IEntityType> ExplicitEntityTypesMap { get; }
        }
    }
}