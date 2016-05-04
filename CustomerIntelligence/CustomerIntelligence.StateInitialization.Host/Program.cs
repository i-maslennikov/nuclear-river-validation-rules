using System;
using System.Collections.Generic;
using System.Diagnostics;

using NuClear.Assembling.TypeProcessing;
using NuClear.CustomerIntelligence.StateInitialization.Host.Assembling;
using NuClear.CustomerIntelligence.Storage.Identitites.Connections;
using NuClear.Replication.Core;
using NuClear.StateInitialization.Core;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.CustomerIntelligence.StateInitialization.Host
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            StateInitializationRoot.Instance.PerformTypesMassProcessing(Array.Empty<IMassProcessor>(), true, typeof(object));

            var commands = new List<ICommand>();
            foreach (var mode in args)
            {
                switch (mode)
                {
                    case "-fact":
                        commands.Add(BulkReplicationCommands.ErmToFacts);
                        break;
                    case "-ci":
                        commands.Add(BulkReplicationCommands.FactsToCi);
                        break;
                    default:
                        Console.WriteLine($"Unknown argument: {mode}");
                        break;
                }
            }

            var connectionStringSettings = new ConnectionStringSettingsAspect(new Dictionary<IConnectionStringIdentity, string>
                {
                    { ErmConnectionStringIdentity.Instance, ConnectionStringName.Erm },
                    { FactsConnectionStringIdentity.Instance, ConnectionStringName.Facts },
                    { CustomerIntelligenceConnectionStringIdentity.Instance, ConnectionStringName.CustomerIntelligence },
                });

            var bulkReplicationActor = new BulkReplicationActor(new DataObjectTypesProviderFactory(), connectionStringSettings);

            var sw = Stopwatch.StartNew();
            bulkReplicationActor.ExecuteCommands(commands);
            Console.WriteLine($"Total time: {sw.ElapsedMilliseconds}ms");
        }
    }
}
