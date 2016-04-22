using System;
using System.Collections.Generic;

using Moq;

using NuClear.CustomerIntelligence.Domain;
using NuClear.CustomerIntelligence.Domain.Model;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.Core.API.Settings;
using NuClear.Replication.Core.Facts;
using NuClear.Tracing.API;

using NUnit.Framework;

using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;
using Erm = NuClear.CustomerIntelligence.Domain.Model.Erm;

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

            SourceDb.Has(new Erm::Firm { Id = 2 })
                    .Has(new Erm::FirmAddress { Id = 1, FirmId = 1 }, new Erm::FirmAddress { Id = 2, FirmId = 2 });

            TargetDb.Has(new Facts::Firm { Id = 1 });

            var inputOperations = new[]
                                  {
                                      new SyncFactCommand(typeof(Facts::FirmAddress), 1),
                                      new SyncFactCommand(typeof(Facts::Firm), 2),
                                      new SyncFactCommand(typeof(Facts::FirmAddress), 2),
                                  };

            //act
            transformation.Replicate(inputOperations);

            //assert
            Assert.That(factoryInvocationOrder.Count, Is.EqualTo(2));
            Assert.That(factoryInvocationOrder[0], Is.EqualTo(typeof(Facts::Firm)));
            Assert.That(factoryInvocationOrder[1], Is.EqualTo(typeof(Facts::FirmAddress)));
        }
    }
}