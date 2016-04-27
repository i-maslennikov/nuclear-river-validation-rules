using System;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.Erm;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.Replication.Tests.Specifications
{
    internal partial class ErmMapToFactsSpecsTests
    {
        [Test]
        public void ShouldTransformAppointmentToActivity()
        {
            ShouldTransform<Appointment, AppointmentReference>();
        }

        [Test]
        public void ShouldTransformPhonecallToActivity()
        {
            ShouldTransform<Phonecall, PhonecallReference>();
        }

        [Test]
        public void ShouldTransformTaskToActivity()
        {
            ShouldTransform<Task, TaskReference>();
        }

        [Test]
        public void ShouldTransformLetterToActivity()
        {
            ShouldTransform<Letter, LetterReference>();
        }

        private void ShouldTransform<TActivity, TActivityReference>()
            where TActivity : ActivityBase, new()
            where TActivityReference : ActivityReference, new()
        {
            const int ActivityStatusCompleted = 2;
            const int RegardingObjectReference = 1;

            const long FirmId = 111;
            const long ClientId = 222;
            var date = DateTimeOffset.Parse("2015-01-01");

            SourceDb.Has(new TActivity { Id = 1, ModifiedOn = date, IsActive = true, Status = ActivityStatusCompleted },
                         new TActivity { Id = 2, ModifiedOn = date, IsActive = true, Status = ActivityStatusCompleted },
                         new TActivity { Id = 3, ModifiedOn = date, IsActive = true, Status = ActivityStatusCompleted },
                         new TActivity { Id = 4, ModifiedOn = date, IsActive = true, Status = ActivityStatusCompleted })
                    .Has(new TActivityReference { ActivityId = 1, Reference = RegardingObjectReference, ReferencedObjectId = ClientId, ReferencedType = EntityTypeIds.Client },
                         new TActivityReference { ActivityId = 1, Reference = RegardingObjectReference, ReferencedObjectId = FirmId, ReferencedType = EntityTypeIds.Firm },
                         new TActivityReference { ActivityId = 2, Reference = RegardingObjectReference, ReferencedObjectId = FirmId, ReferencedType = EntityTypeIds.Firm },
                         new TActivityReference { ActivityId = 3, Reference = RegardingObjectReference, ReferencedObjectId = ClientId, ReferencedType = EntityTypeIds.Client });

            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).By(x => x.Id, 1).Count(), Is.EqualTo(1));
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).By(x => x.Id, 1).Single().ClientId, Is.EqualTo(ClientId));
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).By(x => x.Id, 1).Single().FirmId, Is.EqualTo(FirmId));
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).By(x => x.Id, 1).Single().ModifiedOn, Is.EqualTo(date));

            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).By(x => x.Id, 2).Count(), Is.EqualTo(1));
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).By(x => x.Id, 2).Single().ClientId, Is.Null);
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).By(x => x.Id, 2).Single().FirmId, Is.EqualTo(FirmId));
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).By(x => x.Id, 2).Single().ModifiedOn, Is.EqualTo(date));

            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).By(x => x.Id, 3).Count(), Is.EqualTo(1));
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).By(x => x.Id, 3).Single().ClientId, Is.EqualTo(ClientId));
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).By(x => x.Id, 3).Single().FirmId, Is.Null);
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).By(x => x.Id, 3).Single().ModifiedOn, Is.EqualTo(date));

            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).By(x => x.Id, 4).Count(), Is.EqualTo(1));
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).By(x => x.Id, 4).Single().ClientId, Is.Null);
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).By(x => x.Id, 4).Single().FirmId, Is.Null);
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).By(x => x.Id, 4).Single().ModifiedOn, Is.EqualTo(date));
        }
   }
}