using LinqToDB.Data;
using LinqToDB.Mapping;

using NuClear.ValidationRules.Storage;

namespace NuClear.ValidationRules.Querying.Host.DataAccess
{
    public class DataConnectionFactory
    {
        private const string ConfigurationString = "Messages";
        private readonly MappingSchema _schema = Schema.Messages;

        public DataConnection CreateDataConnection()
        {
            var connection = new DataConnection(ConfigurationString);
            connection.AddMappingSchema(_schema);
            //connection.BeginTransaction(System.Data.IsolationLevel.Snapshot);
            return connection;
        }
    }
}