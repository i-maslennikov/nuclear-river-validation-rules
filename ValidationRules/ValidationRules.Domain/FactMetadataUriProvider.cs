using System;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.AdvancedSearch.Common.Metadata.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;

namespace NuClear.ValidationRules.Domain
{
    public sealed class FactMetadataUriProvider : IMetadataUriProvider
    {
        public Uri GetFor(Type type)
        {
            return ReplicationMetadataIdentity.Instance.Id.WithRelative(new Uri($"{ReplicationMetadataName.PriceContextFacts}/{type.Name}", UriKind.Relative));
        }
    }

    public sealed class AggregateMetadataUriProvider : IMetadataUriProvider
    {
        public Uri GetFor(Type type)
        {
            return ReplicationMetadataIdentity.Instance.Id.WithRelative(new Uri($"{ReplicationMetadataName.PriceContextAggregates}/{type.Name}", UriKind.Relative));
        }
    }
}