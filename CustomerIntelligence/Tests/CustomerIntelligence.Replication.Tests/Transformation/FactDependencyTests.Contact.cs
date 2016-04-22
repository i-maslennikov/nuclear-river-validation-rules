using NuClear.CustomerIntelligence.Domain.EntityTypes;

using NUnit.Framework;

using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;
using Erm = NuClear.CustomerIntelligence.Domain.Model.Erm;
using CI = NuClear.CustomerIntelligence.Domain.Model.CI;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.CustomerIntelligence.Replication.Tests.Transformation
{
    [TestFixture]
    internal partial class FactDependencyTests
    {
        [Test]
        public void ShouldRecalulateClientIfContactCreated()
        {
            SourceDb.Has(new Erm::Contact { Id = 1, ClientId = 1 });

            TargetDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Contact>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeClient.Instance, 1));
        }

        [Test]
        public void ShouldRecalulateClientIfContactDeleted()
        {
            TargetDb.Has(new Facts::Contact { Id = 1, ClientId = 1 })
                   .Has(new Facts::Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Contact>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeClient.Instance, 1));
        }

        [Test]
        public void ShouldRecalulateClientIfContactUpdated()
        {
            SourceDb.Has(new Erm::Contact { Id = 1, ClientId = 1, Website = "asdf" });

            TargetDb.Has(new Facts::Contact { Id = 1, ClientId = 1 })
                   .Has(new Facts::Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Contact>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeClient.Instance, 1));
        }
    }
}