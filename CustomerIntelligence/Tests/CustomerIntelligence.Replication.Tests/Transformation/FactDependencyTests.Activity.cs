using System;

using NuClear.CustomerIntelligence.Domain.EntityTypes;
using NuClear.CustomerIntelligence.Domain.Model.Erm;
using NuClear.River.Common.Metadata;

using NUnit.Framework;

using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;

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

            TargetDb.Has(new Facts::Activity { Id = 1, ClientId = 2, FirmId = 3, ModifiedOn = DateTimeOffset.Now });
            TargetDb.Has(new Facts::Client { Id = 2 });
            TargetDb.Has(new Facts::Firm { Id = 3 });
            TargetDb.Has(new Facts::Firm { Id = 4, ClientId = 2 });
            TargetDb.Has(new Facts::Firm { Id = 5 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Activity>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeFirm.Instance, 3),
                                          Aggregate.Recalculate(EntityTypeFirm.Instance, 4));
        }

        [Test]
        public void ShouldRecalculateFirmsIfActivityCreated()
        {
            SourceDb.Has(new Appointment { Id = 1, ModifiedOn = DateTimeOffset.Now, IsActive = true, Status = ActivityStatusCompleted })
                 .Has(new AppointmentReference { ActivityId = 1, Reference = RegardingObjectReference, ReferencedObjectId = 2, ReferencedType = EntityTypeIds.Client },
                      new AppointmentReference { ActivityId = 1, Reference = RegardingObjectReference, ReferencedObjectId = 3, ReferencedType = EntityTypeIds.Firm });

            TargetDb.Has(new Facts::Client { Id = 2 });
            TargetDb.Has(new Facts::Firm { Id = 3 });
            TargetDb.Has(new Facts::Firm { Id = 4, ClientId = 2 });
            TargetDb.Has(new Facts::Firm { Id = 5 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Activity>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeFirm.Instance, 3),
                                          Aggregate.Recalculate(EntityTypeFirm.Instance, 4));
        }
        [Test]
        public void ShouldRecalculateFirmsIfActivityDeleted()
        {
            TargetDb.Has(new Facts::Activity { Id = 1, ClientId = 2, FirmId = 3, ModifiedOn = DateTimeOffset.Now });
            TargetDb.Has(new Facts::Client { Id = 2 });
            TargetDb.Has(new Facts::Firm { Id = 3 });
            TargetDb.Has(new Facts::Firm { Id = 4, ClientId = 2 });
            TargetDb.Has(new Facts::Firm { Id = 5 });

            Transformation.Create(Query, RepositoryFactory)
                          .ApplyChanges<Facts::Activity>(1)
                          .VerifyDistinct(Aggregate.Recalculate(EntityTypeFirm.Instance, 3),
                                          Aggregate.Recalculate(EntityTypeFirm.Instance, 4));
        }
    }
}