using NuClear.CustomerIntelligence.Storage.Model.Erm;

using NUnit.Framework;

using Client = NuClear.CustomerIntelligence.Storage.Model.Facts.Client;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.CustomerIntelligence.Replication.Tests.Transformation
{
    [TestFixture]
    internal partial class FactDependencyTests
    {
        [Test]
        public void ShouldRecalulateClientIfContactCreated()
        {
            SourceDb.Has(new Contact { Id = 1, ClientId = 1 });

            TargetDb.Has(new Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Storage.Model.Facts.Contact>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeClient.Instance, 1));
        }

        [Test]
        public void ShouldRecalulateClientIfContactDeleted()
        {
            TargetDb.Has(new Storage.Model.Facts.Contact { Id = 1, ClientId = 1 })
                   .Has(new Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Storage.Model.Facts.Contact>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeClient.Instance, 1));
        }

        [Test]
        public void ShouldRecalulateClientIfContactUpdated()
        {
            SourceDb.Has(new Contact { Id = 1, ClientId = 1, Website = "asdf" });

            TargetDb.Has(new Storage.Model.Facts.Contact { Id = 1, ClientId = 1 })
                   .Has(new Client { Id = 1 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Storage.Model.Facts.Contact>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeClient.Instance, 1));
        }
    }
}