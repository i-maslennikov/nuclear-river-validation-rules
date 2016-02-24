using LinqToDB.Data;

using NuClear.Replication.Bulk.API.Storage;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Bulk.Api.Storage
{
    public class DatabaseStorage : IStorage
    {
        private readonly DataConnection _dataConnection;

        public DatabaseStorage(DataConnection dataConnection)
        {
            _dataConnection = dataConnection;
        }

        public void Dispose()
        {
            _dataConnection.Dispose();
        }

        public IQuery GetReadAccess()
        {
            return new LinqToDbQuery(_dataConnection);
        }

        public DataConnection GetWriteAccess()
        {
            return _dataConnection;
        }
    }
}