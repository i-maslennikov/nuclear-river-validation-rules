using System;

namespace NuClear.River.Common.Metadata.Elements
{
    public interface IMetadataUriProvider
    {
        Uri GetFor(Type type);
    }
}
