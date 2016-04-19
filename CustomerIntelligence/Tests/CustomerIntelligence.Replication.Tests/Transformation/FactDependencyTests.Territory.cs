using NuClear.CustomerIntelligence.Domain.EntityTypes;

using NUnit.Framework;

using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;
using Erm = NuClear.CustomerIntelligence.Domain.Model.Erm;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.CustomerIntelligence.Replication.Tests.Transformation
{
    [TestFixture]
    internal partial class FactDependencyTests
    {
        [Test]
        public void ShouldInitializeTerritoryIfTerritoryCreated()
        {
            SourceDb.Has(new Erm::Territory { Id = 1, OrganizationUnitId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Territory>(1)
                          .VerifyDistinct(Aggregate.Initialize(EntityTypeTerritory.Instance, 1));
        }

        [Test]
        public void ShouldDestroyTerritoryIfTerritoryDeleted()
        {
            TargetDb.Has(new Facts::Territory { Id = 1, OrganizationUnitId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Territory>(1)
                          .VerifyDistinct(Aggregate.Destroy(EntityTypeTerritory.Instance, 1));
        }

        [Test]
        public void ShouldRecalculateTerritoryIfTerritoryUpdated()
        {
            SourceDb.Has(new Erm::Territory { Id = 1, OrganizationUnitId = 2 });
            TargetDb.Has(new Facts::Territory { Id = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Territory>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeTerritory.Instance, 1));
        }
    }
}