using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Model.Common.Operations.Identity;

namespace NuClear.Replication.OperationsProcessing.Metadata
{
    public sealed class OperationRegistryMetadataElement : MetadataElement<OperationRegistryMetadataElement, OperationRegistryMetadataElementBuilder>
    {
        private IMetadataElementIdentity _identity;

        public OperationRegistryMetadataElement(IMetadataElementIdentity identity, IEnumerable<IMetadataFeature> features) : base(features)
        {
            _identity = identity;
            AllowedOperations = new HashSet<StrictOperationIdentity>(Features.OfType<AllowedOperationFeature>().Select(f => f.OperationIdentity));
            IgnoredOperations = new HashSet<StrictOperationIdentity>(Features.OfType<IgnoredOperationFeature>().Select(f => f.OperationIdentity));
        }

        public override IMetadataElementIdentity Identity => _identity;

        public IEnumerable<StrictOperationIdentity> AllowedOperations { get; }

        public IEnumerable<StrictOperationIdentity> IgnoredOperations { get; }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            _identity = actualMetadataElementIdentity;
        }

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