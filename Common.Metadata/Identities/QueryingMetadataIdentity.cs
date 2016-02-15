using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

namespace NuClear.River.Common.Metadata.Identities
{
    public sealed class QueryingMetadataIdentity : MetadataKindIdentityBase<QueryingMetadataIdentity>
    {
        private readonly Uri _id = Metamodeling.Elements.Identities.Builder.Metadata.Id.For("Querying");

        public override Uri Id
        {
            get
            {
                return _id;
            }
        }

        public override string Description
        {
            get
            {
                return "Advanced Search";
            }
        }
    }
}