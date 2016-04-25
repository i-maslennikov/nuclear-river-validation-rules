using System;
using System.Collections.Generic;

using Moq;

using NuClear.CustomerIntelligence.Storage.Model.Erm;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Replication.Core.Settings;
using NuClear.Tracing.API;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.Replication.Tests.Transformation
{
    [TestFixture]
    internal class ErmFactsTransformationFixture : TransformationFixtureBase
    {
        [Test]
        public void ShouldProcessFactAccordingToPriority()
        {
            //arrange
            var factProcessor = new Mock<IFactProcessor>();
            factProcessor.Setup(x => x.Execute(It.IsAny<IReadOnlyCollection<SyncFactCommand>>()))
                         .Returns(new IOperation[0]);

            var provider = new MetadataProvider(new[] { new FactsReplicationMetadataSource() }, new IMetadataProcessor[0]);

            var factoryInvocationOrder = new List<Type>();
            var factProcessorFactory = new Mock<IFactProcessorFactory>();
            factProcessorFactory.Setup(x => x.Create(It.IsAny<IMetadataElement>()))
                                .Callback<IMetadataElement>(element =>
                                                            {
                                                                var type = element.GetType().GenericTypeArguments[0];
                                                                factoryInvocationOrder.Add(type);
                                                            })
                                .Returns(factProcessor.Object);

            var transformation = new FactsReplicator(Mock.Of<ITracer>(),
                                                     Mock.Of<IReplicationSettings>(),
                                                     provider,
                                                     factProcessorFactory.Object,
                                                     new CustomerIntelligenceFactTypePriorityComparer());

            SourceDb.Has(new Firm { Id = 2 })
                    .Has(new FirmAddress { Id = 1, FirmId = 1 }, new FirmAddress { Id = 2, FirmId = 2 });

            TargetDb.Has(new Storage.Model.Facts.Firm { Id = 1 });

            var inputOperations = new[]
                                  {
                                      new SyncFactCommand(typeof(Storage.Model.Facts.FirmAddress), 1),
                                      new SyncFactCommand(typeof(Storage.Model.Facts.Firm), 2),
                                      new SyncFactCommand(typeof(Storage.Model.Facts.FirmAddress), 2),
                                  };

            //act
            transformation.Replicate(inputOperations);

            //assert
            Assert.That(factoryInvocationOrder.Count, Is.EqualTo(2));
            Assert.That(factoryInvocationOrder[0], Is.EqualTo(typeof(Storage.Model.Facts.Firm)));
            Assert.That(factoryInvocationOrder[1], Is.EqualTo(typeof(Storage.Model.Facts.FirmAddress)));
        }
    }
}