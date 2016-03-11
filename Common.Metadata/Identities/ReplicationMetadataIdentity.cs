using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

using MetadataBuilder = NuClear.Metamodeling.Elements.Identities.Builder.Metadata;

namespace NuClear.River.Common.Metadata.Identities
{
    public sealed class ReplicationMetadataIdentity : MetadataKindIdentityBase<ReplicationMetadataIdentity>
    {
        public override Uri Id => MetadataBuilder.Id.For(MetadataBuilder.Id.DefaultRoot, "Replication");

        public override string Description => "Replication system descriptive metadata";
    }
}