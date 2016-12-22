using System.Collections.Generic;

using LinqToDB;
using LinqToDB.Data;

namespace NuClear.ValidationRules.SingleCheck.Store
{
    public sealed class Linq2DbStore : IStore
    {
        private readonly DataConnection _connection;

        public Linq2DbStore(DataConnection connection)
        {
            _connection = connection;
        }

        void IStore.Add<T>(T entity)
        {
            _connection.Insert(entity);
        }

        void IStore.AddRange<T>(IEnumerable<T> entities)
        {
            _connection.BulkCopy(entities);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}