using System;
using System.Collections.Generic;

using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Common.Identities.Connections;
using NuClear.AdvancedSearch.Common.Metadata.Model.Operations;
using NuClear.Assembling.TypeProcessing;
using NuClear.CustomerIntelligence.Domain.Model;
using NuClear.CustomerIntelligence.OperationsProcessing;
using NuClear.CustomerIntelligence.OperationsProcessing.Contexts;
using NuClear.CustomerIntelligence.OperationsProcessing.Final;
using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.CustomerIntelligence.OperationsProcessing.Transports.SQLStore;
using NuClear.CustomerIntelligence.Storage.Identitites.Connections;
using NuClear.DI.Unity.Config;
using NuClear.DI.Unity.Config.RegistrationResolvers;
using NuClear.IdentityService.Client.Interaction;
using NuClear.Jobs.Schedulers;
using NuClear.Jobs.Unity;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Actors.Transformers;
using NuClear.Messaging.API.Processing.Actors.Validators;
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
using NuClear.Messaging.DI.Factories.Unity.Transformers.Resolvers;
using NuClear.Messaging.DI.Factories.Unity.Validators;
using NuClear.Messaging.Transports.CorporateBus;
using NuClear.Messaging.Transports.CorporateBus.API;
using NuClear.Messaging.Transports.ServiceBus;
using NuClear.Messaging.Transports.ServiceBus.API;
using NuClear.Messaging.Transports.ServiceBus.LockRenewer;
using NuClear.Metamodeling.Provider;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.OperationsLogging.Transports.ServiceBus.Serialization.ProtoBuf;
using NuClear.OperationsProcessing.Transports.ServiceBus.Primary;
using NuClear.Replication.Core.Aggregates;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.Core.API.Settings;
using NuClear.Replication.Core.Facts;
using NuClear.Replication.EntryPoint.Factories;
using NuClear.Replication.EntryPoint.Factories.Messaging.Processor;
using NuClear.Replication.EntryPoint.Factories.Messaging.Receiver;
using NuClear.Replication.EntryPoint.Factories.Messaging.Transformer;
using NuClear.Replication.EntryPoint.Factories.Replication;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.Replication.OperationsProcessing.Transports.CorporateBus;
using NuClear.Replication.OperationsProcessing.Transports.File;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;
using NuClear.Security.API;
using NuClear.Security.API.UserContext;
using NuClear.Security.API.UserContext.Identity;
using NuClear.Security;
using NuClear.Settings.API;
using NuClear.Settings.Unity;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.Telemetry;
using NuClear.Tracing.API;
using NuClear.WCF.Client;
using NuClear.WCF.Client.Config;

using Quartz.Spi;

namespace NuClear.Replication.EntryPoint.DI
{
    public static partial class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(ISettingsContainer settingsContainer, ITracer tracer, ITracerContextManager tracerContextManager)
        {
            IUnityContainer container = new UnityContainer();
            var massProcessors = new IMassProcessor[]
                                 {
                                     new TaskServiceJobsMassProcessor(container),
                                 };
            var storageSettings = settingsContainer.AsSettings<ISqlStoreSettingsAspect>();

            container.RegisterValidationRulesContexts()
                     .AttachQueryableContainerExtension()
                     .UseParameterResolvers(ParameterResolvers.Defaults)
                     .ConfigureValidationRulesMetadata()
                     .ConfigureSettingsAspects(settingsContainer)
                     .ConfigureTracing(tracer, tracerContextManager)
                     .ConfigureSecurityAspects()
                     .ConfigureQuartz()
                     .ConfigureIdentityInfrastructure()
                     .ConfigureWcf()
                     .ConfigureOperationsProcessing()
                     .ConfigureValidationRulesStorage(storageSettings, EntryPointSpecificLifetimeManagerFactory)
                     .ConfigureReplication(EntryPointSpecificLifetimeManagerFactory);

            ReplicationRoot.Instance.PerformTypesMassProcessing(massProcessors, true, typeof(object));

            return container;
        }

        private static LifetimeManager EntryPointSpecificLifetimeManagerFactory()
        {
            return Lifetime.PerScope;
        }

        private static IUnityContainer ConfigureTracing(this IUnityContainer container, ITracer tracer, ITracerContextManager tracerContextManager)
        {
            return container.RegisterInstance(tracer)
                            .RegisterInstance(tracerContextManager);
        }

