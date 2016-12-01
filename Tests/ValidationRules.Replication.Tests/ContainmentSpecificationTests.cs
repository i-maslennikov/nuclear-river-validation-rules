using System.Linq;

using NuClear.Storage.API.Specifications;

using NUnit.Framework;

namespace NuClear.ValidationRules.Replication.Tests
{
    [TestFixture]
    public sealed class ContainmentSpecificationTests
    {
        private const int MsSqlBatchLimitation = 5000;

        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(MsSqlBatchLimitation, 1)]
        [TestCase(MsSqlBatchLimitation + 1, 2)]
        public void TestBatches(int idCount, int batchCount)
        {
            var spec = Specification<Foo>.Create(foo => foo.Id, Enumerable.Repeat(0, idCount).ToArray());
            var batchSpecs = ((IBatchableSpecification<Foo>)spec).SplitToBatches();
            Assert.AreEqual(batchCount, batchSpecs.Count);
        }

        [Test]
        public void TestFilter()
        {
            var spec = Specification<Foo>.Create(foo => foo.Id, Enumerable.Range(0, MsSqlBatchLimitation).ToArray());
            var batchSpec = ((IBatchableSpecification<Foo>)spec).SplitToBatches().Single();
            var items = new []
                {
                    new Foo { Id = -1 },
                    new Foo { Id = 0 },
                    new Foo { Id = MsSqlBatchLimitation - 1 },
                    new Foo { Id = MsSqlBatchLimitation },
                };

            var filteredItems = items.AsQueryable().Where(batchSpec);

            Assert.AreEqual(0, filteredItems.Count(x => x.Id == -1));
            Assert.AreEqual(1, filteredItems.Count(x => x.Id == 0));
            Assert.AreEqual(1, filteredItems.Count(x => x.Id == MsSqlBatchLimitation - 1));
            Assert.AreEqual(0, filteredItems.Count(x => x.Id == MsSqlBatchLimitation));
        }

        class Foo
        {
            public int Id { get; set; }
        }
    }
}
