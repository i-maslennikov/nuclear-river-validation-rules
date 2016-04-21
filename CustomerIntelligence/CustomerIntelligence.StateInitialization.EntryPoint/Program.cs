using System;
using System.Collections.Generic;
using System.Diagnostics;

using NuClear.CustomerIntelligence.Storage;
using NuClear.Replication.Bulk.API;
using NuClear.Replication.Bulk.API.Commands;
using NuClear.Replication.Bulk.API.Storage;
using NuClear.River.Common.Metadata;

namespace NuClear.CustomerIntelligence.StateInitialization.EntryPoint
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            var commands = new List<ICommand>();
            foreach (var mode in args)
            {
                switch (mode)
                {
                    case "-fact":
                        commands.Add(new ReplaceDataObjectsInBulkCommand(
                                         new StorageDescriptor(ConnectionStringName.Erm, Schema.Erm),
                                         new StorageDescriptor(ConnectionStringName.Facts, Schema.Facts)));
                        break;
                    case "-ci":
                        commands.Add(new ReplaceDataObjectsInBulkCommand(
                                         new StorageDescriptor(ConnectionStringName.Facts, Schema.Facts),
                                         new StorageDescriptor(ConnectionStringName.CustomerIntelligence, Schema.CustomerIntelligence)));
                        break;
                    default:
                        Console.WriteLine($"Unknown argument: {mode}");
                        break;
                }
            }

            var bulkReplicationActor = new BulkReplicationActor(new DataObjectTypesProviderFactory());

            var sw = Stopwatch.StartNew();
            bulkReplicationActor.ExecuteCommands(commands);
            Console.WriteLine($"Total time: {sw.ElapsedMilliseconds}ms");
        }
    }
}
