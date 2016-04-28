using NuClear.CustomerIntelligence.Storage.Model.Facts;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.CustomerIntelligence.Replication.Tests.Actors
{
    [TestFixture]
    internal partial class SyncDataObjectsActorTests
    {
        [Test]
        public void ShouldRecalculateClientAndFirmIfFirmAddressUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.FirmAddress { Id = 1, FirmId = 1, TerritoryId = 1 })
                    .Has(new Storage.Model.Erm.Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 })
                    .Has(new Storage.Model.Erm.Client { Id = 1 });

            TargetDb.Has(new FirmAddress { Id = 1, FirmId = 1 });
            TargetDb.Has(new Firm { Id = 1, OrganizationUnitId = 1, ClientId = 1 });
            TargetDb.Has(new Client { Id = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<FirmAddress>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(1),
                                 DataObject.RelatedDataObjectOutdated<Client>(1));
        }
    }
}