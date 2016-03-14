using System.Linq;

using NuClear.CustomerIntelligence.Domain.Model;
using NuClear.CustomerIntelligence.Domain.Model.Facts;
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
            var data = new AggregateOperation[]
                       {
                           new DestroyAggregate(0, 0),
                           new InitializeAggregate(0, 0),
                           new RecalculateAggregate(0, 0),
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
    }
}