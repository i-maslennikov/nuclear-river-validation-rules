﻿using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Storage
{
    public static partial class Schema
    {
        private const string MessagesSchema = "Messages";

        public static MappingSchema Messages
        {
            get
            {
                var schema = new MappingSchema(nameof(Messages), new SqlServerMappingSchema());
                var config = schema.GetFluentMappingBuilder();

                config.Entity<Version>()
                      .HasSchemaName(MessagesSchema)
                      .HasPrimaryKey(x => x.Id);

                config.Entity<Version.ErmState>()
                      .HasSchemaName(MessagesSchema);

                config.Entity<Version.ValidationResult>()
                      .HasSchemaName(MessagesSchema);

                config.Entity<Version.ValidationResultForBulkDelete>()
                      .HasTableName(nameof(Version.ValidationResult))
                      .HasSchemaName(MessagesSchema);

                config.Entity<Version.ValidationResultByOrder>()
                      .HasSchemaName(MessagesSchema);

                return schema;
            }
        }
    }
}