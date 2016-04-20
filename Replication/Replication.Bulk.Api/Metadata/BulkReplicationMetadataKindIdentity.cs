using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

namespace NuClear.Replication.Bulk.API.Metadata
{
    public sealed class BulkReplicationMetadataKindIdentity : MetadataKindIdentityBase<BulkReplicationMetadataKindIdentity>
    {
        public override Uri Id { get; } = Metamodeling.Elements.Identities.Builder.Metadata.Id.For(Metamodeling.Elements.Identities.Builder.Metadata.Id.DefaultRoot, "BulkReplication");
        public override string Description => "Bulk replication context description";
    }
}