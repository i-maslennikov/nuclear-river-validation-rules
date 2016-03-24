using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.River.Common.Metadata.Identities;

namespace NuClear.CustomerIntelligence.Domain
{
    public partial class QueryingMetadataSource : MetadataSourceBase<QueryingMetadataIdentity>
    {
        public QueryingMetadataSource()
        {
            Metadata = new Dictionary<Uri, IMetadataElement>
                        {
                            { CustomerIntelligence.Context.Identity.Id, CustomerIntelligence.Context },
                        };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}