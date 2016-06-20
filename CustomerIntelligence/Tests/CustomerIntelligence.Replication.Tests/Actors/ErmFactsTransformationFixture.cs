using System;
using System.Collections.Generic;
using System.Linq;

using Moq;

using NuClear.CustomerIntelligence.Replication.Accessors;
using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.CustomerIntelligence.Storage;
using NuClear.CustomerIntelligence.Storage.Model.Erm;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.Commands;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.Replication.Tests.Actors
{
    [TestFixture]
    internal class ErmFactsTransformationFixture : DataFixtureBase
    {
        [Test]
        public void ShouldCreateActorsAccordingToPriority()
        {
            //arrange
            var factoryInvocationOrder = new List<Type>();
            var actorFactory = new Mock<IDataObjectsActorFactory>();
            actorFactory.Setup(x => x.Create())
                        .Callback(() =>
                                      {
                                          var dataObjectTypesProvider = new DataObjectTypesProvider();
                                          factoryInvocationOrder.AddRange(dataObjectTypesProvider.Get<ISyncDataObjectCommand>());
                                      })
                        .Returns(Array.Empty<IActor>());

            //act
            actorFactory.Object.Create();

            //assert
            Assert.That(factoryInvocationOrder.Count, Is.EqualTo(17));
            Assert.That(factoryInvocationOrder[0], Is.EqualTo(typeof(Storage.Model.Facts.Project)));
            Assert.That(factoryInvocationOrder[1], Is.EqualTo(typeof(Storage.Model.Facts.Category)));
            Assert.That(factoryInvocationOrder[2], Is.EqualTo(typeof(Storage.Model.Facts.CategoryGroup)));
            Assert.That(factoryInvocationOrder[3], Is.EqualTo(typeof(Storage.Model.Facts.Territory)));
            Assert.That(factoryInvocationOrder[4], Is.EqualTo(typeof(Storage.Model.Facts.Client)));
            Assert.That(factoryInvocationOrder[5], Is.EqualTo(typeof(Storage.Model.Facts.Firm)));
            Assert.That(factoryInvocationOrder[6], Is.EqualTo(typeof(Storage.Model.Facts.Account)));
            Assert.That(factoryInvocationOrder[7], Is.EqualTo(typeof(Storage.Model.Facts.Activity)));
            Assert.That(factoryInvocationOrder[8], Is.EqualTo(typeof(Storage.Model.Facts.CategoryFirmAddress)));
            Assert.That(factoryInvocationOrder[9], Is.EqualTo(typeof(Storage.Model.Facts.CategoryOrganizationUnit)));
            Assert.That(factoryInvocationOrder[10], Is.EqualTo(typeof(Storage.Model.Facts.Contact)));
            Assert.That(factoryInvocationOrder[11], Is.EqualTo(typeof(Storage.Model.Facts.FirmAddress)));
            Assert.That(factoryInvocationOrder[12], Is.EqualTo(typeof(Storage.Model.Facts.FirmContact)));
            Assert.That(factoryInvocationOrder[13], Is.EqualTo(typeof(Storage.Model.Facts.LegalPerson)));
            Assert.That(factoryInvocationOrder[14], Is.EqualTo(typeof(Storage.Model.Facts.Order)));
            Assert.That(factoryInvocationOrder[15], Is.EqualTo(typeof(Storage.Model.Facts.SalesModelCategoryRestriction)));
            Assert.That(factoryInvocationOrder[16], Is.EqualTo(typeof(Storage.Model.Facts.Lead)));
        }

        [Test]
        public void ShouldExecuteDataObjectTypeSpecificCommandsOnly()
        {
            var accessor = new FirmAccessor(Query);
            var comparerFactory = new EqualityComparerFactory(new LinqToDbPropertyProvider(Schema.Erm, Schema.Facts, Schema.CustomerIntelligence));
            var actor = new SyncDataObjectsActor<Storage.Model.Facts.Firm>(Query,
                                                                           RepositoryFactory.Create<Storage.Model.Facts.Firm>(),
                                                                           comparerFactory,
                                                                           accessor,
                                                                           accessor);

            SourceDb.Has(new Firm { Id = 2 })
                    .Has(new FirmAddress { Id = 1, FirmId = 1 }, new FirmAddress { Id = 2, FirmId = 2 });

            TargetDb.Has(new Storage.Model.Facts.Firm { Id = 1 });

            var commands = new[]
                {
                    new SyncDataObjectCommand(typeof(Storage.Model.Facts.FirmAddress), 1),
                    new SyncDataObjectCommand(typeof(Storage.Model.Facts.Firm), 2),
                    new SyncDataObjectCommand(typeof(Storage.Model.Facts.FirmAddress), 2),
                };

            //act
            var events = actor.ExecuteCommands(commands);

            //assert
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events.Single(), Is.EqualTo(new DataObjectCreatedEvent(typeof(Storage.Model.Facts.Firm), 2)));
        }
    }
}