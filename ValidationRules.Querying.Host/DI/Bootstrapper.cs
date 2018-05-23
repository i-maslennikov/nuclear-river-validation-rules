using System;
using System.Collections.Generic;

using System.Linq;
using System.Web.Http.ExceptionHandling;

using Microsoft.Practices.Unity;

using NuClear.Model.Common.Entities;
using NuClear.River.Hosting.Common.Identities.Connections;
using NuClear.River.Hosting.Common.Settings;
using NuClear.Settings.API;
using NuClear.Settings.Unity;
using NuClear.Tracing.API;
using NuClear.Tracing.Environment;
using NuClear.Tracing.Log4Net.Config;
using NuClear.ValidationRules.Querying.Host.Composition;
using NuClear.ValidationRules.Querying.Host.Composition.Composers;
using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.SingleCheck;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;

using ValidationRules.Hosting.Common;

using IConnectionStringSettings = NuClear.Storage.API.ConnectionStrings.IConnectionStringSettings;

namespace NuClear.ValidationRules.Querying.Host.DI
{
    public static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity()
        {
            var settings = new QueryingServiceSettings();

            return new UnityContainer()
                   .ConfigureTracer(settings.AsSettings<IEnvironmentSettings>(),
                                    settings.AsSettings<IConnectionStringSettings>())
                   .ConfigureSettingsAspects(settings)
                   .ConfigureDataAccess()
                   .RegisterImplementersCollection<IMessageComposer>()
                   .RegisterImplementersCollection<IDistinctor>()
                   .ConfigureSeverityProvider()
                   .ConfigureNameResolvingService()
                   .ConfigureSingleCheck()
                   .ConfigureOperationsProcessing();
        }

        private static IUnityContainer ConfigureTracer(
            this IUnityContainer container,
            IEnvironmentSettings environmentSettings,
            IConnectionStringSettings connectionStringSettings)
        {

            var logstashUrl = new Uri(connectionStringSettings.GetConnectionString(LoggingConnectionStringIdentity.Instance));

            var tracer = Log4NetTracerBuilder.Use
                                             .ApplicationXmlConfig
                                             .WithGlobalProperties(x =>
                                                x.Property(TracerContextKeys.Tenant, environmentSettings.EnvironmentName)
                                                .Property(TracerContextKeys.EntryPoint, environmentSettings.EntryPointName)
                                                .Property(TracerContextKeys.EntryPointHost, NetworkInfo.ComputerFQDN)
                                                .Property(TracerContextKeys.EntryPointInstanceId, Guid.NewGuid().ToString()))
                                             .Logstash(logstashUrl)
                                             .Build;

            return container.RegisterInstance(tracer)
                            .RegisterType<IExceptionLogger, ExceptionTracer>("log4net", new ContainerControlledLifetimeManager());
        }

        private static IUnityContainer ConfigureDataAccess(this IUnityContainer container)
        {
            return container
                .RegisterType<DataConnectionFactory>(new ContainerControlledLifetimeManager())
                .RegisterType<ValidationResultRepositiory>(new PerResolveLifetimeManager());
        }

        private static IUnityContainer RegisterImplementersCollection<TContract>(this IUnityContainer container) where TContract : class
        {
            var interfaceType = typeof(TContract);
            var implementerTypes = interfaceType.Assembly.GetTypes()
                                                .Where(x => interfaceType.IsAssignableFrom(x) && x.IsClass && !x.IsAbstract)
                                                .ToArray();

            container.RegisterType(typeof(IReadOnlyCollection<TContract>),
                                   new InjectionFactory(c => implementerTypes.Select(t => c.Resolve(t)).Cast<TContract>().ToArray()));

            return container;
        }

        private static IUnityContainer ConfigureSeverityProvider(this IUnityContainer container) =>
            container.RegisterType<IMessageSeverityProvider, MessageSeverityProvider>(new ContainerControlledLifetimeManager());

        private static IUnityContainer ConfigureNameResolvingService(this IUnityContainer container)
        {
            var interfaceType = typeof(IEntityType);
            var type = typeof(EntityTypeOrder);
            var entityTypes = type.Assembly.GetTypes()
                                  .Where(x => interfaceType.IsAssignableFrom(x) && x.IsClass && !x.IsAbstract)
                                  .ToArray();

            container.RegisterType(typeof(IReadOnlyCollection<IEntityType>),
                                   new InjectionFactory(c => entityTypes.Select(t => c.Resolve(t)).Cast<IEntityType>().ToArray()));

            return container;
        }

        private static IUnityContainer ConfigureSingleCheck(this IUnityContainer container)
        {
            container.RegisterType<PipelineFactory>();

            return container;
        }

        private static IUnityContainer ConfigureOperationsProcessing(this IUnityContainer container)
        {
            return container
                .RegisterType<IKafkaSettingsFactory, KafkaSettingsFactory>(new ContainerControlledLifetimeManager(), new InjectionConstructor(typeof(IConnectionStringSettings), typeof(IEnvironmentSettings)))
                .RegisterType<KafkaMessageFlowInfoProvider>(new ContainerControlledLifetimeManager());
        }
    }
}