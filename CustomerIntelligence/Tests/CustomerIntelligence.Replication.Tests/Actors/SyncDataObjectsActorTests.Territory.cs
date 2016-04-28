using NuClear.CustomerIntelligence.Storage.Model.Facts;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.CustomerIntelligence.Replication.Tests.Actors
{
    [TestFixture]
    internal partial class SyncDataObjectsActorTests
    {
        [Test]
        public void ShouldInitializeTerritoryIfTerritoryCreated()
        {
            SourceDb.Has(new Storage.Model.Erm.Territory { Id = 1, OrganizationUnitId = 2 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Territory>(1)
                 .VerifyDistinct(DataObject.Created<Territory>(1));
        }

        [Test]
        public void ShouldDestroyTerritoryIfTerritoryDeleted()
        {
            TargetDb.Has(new Territory { Id = 1, OrganizationUnitId = 2 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Territory>(1)
                 .VerifyDistinct(DataObject.Deleted<Territory>(1));
        }

        [Test]
        public void ShouldRecalculateTerritoryIfTerritoryUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.Territory { Id = 1, OrganizationUnitId = 2 });
            TargetDb.Has(new Territory { Id = 1, OrganizationUnitId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Territory>(1)
                 .VerifyDistinct(DataObject.Updated<Territory>(1));
        }
    }
}