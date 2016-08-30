using NUnit.Framework;

namespace NuClear.ValidationRules.Replication.Tests
{
    [TestFixture]
    public sealed class ResultBuilderTests
    {
        [Test]
        public void ShouldReplacePreviousValue()
        {
            int resultWithReplcedValues = new ResultBuilder().WhenSingle(Result.Info)
                                                             .WhenSingle(Result.Warning)
                                                             .WhenSingle(Result.Error);

            int simpleResult = new ResultBuilder().WhenSingle(Result.Error);

            Assert.That(resultWithReplcedValues, Is.EqualTo(simpleResult));
        }

        [TestCase(Result.Info)]
        [TestCase(Result.Warning)]
        [TestCase(Result.Error)]
        public void SingleResultShouldNotInterfere(Result r)
        {
            int result = new ResultBuilder().WhenSingle(r).WhenMass(0).WhenMassPrerelease(0).WhenMassRelease(0);
            int single = new ResultBuilder().WhenSingle(r);

            Assert.That(result ^ single, Is.EqualTo(0));
        }

        [TestCase(Result.Info)]
        [TestCase(Result.Warning)]
        [TestCase(Result.Error)]
        public void MassResultShouldNotInterfere(Result r)
        {
            int result = new ResultBuilder().WhenMass(r).WhenMassPrerelease(0).WhenMassRelease(0).WhenSingle(0);
            int mass = new ResultBuilder().WhenMass(r);

            Assert.That(result & mass, Is.EqualTo(mass));
        }

        [TestCase(Result.Info)]
        [TestCase(Result.Warning)]
        [TestCase(Result.Error)]
        public void MassPrereleaseShouldNotInterfere(Result r)
        {
            int result = new ResultBuilder().WhenMassPrerelease(r).WhenMassRelease(0).WhenSingle(0).WhenMass(0);
            int massPrerelease = new ResultBuilder().WhenMassPrerelease(r);

            Assert.That(result & massPrerelease, Is.EqualTo(massPrerelease));
        }

        [TestCase(Result.Info)]
        [TestCase(Result.Warning)]
        [TestCase(Result.Error)]
        public void MassReleaseResultShouldNotInterfere(Result r)
        {
            int result = new ResultBuilder().WhenMassRelease(r).WhenSingle(r).WhenMass(r).WhenMassPrerelease(r);
            int massRelease = new ResultBuilder().WhenMassRelease(r);

            Assert.That(result & massRelease, Is.EqualTo(massRelease));
        }
    }
}
