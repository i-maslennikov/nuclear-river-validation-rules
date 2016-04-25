using System;

using NuClear.CustomerIntelligence.Storage.Model.Erm;
using NuClear.CustomerIntelligence.Storage.Model.Facts;

using NUnit.Framework;

using Client = NuClear.CustomerIntelligence.Storage.Model.Facts.Client;
using Firm = NuClear.CustomerIntelligence.Storage.Model.Facts.Firm;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.CustomerIntelligence.Replication.Tests.Transformation
{
    [TestFixture]
    internal partial class FactDependencyTests
    {
        private const int ActivityStatusCompleted = 2;
        private const int RegardingObjectReference = 1;

        [Test]
        public void ShouldRecalculateFirmsIfActivityUpdated()
        {
            SourceDb.Has(new Appointment { Id = 1, ModifiedOn = DateTimeOffset.Now, IsActive = true, Status = ActivityStatusCompleted })
                 .Has(new AppointmentReference { ActivityId = 1, Reference = RegardingObjectReference, ReferencedObjectId = 2, ReferencedType = EntityTypeIds.Client },
                      new AppointmentReference { ActivityId = 1, Reference = RegardingObjectReference, ReferencedObjectId = 3, ReferencedType = EntityTypeIds.Firm });

            TargetDb.Has(new Activity { Id = 1, ClientId = 2, FirmId = 3, ModifiedOn = DateTimeOffset.Now.AddDays(-1) });
            TargetDb.Has(new Client { Id = 2 });
            TargetDb.Has(new Firm { Id = 3 });
            TargetDb.Has(new Firm { Id = 4, ClientId = 2 });
            TargetDb.Has(new Firm { Id = 5 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Activity>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeFirm.Instance, 3),
                                          Aggregate.Recalculate(EntityTypeFirm.Instance, 4));
        }

        [Test]
        public void ShouldRecalculateFirmsIfActivityCreated()
        {
            SourceDb.Has(new Appointment { Id = 1, ModifiedOn = DateTimeOffset.Now, IsActive = true, Status = ActivityStatusCompleted })
                 .Has(new AppointmentReference { ActivityId = 1, Reference = RegardingObjectReference, ReferencedObjectId = 2, ReferencedType = EntityTypeIds.Client },
                      new AppointmentReference { ActivityId = 1, Reference = RegardingObjectReference, ReferencedObjectId = 3, ReferencedType = EntityTypeIds.Firm });

            TargetDb.Has(new Client { Id = 2 });
            TargetDb.Has(new Firm { Id = 3 });
            TargetDb.Has(new Firm { Id = 4, ClientId = 2 });
            TargetDb.Has(new Firm { Id = 5 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Activity>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeFirm.Instance, 3),
                                          Aggregate.Recalculate(EntityTypeFirm.Instance, 4));
        }
        [Test]
        public void ShouldRecalculateFirmsIfActivityDeleted()
        {
            TargetDb.Has(new Activity { Id = 1, ClientId = 2, FirmId = 3, ModifiedOn = DateTimeOffset.Now });
            TargetDb.Has(new Client { Id = 2 });
            TargetDb.Has(new Firm { Id = 3 });
            TargetDb.Has(new Firm { Id = 4, ClientId = 2 });
            TargetDb.Has(new Firm { Id = 5 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Activity>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeFirm.Instance, 3),
                                          Aggregate.Recalculate(EntityTypeFirm.Instance, 4));
        }
    }
}