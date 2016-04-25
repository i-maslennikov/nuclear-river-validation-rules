using NuClear.CustomerIntelligence.Storage.Model.Erm;

using NUnit.Framework;

using Project = NuClear.CustomerIntelligence.Storage.Model.Facts.Project;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.CustomerIntelligence.Replication.Tests.Transformation
{
    [TestFixture]
    internal partial class FactDependencyTests
    {
        [Test]
        public void ShouldRecalulateProjectIfCategoryOrganizationUnitCreated()
        {
            SourceDb.Has(new CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 1 });

            TargetDb.Has(new Project { Id = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Storage.Model.Facts.CategoryOrganizationUnit>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeProject.Instance, 1));
        }

        [Test]
        public void ShouldRecalulateProjectIfCategoryOrganizationUnitDeleted()
        {
            TargetDb.Has(new Storage.Model.Facts.CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Project { Id = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Storage.Model.Facts.CategoryOrganizationUnit>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeProject.Instance, 1));
        }

        [Test]
        public void ShouldRecalulateProjectIfCategoryOrganizationUnitUpdated()
        {
            SourceDb.Has(new CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 1, CategoryGroupId = 1 });

            TargetDb.Has(new Storage.Model.Facts.CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Project { Id = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Storage.Model.Facts.CategoryOrganizationUnit>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeProject.Instance, 1));
        }

        [Test]
        public void ShouldRecalculateClientAndFirmIfCategoryOrganizationUnitUpdated()
        {
            SourceDb.Has(new CategoryOrganizationUnit { Id = 1, CategoryGroupId = 1, CategoryId = 1, OrganizationUnitId = 1, IsActive = false })
                 .Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 })
                 .Has(new FirmAddress { Id = 1, FirmId = 1 })
                 .Has(new Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 })
                 .Has(new Client { Id = 1 });

            TargetDb.Has(new Storage.Model.Facts.CategoryOrganizationUnit { Id = 1, CategoryGroupId = 1, CategoryId = 1, OrganizationUnitId = 1 });
            TargetDb.Has(new Storage.Model.Facts.CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 });
            TargetDb.Has(new Storage.Model.Facts.FirmAddress { Id = 1, FirmId = 1 });
            TargetDb.Has(new Storage.Model.Facts.Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 });
            TargetDb.Has(new Storage.Model.Facts.Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Storage.Model.Facts.CategoryOrganizationUnit>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeFirm.Instance, 1),
                                          Aggregate.Recalculate(EntityTypeClient.Instance, 1));
        }
    }
}