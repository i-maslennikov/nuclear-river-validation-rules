using System.Diagnostics;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Querying.Metadata.Elements;
using NuClear.Querying.Metadata.Features;
using NuClear.Querying.Metadata.Identities;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.Querying.Tests
{
    [TestFixture]
    internal class MetadataTests
    {
        [Test]
        public void ShouldReturnMetadataByIdentity()
        {
            MetadataSet metadataSet;

            Assert.That(TestMetadataProvider.Instance.TryGetMetadata<QueryingMetadataIdentity>(out metadataSet), Is.True);
            Assert.That(metadataSet, Is.Not.Null);
        }

        [TestCase("CustomerIntelligence")]
        public void ShouldReturnMetadataByContextId(string name)
        {
            BoundedContextElement contextElement;

            var id = Metadata.Id.For<QueryingMetadataIdentity>(name);
            Assert.That(TestMetadataProvider.Instance.TryGetMetadata(id, out contextElement), Is.True);
            Assert.That(contextElement, Is.Not.Null);
        }

        [TestCase("CustomerIntelligence", MetadataKind.Identity | MetadataKind.Elements | MetadataKind.Features, Explicit = true, Description = "Used to get dump of hierarchy (manual run)")]
        [TestCase("CustomerIntelligence/ConceptualModel/CategoryGroup", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/CategoryGroup'},'Features':[{'EntitySetName':'CategoryGroups'}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Project", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/Project'},'Features':[{'EntitySetName':'Projects'}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Project/Categories", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/Project/Categories'},'Features':[{'Cardinality':'Many','Target':{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/Category'},'Features':[{'EntitySetName':'Categories'}]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Category", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/Category'},'Features':[{'EntitySetName':'Categories'}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Territory", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/Territory'},'Features':[{'EntitySetName':'Territories'}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/Firm'},'Features':[{'EntitySetName':'Firms'}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Balances", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/Firm/Balances'},'Features':[{'Cardinality':'Many','Target':{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/FirmBalance'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Categories1", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/Firm/Categories1'},'Features':[{'Cardinality':'Many','Target':{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/FirmCategory1'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Categories2", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/Firm/Categories2'},'Features':[{'Cardinality':'Many','Target':{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/FirmCategory2'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Categories3", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/Firm/Categories3'},'Features':[{'Cardinality':'Many','Target':{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/FirmCategory3'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/CategoryGroup", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/Firm/CategoryGroup'},'Features':[{'Cardinality':'One','Target':{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/CategoryGroup'},'Features':[{'EntitySetName':'CategoryGroups'}]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Client", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/Firm/Client'},'Features':[{'Cardinality':'OptionalOne','Target':{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/Client'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/OwnerId", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/Firm/OwnerId'}}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Territories", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/Firm/Territories'},'Features':[{'Cardinality':'Many','Target':{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/FirmTerritory'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmBalance", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/FirmBalance'},'Features':[]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmCategory1", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/FirmCategory1'},'Features':[]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmCategory2", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/FirmCategory2'},'Features':[]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmCategory3", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/FirmCategory3'},'Features':[]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Client", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/Client'},'Features':[]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Client/CategoryGroup", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/Client/CategoryGroup'},'Features':[{'Cardinality':'One','Target':{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/CategoryGroup'},'Features':[{'EntitySetName':'CategoryGroups'}]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Client/Contacts", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/Client/Contacts'},'Features':[{'Cardinality':'Many','Target':{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/ClientContact'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/ClientContact", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/ClientContact'},'Features':[]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmTerritory", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/Querying/CustomerIntelligence/ConceptualModel/FirmTerritory'},'Features':[]}")]
        public string ShouldReturnMetadata(string path, MetadataKind properties)
        {
            IMetadataElement element;

            Assert.That(TryGetMetadata(path, out element), Is.True);
            Assert.That(element, Is.Not.Null);

            var ignoredFeatures = new[] { typeof(EntityIdentityFeature), typeof(ElementMappingFeature), typeof(EntityRelationContainmentFeature) };
            element.Dump(properties, ignoredFeatures);

            Debug.WriteLine(element.Serialize(properties, ignoredFeatures));

            return element.Serialize(properties, ignoredFeatures);
        }

        [TestCase("CustomerIntelligence")]
        [TestCase("CustomerIntelligence/ConceptualModel")]
        [TestCase("CustomerIntelligence/ConceptualModel/CategoryGroup")]
        [TestCase("CustomerIntelligence/ConceptualModel/Client")]
        [TestCase("CustomerIntelligence/ConceptualModel/ClientContact")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmBalance")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmCategory1")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmCategory2")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmCategory3")]
        [TestCase("CustomerIntelligence/ConceptualModel/Project")]
        [TestCase("CustomerIntelligence/ConceptualModel/Category")]
        [TestCase("CustomerIntelligence/ConceptualModel/Territory")]
        [TestCase("CustomerIntelligence/StoreModel")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.CategoryGroup")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.Client")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.ClientContact")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.FirmView")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.FirmBalance")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.FirmCategory1")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.FirmCategory2")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.FirmCategory3")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.Project")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.ProjectCategory")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.Territory")]
        public void ShouldBeDeclared(string path)
        {
            IMetadataElement element;

            Assert.That(TryGetMetadata(path, out element), Is.True);
            Assert.That(element, Is.Not.Null);
        }

        private static bool TryGetMetadata(string path, out IMetadataElement element)
        {
            var id = Metadata.Id.For<QueryingMetadataIdentity>(path);
            return TestMetadataProvider.Instance.TryGetMetadata(id, out element);
        }
    }
}
