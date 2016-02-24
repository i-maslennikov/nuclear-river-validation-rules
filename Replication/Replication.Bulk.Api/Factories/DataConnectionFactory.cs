using System;

using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;

using NuClear.Replication.Bulk.Api.Storage;
using NuClear.Replication.Bulk.Metadata;
using NuClear.River.Common.Metadata;
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

        public IStorage CreateStorage(IStorageDescriptorFeature storageDescriptorFeature)
        {
            if (storageDescriptorFeature == null)
            {
                throw new ArgumentNullException(nameof(storageDescriptorFeature));
            }

            var db = storageDescriptorFeature as StorageDescriptorFeature;
            if (db != null)
            {
                return new DatabaseStorage(CreateConnection(db));
            }

            var config = storageDescriptorFeature as ConfigStorageDescriptorFeature;
            if (config != null)
            {
                var path = _connectionStringSettings.GetConnectionString(config.PathIdentity);
                return new FileStorage(path, (IConfigParser)Activator.CreateInstance(config.ParcerType));
            }

            throw new ArgumentException($"Unsupported storage descriptor type {storageDescriptorFeature.GetType().Name}", nameof(storageDescriptorFeature));
        }

        private DataConnection CreateConnection(StorageDescriptorFeature storageDescriptorFeature)
        {
            var connectionString = _connectionStringSettings.GetConnectionString(storageDescriptorFeature.ConnectionStringName);
            var connection = SqlServerTools.CreateDataConnection(connectionString);
            connection.AddMappingSchema(storageDescriptorFeature.MappingSchema);
            connection.CommandTimeout = (int)TimeSpan.FromMinutes(30).TotalMilliseconds;
            return connection;
        }
    }
}