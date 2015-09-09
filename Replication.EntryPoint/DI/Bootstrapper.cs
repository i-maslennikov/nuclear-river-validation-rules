﻿using System;
using System.Collections.Generic;
using System.Transactions;

using LinqToDB.Mapping;

using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Replication.API.Identitites.Connections;
using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates;
using NuClear.AdvancedSearch.Replication.API.Transforming.Facts;
using NuClear.AdvancedSearch.Replication.API.Transforming.Statistics;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.EntryPoint.Factories;
using NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Messaging.Processor;
using NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Messaging.Receiver;
using NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Messaging.Transformer;
using NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Replication;
using NuClear.AdvancedSearch.Replication.EntryPoint.Settings;
using NuClear.AdvancedSearch.Settings;
using NuClear.Aggregates.Storage.DI.Unity;
using NuClear.Assembling.TypeProcessing;
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
using NuClear.Metamodeling.Domain.Processors.Concrete;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Processors.Concrete;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Metamodeling.Validators;
using NuClear.Model.Common.Operations.Identity;
using NuClear.OperationsLogging.Transports.ServiceBus.Serialization.ProtoBuf;
using NuClear.OperationsProcessing.Transports.ServiceBus.Primary;
using NuClear.Replication.OperationsProcessing.Final;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;
using NuClear.Replication.OperationsProcessing.Metadata.Model;
using NuClear.Replication.OperationsProcessing.Metadata.Operations;
using NuClear.Replication.OperationsProcessing.Performance;
using NuClear.Replication.OperationsProcessing.Transports.CorporateBus;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;
using NuClear.Security;
using NuClear.Security.API;
using NuClear.Security.API.UserContext;
using NuClear.Security.API.UserContext.Identity;
using NuClear.Settings.API;
using NuClear.Settings.Unity;
using NuClear.Storage.ConnectionStrings;
using NuClear.Storage.Core;
using NuClear.Storage.LinqToDB;
using NuClear.Storage.LinqToDB.Connections;
using NuClear.Storage.Readings;
using NuClear.Storage.UseCases;
using NuClear.Storage.Writings;
using NuClear.Telemetry;
using NuClear.Tracing.API;
using NuClear.WCF.Client;
using NuClear.WCF.Client.Config;

using Quartz.Spi;

