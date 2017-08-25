using System;
using System.Collections.Generic;

using LinqToDB.Data;
using LinqToDB.Mapping;

using NuClear.ValidationRules.Storage;

namespace NuClear.ValidationRules.Querying.Host.DataAccess
{
    public class DataConnectionFactory
    {
        private static readonly Dictionary<string, MappingSchema> Schemas = new Dictionary<string, MappingSchema>
        {
            { "Facts", Schema.Facts },
            { "Messages", Schema.Messages},
        };

        public DataConnection CreateDataConnection(string configurationString)
        {
            if (!Schemas.TryGetValue(configurationString, out var schema))
            {
                throw new ArgumentException(nameof(configurationString));
            }

            var connection = new DataConnection(configurationString);
            connection.AddMappingSchema(schema);
            //connection.BeginTransaction(System.Data.IsolationLevel.Snapshot);
            return connection;
        }
    }
}