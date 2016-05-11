using NuClear.CustomerIntelligence.OperationsProcessing.Transports.SQLStore;
using NuClear.CustomerIntelligence.Replication;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.CustomerIntelligence.Storage.Model.Facts;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Tests.Transports.SQLStore
{
    [TestFixture]
    public class XmlOperationSerializerTests
    {
        [Test]
        public void ShouldWorkWithDataObjectCreatedEvent()
        {
            var serializer = new XmlEventSerializer();
            var @event = new DataObjectCreatedEvent(typeof(Firm), 1L);

            var serialized = serializer.Serialize(@event);
            var deserialized = serializer.Deserialize(serialized);

            Assert.That(deserialized, Is.EqualTo(@event));
        }

        [Test]
        public void ShouldWorkWithDataObjectUpdatedEvent()
        {
            var serializer = new XmlEventSerializer();
            var @event = new DataObjectUpdatedEvent(typeof(Firm), 1L);

            var serialized = serializer.Serialize(@event);
            var deserialized = serializer.Deserialize(serialized);

            Assert.That(deserialized, Is.EqualTo(@event));
        }

        [Test]
        public void ShouldWorkWithDataObjectDeletedEvent()
        {
            var serializer = new XmlEventSerializer();
            var @event = new DataObjectDeletedEvent(typeof(Firm), 1L);

            var serialized = serializer.Serialize(@event);
            var deserialized = serializer.Deserialize(serialized);

            Assert.That(deserialized, Is.EqualTo(@event));
        }

        [Test]
        public void ShouldWorkWithDataObjectReplacedEvent()
        {
            var serializer = new XmlEventSerializer();
            var @event = new DataObjectReplacedEvent(typeof(Firm), 1L);

            var serialized = serializer.Serialize(@event);
            var deserialized = serializer.Deserialize(serialized);

            Assert.That(deserialized, Is.EqualTo(@event));
        }

        [Test]
        public void ShouldWorkWithRelatedDataObjectOutdatedEventOfLong()
        {
            var serializer = new XmlEventSerializer();
            var @event = new RelatedDataObjectOutdatedEvent<long>(typeof(Firm), 1L);

            var serialized = serializer.Serialize(@event);
            var deserialized = serializer.Deserialize(serialized);

            Assert.That(deserialized, Is.EqualTo(@event));
        }

        [Test]
        public void ShouldWorkWithRelatedDataObjectOutdatedEventOfStatisticsKey()
        {
            var serializer = new XmlEventSerializer();
            var @event = new RelatedDataObjectOutdatedEvent<StatisticsKey>(typeof(Firm), new StatisticsKey { ProjectId = 1L, CategoryId = 1L });

            var serialized = serializer.Serialize(@event);
            var deserialized = serializer.Deserialize(serialized);

            Assert.That(deserialized, Is.EqualTo(@event));
        }
    }
}
