using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http.ExceptionHandling;

using Microsoft.Practices.Unity;

using NuClear.Tracing.API;
using NuClear.Tracing.Environment;
using NuClear.Tracing.Log4Net;
using NuClear.Tracing.Log4Net.Config;
using NuClear.ValidationRules.Querying.Host.Composition;
using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.SingleCheck;

namespace NuClear.ValidationRules.Querying.Host.DI
{
    public static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity()
        {
            return new UnityContainer()
                .ConfigureTracer()
                .ConfigureDataAccess()
                .ConfigureComposers()
                .ConfigureDistinctors()
                .ConfigureSingleCheck();
        }

        private static IUnityContainer ConfigureTracer(
            this IUnityContainer container)
        {
            var environmentName = ConfigurationManager.AppSettings["TargetEnvironmentName"];
            var entryPointName = ConfigurationManager.AppSettings["EntryPointName"];

            var tracerContextEntryProviders =
                    new ITracerContextEntryProvider[]
                    {
                        new TracerContextConstEntryProvider(TracerContextKeys.Required.Environment, environmentName),
                        new TracerContextConstEntryProvider(TracerContextKeys.Required.EntryPoint, entryPointName),
                        new TracerContextConstEntryProvider(TracerContextKeys.Required.EntryPointHost, NetworkInfo.ComputerFQDN),
                        new TracerContextConstEntryProvider(TracerContextKeys.Required.EntryPointInstanceId, Guid.NewGuid().ToString()),
                        new TracerContextSelfHostedEntryProvider(TracerContextKeys.Required.UserAccount)
                    };

            var logstashUrl = new Uri(ConfigurationManager.ConnectionStrings["Logging"].ConnectionString);

            var tracerContextManager = new TracerContextManager(tracerContextEntryProviders);
            var tracer = Log4NetTracerBuilder.Use
                                             .ApplicationXmlConfig
                                             .Logstash(logstashUrl)
                                             .Build;

            return container.RegisterInstance(tracer)
                            .RegisterInstance(tracerContextManager)
                            .RegisterType<IExceptionLogger, ExceptionTracer>("log4net", new ContainerControlledLifetimeManager());
        }

        private static IUnityContainer ConfigureDataAccess(this IUnityContainer container)
        {
            return container
                .RegisterType<DataConnectionFactory>(new ContainerControlledLifetimeManager())
                .RegisterType<MessageRepositiory>(new PerResolveLifetimeManager());
        }

        private static IUnityContainer ConfigureComposers(this IUnityContainer container)
        {
            var interfaceType = typeof(IMessageComposer);
            var serializerTypes = interfaceType.Assembly.GetTypes()
                                               .Where(x => interfaceType.IsAssignableFrom(x) && x.IsClass && !x.IsAbstract)
                                               .ToArray();

            container.RegisterType(typeof(IReadOnlyCollection<IMessageComposer>),
                                   new InjectionFactory(c => serializerTypes.Select(t => c.Resolve(t)).Cast<IMessageComposer>().ToArray()));

            return container;
        }

        private static IUnityContainer ConfigureDistinctors(this IUnityContainer container)
        {
            var interfaceType = typeof(IDistinctor);
            var serializerTypes = interfaceType.Assembly.GetTypes()
                                               .Where(x => interfaceType.IsAssignableFrom(x) && x.IsClass && !x.IsAbstract)
                                               .ToArray();

            container.RegisterType(typeof(IReadOnlyCollection<IDistinctor>),
                                   new InjectionFactory(c => serializerTypes.Select(t => c.Resolve(t)).Cast<IDistinctor>().ToArray()));

            return container;
        }

        private static IUnityContainer ConfigureSingleCheck(this IUnityContainer container)
        {
            container.RegisterType<PipelineFactory>();

            return container;
        }
    }
}