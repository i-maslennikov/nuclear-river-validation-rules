using Microsoft.Practices.Unity;

using Moq;

using NuClear.CustomerIntelligence.OperationsProcessing.Tests.Mocks.Receiver;
using NuClear.DI.Unity.Config;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Actors.Transformers;
using NuClear.Messaging.API.Processing.Processors;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Messaging.API.Receivers;
using NuClear.Messaging.DI.Factories.Unity.Accumulators;
using NuClear.Messaging.DI.Factories.Unity.Common;
using NuClear.Messaging.DI.Factories.Unity.Handlers;
using NuClear.Messaging.DI.Factories.Unity.Processors;
using NuClear.Messaging.DI.Factories.Unity.Processors.Resolvers;
using NuClear.Messaging.DI.Factories.Unity.Receivers;
using NuClear.Messaging.DI.Factories.Unity.Receivers.Resolvers;
using NuClear.Messaging.DI.Factories.Unity.Stages;
using NuClear.Messaging.DI.Factories.Unity.Transformers;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.OperationsProcessing.API;
using NuClear.OperationsProcessing.API.Primary;
using NuClear.Replication.OperationsProcessing.Metadata;
using NuClear.Replication.OperationsProcessing.Primary;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Tests.DI
{
    public static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(this IUnityContainer container, MockMessageReceiver receiver, MessageProcessingStage[] stages)
        {
            var settings = new PerformedOperationsPrimaryFlowProcessorSettings { AppropriatedStages = stages };

            var metadataProvider = new MetadataProvider(new IMetadataSource[]
            {
                new PerformedOperationsMessageFlowsMetadataSource(),
                new OperationRegistryMetadataSource()
            },
            new IMetadataProcessor[]
            {
                new TunedReferencesEvaluatorProcessor()
            });

            return container
                        .RegisterContexts()
                        .RegisterType(typeof(IOperationRegistry<>), typeof(OperationRegistry<>), Lifetime.Singleton)
                        .RegisterType<IEntityTypeExplicitMapping, ErmToFactsEntityTypeExplicitMapping>(Lifetime.Singleton)
                        .RegisterInstance(Mock.Of<ITelemetryPublisher>())
                        .RegisterType<ITracer, NullTracer>()
                        .RegisterInstance<IMetadataProvider>(metadataProvider)
                        .RegisterInstance<IPerformedOperationsFlowProcessorSettings>(settings)
                        .RegisterInstance(receiver)
                        .RegisterType<IMessageReceiverFactory, UnityMessageReceiverFactory>(Lifetime.Singleton)
                        .RegisterType<IMessageFlowProcessorResolveStrategy, Mocks.Processor.PrimaryProcessorResolveStrategy>("primary", Lifetime.Singleton)
                        .RegisterType<IMessageFlowReceiverResolveStrategy, MockReceiverResolveStrategy>("primary", Lifetime.Singleton)
                        .RegisterType<IMessageProcessingStagesFactory, UnityMessageProcessingStagesFactory>(Lifetime.Singleton)
                        .RegisterType<IMessageTransformerFactory, UnityMessageTransformerFactory>(Lifetime.Singleton)
                        .RegisterType<IMessageFlowProcessorFactory, UnityMessageFlowProcessorFactory>(Lifetime.Singleton)
                        .RegisterType<IParentContainerUsedRegistrationsContainer, ParentContainerUsedRegistrationsContainer>(Lifetime.Singleton)
                        .RegisterType<IMessageProcessingHandlerFactory, UnityMessageProcessingHandlerFactory>(Lifetime.PerScope)
                        .RegisterType<IMessageProcessingContextAccumulatorFactory, UnityMessageProcessingContextAccumulatorFactory>(Lifetime.PerScope);
        }

        private static IUnityContainer RegisterContexts(this IUnityContainer container)
        {
            return container.RegisterInstance(EntityTypeMap.CreateErmContext())
                            .RegisterInstance(EntityTypeMap.CreateCustomerIntelligenceContext())
                            .RegisterInstance(EntityTypeMap.CreateFactsContext());
        }
    }
}
