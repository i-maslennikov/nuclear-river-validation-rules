using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;

namespace NuClear.ValidationRules.Storage.SchemaInitializer
{
    public sealed class SqlSchemaService
    {
        private readonly DataConnection _dataConnection;

        public SqlSchemaService(DataConnection dataConnection)
        {
            _dataConnection = dataConnection;
        }

        public void CreateTablesWithIndices(MappingSchema schema, IEnumerable<Type> dataObjectTypes)
        {
            foreach (var dataObjectType in dataObjectTypes)
            {
                var tableManager = TableManager.Create(dataObjectType);
                tableManager.CreateTable(_dataConnection);

                var indexManager = new IndexManager(dataObjectType);
                indexManager.CreateIndices(_dataConnection);
            }
        }

        public void DeleteAllTablesInSchema(string schemaName)
        {
            var tables = _dataConnection.GetTable<TableInfo>().Where(x => x.Schema == schemaName).ToArray();
            foreach (var table in tables)
            {
                _dataConnection.DropTable<object>(tableName: table.Name, schemaName: table.Schema);
            }
        }

        private abstract class TableManager
        {
            public static TableManager Create(Type dataObjectType)
            {
                var managerType = typeof(TableManagerImpl<>).MakeGenericType(dataObjectType);
                return (TableManager)Activator.CreateInstance(managerType);
            }

            public abstract void CreateTable(DataConnection db);

            private sealed class TableManagerImpl<T> : TableManager where T : class
            {
                public override void CreateTable(DataConnection db)
                {
                    db.CreateTable<T>();
                }
            }
        }

        private sealed class IndexManager
        {
            private readonly Type _dataObjectType;

            public IndexManager(Type dataObjectType)
            {
                _dataObjectType = dataObjectType;
            }

            public void CreateIndices(DataConnection db)
            {
                var table = db.MappingSchema.GetAttribute<TableAttribute>(_dataObjectType);
                var indices = db.MappingSchema.GetAttributes<SchemaExtensions.IndexAttribute>(_dataObjectType);
                foreach (var index in indices)
                {
                    var command = db.CreateCommand();
                    command.CommandText = $"CREATE INDEX IX_{table.Name}_{string.Join("_", index.Fields.Select(x => x.Name))} ON [{table.Schema ?? "dbo"}].[{table.Name ?? _dataObjectType.Name}] "
                                          + $"({string.Join(", ", index.Fields.Select(x => "[" + x.Name + "]"))})"
                                          + (index.Include.Any() ? $" INCLUDE ({string.Join(", ", index.Include.Select(x => "[" + x.Name + "]"))})" : string.Empty);
                    command.ExecuteNonQuery();
                }
            }
        }

	    // ReSharper disable once ClassNeverInstantiated.Local
		[Table(Name = "TABLES", Schema = "INFORMATION_SCHEMA")]
	    private sealed class TableInfo
	    {
		    [Column(Name = "TABLE_SCHEMA")]
		    public string Schema { get; set; }

		    [Column(Name = "TABLE_NAME")]
		    public string Name { get; set; }
	    }
	}
}
