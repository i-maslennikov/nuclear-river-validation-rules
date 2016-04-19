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
        public void ShouldRecalulateProjectIfCategoryOrganizationUnitCreated()
        {
            SourceDb.Has(new Erm::CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 1 });

            TargetDb.Has(new Facts::Project { Id = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::CategoryOrganizationUnit>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeProject.Instance, 1));
        }

        [Test]
        public void ShouldRecalulateProjectIfCategoryOrganizationUnitDeleted()
        {
            TargetDb.Has(new Facts::CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Project { Id = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::CategoryOrganizationUnit>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeProject.Instance, 1));
        }

        [Test]
        public void ShouldRecalulateProjectIfCategoryOrganizationUnitUpdated()
        {
            SourceDb.Has(new Erm::CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 1, CategoryGroupId = 1 });

            TargetDb.Has(new Facts::CategoryOrganizationUnit { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Facts::Project { Id = 1, OrganizationUnitId = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::CategoryOrganizationUnit>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeProject.Instance, 1));
        }

        [Test]
        public void ShouldRecalculateClientAndFirmIfCategoryOrganizationUnitUpdated()
        {
            SourceDb.Has(new Erm::CategoryOrganizationUnit { Id = 1, CategoryGroupId = 1, CategoryId = 1, OrganizationUnitId = 1, IsActive = false })
                 .Has(new Erm::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 })
                 .Has(new Erm::FirmAddress { Id = 1, FirmId = 1 })
                 .Has(new Erm::Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 })
                 .Has(new Erm::Client { Id = 1 });

            TargetDb.Has(new Facts::CategoryOrganizationUnit { Id = 1, CategoryGroupId = 1, CategoryId = 1, OrganizationUnitId = 1 });
            TargetDb.Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 });
            TargetDb.Has(new Facts::FirmAddress { Id = 1, FirmId = 1 });
            TargetDb.Has(new Facts::Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 });
            TargetDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::CategoryOrganizationUnit>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeFirm.Instance, 1),
                                          Aggregate.Recalculate(EntityTypeClient.Instance, 1));
        }
    }
}