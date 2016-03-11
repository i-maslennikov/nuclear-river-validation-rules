using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using LinqToDB.Mapping;

using Microsoft.Practices.Unity;

using NuClear.Aggregates.Storage.DI.Unity;
using NuClear.CustomerIntelligence.Domain;
using NuClear.CustomerIntelligence.Domain.Model;
using NuClear.CustomerIntelligence.OperationsProcessing;
using NuClear.CustomerIntelligence.OperationsProcessing.Contexts;
using NuClear.CustomerIntelligence.Storage;
using NuClear.DI.Unity.Config;
using NuClear.Metamodeling.Domain.Processors.Concrete;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Processors.Concrete;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Metamodeling.Validators;
using NuClear.Model.Common.Entities;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Aggregates;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.Core.API.Settings;
using NuClear.Replication.Core.Facts;
using NuClear.River.Common.Metadata.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Writings;
using NuClear.Storage.Core;
using NuClear.Storage.LinqToDB;
using NuClear.Storage.LinqToDB.Connections;
using NuClear.Storage.LinqToDB.Writings;
using NuClear.Storage.Readings;
using NuClear.Tracing.API;

using Schema = NuClear.CustomerIntelligence.Storage.Schema;
using TransportSchema = NuClear.Replication.OperationsProcessing.Transports.SQLStore.Schema;

namespace NuClear.Replication.EntryPoint.DI
{
    public static partial class Bootstrapper
    {
        private static IUnityContainer ConfigureCustomerIntelligenceMetadata(this IUnityContainer container)
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
            container.RegisterOne2ManyTypesPerTypeUniqueness(typeof(IMetadataSource), typeof(FactsReplicationMetadataSource), Lifetime.Singleton);
            container.RegisterOne2ManyTypesPerTypeUniqueness(typeof(IMetadataSource), typeof(ImportDocumentMetadataSource), Lifetime.Singleton);
            container.RegisterOne2ManyTypesPerTypeUniqueness(typeof(IMetadataSource), typeof(AggregateConstructionMetadataSource), Lifetime.Singleton);
            container.RegisterOne2ManyTypesPerTypeUniqueness(typeof(IMetadataSource), typeof(StatisticsRecalculationMetadataSource), Lifetime.Singleton);

            return container;
        }

        private static IUnityContainer ConfigureCustomerIntelligenceStorage(this IUnityContainer container, ISqlStoreSettingsAspect storageSettings, Func<LifetimeManager> entryPointSpecificLifetimeManagerFactory)
        {
            // разрешаем update на таблицу состоящую только из Primary Keys
            LinqToDB.Common.Configuration.Linq.IgnoreEmptyUpdate = true;

            var transactionOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero };

            var schemaMapping = new Dictionary<string, MappingSchema>
                                {
                                    { Scope.Erm, Schema.Erm },
                                    { Scope.Facts, Schema.Facts },
                                    { Scope.CustomerIntelligence, Schema.CustomerIntelligence },
                                    { Scope.Transport, TransportSchema.Transport },
                                };

            return container
                .RegisterType<IEqualityComparerFactory, EqualityComparerFactory>(Lifetime.Singleton)
                .RegisterType<IPendingChangesHandlingStrategy, NullPendingChangesHandlingStrategy>(Lifetime.Singleton)
                .RegisterType<IStorageMappingDescriptorProvider, StorageMappingDescriptorProvider>(Lifetime.Singleton)
                .RegisterType<IEntityContainerNameResolver, DefaultEntityContainerNameResolver>(Lifetime.Singleton)
                .RegisterType<IManagedConnectionStateScopeFactory, ManagedConnectionStateScopeFactory>(Lifetime.Singleton)
                .RegisterType<IDomainContextScope, DomainContextScope>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType<ScopedDomainContextsStore>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IReadableDomainContext, CachingReadableDomainContext>(entryPointSpecificLifetimeManagerFactory())
                .RegisterInstance<ILinqToDbModelFactory>(
                    new LinqToDbModelFactory(schemaMapping, transactionOptions, storageSettings.SqlCommandTimeout), Lifetime.Singleton)
                .RegisterInstance<IObjectPropertyProvider>(
                    new LinqToDbPropertyProvider(schemaMapping.Values.ToArray()), Lifetime.Singleton)
                .RegisterType<IWritingStrategyFactory, WritingStrategyFactory>()
                .RegisterType<IReadableDomainContextFactory, LinqToDBDomainContextFactory>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IModifiableDomainContextFactory, LinqToDBDomainContextFactory>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IQuery, Query>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType(typeof(IRepository<>), typeof(LinqToDBRepository<>), entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IReadableDomainContextProvider, ReadableDomainContextProvider>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType<IModifiableDomainContextProvider, ModifiableDomainContextProvider>(entryPointSpecificLifetimeManagerFactory())
                .RegisterType(typeof(IBulkRepository<>), typeof(BulkRepository<>), entryPointSpecificLifetimeManagerFactory())
                .ConfigureReadWriteModels();
        }

        private static IUnityContainer RegisterCustomerIntelligenceContexts(this IUnityContainer container)
        {
            return container.RegisterInstance(EntityTypeMap.CreateErmContext())
                            .RegisterInstance(EntityTypeMap.CreateCustomerIntelligenceContext())
                            .RegisterInstance(EntityTypeMap.CreateFactsContext());
        }

        private static IUnityContainer RegisterCustomerIntelligenceFactsReplicator(this IUnityContainer container, Func<LifetimeManager> entryPointSpecificLifetimeManagerFactory)
        {
            return container.RegisterType<IFactsReplicator, FactsReplicator>(entryPointSpecificLifetimeManagerFactory(),
                                                                             new InjectionFactory(c => new FactsReplicator(
                                                                                                           c.Resolve<ITracer>(),
                                                                                                           c.Resolve<IReplicationSettings>(),
                                                                                                           c.Resolve<IMetadataProvider>(),
                                                                                                           c.Resolve<IFactProcessorFactory>(),
                                                                                                           new CustomerIntelligenceFactTypePriorityComparer(),
                                                                                                           new FactMetadataUriProvider())));
        }

        private static IUnityContainer RegisterCustomerIntelligenceAggregatesConstructor(this IUnityContainer container, Func<LifetimeManager> entryPointSpecificLifetimeManagerFactory)
        {
            return container.RegisterType<IAggregatesConstructor, AggregatesConstructor<CustomerIntelligenceSubDomain>>(entryPointSpecificLifetimeManagerFactory(),
                                                                             new InjectionFactory(c => new AggregatesConstructor<CustomerIntelligenceSubDomain>(
                                                                                                           c.Resolve<IMetadataProvider>(),
                                                                                                           c.Resolve<IAggregateProcessorFactory>(),
                                                                                                           c.Resolve<IEntityTypeMappingRegistry<CustomerIntelligenceSubDomain>>(),
                                                                                                           new AggregateMetadataUriProvider()
                                                                                                           )));
        }
    }
}