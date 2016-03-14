using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

using MetadataBuilder = NuClear.Metamodeling.Elements.Identities.Builder.Metadata;

namespace NuClear.River.Common.Metadata.Identities
{
    public sealed class OperationRegistryMetadataIdentity : MetadataKindIdentityBase<OperationRegistryMetadataIdentity>
    {
        public override Uri Id => MetadataBuilder.Id.For(MetadataBuilder.Id.DefaultRoot, "OperationsRegistry");

        public override string Description => "Operations registry descriptive metadata";
    }
}