using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Confluent.Kafka;

using NuClear.Assembling.TypeProcessing;
using NuClear.Messaging.API.Flows;
using NuClear.Replication.Core;
using NuClear.River.Hosting.Common.Settings;
using NuClear.StateInitialization.Core.Actors;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.Tracing.API;
using NuClear.Tracing.Environment;
using NuClear.Tracing.Log4Net.Config;
using NuClear.ValidationRules.OperationsProcessing.Facts.AmsFactsFlow;
using NuClear.ValidationRules.OperationsProcessing.Facts.RulesetFactsFlow;
using NuClear.ValidationRules.StateInitialization.Host.Assembling;
using NuClear.ValidationRules.StateInitialization.Host.Kafka;
using NuClear.ValidationRules.StateInitialization.Host.Kafka.Ams;
using NuClear.ValidationRules.StateInitialization.Host.Kafka.Rulesets;
using NuClear.ValidationRules.Storage.Connections;

using ValidationRules.Hosting.Common;
using ValidationRules.Hosting.Common.Settings;
using ValidationRules.Hosting.Common.Settings.Connections;
using ValidationRules.Hosting.Common.Settings.Kafka;

namespace NuClear.ValidationRules.StateInitialization.Host
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            StateInitializationRoot.Instance.PerformTypesMassProcessing(Array.Empty<IMassProcessor>(), true, typeof(object));

            var commands = new List<ICommand>();

            if (args.Contains("-facts"))
            {
                commands.Add(BulkReplicationCommands.ErmToFacts);
                // Надо подумать о лишней обёртке
                commands.Add(new KafkaReplicationCommand(AmsFactsFlow.Instance, BulkReplicationCommands.AmsToFacts));
                commands.Add(new KafkaReplicationCommand(RulesetFactsFlow.Instance, BulkReplicationCommands.RulesetsToFacts));
                commands.Add(SchemaInitializationCommands.WebApp);
                commands.Add(SchemaInitializationCommands.Facts);
            }

            if (args.Contains("-aggregates"))
            {
                commands.Add(BulkReplicationCommands.FactsToAggregates);
                commands.Add(SchemaInitializationCommands.WebApp);
                commands.Add(SchemaInitializationCommands.Aggregates);
            }

            if (args.Contains("-messages"))
            {
                commands.Add(BulkReplicationCommands.AggregatesToMessages);
                commands.Add(SchemaInitializationCommands.WebApp);
                commands.Add(SchemaInitializationCommands.Messages);
            }

            var connectionStrings = ConnectionStrings.For(ErmConnectionStringIdentity.Instance,
                                                          AmsConnectionStringIdentity.Instance,
                                                          FactsConnectionStringIdentity.Instance,
                                                          AggregatesConnectionStringIdentity.Instance,
                                                          MessagesConnectionStringIdentity.Instance,
                                                          RulesetConnectionStringIdentity.Instance);
            var connectionStringSettings = new ConnectionStringSettingsAspect(connectionStrings);
            var environmentSettings = new EnvironmentSettingsAspect();
            var businessModelSettings = new BusinessModelSettingsAspect();

            var tracer = CreateTracer(environmentSettings, businessModelSettings);

            var kafkaSettingsFactory =
                new KafkaSettingsFactory(new Dictionary<IMessageFlow, string>
                                             {
                                                 [AmsFactsFlow.Instance] =
                                                     connectionStringSettings.GetConnectionString(AmsConnectionStringIdentity.Instance),
                                                 [RulesetFactsFlow.Instance] =
                                                     connectionStringSettings.GetConnectionString(RulesetConnectionStringIdentity.Instance)
                                             },
                                         environmentSettings,
                                         Offset.Beginning);

            var kafkaMessageFlowReceiverFactory = new KafkaMessageFlowReceiverFactory(new NullTracer(), kafkaSettingsFactory);

            var dataObjectTypesProviderFactory = new DataObjectTypesProviderFactory();
            var bulkReplicationActor = new BulkReplicationActor(dataObjectTypesProviderFactory, connectionStringSettings);
            var kafkaReplicationActor = new KafkaReplicationActor(connectionStringSettings,
                                                                  dataObjectTypesProviderFactory,
                                                                  kafkaMessageFlowReceiverFactory,
                                                                  new KafkaMessageFlowInfoProvider(kafkaSettingsFactory),
                                                                  new IBulkCommandFactory<Message>[]
                                                                      {
                                                                          new AmsFactsBulkCommandFactory(),
                                                                          new RulesetFactsBulkCommandFactory(businessModelSettings)
                                                                      },
                                                                  tracer);

            var schemaInitializationActor = new SchemaInitializationActor(connectionStringSettings);

            var sw = Stopwatch.StartNew();
            schemaInitializationActor.ExecuteCommands(commands);
            bulkReplicationActor.ExecuteCommands(commands.Where(x => x == BulkReplicationCommands.ErmToFacts).ToList());
            kafkaReplicationActor.ExecuteCommands(commands);
            bulkReplicationActor.ExecuteCommands(commands.Where(x => x != BulkReplicationCommands.ErmToFacts).ToList());

            Console.WriteLine($"Total time: {sw.ElapsedMilliseconds}ms");
        }

        private static ITracer CreateTracer(IEnvironmentSettings environmentSettings, IBusinessModelSettings businessModelSettings)
        {
            return Log4NetTracerBuilder.Use
                                       .ApplicationXmlConfig
                                       .Console
                                       .WithGlobalProperties(x =>
                                                                 x.Property(TracerContextKeys.Tenant, environmentSettings.EnvironmentName)
                                                                  .Property(TracerContextKeys.EntryPoint, environmentSettings.EntryPointName)
                                                                  .Property(TracerContextKeys.EntryPointHost, NetworkInfo.ComputerFQDN)
                                                                  .Property(TracerContextKeys.EntryPointInstanceId, Guid.NewGuid().ToString())
                                                                  .Property(nameof(IBusinessModelSettings.BusinessModel), businessModelSettings.BusinessModel))
                                       .Build;
        }

    }
}
