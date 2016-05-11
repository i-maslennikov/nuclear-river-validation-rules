using System;
using System.Collections.Generic;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;

using Microsoft.Practices.Unity;

using NuClear.Aggregates.Storage.DI.Unity;
using NuClear.CustomerIntelligence.Storage;
using NuClear.CustomerIntelligence.Storage.Identitites.Connections;
using NuClear.DI.Unity.Config;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Querying.Edm;
using NuClear.Querying.Edm.EF;
using NuClear.Querying.Edm.Emit;
using NuClear.Querying.Http;
using NuClear.Querying.Http.Emit;
using NuClear.Querying.Storage;
using NuClear.River.Hosting.Common.Identities.Connections;
using NuClear.River.Hosting.Common.Settings;
using NuClear.Settings.API;
using NuClear.Settings.Unity;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.Storage.API.Feedback;
using NuClear.Storage.API.Readings;
using NuClear.Storage.Core;
using NuClear.Storage.EntityFramework;
using NuClear.Storage.Readings;
using NuClear.Tracing.API;
using NuClear.Tracing.Environment;
using NuClear.Tracing.Log4Net;
using NuClear.Tracing.Log4Net.Config;

namespace NuClear.CustomerIntelligence.Querying.Host.DI
{
    internal static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(ISettingsContainer settingsContainer)
        {
            var container = new UnityContainer()
                .ConfigureTracer(settingsContainer.AsSettings<IEnvironmentSettings>(), settingsContainer.AsSettings<IConnectionStringSettings>())
                .ConfigureSettingsAspects(settingsContainer)
                .ConfigureMetadata()
                .ConfigureEdmModels()
                .ConfigureStorage(EntryPointSpecificLifetimeManagerFactory)
                .ConfigureWebApiOData();

            return container;
        }

        private static LifetimeManager EntryPointSpecificLifetimeManagerFactory()
        {
            return Lifetime.PerScope;
        }

        private static IUnityContainer ConfigureTracer(
            this IUnityContainer container,
            IEnvironmentSettings environmentSettings,
            IConnectionStringSettings connectionStringSettings)
        {
            var tracerContextEntryProviders =
                    new ITracerContextEntryProvider[]
                    {
                        new TracerContextConstEntryProvider(TracerContextKeys.Required.Environment, environmentSettings.EnvironmentName),
                        new TracerContextConstEntryProvider(TracerContextKeys.Required.EntryPoint, environmentSettings.EntryPointName),
                        new TracerContextConstEntryProvider(TracerContextKeys.Required.EntryPointHost, NetworkInfo.ComputerFQDN),
                        new TracerContextConstEntryProvider(TracerContextKeys.Required.EntryPointInstanceId, Guid.NewGuid().ToString()),
                        new TracerContextSelfHostedEntryProvider(TracerContextKeys.Required.UserAccount) // TODO {all, 08.05.2015}: Если появится авторизация, надо будет доработать логирование
                    };

            var tracerContextManager = new TracerContextManager(tracerContextEntryProviders);
            var tracer = Log4NetTracerBuilder.Use
                                             .DefaultXmlConfig
                                             .EventLog
                                             .Logstash(new Uri(connectionStringSettings.GetConnectionString(LoggingConnectionStringIdentity.Instance)))
                                             .Build;

            return container.RegisterInstance(tracer)
                            .RegisterInstance(tracerContextManager)
                            .RegisterType<IExceptionLogger, ExceptionTracer>("log4net", Lifetime.Singleton);
        }

        private static IUnityContainer ConfigureMetadata(this IUnityContainer container)
        {
            var metadataSources = new IMetadataSource[]
            {
                new QueryingMetadataSource()
            };

            var metadataProcessors = new IMetadataProcessor[] { };

            return container.RegisterType<IMetadataProvider, MetadataProvider>(Lifetime.Singleton, new InjectionConstructor(metadataSources, metadataProcessors));
        }

        private static IUnityContainer ConfigureEdmModels(this IUnityContainer container)
        {
            return container.RegisterType<IEdmModelAnnotator, EdmModelAnnotator>(Lifetime.Singleton)
                            .RegisterType<IEdmModelBuilder, EdmModelBuilder>(Lifetime.Singleton)
                            .RegisterType<IClrTypeBuilder, EmitClrTypeResolver>(Lifetime.Singleton)
                            .RegisterType<IClrTypeProvider, EmitClrTypeResolver>(Lifetime.Singleton);
        }

        private static IUnityContainer ConfigureStorage(
            this IUnityContainer container,
            Func<LifetimeManager> entryPointSpecificLifetimeManagerFactory)
        {
            return container
                        .RegisterType<IPendingChangesHandlingStrategy, NullPendingChangesHandlingStrategy>(Lifetime.Singleton)
                        .RegisterType<IDurationEstimator, NullDurationEstimator>(Lifetime.Singleton)
                        .RegisterType<IProducedQueryLogAccessor, NullProducedQueryLogAccessor>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IPersistedChangesTracker, NullPersistedChangesTracker>(Lifetime.Singleton)
                        .RegisterType<IEFDbModelFactory, DbModelFactory>(Lifetime.Singleton)
                        .RegisterType<IEntityContainerNameResolver, CustomerIntelligenceEntityContainerNameResolver>(Lifetime.Singleton)
                        .RegisterType<IStorageMappingDescriptorProvider, StorageMappingDescriptorProvider>(Lifetime.Singleton)
                        .RegisterType<IReadableDomainContextFactory, EFDomainContextFactory>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IModifiableDomainContextFactory, EFDomainContextFactory>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IReadableDomainContext, CachingReadableDomainContext>(entryPointSpecificLifetimeManagerFactory())

                        .RegisterType<IDomainContextScope, DomainContextScope>(entryPointSpecificLifetimeManagerFactory())
                        .RegisterType<IReadableDomainContextProvider, ReadableDomainContextProvider>(entryPointSpecificLifetimeManagerFactory())

                        .RegisterType<IQuery, Query>(Lifetime.PerResolve)
                        .ConfigureReadWriteModels();
        }

        private static IUnityContainer ConfigureReadWriteModels(this IUnityContainer container)
        {
            var readConnectionStringNameMap = new Dictionary<string, IConnectionStringIdentity>
                {
                    { CustomerIntelligenceEntityContainer.Name, CustomerIntelligenceConnectionStringIdentity.Instance },
                };

            var writeConnectionStringNameMap = new Dictionary<string, IConnectionStringIdentity>();

            return container.RegisterInstance<IConnectionStringIdentityResolver>(new ConnectionStringIdentityResolver(readConnectionStringNameMap, writeConnectionStringNameMap));
        }

        private static IUnityContainer ConfigureWebApiOData(this IUnityContainer container)
        {
            return container
                .RegisterType<IDynamicAssembliesRegistry, DynamicAssembliesRegistry>(Lifetime.Singleton)
                .RegisterType<IDynamicAssembliesResolver, DynamicAssembliesRegistry>(Lifetime.Singleton)

                // custom IHttpControllerTypeResolver
                .RegisterType<IHttpControllerTypeResolver, ControllerTypeResolver>(Lifetime.Singleton);
        }
    }
}