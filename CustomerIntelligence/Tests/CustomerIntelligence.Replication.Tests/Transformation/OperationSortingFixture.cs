using System.Linq;

using NuClear.CustomerIntelligence.Domain.Model;
using NuClear.CustomerIntelligence.Domain.Model.Facts;
using NuClear.Model.Common;
using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata.Model.Operations;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.Replication.Tests.Transformation
{
    [TestFixture]
    internal class OperationSortingFixture
    {
        [Test]
        public void ShouldSortAggregationOperationsAccordingPriority()
        {
            var comparer = new AggregateOperationPriorityComparer();
            var type = new SampleEntityType();
            var data = new AggregateOperation[]
                       {
                           new DestroyAggregate(type, 0),
                           new InitializeAggregate(type, 0),
                           new RecalculateAggregate(type, 0),
                       };

            var sortedData = data.OrderByDescending(x => x.GetType(), comparer).ToArray();

            Assert.That(sortedData[0], Is.InstanceOf<DestroyAggregate>());
            Assert.That(sortedData[1], Is.InstanceOf<InitializeAggregate>());
            Assert.That(sortedData[2], Is.InstanceOf<RecalculateAggregate>());
        }

        [Test]
        public void ShouldSortFactTypesAccordingPriority()
        {
            var comparer = new CustomerIntelligenceFactTypePriorityComparer();
            var data = new[] { typeof(Client), typeof(Project), typeof(object) };

            var sortedData = data.OrderByDescending(x => x, comparer).ToArray();

            Assert.That(sortedData[0], Is.EqualTo(typeof(Project)));
            Assert.That(sortedData[1], Is.EqualTo(typeof(Client)));
            Assert.That(sortedData[2], Is.EqualTo(typeof(object)));
        }

        internal class SampleEntityType : IEntityType
        {
            public bool Equals(IIdentity other)
            {
                return ReferenceEquals(this, other);
            }

            public int Id { get; } = 1;
            public string Description { get; } = "SampleEntityType";
        }
    }
}