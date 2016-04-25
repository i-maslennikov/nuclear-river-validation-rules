using NuClear.CustomerIntelligence.Storage.Model.Erm;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.CustomerIntelligence.Replication.Tests.Transformation
{
    [TestFixture]
    internal partial class FactDependencyTests
    {
        [Test]
        public void ShouldInitializeCategoryGroupIfCategoryGroupCreated()
        {
            SourceDb.Has(
                new CategoryGroup { Id = 1, Name = "Name", Rate = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Storage.Model.Facts.CategoryGroup>(1)
                          .VerifyDistinct(Aggregate.Initialize(EntityTypeCategoryGroup.Instance, 1));
        }

        [Test]
        public void ShouldDestroyCategoryGroupIfCategoryGroupDeleted()
        {
            TargetDb.Has(
                new Storage.Model.Facts.CategoryGroup { Id = 1, Name = "Name", Rate = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Storage.Model.Facts.CategoryGroup>(1)
                          .VerifyDistinct(Aggregate.Destroy(EntityTypeCategoryGroup.Instance, 1));
        }

        [Test]
        public void ShouldRecalculateCategoryGroupIfCategoryGroupUpdated()
        {
            SourceDb.Has(
                new CategoryGroup { Id = 1, Name = "FooBar", Rate = 2 });
            TargetDb.Has(
                new Storage.Model.Facts.CategoryGroup { Id = 1, Name = "Name", Rate = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Storage.Model.Facts.CategoryGroup>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeCategoryGroup.Instance, 1));
        }

        [Test]
        public void ShouldRecalculateClientAndFirmIfCategoryGroupUpdated()
        {
            SourceDb.Has(new CategoryGroup { Id = 1, Name = "Name 2", Rate = 1 })
                 .Has(new CategoryOrganizationUnit { Id = 1, CategoryGroupId = 1, CategoryId = 1, OrganizationUnitId = 1 })
                 .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 })
                 .Has(new FirmAddress { Id = 1, FirmId = 1 })
                 .Has(new Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 })
                 .Has(new Client { Id = 1 });


            TargetDb.Has(new Storage.Model.Facts.CategoryGroup { Id = 1, Name = "Name", Rate = 1 })
                   .Has(new Storage.Model.Facts.CategoryOrganizationUnit { Id = 1, CategoryGroupId = 1, CategoryId = 1, OrganizationUnitId = 1 })
                   .Has(new Storage.Model.Facts.CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 })
                   .Has(new Storage.Model.Facts.FirmAddress { Id = 1, FirmId = 1 })
                   .Has(new Storage.Model.Facts.Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 })
                   .Has(new Storage.Model.Facts.Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Storage.Model.Facts.CategoryGroup>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeFirm.Instance, 1),
                                          Aggregate.Recalculate(EntityTypeClient.Instance, 1),
                                          Aggregate.Recalculate(EntityTypeCategoryGroup.Instance, 1));
        }
    }
}