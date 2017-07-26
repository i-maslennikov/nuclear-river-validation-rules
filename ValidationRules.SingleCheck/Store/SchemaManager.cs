using System;
using System.Collections.Generic;
using System.Transactions;

using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;

using NuClear.ValidationRules.Storage.Model.WebApp;
using NuClear.ValidationRules.Storage.SchemaInitializer;

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
            var schema = new MappingSchema(@lock.Id.ToString(), _baseSchema);
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

        private void InitializeMappingSchema(MappingSchema schema, Lock @lock)
        {
            using (var scope = CreateTransaction())
            using (var db = new DataConnection("Messages").AddMappingSchema(schema))
            {
                var service = new SqlSchemaService(db);
                service.CreateTablesWithIndices(schema, _dataObjectTypes);

				@lock.IsNew = false;
                db.Update(@lock);

                scope.Complete();
            }
        }

        private TransactionScope CreateTransaction()
            => new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero });
    }
}