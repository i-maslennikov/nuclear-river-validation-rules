using System;

namespace NuClear.Querying.Metadata.Metadata.Tests
{
    [Flags]
    internal enum MetadataKind
    {
        Identity = 1,
        Elements = 2,
        Features = 4,
    }
}