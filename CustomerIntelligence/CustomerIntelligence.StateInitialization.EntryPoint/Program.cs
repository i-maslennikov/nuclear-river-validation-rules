using System;
using System.Configuration;
using System.Diagnostics;

using NuClear.CustomerIntelligence.Domain;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Replication.Bulk.API;
using NuClear.Replication.Bulk.API.Factories;
using NuClear.Replication.Bulk.API.Storage;
using NuClear.River.Common.Metadata;

namespace NuClear.CustomerIntelligence.StateInitialization.EntryPoint
{
    public sealed class Program
    {
        private static readonly MetadataProvider DefaultProvider
            = new MetadataProvider(
                new IMetadataSource[]
                {
                    new BulkReplicationMetadataSource(),
                    new FactsReplicationMetadataSource(),
                    new AggregateConstructionMetadataSource(),
                },
                new IMetadataProcessor[] { new TunedReferencesEvaluatorProcessor() });

        public static void Main(string[] args)
        {
            var connectionStringSettings = new StateInitializationConnectionStringSettings(ConfigurationManager.ConnectionStrings);
            var viewRemover = new ViewRemover(connectionStringSettings);
            var connectionFactory = new DataConnectionFactory(connectionStringSettings);
            var runner = new BulkReplicationRunner(DefaultProvider, connectionFactory, viewRemover);

            foreach (var mode in args)
            {
                var sw = Stopwatch.StartNew();
                runner.Run(mode);
                Console.WriteLine($"{mode}, {sw.ElapsedMilliseconds}ms");
            }
        }
    }
}
