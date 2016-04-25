using NuClear.CustomerIntelligence.Storage.Model.Erm;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.CustomerIntelligence.Replication.Tests.Transformation
{
    [TestFixture]
    internal partial class FactDependencyTests
    {
        [Test]
        public void ShouldRecalculateClientAndFirmIfCategoryFirmAddressUpdated()
        {
            SourceDb.Has(new CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1, IsActive = false })
                 .Has(new FirmAddress { Id = 1, FirmId = 1 })
                 .Has(new Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 })
                 .Has(new Client { Id = 1 });

            TargetDb.Has(new Storage.Model.Facts.CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 });
            TargetDb.Has(new Storage.Model.Facts.FirmAddress { Id = 1, FirmId = 1 });
            TargetDb.Has(new Storage.Model.Facts.Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 });
            TargetDb.Has(new Storage.Model.Facts.Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Storage.Model.Facts.CategoryFirmAddress>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeFirm.Instance, 1),
                                          Aggregate.Recalculate(EntityTypeClient.Instance, 1));
        }
    }
}