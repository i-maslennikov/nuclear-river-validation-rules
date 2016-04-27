using NuClear.CustomerIntelligence.Querying.Host;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;

namespace NuClear.CustomerIntelligence.Querying.Tests
{
    internal static class TestMetadataProvider
    {
        public static IMetadataProvider Instance { get; } = new MetadataProvider(
            new IMetadataSource[]
                {
                    new QueryingMetadataSource()
                },
            new IMetadataProcessor[] { });
    }
}
