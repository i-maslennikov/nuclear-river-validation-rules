using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.CustomerIntelligence.Storage.Model.Bit;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.CustomerIntelligence.Replication.Tests.Actors
{
    [TestFixture]
    internal partial class SyncDataObjectsActorTests
    {
        [Test]
        public void ShouldInitializeProjectIfProjectCreated()
        {
            SourceDb.Has(new Storage.Model.Erm.Project { Id = 1, OrganizationUnitId = 2 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Project>(1)
                 .VerifyDistinct(DataObject.Created<Project>(1));
        }

        [Test]
        public void ShouldDestroyProjectIfProjectDeleted()
        {
            TargetDb.Has(new Project { Id = 1, OrganizationUnitId = 2 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Project>(1)
                 .VerifyDistinct(DataObject.Deleted<Project>(1));
        }

        [Test]
        public void ShouldRecalculateDependentAggregatesIfProjectUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.Project { Id = 1, OrganizationUnitId = 2 });

            TargetDb.Has(new Project { Id = 1, OrganizationUnitId = 1 })
                    .Has(new Territory { Id = 1, OrganizationUnitId = 1 })
                    .Has(new Territory { Id = 2, OrganizationUnitId = 2 })
                    .Has(new Firm { Id = 1, OrganizationUnitId = 1 })
                    .Has(new Firm { Id = 2, OrganizationUnitId = 2 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Project>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Territory>(1),
                                 DataObject.RelatedDataObjectOutdated<Firm>(1),
                                 DataObject.RelatedDataObjectOutdated<Territory>(2),
                                 DataObject.RelatedDataObjectOutdated<Firm>(2),
                                 DataObject.Updated<Project>(1));
        }
    }
}