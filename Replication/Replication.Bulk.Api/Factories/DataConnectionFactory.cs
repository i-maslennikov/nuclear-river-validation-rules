using System;

using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;

using NuClear.Replication.Bulk.Metadata;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.Replication.Bulk.API.Factories
{
    public class DataConnectionFactory
    {
        private readonly IConnectionStringSettings _connectionStringSettings;

        public DataConnectionFactory(IConnectionStringSettings connectionStringSettings)
        {
            _connectionStringSettings = connectionStringSettings;
        }

        public DataConnection CreateConnection(StorageDescriptorFeature storageDescriptorFeature)
        {
            var connectionString = _connectionStringSettings.GetConnectionString(storageDescriptorFeature.ConnectionStringName);
            var connection = SqlServerTools.CreateDataConnection(connectionString);
            connection.AddMappingSchema(storageDescriptorFeature.MappingSchema);
            connection.CommandTimeout = (int)TimeSpan.FromMinutes(30).TotalMilliseconds;
            return connection;
        }
    }
}