using Moq;

using NuClear.CustomerIntelligence.OperationsProcessing.Transports.SQLStore;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus;
using NuClear.River.Common.Metadata.Model.Operations;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Tests.Transports.SQLStore
{
    [TestFixture]
    class EntityReferenceSerializerTests
    {
        [Test]
        public void ShouldDealWithLongKeys()
        {
            var m = new Mock<IEntityTypeParser>();
            m.Setup(x => x.Parse(It.IsAny<int>())).Returns<int>(id => new UnknownEntityType(id));
            var serializer = new EntityReferenceSerializer(m.Object);

            var xml = serializer.Serialize("entity", new EntityReference(new UnknownEntityType(42), 123456789L));
            var reference = serializer.Deserialize(xml);

            Assert.That(reference.EntityType.Id, Is.EqualTo(42));
            Assert.That(reference.EntityKey, Is.InstanceOf<long>());
            Assert.That(reference.EntityKey, Is.EqualTo(123456789));
        }

        [Test]
        public void ShouldDealWithStatisticsKeys()
        {
            var m = new Mock<IEntityTypeParser>();
            m.Setup(x => x.Parse(It.IsAny<int>())).Returns<int>(id => new UnknownEntityType(id));
            var serializer = new EntityReferenceSerializer(m.Object);

            var xml = serializer.Serialize("entity", new EntityReference(new UnknownEntityType(42), new StatisticsKey {CategoryId = 123456789, ProjectId = 987654321}));
            var reference = serializer.Deserialize(xml);

            Assert.That(reference.EntityType.Id, Is.EqualTo(42));
            Assert.That(reference.EntityKey, Is.InstanceOf<StatisticsKey>());
            Assert.That(reference.EntityKey, Is.EqualTo(new StatisticsKey { CategoryId = 123456789, ProjectId = 987654321 }));
        }
    }
}
