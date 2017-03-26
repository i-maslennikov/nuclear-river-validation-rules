using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;

using NuClear.ValidationRules.Storage;
using NuClear.ValidationRules.Storage.Model.WebApp;

namespace NuClear.ValidationRules.SingleCheck.Store
{
    public sealed class SchemaManager
    {
        private readonly MappingSchema _baseSchema;
        private readonly IReadOnlyCollection<Type> _dataObjectTypes;

        public SchemaManager(MappingSchema baseSchema, IReadOnlyCollection<Type> dataObjectTypes)
        {
            _baseSchema = baseSchema;
            _dataObjectTypes = dataObjectTypes;
        }

        public MappingSchema GetSchema(Lock @lock)
        {
            var schema = new MappingSchema(_baseSchema);
            var builder = schema.GetFluentMappingBuilder();
            foreach (var dataObjectType in _dataObjectTypes)
            {
                var baseTable = _baseSchema.GetAttribute<TableAttribute>(dataObjectType);
                if (baseTable != null)
                {
                    var attribute = new TableAttribute { Name = $"{baseTable.Schema}_{baseTable.Name ?? dataObjectType.Name}_{@lock.Id}", Schema = "WebApp", IsColumnAttributeRequired = false };
                    builder.HasAttribute(dataObjectType, attribute);
                }
            }

            if (@lock.IsNew)
            {
                InitializeMappingSchema(schema, @lock);
            }

            return schema;
        }

        public void DestroySchema(Lock @lock)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
            using (var db = new DataConnection("Messages").AddMappingSchema(_baseSchema))
            {
                var poolTables = db.GetTable<TableInfo>().Where(x => x.Name.EndsWith($"_{@lock.Id}")).ToArray();
                foreach (var table in poolTables)
                {
                    db.DropTable<object>(tableName: table.Name, schemaName: table.Schema);
                }

                db.Delete(@lock);
                scope.Complete();
            }
        }

        private void InitializeMappingSchema(MappingSchema schema, Lock @lock)
        {
            using (var scope = CreateTransaction())
            using (var db = new DataConnection("Messages").AddMappingSchema(schema))
            {
                foreach (var dataObjectType in _dataObjectTypes)
                {
                    var tableManager = TableManager.Create(dataObjectType);
                    tableManager.CreateTable(db);

                    var indexManager = new IndexManager(dataObjectType);
                    indexManager.CreateIndices(db);
                }

                @lock.IsNew = false;
                db.Update(@lock);

                scope.Complete();
            }
        }

        private TransactionScope CreateTransaction()
            => new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero });

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

        private class IndexManager
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
                    command.CommandText = $"create index ix_auto_{table.Name}_{string.Join("_", index.Fields.Select(x => x.Name))} on [{table.Schema ?? "dbo"}].[{table.Name ?? _dataObjectType.Name}] "
                                          + $"({string.Join(", ", index.Fields.Select(x => "[" + x.Name + "]"))})"
                                          + (index.Include.Any() ? $" include ({string.Join(", ", index.Include.Select(x => "[" + x.Name + "]"))})" : string.Empty);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}