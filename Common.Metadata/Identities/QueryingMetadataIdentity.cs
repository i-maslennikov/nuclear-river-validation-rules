using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

namespace NuClear.River.Common.Metadata.Identities
{
    public sealed class QueryingMetadataIdentity : MetadataKindIdentityBase<QueryingMetadataIdentity>
    {
        public override Uri Id { get; } = Metamodeling.Elements.Identities.Builder.Metadata.Id.For("Querying");

        public override string Description => "Advanced Search";
    }
}