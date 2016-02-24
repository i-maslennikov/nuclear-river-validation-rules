using System.Collections.Generic;
using System.Linq;

using NuClear.River.Common.Metadata.Context;

using NUnit.Framework;

namespace NuClear.Replication.Core.Tests
{
    [TestFixture]
    public class PredicateXmlSerializerTests
    {
        [Test]
        public void TestSerialization()
        {
            var child1 = new Predicate(new Dictionary<string, string> { { "id", "hasProperty" }, { "property", "name" }, { "value", "foobar" } }, new Predicate[0]);
            var child2 = new Predicate(new Dictionary<string, string> { { "id", "hasIdentity" }, { "value", "999" } }, new Predicate[0]);
            var root = new Predicate(new Dictionary<string, string> { { "id", "and" } }, new[] { child1, child2 });
            var serializer = new PredicateXmlSerializer();

            var serialized = serializer.Serialize(root);
            var restoredRoot = serializer.Deserialize(serialized);

            Assert.That(restoredRoot.Properties.Count, Is.EqualTo(1));
            Assert.That(restoredRoot.Properties["id"], Is.EqualTo("and"));

            Assert.That(restoredRoot.Childs.Count, Is.EqualTo(2));

            Assert.That(restoredRoot.Childs.First().Properties, Has.Count.EqualTo(3));
            Assert.That(restoredRoot.Childs.First().Properties["id"], Is.EqualTo("hasProperty"));
            Assert.That(restoredRoot.Childs.First().Properties["property"], Is.EqualTo("name"));
            Assert.That(restoredRoot.Childs.First().Properties["value"], Is.EqualTo("foobar"));

            Assert.That(restoredRoot.Childs.Last().Properties, Has.Count.EqualTo(2));
            Assert.That(restoredRoot.Childs.Last().Properties["id"], Is.EqualTo("hasIdentity"));
            Assert.That(restoredRoot.Childs.Last().Properties["value"], Is.EqualTo("999"));
        }
    }
}
