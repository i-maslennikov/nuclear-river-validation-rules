using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.StateInitialization.Core.Commands;
using NuClear.StateInitialization.Core.Factories;
using NuClear.StateInitialization.Core.Storage;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.StateInitialization.Core
{
    public sealed class BulkReplicationActor : IActor
    {
        private readonly IDataObjectTypesProviderFactory _dataObjectTypesProviderFactory;
        private readonly IConnectionStringSettings _connectionStringSettings;

        public BulkReplicationActor(IDataObjectTypesProviderFactory dataObjectTypesProviderFactory, IConnectionStringSettings connectionStringSettings)
        {
            _dataObjectTypesProviderFactory = dataObjectTypesProviderFactory;
            _connectionStringSettings = connectionStringSettings;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            foreach (var command in commands.Cast<ReplaceDataObjectsInBulkCommand>())
            {
                var commandStopwatch = Stopwatch.StartNew();

                using (ViewRemover.TemporaryRemoveViews(GetConnectionString(command.TargetStorageDescriptor)))
                using (var actorsFactory = new ReplaceDataObjectsInBulkActorFactory(
                    _dataObjectTypesProviderFactory.Create(command),
                    () => CreateDataConnection(command.SourceStorageDescriptor),
                    () => CreateDataConnection(command.TargetStorageDescriptor)))
                {
                    Parallel.ForEach(actorsFactory.Create(),
                                     actor =>
                                         {
                                             var sw = Stopwatch.StartNew();
                                             actor.ExecuteCommands(new[] { command });
                                             sw.Stop();

                                             Console.WriteLine($"{actor.GetType().GetFriendlyName()}: {sw.Elapsed.TotalSeconds} seconds");
                                         });
                }


                commandStopwatch.Stop();
                Console.WriteLine($"[{command.SourceStorageDescriptor.ConnectionStringIdentity}] -> [{command.TargetStorageDescriptor.ConnectionStringIdentity}]: {commandStopwatch.Elapsed.TotalSeconds} seconds");
            }

            return Array.Empty<IEvent>();
        }

        private string GetConnectionString(StorageDescriptor storageDescriptor)
            => _connectionStringSettings.GetConnectionString(storageDescriptor.ConnectionStringIdentity);

        private DataConnection CreateDataConnection(StorageDescriptor storageDescriptor)
        {
            var connectionString = GetConnectionString(storageDescriptor);
            var connection = SqlServerTools.CreateDataConnection(connectionString);
            connection.AddMappingSchema(storageDescriptor.MappingSchema);
            connection.CommandTimeout = (int)TimeSpan.FromMinutes(30).TotalMilliseconds;
            return connection;
        }
    }
}