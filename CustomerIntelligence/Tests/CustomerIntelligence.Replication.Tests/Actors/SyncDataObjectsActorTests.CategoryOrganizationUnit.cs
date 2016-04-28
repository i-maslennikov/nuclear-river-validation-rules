using NuClear.CustomerIntelligence.Storage.Model.Facts;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.CustomerIntelligence.Replication.Tests.Actors
{
    [TestFixture]
    internal partial class SyncDataObjectsActorTests
    {
        [Test]
        public void ShouldRecalulateProjectIfCategoryOrganizationUnitCreated()
        {
            SourceDb.Has(new Storage.Model.Erm.CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 1 });
            TargetDb.Has(new Project { Id = 1, OrganizationUnitId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<CategoryOrganizationUnit>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Project>(1));
        }

        [Test]
        public void ShouldRecalulateProjectIfCategoryOrganizationUnitDeleted()
        {
            TargetDb.Has(new CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                    .Has(new Project { Id = 1, OrganizationUnitId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<CategoryOrganizationUnit>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Project>(1));
        }

        [Test]
        public void ShouldRecalulateProjectIfCategoryOrganizationUnitUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 1, CategoryGroupId = 1 });

            TargetDb.Has(new CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                    .Has(new Project { Id = 1, OrganizationUnitId = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<CategoryOrganizationUnit>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Project>(1));
        }

        [Test]
        public void ShouldRecalculateClientAndFirmIfCategoryOrganizationUnitUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.CategoryOrganizationUnit { Id = 1, CategoryGroupId = 1, CategoryId = 1, OrganizationUnitId = 1, IsActive = false })
                    .Has(new Storage.Model.Erm.CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 })
                    .Has(new Storage.Model.Erm.FirmAddress { Id = 1, FirmId = 1 })
                    .Has(new Storage.Model.Erm.Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 })
                    .Has(new Storage.Model.Erm.Client { Id = 1 });

            TargetDb.Has(new CategoryOrganizationUnit { Id = 1, CategoryGroupId = 1, CategoryId = 1, OrganizationUnitId = 1 })
                    .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 })
                    .Has(new FirmAddress { Id = 1, FirmId = 1 })
                    .Has(new Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 })
                    .Has(new Client { Id = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<CategoryOrganizationUnit>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1),
                                 DataObject.RelatedDataObjectOutdated<Client>(1));
        }
    }
}