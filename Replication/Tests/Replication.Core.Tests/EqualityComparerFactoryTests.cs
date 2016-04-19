using System.Collections.Generic;
using System.Reflection;

using Moq;

using NuClear.Replication.Core.API.Equality;

using NUnit.Framework;

namespace NuClear.Replication.Core.Tests
{
    [TestFixture]
    public class EqualityComparerFactoryTests
    {
        class SampleEntity
        {
            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = Id;
                    hashCode = (hashCode * 397) ^ NullableId.GetHashCode();
                    hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                    return hashCode;
                }
            }

            public int Id { get; set; }
            public int? NullableId { get; set; }
            public string Name { get; set; }
        }

        [TestCase(1, 1, "abc")]
        [TestCase(1, null, "abc")]
        [TestCase(1, 1, null)]
        public void GetHashCodeShouldBeEqaulToResharperGenerated(int id, int? nullable, string name)
        {
            var provider = new Mock<IObjectPropertyProvider>();
            var props = typeof(SampleEntity).GetProperties();
            provider.Setup(x => x.GetProperties<SampleEntity>()).Returns(props);
            provider.Setup(x => x.GetPrimaryKeyProperties<SampleEntity>()).Returns(new List<PropertyInfo>());

            var factory = new EqualityComparerFactory(provider.Object);
            var comparer = factory.CreateCompleteComparer<SampleEntity>();

            var left = new SampleEntity { Id = id, NullableId = nullable, Name = name };

            Assert.That(comparer.GetHashCode(left), Is.EqualTo(left.GetHashCode()));
        }
    }
}