        private static IUnityContainer ConfigureSecurityAspects(this IUnityContainer container)
        {
            return container
                .RegisterType<IUserAuthenticationService, NullUserAuthenticationService>(Lifetime.PerScope)
                .RegisterType<IUserProfileService, NullUserProfileService>(Lifetime.PerScope)
                .RegisterType<IUserContext, UserContext>(Lifetime.PerScope, new InjectionFactory(c => new UserContext(null, null)))
                .RegisterType<IUserLogonAuditor, LoggerContextUserLogonAuditor>(Lifetime.Singleton)
                .RegisterType<IUserIdentityLogonService, UserIdentityLogonService>(Lifetime.PerScope)
                .RegisterType<ISignInService, WindowsIdentitySignInService>(Lifetime.PerScope)
                .RegisterType<IUserImpersonationService, UserImpersonationService>(Lifetime.PerScope);
        }

        private static IUnityContainer ConfigureQuartz(this IUnityContainer container)
        {
            return container
                .RegisterType<IJobFactory, JobFactory>(Lifetime.Singleton, new InjectionConstructor(container.Resolve<UnityJobFactory>(), container.Resolve<ITracer>()))
                .RegisterType<IJobStoreFactory, JobStoreFactory>(Lifetime.Singleton)
                .RegisterType<ISchedulerManager, SchedulerManager>(Lifetime.Singleton);
        }

        private static IUnityContainer ConfigureWcf(this IUnityContainer container)
        {
            return container
                .RegisterType<IServiceClientSettingsProvider, ServiceClientSettingsProvider>(Lifetime.Singleton)
                .RegisterType<IClientProxyFactory, ClientProxyFactory>(Lifetime.Singleton);
        }

        private static IUnityContainer ConfigureIdentityInfrastructure(this IUnityContainer container)
        {
            return container.RegisterType<IIdentityGenerator, IdentityGenerator>(Lifetime.Singleton)
                            .RegisterType<IIdentityServiceClient, IdentityServiceClient>(Lifetime.Singleton);
        }

        private static IUnityContainer ConfigureOperationsProcessing(this IUnityContainer container)
        {
            container.RegisterType<IOperationIdentityRegistry, OperationIdentityRegistry>(Lifetime.Singleton,
                new InjectionConstructor(OperationIdentityMetadata.AllOperationIdentities));

#if DEBUG
            container.RegisterType<ITelemetryPublisher, DebugTelemetryPublisher>(Lifetime.Singleton);
#else
            container.RegisterType<ITelemetryPublisher, LogstashTelemetryPublisher>(Lifetime.Singleton);
#endif

            // primary
            container.RegisterTypeWithDependencies(typeof(CorporateBusOperationsReceiver), Lifetime.PerScope, null)
                     .RegisterTypeWithDependencies(typeof(FileReceiver), Lifetime.PerScope, null)
                     .RegisterTypeWithDependencies(typeof(ServiceBusOperationsReceiverTelemetryDecorator), Lifetime.PerScope, null)
                     .RegisterOne2ManyTypesPerTypeUniqueness<IRuntimeTypeModelConfigurator, ProtoBufTypeModelForTrackedUseCaseConfigurator>(Lifetime.Singleton)
                     .RegisterValidationRulesTrackedUseCaseConfigurator()
                     .RegisterTypeWithDependencies(typeof(BinaryEntireBrokeredMessage2TrackedUseCaseTransformer), Lifetime.Singleton, null)
                     .RegisterType<IOperationSender<AggregateOperation>, SqlStoreSender<AggregateOperation, AggregatesFlow>>(Lifetime.PerScope)
                     .RegisterType<IOperationSender<RecalculateStatisticsOperation>, SqlStoreSender<RecalculateStatisticsOperation, StatisticsFlow>>(Lifetime.PerScope)
                     .RegisterValidationRulesAggregateOperationSerializer()
                     .RegisterType<IOperationSerializer<RecalculateStatisticsOperation>, StatisticsOperationSerializer>();

            // final
            container.RegisterTypeWithDependencies(typeof(SqlStoreReceiverTelemetryDecorator), Lifetime.PerScope, null)
                     .RegisterTypeWithDependencies(typeof(AggregateOperationAggregatableMessageHandler), Lifetime.PerResolve, null);


            return container.RegisterInstance<IParentContainerUsedRegistrationsContainer>(new ParentContainerUsedRegistrationsContainer(typeof(IUserContext)), Lifetime.Singleton)
                            .RegisterType(typeof(ServiceBusMessageFlowReceiver), Lifetime.Singleton)
                            .RegisterType(typeof(CorporateBusMessageFlowReceiver), Lifetime.PerResolve)
                            .RegisterType<IServiceBusLockRenewer, NullServiceBusLockRenewer>(Lifetime.Singleton)
                            .RegisterType<ICorporateBusMessageFlowReceiverFactory, UnityCorporateBusMessageFlowReceiverFactory>(Lifetime.PerScope)
                            .RegisterType<IServiceBusMessageFlowReceiverFactory, UnityServiceBusMessageFlowReceiverFactory>(Lifetime.PerScope)
                            .RegisterType<IMessageProcessingStagesFactory, UnityMessageProcessingStagesFactory>(Lifetime.PerScope)
                            .RegisterType<IMessageFlowProcessorFactory, UnityMessageFlowProcessorFactory>(Lifetime.PerScope)
                            .RegisterType<IMessageReceiverFactory, UnityMessageReceiverFactory>(Lifetime.PerScope)

                            .RegisterOne2ManyTypesPerTypeUniqueness<IMessageFlowProcessorResolveStrategy, PrimaryProcessorResolveStrategy>(Lifetime.Singleton)
                            .RegisterOne2ManyTypesPerTypeUniqueness<IMessageFlowProcessorResolveStrategy, FinalProcessorResolveStrategy>(Lifetime.PerScope)

                            .RegisterOne2ManyTypesPerTypeUniqueness<IMessageFlowReceiverResolveStrategy, CorporateBusReceiverResolveStrategy>(Lifetime.PerScope)
                            .RegisterOne2ManyTypesPerTypeUniqueness<IMessageFlowReceiverResolveStrategy, FileReceiverResolveStrategy>(Lifetime.PerScope)
                            .RegisterOne2ManyTypesPerTypeUniqueness<IMessageFlowReceiverResolveStrategy, ServiceBusReceiverResolveStrategy>(Lifetime.PerScope)
                            .RegisterOne2ManyTypesPerTypeUniqueness<IMessageFlowReceiverResolveStrategy, FinalReceiverResolveStrategy>(Lifetime.PerScope)

                            .RegisterType<IMessageValidatorFactory, UnityMessageValidatorFactory>(Lifetime.PerScope)
                            .RegisterType<IMessageTransformerFactory, UnityMessageTransformerFactory>(Lifetime.PerScope)

                            .RegisterOne2ManyTypesPerTypeUniqueness<IMessageTransformerResolveStrategy, PrimaryMessageTransformerResolveStrategy>(Lifetime.PerScope)
                            .RegisterType<IMessageProcessingHandlerFactory, UnityMessageProcessingHandlerFactory>(Lifetime.PerScope)
                            .RegisterType<IMessageProcessingContextAccumulatorFactory, UnityMessageProcessingContextAccumulatorFactory>(Lifetime.PerScope);
        }

