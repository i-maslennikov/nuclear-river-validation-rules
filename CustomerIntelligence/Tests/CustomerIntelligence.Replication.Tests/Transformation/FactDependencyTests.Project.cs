using NuClear.CustomerIntelligence.Storage.Model.Erm;

using NUnit.Framework;

using Firm = NuClear.CustomerIntelligence.Storage.Model.Facts.Firm;
using Territory = NuClear.CustomerIntelligence.Storage.Model.Facts.Territory;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.CustomerIntelligence.Replication.Tests.Transformation
{
    [TestFixture]
    internal partial class FactDependencyTests
    {
        [Test]
        public void ShouldInitializeProjectIfProjectCreated()
        {
            SourceDb.Has(new Project { Id = 1, OrganizationUnitId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Storage.Model.Facts.Project>(1)
                          .VerifyDistinct(Statistics.Operation(1),
                                          Aggregate.Initialize(EntityTypeProject.Instance, 1));
        }

        [Test]
        public void ShouldDestroyProjectIfProjectDeleted()
        {
            TargetDb.Has(new Storage.Model.Facts.Project { Id = 1, OrganizationUnitId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Storage.Model.Facts.Project>(1)
                          .VerifyDistinct(Statistics.Operation(1),
                                          Aggregate.Destroy(EntityTypeProject.Instance, 1));
        }

        [Test]
        public void ShouldRecalculateDependentAggregatesIfProjectUpdated()
        {
            SourceDb.Has(new Project { Id = 1, OrganizationUnitId = 2 });

            TargetDb.Has(new Storage.Model.Facts.Project { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Territory { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Territory { Id = 2, OrganizationUnitId = 2 })
                   .Has(new Firm { Id = 1, OrganizationUnitId = 1 })
                   .Has(new Firm { Id = 2, OrganizationUnitId = 2 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Storage.Model.Facts.Project>(1)
                          .VerifyDistinct(Statistics.Operation(1),
                                          Aggregate.Recalculate(EntityTypeTerritory.Instance, 1),
                                          Aggregate.Recalculate(EntityTypeFirm.Instance, 1),
                                          Aggregate.Recalculate(EntityTypeProject.Instance, 1),
                                          Aggregate.Recalculate(EntityTypeTerritory.Instance, 2),
                                          Aggregate.Recalculate(EntityTypeFirm.Instance, 2));
        }
    }
}