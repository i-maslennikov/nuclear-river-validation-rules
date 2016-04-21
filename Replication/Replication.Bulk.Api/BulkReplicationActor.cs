using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;

using NuClear.Replication.Bulk.API.Commands;
using NuClear.Replication.Bulk.API.Factories;
using NuClear.Replication.Bulk.API.Storage;
using NuClear.Replication.Core.API;
using NuClear.River.Common.Metadata;

namespace NuClear.Replication.Bulk.API
{
    public sealed class BulkReplicationActor : IActor
    {
        private readonly IDataObjectTypesProviderFactory _dataObjectTypesProviderFactory;

        public BulkReplicationActor(IDataObjectTypesProviderFactory dataObjectTypesProviderFactory)
        {
            _dataObjectTypesProviderFactory = dataObjectTypesProviderFactory;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            foreach (var command in commands.Cast<ReplaceDataObjectsInBulkCommand>())
            {
                using (ViewRemover.TemporaryRemoveViews(GetConnectionString(command.TargetStorageDescriptor)))
                {
                    var sourceDataConnnection = CreateDataConnection(command.SourceStorageDescriptor);
                    var targetDataConnnection = CreateDataConnection(command.TargetStorageDescriptor);
                    try
                    {
                        var dataObjectTypesProvider = _dataObjectTypesProviderFactory.Create(command);
                        var actorsFactory = new ReplaceDataObjectsInBulkActorFactory(dataObjectTypesProvider, sourceDataConnnection, targetDataConnnection);
                        var actors = actorsFactory.Create().AsParallel().AsOrdered();
                        foreach (var actor in actors)
                        {
                            var sw = Stopwatch.StartNew();
                            actor.ExecuteCommands(new[] { command });
                            sw.Stop();

                            Console.WriteLine($"{GetFriendlyTypeName(actor.GetType())}: {sw.Elapsed.TotalSeconds} seconds");
                        }
                    }
                    finally
                    {
                        sourceDataConnnection.Dispose();
                        targetDataConnnection.Dispose();
                    }
                }
            }

            return Array.Empty<IEvent>();
        }

        private static string GetConnectionString(StorageDescriptor storageDescriptor)
            => ConfigurationManager.ConnectionStrings[storageDescriptor.ConnectionStringName].ConnectionString;

        private static DataConnection CreateDataConnection(StorageDescriptor storageDescriptor)
        {
            var connectionString = GetConnectionString(storageDescriptor);
            var connection = SqlServerTools.CreateDataConnection(connectionString);
            connection.AddMappingSchema(storageDescriptor.MappingSchema);
            connection.CommandTimeout = (int)TimeSpan.FromMinutes(30).TotalMilliseconds;
            return connection;
        }

        private static string GetFriendlyTypeName(Type type)
        {
            var friendlyName = type.Name;
            if (type.IsGenericType)
            {
                var backtickIndex = friendlyName.IndexOf('`');
                if (backtickIndex > 0)
                {
                    friendlyName = friendlyName.Remove(backtickIndex);
                }

                friendlyName += "[";

                var typeParameters = type.GetGenericArguments();
                for (var i = 0; i < typeParameters.Length; ++i)
                {
                    var typeParamName = typeParameters[i].Name;
                    friendlyName += i == 0 ? typeParamName : "," + typeParamName;
                }

                friendlyName += "]";
            }

            return friendlyName;
        }
    }
}