        private static IUnityContainer ConfigureReplication(this IUnityContainer container, Func<LifetimeManager> entryPointSpecificLifetimeManagerFactory)
        {
            return container
                .RegisterValidationRulesFactsReplicator(entryPointSpecificLifetimeManagerFactory)
                .RegisterType<IStatisticsImporterFactory, UnityStatisticsImporterFactory>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IAggregatesConstructor, AggregatesConstructor>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IStatisticsRecalculator, StatisticsRecalculator>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IAggregateProcessorFactory, UnityAggregateProcessorFactory>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IFactDependencyProcessorFactory, UnityFactDependencyProcessorFactory>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IStatisticsProcessorFactory, UnityStatisticsProcessorFactory>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IValueObjectProcessorFactory, UnityValueObjectProcessorFactory>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IFactProcessorFactory, UnityFactProcessorFactory>(entryPointSpecificLifetimeManagerFactory());

        }

        private static IUnityContainer ConfigureReadWriteModels(this IUnityContainer container)
        {
            var readConnectionStringNameMap = new Dictionary<string, IConnectionStringIdentity>
                {
                    { Scope.Erm, ErmConnectionStringIdentity.Instance },
                    { Scope.Facts, FactsConnectionStringIdentity.Instance },
                    { Scope.CustomerIntelligence, CustomerIntelligenceConnectionStringIdentity.Instance },
                    { Scope.Transport, TransportConnectionStringIdentity.Instance }
                };

            var writeConnectionStringNameMap = new Dictionary<string, IConnectionStringIdentity>
                {
                    { Scope.Facts, FactsConnectionStringIdentity.Instance },
                    { Scope.CustomerIntelligence, CustomerIntelligenceConnectionStringIdentity.Instance },
                    { Scope.Transport, TransportConnectionStringIdentity.Instance }
                };

            return container.RegisterInstance<IConnectionStringIdentityResolver>(new ConnectionStringIdentityResolver(readConnectionStringNameMap, writeConnectionStringNameMap));
        }

        private static class Scope
        {
            public const string Erm = "Erm";
            public const string Facts = "Facts";
            public const string CustomerIntelligence = "CustomerIntelligence";
            public const string Transport = "Transport";
        }
    }
}