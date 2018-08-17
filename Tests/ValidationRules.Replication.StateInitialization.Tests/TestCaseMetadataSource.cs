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
        public TestCaseMetadataSource()
        {
            Tests = DiscoverTests();
            Metadata = Tests.OfType<IMetadataElement>()
                            .ToDictionary(x => x.Identity.Id);
        }

        public IReadOnlyCollection<TestCaseMetadataElement> Tests { get; }
        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }

        private static IReadOnlyCollection<TestCaseMetadataElement> DiscoverTests()
        {
            var acts = new[] { ErmToFacts, FactsToAggregates, AggregatesToMessages };

            var discoveredDataTests = ScanForDatasets();

            var tests = from arrangeMetadataElement in discoveredDataTests
                        from actMetadataElement in acts
                        where actMetadataElement.Requirements.All(requirement => arrangeMetadataElement.Contexts.Contains(requirement))
                              && arrangeMetadataElement.Contexts.Contains(actMetadataElement.Target)
                        select new TestCaseMetadataElement(arrangeMetadataElement, actMetadataElement, AssertOnlyMentionedTypes);

            return tests.OrderBy(x => x.Identity.Id.ToString())
                        .ToList();
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

        private static readonly ActMetadataElement AggregatesToMessages =
            ActMetadataElement.Config
                              .Source(ContextName.Aggregates)
                              .Target(ContextName.Messages)
                              .Action<BulkReplicationAdapter<Messages>>();

        private static readonly AssertMetadataElement AssertOnlyMentionedTypes =
            AssertMetadataElement.Config
                                 .Filter((type, context) => context.Keys.Contains(type) || type == typeof(ValidationRules.Storage.Model.Messages.Version.ValidationResult));
    }
}