using Schema = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Schema;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.DI
{
    public static partial class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(ISettingsContainer settingsContainer, ITracer tracer, ITracerContextManager tracerContextManager)
        {
            EntityTypeMap.Initialize();

            IUnityContainer container = new UnityContainer();
            var massProcessors = new IMassProcessor[]
                                 {
                                     new TaskServiceJobsMassProcessor(container),
                                 };
            var storageSettings = settingsContainer.AsSettings<ISqlSettingsAspect>();

            container.AttachQueryableContainerExtension()
                     .UseParameterResolvers(ParameterResolvers.Defaults)
                     .ConfigureMetadata()
                     .ConfigureSettingsAspects(settingsContainer)
                     .ConfigureTracing(tracer, tracerContextManager)
                     .ConfigureSecurityAspects()
                     .ConfigureQuartz()
                     .ConfigureIdentityInfrastructure()
                     .ConfigureWcf()
                     .ConfigureOperationsProcessing()
                     .ConfigureStorage(storageSettings, EntryPointSpecificLifetimeManagerFactory)
                     .ConfigureLinq2Db();

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

        private static IUnityContainer ConfigureMetadata(this IUnityContainer container)
        {
            // provider
            container.RegisterType<IMetadataProvider, MetadataProvider>(Lifetime.Singleton);

            // processors
            container.RegisterOne2ManyTypesPerTypeUniqueness<IMetadataProcessor, ReferencesEvaluatorProcessor>(Lifetime.Singleton);
            container.RegisterOne2ManyTypesPerTypeUniqueness<IMetadataProcessor, Feature2PropertiesLinkerProcessor>(Lifetime.Singleton);

            // validators
            container.RegisterType<IMetadataValidatorsSuite, MetadataValidatorsSuite>(Lifetime.Singleton);

            // register matadata sources without massprocessor
            container.RegisterOne2ManyTypesPerTypeUniqueness(typeof(IMetadataSource), typeof(PerformedOperationsMessageFlowsMetadataSource), Lifetime.Singleton);

            container.RegisterType<API.Transforming.IMetadataSource<IFactInfo>, ErmFactsTransformationMetadata>();
            container.RegisterType<API.Transforming.IMetadataSource<IAggregateInfo>, CustomerIntelligenceTransformationMetadata>();
            container.RegisterType<API.Transforming.IMetadataSource<IStatisticsInfo>, StatisticsFinalTransformationMetadata>();

            return container;
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
                     .RegisterTypeWithDependencies(typeof(ServiceBusOperationsReceiverTelemetryDecorator), Lifetime.PerScope, null)
                     .RegisterOne2ManyTypesPerTypeUniqueness<IRuntimeTypeModelConfigurator, ProtoBufTypeModelForTrackedUseCaseConfigurator>(Lifetime.Singleton)
                     .RegisterOne2ManyTypesPerTypeUniqueness<IRuntimeTypeModelConfigurator, TrackedUseCaseConfigurator>(Lifetime.Singleton)
                     .RegisterTypeWithDependencies(typeof(BinaryEntireBrokeredMessage2TrackedUseCaseTransformer), Lifetime.Singleton, null);

            // final
            container.RegisterTypeWithDependencies(typeof(SqlStoreReceiverTelemetryDecorator), Lifetime.PerScope, null)
                     .RegisterTypeWithDependencies(typeof(AggregateOperationAggregatableMessageHandler), Lifetime.PerResolve, null);


            return container.RegisterInstance<IParentContainerUsedRegistrationsContainer>(new ParentContainerUsedRegistrationsContainer(typeof(IUserContext)), Lifetime.Singleton)
                            .RegisterType(typeof(ServiceBusMessageFlowReceiver), Lifetime.Singleton)
                            .RegisterType(typeof(CorporateBusMessageFlowReceiver), Lifetime.PerResolve)
                            .RegisterType<IServiceBusLockRenewer, ReceivedMessagesLockRenewalManager>(Lifetime.Singleton)
                            .RegisterType<ICorporateBusMessageFlowReceiverFactory, UnityCorporateBusMessageFlowReceiverFactory>(Lifetime.PerScope)
                            .RegisterType<IServiceBusMessageFlowReceiverFactory, UnityServiceBusMessageFlowReceiverFactory>(Lifetime.PerScope)
                            .RegisterType<IMessageProcessingStagesFactory, UnityMessageProcessingStagesFactory>(Lifetime.PerScope)
                            .RegisterType<IMessageFlowProcessorFactory, UnityMessageFlowProcessorFactory>(Lifetime.PerScope)
                            .RegisterType<IMessageReceiverFactory, UnityMessageReceiverFactory>(Lifetime.PerScope)

                            .RegisterOne2ManyTypesPerTypeUniqueness<IMessageFlowProcessorResolveStrategy, PrimaryProcessorResolveStrategy>(Lifetime.Singleton)
                            .RegisterOne2ManyTypesPerTypeUniqueness<IMessageFlowProcessorResolveStrategy, FinalProcessorResolveStrategy>(Lifetime.PerScope)

                            .RegisterOne2ManyTypesPerTypeUniqueness<IMessageFlowReceiverResolveStrategy, CorporateBusReceiverResolveStrategy>(Lifetime.PerScope)
                            .RegisterOne2ManyTypesPerTypeUniqueness<IMessageFlowReceiverResolveStrategy, ServiceBusReceiverResolveStrategy>(Lifetime.PerScope)
                            .RegisterOne2ManyTypesPerTypeUniqueness<IMessageFlowReceiverResolveStrategy, FinalReceiverResolveStrategy>(Lifetime.PerScope)

                            .RegisterType<IMessageValidatorFactory, UnityMessageValidatorFactory>(Lifetime.PerScope)
                            .RegisterType<IMessageTransformerFactory, UnityMessageTransformerFactory>(Lifetime.PerScope)

                            .RegisterOne2ManyTypesPerTypeUniqueness<IMessageTransformerResolveStrategy, PrimaryMessageTransformerResolveStrategy>(Lifetime.PerScope)
                            .RegisterType<IMessageProcessingHandlerFactory, UnityMessageProcessingHandlerFactory>(Lifetime.PerScope)
                            .RegisterType<IMessageProcessingContextAccumulatorFactory, UnityMessageProcessingContextAccumulatorFactory>(Lifetime.PerScope);
        }

        private static IUnityContainer ConfigureStorage(this IUnityContainer container, ISqlSettingsAspect storageSettings, Func<LifetimeManager> entryPointSpecificLifetimeManagerFactory)
        {
            var transactionOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero };

            return container
                .RegisterType<IPendingChangesHandlingStrategy, NullPendingChangesHandlingStrategy>(Lifetime.Singleton)
                .RegisterType<IStorageMappingDescriptorProvider, StorageMappingDescriptorProvider>(Lifetime.Singleton)
                .RegisterType<IEntityContainerNameResolver, DefaultEntityContainerNameResolver>(Lifetime.Singleton)
                .RegisterType<IManagedConnectionStateScopeFactory, ManagedConnectionStateScopeFactory>(Lifetime.Singleton)
                .RegisterType<IDomainContextScope, DomainContextScope>(EntryPointSpecificLifetimeManagerFactory())
                .RegisterType<ScopedDomainContextsStore>(EntryPointSpecificLifetimeManagerFactory())
                .RegisterType<IReadableDomainContext, CachingReadableDomainContext>(EntryPointSpecificLifetimeManagerFactory())
                .RegisterType<IProcessingContext, ProcessingContext>(entryPointSpecificLifetimeManagerFactory())
                .RegisterInstance<ILinqToDbModelFactory>(new LinqToDbModelFactory(new Dictionary<string, MappingSchema>
                                                                                  {
                                                                                      { Scope.Erm, Schema.Erm },
                                                                                      { Scope.Facts, Schema.Facts },
                                                                                      { Scope.CustomerIntelligence, Schema.CustomerIntelligence },
                                                                                      { Scope.Transport, NuClear.Replication.OperationsProcessing.Transports.SQLStore.Schema.Transport },
                                                                                  },
                                                                                  transactionOptions,
                                                                                  storageSettings.SqlCommandTimeout),
                                                         Lifetime.Singleton)
                .RegisterType<IReadableDomainContextFactory, LinqToDBDomainContextFactory>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IModifiableDomainContextFactory, LinqToDBDomainContextFactory>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IQuery, Query>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType(typeof(IRepository<>), typeof(LinqToDBRepository<>), entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IReadableDomainContextProvider, ReadableDomainContextProvider>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IModifiableDomainContextProvider, ModifiableDomainContextProvider>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IAggregateProcessorFactory, UnityAggregateProcessorFactory>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IFactDependencyProcessorFactory, UnityFactDependencyProcessorFactory>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IStatisticsProcessorFactory, UnityStatisticsProcessorFactory>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IValueObjectProcessorFactory, UnityValueObjectProcessorFactory>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IFactProcessorFactory, UnityFactProcessorFactory>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType(typeof(IDataChangesApplier<>), typeof(DataChangesApplier<>), entryPointSpecificLifetimeManagerFactory())
                .ConfigureReadWriteModels();
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
    }
}