using Moq;

using NuClear.CustomerIntelligence.OperationsProcessing.Transports.SQLStore;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;
using NuClear.River.Common.Metadata.Model.Operations;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Tests.Transports.SQLStore
{
    [TestFixture]
    public class XmlOperationSerializerTests
    {
        [Test]
        public void ShouldDealWithInitializeAggregate()
        {
            var m = new Mock<IEntityTypeParser>();
            m.Setup(x => x.Parse(It.IsAny<int>())).Returns<int>(id => new UnknownEntityType(id));
            var serializer = new XmlOperationSerializer(new EntityReferenceSerializer(m.Object));

            var pbo = serializer.Serialize(new InitializeAggregate(new EntityReference(new UnknownEntityType(1), 1L)));
            var reference = serializer.Deserialize(pbo);

            Assert.That(reference, Is.EqualTo(new InitializeAggregate(new EntityReference(new UnknownEntityType(1), 1L))));
        }

        [Test]
        public void ShouldDealWithRecalculateAggregate()
        {
            var m = new Mock<IEntityTypeParser>();
            m.Setup(x => x.Parse(It.IsAny<int>())).Returns<int>(id => new UnknownEntityType(id));
            var serializer = new XmlOperationSerializer(new EntityReferenceSerializer(m.Object));

            var pbo = serializer.Serialize(new RecalculateAggregate(new EntityReference(new UnknownEntityType(1), 1L)));
            var reference = serializer.Deserialize(pbo);

            Assert.That(reference, Is.EqualTo(new RecalculateAggregate(new EntityReference(new UnknownEntityType(1), 1L))));
        }

        [Test]
        public void ShouldDealWithDestroyAggregate()
        {
            var m = new Mock<IEntityTypeParser>();
            m.Setup(x => x.Parse(It.IsAny<int>())).Returns<int>(id => new UnknownEntityType(id));
            var serializer = new XmlOperationSerializer(new EntityReferenceSerializer(m.Object));

            var pbo = serializer.Serialize(new DestroyAggregate(new EntityReference(new UnknownEntityType(1), 1L)));
            var reference = serializer.Deserialize(pbo);

            Assert.That(reference, Is.EqualTo(new DestroyAggregate(new EntityReference(new UnknownEntityType(1), 1L))));
        }

        [Test]
        public void ShouldDealWithRecalculateAggregatePart()
        {
            var m = new Mock<IEntityTypeParser>();
            m.Setup(x => x.Parse(It.IsAny<int>())).Returns<int>(id => new UnknownEntityType(id));
            var serializer = new XmlOperationSerializer(new EntityReferenceSerializer(m.Object));

            var pbo = serializer.Serialize(new RecalculateAggregatePart(new EntityReference(new UnknownEntityType(1), 1L), new EntityReference(new UnknownEntityType(2), 2L)));
            var reference = serializer.Deserialize(pbo);

            Assert.That(reference, Is.EqualTo(new RecalculateAggregatePart(new EntityReference(new UnknownEntityType(1), 1L), new EntityReference(new UnknownEntityType(2), 2L))));
        }
    }
}
