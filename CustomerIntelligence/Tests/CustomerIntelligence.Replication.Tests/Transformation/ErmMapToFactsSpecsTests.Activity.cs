using System;
using System.Linq;

using NuClear.River.Common.Metadata;

using NUnit.Framework;

using Erm = NuClear.CustomerIntelligence.Domain.Model.Erm;
using Specs = NuClear.CustomerIntelligence.Domain.Specifications.Specs;

namespace NuClear.CustomerIntelligence.Replication.Tests.Transformation
{
    internal partial class ErmMapToFactsSpecsTests
    {
        [Test]
        public void ShouldTransformAppointmentToActivity()
        {
            ShouldTransform<Erm::Appointment, Erm::AppointmentReference>();
        }

        [Test]
        public void ShouldTransformPhonecallToActivity()
        {
            ShouldTransform<Erm::Phonecall, Erm::PhonecallReference>();
        }

        [Test]
        public void ShouldTransformTaskToActivity()
        {
            ShouldTransform<Erm::Task, Erm::TaskReference>();
        }

        [Test]
        public void ShouldTransformLetterToActivity()
        {
            ShouldTransform<Erm::Letter, Erm::LetterReference>();
        }

        private void ShouldTransform<TActivity, TActivityReference>()
            where TActivity : Erm::ActivityBase, new()
            where TActivityReference : Erm::ActivityReference, new()
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

            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).ById(1).Count(), Is.EqualTo(1));
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).ById(1).Single().ClientId, Is.EqualTo(ClientId));
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).ById(1).Single().FirmId, Is.EqualTo(FirmId));
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).ById(1).Single().ModifiedOn, Is.EqualTo(date));

            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).ById(2).Count(), Is.EqualTo(1));
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).ById(2).Single().ClientId, Is.Null);
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).ById(2).Single().FirmId, Is.EqualTo(FirmId));
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).ById(2).Single().ModifiedOn, Is.EqualTo(date));

            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).ById(3).Count(), Is.EqualTo(1));
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).ById(3).Single().ClientId, Is.EqualTo(ClientId));
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).ById(3).Single().FirmId, Is.Null);
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).ById(3).Single().ModifiedOn, Is.EqualTo(date));

            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).ById(4).Count(), Is.EqualTo(1));
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).ById(4).Single().ClientId, Is.Null);
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).ById(4).Single().FirmId, Is.Null);
            Assert.That(Specs.Map.Erm.ToFacts.Activities.Map(Query).ById(4).Single().ModifiedOn, Is.EqualTo(date));
        }
   }
}