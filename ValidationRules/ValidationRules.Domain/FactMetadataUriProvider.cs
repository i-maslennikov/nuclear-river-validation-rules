using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;

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