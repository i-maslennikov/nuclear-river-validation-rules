using NuClear.CustomerIntelligence.Domain;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;

namespace NuClear.CustomerIntelligence.Querying.Tests
{
    internal static class TestMetadataProvider
    {
        public static IMetadataProvider Instance = new MetadataProvider(new IMetadataSource[]
            {
                new QueryingMetadataSource()
            }, new IMetadataProcessor[] {});
    }
}
