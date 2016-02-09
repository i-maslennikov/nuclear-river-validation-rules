using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource : MetadataSourceBase<TestCaseMetadataIdentity>
    {
        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
            => Tests().ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);

        private static IEnumerable<TestCaseMetadataElement> Tests()
        {
            var acts = new[] { ErmToFacts, FactsToAggregates };

            var tests = from arrangeMetadataElement in ScanForDatasets()
                        from actMetadataElement in acts
                        where actMetadataElement.Requirements.All(requirement => arrangeMetadataElement.Contexts.Contains(requirement))
                              && arrangeMetadataElement.Contexts.Contains(actMetadataElement.Target)
                        select new TestCaseMetadataElement(arrangeMetadataElement, actMetadataElement);

            return tests.OrderBy(x => x.Identity.Id.ToString());
        }

        private static IEnumerable<ArrangeMetadataElement> ScanForDatasets()
        {
            return typeof(TestCaseMetadataSource)
                .GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(property => property.PropertyType == typeof(ArrangeMetadataElement))
                .Select(property => property.GetValue(null))
                .Cast<ArrangeMetadataElement>();
        }

        private static readonly ActMetadataElement ErmToFacts =
            ActMetadataElement.Config
                              .Source(ContextName.Erm)
                              .Target(ContextName.Facts)
                              .Action<BulkReplicationAdapter<Facts>>();

        private static readonly ActMetadataElement FactsToAggregates =
            ActMetadataElement.Config
                              .Source(ContextName.Facts)
                              .Target(ContextName.Aggregates)
                              .Action<BulkReplicationAdapter<Aggregates>>();
    }
}
