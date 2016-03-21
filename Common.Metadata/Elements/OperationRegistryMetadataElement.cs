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
            AllowedOperations = new HashSet<StrictOperationIdentity>(Features.OfType<AllowedOperationFeature>().Select(f => f.OperationIdentity));
            IgnoredOperations = new HashSet<StrictOperationIdentity>(Features.OfType<IgnoredOperationFeature>().Select(f => f.OperationIdentity));
        }

        public IEnumerable<StrictOperationIdentity> AllowedOperations { get; }

        public IEnumerable<StrictOperationIdentity> IgnoredOperations { get; }

        public sealed class AllowedOperationFeature : IMetadataFeature
        {
            public AllowedOperationFeature(StrictOperationIdentity operationIdentity)
            {
                OperationIdentity = operationIdentity;
            }

            public StrictOperationIdentity OperationIdentity { get; }
        }

        public sealed class IgnoredOperationFeature : IMetadataFeature
        {
            public IgnoredOperationFeature(StrictOperationIdentity operationIdentity)
            {
                OperationIdentity = operationIdentity;
            }

            public StrictOperationIdentity OperationIdentity { get; }
        }
    }
}