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
        public void ShouldRecalculateClientAndFirmIfCategoryFirmAddressUpdated()
        {
            SourceDb.Has(new Erm::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1, IsActive = false })
                 .Has(new Erm::FirmAddress { Id = 1, FirmId = 1 })
                 .Has(new Erm::Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 })
                 .Has(new Erm::Client { Id = 1 });

            TargetDb.Has(new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 1 });
            TargetDb.Has(new Facts::FirmAddress { Id = 1, FirmId = 1 });
            TargetDb.Has(new Facts::Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 });
            TargetDb.Has(new Facts::Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::CategoryFirmAddress>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeFirm.Instance, 1),
                                          Aggregate.Recalculate(EntityTypeClient.Instance, 1));
        }
    }
}