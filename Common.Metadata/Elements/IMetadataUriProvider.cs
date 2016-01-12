using System;

namespace NuClear.AdvancedSearch.Common.Metadata.Elements
{
    public interface IMetadataUriProvider
    {
        Uri GetFor(Type type);
    }
}
