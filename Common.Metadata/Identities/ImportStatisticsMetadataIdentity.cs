using System;

using NuClear.Metamodeling.Kinds;

using MetadataBuilder = NuClear.Metamodeling.Elements.Identities.Builder.Metadata;

namespace NuClear.River.Common.Metadata.Identities
{
    public class ImportDocumentMetadataIdentity : MetadataKindIdentityBase<ImportDocumentMetadataIdentity>
    {
        private readonly Uri _id = new Uri(MetadataBuilder.Id.DefaultRoot, "ImportStatistics");

        public override Uri Id
        {
            get { return _id; }
        }

        public override string Description
        {
            get { return "Statistics import process descriptive metadata"; }
        }
    }
}