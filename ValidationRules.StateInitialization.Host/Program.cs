using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;

using NuClear.Assembling.TypeProcessing;
using NuClear.Replication.Core;
using NuClear.StateInitialization.Core.Actors;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.ValidationRules.StateInitialization.Host.Assembling;
using NuClear.ValidationRules.Storage.Identitites.Connections;

namespace NuClear.ValidationRules.StateInitialization.Host
{
    public sealed class Program
    {
        private static readonly IConnectionStringSettings ConnectionStringSettings =
            new ConnectionStringSettingsAspect(
                new Dictionary<IConnectionStringIdentity, string>
                    {
                        { ErmConnectionStringIdentity.Instance, GetConnectionString(ConnectionStringName.Erm) },
                        { FactsConnectionStringIdentity.Instance, GetConnectionString(ConnectionStringName.Facts) },
                        { AggregatesConnectionStringIdentity.Instance, GetConnectionString(ConnectionStringName.Aggregates) },
                    });

        public static void Main(string[] args)
        {
            StateInitializationRoot.Instance.PerformTypesMassProcessing(Array.Empty<IMassProcessor>(), true, typeof(object));

            var commands = new List<ICommand>();
            foreach (var mode in args)
            {
                switch (mode)
                {
                    case "-facts":
                        commands.Add(BulkReplicationCommands.ErmToFacts);
                        break;
                    case "-aggregates":
                        commands.Add(BulkReplicationCommands.FactsToAggregates);
                        break;
                    default:
                        Console.WriteLine($"Unknown argument: {mode}");
                        break;
                }
            }

            var bulkReplicationActor = new BulkReplicationActor(new DataObjectTypesProviderFactory(), ConnectionStringSettings);

            var sw = Stopwatch.StartNew();
            bulkReplicationActor.ExecuteCommands(commands);
            Console.WriteLine($"Total time: {sw.ElapsedMilliseconds}ms");
        }

        private static string GetConnectionString(string connectionStringName)
            => ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
    }
}
