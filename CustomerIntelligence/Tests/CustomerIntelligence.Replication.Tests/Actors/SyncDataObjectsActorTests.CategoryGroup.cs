
using NuClear.CustomerIntelligence.Storage.Model.Facts;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.CustomerIntelligence.Replication.Tests.Actors
{
    [TestFixture]
    internal partial class SyncDataObjectsActorTests
    {
        [Test]
        public void ShouldInitializeCategoryGroupIfCategoryGroupCreated()
        {
            SourceDb.Has(new Storage.Model.Erm.CategoryGroup { Id = 1, Name = "Name", Rate = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<CategoryGroup>(1)
                 .VerifyDistinct(DataObject.Created<CategoryGroup>(1));
        }

        [Test]
        public void ShouldDestroyCategoryGroupIfCategoryGroupDeleted()
        {
            TargetDb.Has(new CategoryGroup { Id = 1, Name = "Name", Rate = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<CategoryGroup>(1)
                 .VerifyDistinct(DataObject.Deleted<CategoryGroup>(1));
        }

        [Test]
        public void ShouldRecalculateCategoryGroupIfCategoryGroupUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.CategoryGroup { Id = 1, Name = "FooBar", Rate = 2 });
            TargetDb.Has(new CategoryGroup { Id = 1, Name = "Name", Rate = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<CategoryGroup>(1)
                 .VerifyDistinct(DataObject.Updated<CategoryGroup>(1));
        }

        [Test]
        public void ShouldRecalculateClientAndFirmIfCategoryGroupUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.CategoryGroup { Id = 1, Name = "Name 2", Rate = 1 })
                    .Has(new Storage.Model.Erm.CategoryOrganizationUnit { Id = 1, CategoryGroupId = 1, CategoryId = 1, OrganizationUnitId = 1 })
                    .Has(new Storage.Model.Erm.CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 })
                    .Has(new Storage.Model.Erm.FirmAddress { Id = 1, FirmId = 1 })
                    .Has(new Storage.Model.Erm.Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 })
                    .Has(new Storage.Model.Erm.Client { Id = 1 });


            TargetDb.Has(new CategoryGroup { Id = 1, Name = "Name", Rate = 1 })
                    .Has(new CategoryOrganizationUnit { Id = 1, CategoryGroupId = 1, CategoryId = 1, OrganizationUnitId = 1 })
                    .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 })
                    .Has(new FirmAddress { Id = 1, FirmId = 1 })
                    .Has(new Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 })
                    .Has(new Client { Id = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<CategoryGroup>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1),
                                 DataObject.RelatedDataObjectOutdated<Client>(1),
                                 DataObject.Updated<CategoryGroup>(1));
        }
    }
}