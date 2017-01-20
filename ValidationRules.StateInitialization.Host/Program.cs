using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

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
                        { MessagesConnectionStringIdentity.Instance, GetConnectionString(ConnectionStringName.Messages) },
                    });

        public static void Main(string[] args)
        {
            StateInitializationRoot.Instance.PerformTypesMassProcessing(Array.Empty<IMassProcessor>(), true, typeof(object));

            var commands = new List<ICommand>();
            if (args.Contains("-facts"))
            {
                commands.Add(BulkReplicationCommands.ErmToFacts);
            }

            if (args.Contains("-aggregates"))
            {
                commands.Add(BulkReplicationCommands.FactsToAggregates);
            }

            if (args.Contains("-messages"))
            {
                commands.Add(BulkReplicationCommands.AggregatesToMessages);
            }

            var bulkReplicationActor = new BulkReplicationActor(new DataObjectTypesProviderFactory(), ConnectionStringSettings);
            var copyTablesActor = new CopyTablesActor(new DataObjectTypesProviderFactory(), ConnectionStringSettings);

            var sw = Stopwatch.StartNew();
            bulkReplicationActor.ExecuteCommands(commands);
            copyTablesActor.ExecuteCommands(commands);
            Console.WriteLine($"Total time: {sw.ElapsedMilliseconds}ms");
        }

        private static string GetConnectionString(string connectionStringName)
            => ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
    }
}
