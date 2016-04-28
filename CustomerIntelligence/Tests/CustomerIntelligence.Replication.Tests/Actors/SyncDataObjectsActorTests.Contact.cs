using NuClear.CustomerIntelligence.Storage.Model.Facts;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.CustomerIntelligence.Replication.Tests.Actors
{
    [TestFixture]
    internal partial class SyncDataObjectsActorTests
    {
        [Test]
        public void ShouldRecalulateClientIfContactCreated()
        {
            SourceDb.Has(new Storage.Model.Erm.Contact { Id = 1, ClientId = 1 });
            TargetDb.Has(new Client { Id = 1 });

            Actor.Create(Query, RepositoryFactory)
                          .Sync<Contact>(1)
                          .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Client>(1));
        }

        [Test]
        public void ShouldRecalulateClientIfContactDeleted()
        {
            TargetDb.Has(new Contact { Id = 1, ClientId = 1 })
                    .Has(new Client { Id = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Contact>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Client>(1));
        }

        [Test]
        public void ShouldRecalulateClientIfContactUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.Contact { Id = 1, ClientId = 1, Website = "asdf" });

            TargetDb.Has(new Contact { Id = 1, ClientId = 1 })
                    .Has(new Client { Id = 1 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Contact>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Client>(1));
        }
    }
}