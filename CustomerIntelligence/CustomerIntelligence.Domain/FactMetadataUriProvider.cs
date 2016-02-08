using System;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.AdvancedSearch.Common.Metadata.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;

namespace NuClear.CustomerIntelligence.Domain
{
    public sealed class FactMetadataUriProvider : IMetadataUriProvider
    {
        public Uri GetFor(Type type)
        {
            return ReplicationMetadataIdentity.Instance.Id.WithRelative(new Uri($"{ReplicationMetadataName.Facts}/{type.Name}", UriKind.Relative));
        }
    }
}