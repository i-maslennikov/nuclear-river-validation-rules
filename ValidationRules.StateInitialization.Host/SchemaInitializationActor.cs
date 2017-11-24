using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.ValidationRules.Storage;
using NuClear.ValidationRules.Storage.Identitites.Connections;
using NuClear.ValidationRules.Storage.SchemaInitializer;

namespace NuClear.ValidationRules.StateInitialization.Host
{
    public sealed class SchemaInitializationActor : IActor
    {
        private readonly IConnectionStringSettings _connectionStringSettings;

        public SchemaInitializationActor(IConnectionStringSettings connectionStringSettings)
        {
            _connectionStringSettings = connectionStringSettings;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var schemaInitializationCommands = commands.OfType<SchemaInitializationCommand>().Distinct();

            foreach (var cmd in schemaInitializationCommands)
            {
                ExecuteCommand(cmd);
            }

            return Array.Empty<IEvent>();
        }

        private void ExecuteCommand(SchemaInitializationCommand cmd)
        {
            using (var db = CreateDataConnection(cmd))
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                var service = new SqlSchemaService(db);
                foreach (var schema in cmd.SqlSchemas)
                {
                    service.DeleteAllTablesInSchema(schema);
                }

                service.CreateTablesWithIndices(cmd.MappingSchema, cmd.DataTypes);
                scope.Complete();
            }
        }

        private DataConnection CreateDataConnection(SchemaInitializationCommand command)
        {
            var connectionString = _connectionStringSettings.GetConnectionString(command.ConnectionStringIdentity);
            var connection = SqlServerTools.CreateDataConnection(connectionString);
            connection.AddMappingSchema(command.MappingSchema);
            return connection;
        }
    }

    public sealed class SchemaInitializationCommand : ICommand
    {
        public SchemaInitializationCommand(MappingSchema mappingSchema, IReadOnlyCollection<Type> dataTypes, IConnectionStringIdentity connectionStringIdentity, IReadOnlyCollection<string> sqlSchemas)
        {
            MappingSchema = mappingSchema;
            DataTypes = dataTypes;
            SqlSchemas = sqlSchemas;
            ConnectionStringIdentity = connectionStringIdentity;
        }

        public MappingSchema MappingSchema { get; }
        public IReadOnlyCollection<string> SqlSchemas { get; }
        public IReadOnlyCollection<Type> DataTypes { get; }
        public IConnectionStringIdentity ConnectionStringIdentity { get; }
    }

    public static class SchemaInitializationCommands
    {
        public static SchemaInitializationCommand WebApp { get; }
            = new SchemaInitializationCommand(Schema.WebApp, DataObjectTypesProviderFactory.WebAppTypes, FactsConnectionStringIdentity.Instance, 
                new[] { "WebApp" });

        public static SchemaInitializationCommand Facts { get; }
            = new SchemaInitializationCommand(Schema.Facts,
                new HashSet<Type>(DataObjectTypesProviderFactory.FactTypes.Concat(DataObjectTypesProviderFactory.AmsFactTypes)), FactsConnectionStringIdentity.Instance,
                new[] { "Facts" });

        public static SchemaInitializationCommand Aggregates { get; }
			= new SchemaInitializationCommand(Schema.Aggregates, DataObjectTypesProviderFactory.AggregateTypes, AggregatesConnectionStringIdentity.Instance,
				new[] { "AccountAggregates", "AdvertisementAggregates", "ConsistencyAggregates", "FirmAggregates", "PriceAggregates", "ProjectAggregates", "ThemeAggregates", "SystemAggregates" });

        public static SchemaInitializationCommand Messages { get; }
            = new SchemaInitializationCommand(Schema.Messages, DataObjectTypesProviderFactory.MessagesTypes, MessagesConnectionStringIdentity.Instance,
                new[] { "Messages", "MessagesCache" });
    }
}