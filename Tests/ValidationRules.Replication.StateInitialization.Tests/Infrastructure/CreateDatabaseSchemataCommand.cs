using System;
using System.Linq;

using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;

using Microsoft.SqlServer.Management.Smo;

using NuClear.DataTest.Engine;
using NuClear.DataTest.Engine.Command;
using NuClear.DataTest.Metamodel;
using NuClear.Metamodeling.Provider;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests.Infrastructure
{
    public sealed class CreateDatabaseSchemataCommand : Command
    {
        private readonly DataConnectionFactory _dataConnectionFactory;
        private readonly SmoConnectionFactory _smoConnectionFactory;
        private readonly IContextEntityTypesProvider _contextEntityTypesProvider;

        public CreateDatabaseSchemataCommand(IMetadataProvider metadataProvider,
                                             DataConnectionFactory dataConnectionFactory,
                                             SmoConnectionFactory smoConnectionFactory,
                                             IContextEntityTypesProvider contextEntityTypesProvider)
            : base(metadataProvider)
        {
            _dataConnectionFactory = dataConnectionFactory;
            _smoConnectionFactory = smoConnectionFactory;
            _contextEntityTypesProvider = contextEntityTypesProvider;
        }

        protected override void Execute(SchemaMetadataElement metadataElement)
        {
            var database = _smoConnectionFactory.CreateDatabaseConnection(metadataElement.ConnectionStringIdentity);
            using (var dataConnection = _dataConnectionFactory.CreateConnection(metadataElement))
            {
                var entities = _contextEntityTypesProvider.GetTypesFromContext(metadataElement.Context);
                var schemaNames = entities.Select(x => dataConnection.MappingSchema.GetAttribute<TableAttribute>(x)?.Schema)
                                          .Where(x => !string.IsNullOrEmpty(x) && database.Schemas[x] == null)
                                          .Distinct()
                                          .ToArray();

                foreach (var schemaName in schemaNames)
                {
                    var schema = new Schema(database, schemaName);
                    schema.Create();
                }

                foreach (var entity in entities)
                {
                    var factory = (ITableFactory)Activator.CreateInstance(typeof(TableFactory<>).MakeGenericType(entity));
                    factory.CreateTable(dataConnection);
                }
            }
        }

        private interface ITableFactory
        {
            void CreateTable(DataConnection dataConnection);
        }

        private class TableFactory<T> : ITableFactory
        {
            public void CreateTable(DataConnection dataConnection)
            {
                dataConnection.CreateTable<T>();
            }
        }
    }
}
