using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

using MetadataBuilder = NuClear.Metamodeling.Elements.Identities.Builder.Metadata;

namespace NuClear.River.Common.Metadata.Identities
{
    public sealed class ImportDocumentMetadataIdentity : MetadataKindIdentityBase<ImportDocumentMetadataIdentity>
    {
        public override Uri Id => MetadataBuilder.Id.For(MetadataBuilder.Id.DefaultRoot, "ImportDocument");

        public override string Description => "Statistics import process descriptive metadata";
    }
}