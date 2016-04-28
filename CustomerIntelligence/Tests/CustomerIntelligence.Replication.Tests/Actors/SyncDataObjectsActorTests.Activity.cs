using System;

using NuClear.CustomerIntelligence.Storage.Model.Facts;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.CustomerIntelligence.Replication.Tests.Actors
{
    [TestFixture]
    internal partial class SyncDataObjectsActorTests
    {
        private const int ActivityStatusCompleted = 2;
        private const int RegardingObjectReference = 1;

        [Test]
        public void ShouldRecalculateFirmsIfActivityUpdated()
        {
            SourceDb.Has(new Storage.Model.Erm.Appointment { Id = 1, ModifiedOn = DateTimeOffset.Now, IsActive = true, Status = ActivityStatusCompleted })
                    .Has(new Storage.Model.Erm.AppointmentReference { ActivityId = 1, Reference = RegardingObjectReference, ReferencedObjectId = 2, ReferencedType = EntityTypeIds.Client },
                         new Storage.Model.Erm.AppointmentReference { ActivityId = 1, Reference = RegardingObjectReference, ReferencedObjectId = 3, ReferencedType = EntityTypeIds.Firm });

            TargetDb.Has(new Activity { Id = 1, ClientId = 2, FirmId = 3, ModifiedOn = DateTimeOffset.Now.AddDays(-1) })
                    .Has(new Client { Id = 2 })
                    .Has(new Firm { Id = 3 })
                    .Has(new Firm { Id = 4, ClientId = 2 })
                    .Has(new Firm { Id = 5 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Activity>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(3),
                                 DataObject.RelatedDataObjectOutdated<Firm>(4));
        }

        [Test]
        public void ShouldRecalculateFirmsIfActivityCreated()
        {
            SourceDb.Has(new Storage.Model.Erm.Appointment { Id = 1, ModifiedOn = DateTimeOffset.Now, IsActive = true, Status = ActivityStatusCompleted })
                    .Has(new Storage.Model.Erm.AppointmentReference { ActivityId = 1, Reference = RegardingObjectReference, ReferencedObjectId = 2, ReferencedType = EntityTypeIds.Client },
                         new Storage.Model.Erm.AppointmentReference { ActivityId = 1, Reference = RegardingObjectReference, ReferencedObjectId = 3, ReferencedType = EntityTypeIds.Firm });

            TargetDb.Has(new Client { Id = 2 })
                    .Has(new Firm { Id = 3 })
                    .Has(new Firm { Id = 4, ClientId = 2 })
                    .Has(new Firm { Id = 5 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Activity>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(3),
                                 DataObject.RelatedDataObjectOutdated<Firm>(4));
        }
        [Test]
        public void ShouldRecalculateFirmsIfActivityDeleted()
        {
            TargetDb.Has(new Activity { Id = 1, ClientId = 2, FirmId = 3, ModifiedOn = DateTimeOffset.Now })
                    .Has(new Client { Id = 2 })
                    .Has(new Firm { Id = 3 })
                    .Has(new Firm { Id = 4, ClientId = 2 })
                    .Has(new Firm { Id = 5 });

            Actor.Create(Query, RepositoryFactory)
                 .Sync<Activity>(1)
                 .VerifyDistinct(DataObject.RelatedDataObjectOutdated<Firm>(3),
                                 DataObject.RelatedDataObjectOutdated<Firm>(4));
        }
    }
}