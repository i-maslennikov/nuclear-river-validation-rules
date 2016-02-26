using System;
using System.Net.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;
using System.Web.OData.Extensions;

using Microsoft.Practices.Unity;

using NuClear.CustomerIntelligence.Domain;
using NuClear.DI.Unity.Config;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Querying.Edm.Edmx;
using NuClear.Querying.Edm.Emit;
using NuClear.Querying.Storage;
using NuClear.Querying.Web.OData.DynamicControllers;
using NuClear.River.Common.Identities.Connections;
using NuClear.River.Common.Settings;
using NuClear.Settings.API;
using NuClear.Settings.Unity;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.Storage.API.Readings;
using NuClear.Tracing.API;
using NuClear.Tracing.Environment;
using NuClear.Tracing.Log4Net;
using NuClear.Tracing.Log4Net.Config;

namespace NuClear.Querying.Web.OData.DI
{
    internal static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(ISettingsContainer settingsContainer)
        {
            var container = new UnityContainer()
                .ConfigureSettingsAspects(settingsContainer)
                .ConfigureMetadata()
                .ConfigureStoreModel()
                .ConfigureWebApiOData()
                .ConfigureTracer(settingsContainer.AsSettings<IEnvironmentSettings>(), settingsContainer.AsSettings<IConnectionStringSettings>())
                .ConfigureWebApiTracer();

            return container;
        }

        public static IUnityContainer ConfigureTracer(
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
                            .RegisterInstance(tracerContextManager);
        }

        public static IUnityContainer ConfigureWebApiTracer(this IUnityContainer container)
        {
            return container
                .RegisterType<IExceptionLogger, Log4NetWebApiExceptionLogger>("log4net", Lifetime.Singleton);
        }


        public static IUnityContainer ConfigureMetadata(this IUnityContainer container)
        {
            var metadataSources = new IMetadataSource[]
            {
                new QueryingMetadataSource()
            };

            var metadataProcessors = new IMetadataProcessor[] { };

            return container.RegisterType<IMetadataProvider, MetadataProvider>(Lifetime.Singleton, new InjectionConstructor(metadataSources, metadataProcessors));
        }

        public static IUnityContainer ConfigureStoreModel(this IUnityContainer container)
        {
            return container
                .RegisterType<ITypeProvider, EmitTypeProvider>(Lifetime.Singleton)
                .RegisterType<EdmxModelBuilder>(Lifetime.Singleton, new InjectionConstructor(typeof(IMetadataProvider), typeof(ITypeProvider)));
        }

        public static IUnityContainer ConfigureWebApiOData(this IUnityContainer container)
        {
            return container
                .RegisterType<IDynamicAssembliesRegistry, DynamicAssembliesRegistry>(Lifetime.Singleton)
                .RegisterType<IDynamicAssembliesResolver, DynamicAssembliesRegistry>(Lifetime.Singleton)

                // custom IHttpControllerTypeResolver
                .RegisterType<IHttpControllerTypeResolver, DynamicControllerTypeResolver>(Lifetime.Singleton);
        }

        public static IUnityContainer ConfigureHttpRequest(this IUnityContainer container, HttpRequestMessage request)
        {
            var edmModel = request.ODataProperties().Model;

            return container
                .RegisterType<IQuery, ODataQuery>(Lifetime.PerScope)
                .RegisterInstance(edmModel, Lifetime.PerScope);
        }
    }
}