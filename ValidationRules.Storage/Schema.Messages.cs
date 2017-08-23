using System.Xml.Linq;

using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;
using LinqToDB.SqlQuery;

using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Storage
{
    public static partial class Schema
    {
        private const string MessagesSchema = "Messages";

        public static MappingSchema Messages
            => new MappingSchema(nameof(Messages), new SqlServerMappingSchema())
                .RegisterDataTypes()
                .GetFluentMappingBuilder()
                .RegisterMessages()
                .MappingSchema;

        private static FluentMappingBuilder RegisterMessages(this FluentMappingBuilder builder)
        {
            builder.Entity<Version>()
                   .HasSchemaName(MessagesSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity<Version.ErmState>()
                   .HasSchemaName(MessagesSchema);

            builder.Entity<Version.ErmStateBulkDelete>()
                   .HasTableName(nameof(Version.ErmState))
                   .HasSchemaName(MessagesSchema);

            builder.Entity<Version.AmsState>()
                   .HasSchemaName(MessagesSchema);

            builder.Entity<Version.AmsStateBulkDelete>()
                   .HasTableName(nameof(Version.AmsState))
                   .HasSchemaName(MessagesSchema);

            builder.Entity<Version.ValidationResult>()
                   .HasSchemaName(MessagesSchema)
                   .HasIndex(x => new { x.Resolved })
                   .HasIndex(x => new { x.VersionId })
                   .HasIndex(x => new { x.Resolved, x.VersionId }, x => new { x.MessageType, x.MessageParams, x.PeriodStart, x.PeriodEnd, x.ProjectId, x.OrderId });

            builder.Entity<Version.ValidationResultBulkDelete>()
                   .HasTableName(nameof(Version.ValidationResult))
                   .HasSchemaName(MessagesSchema);

            return builder;
        }

        private static MappingSchema RegisterDataTypes(this MappingSchema schema)
        {
            schema.SetDataType(typeof(decimal), new SqlDataType(DataType.Decimal, 19, 4));
            schema.SetDataType(typeof(decimal?), new SqlDataType(DataType.Decimal, 19, 4));
            schema.SetDataType(typeof(string), new SqlDataType(DataType.NVarChar, int.MaxValue));

            // XDocument mapping to nvarchar
            schema.SetDataType(typeof(XDocument), new SqlDataType(DataType.NVarChar, 4000));
            schema.SetConvertExpression<string, XDocument>(x => XDocument.Parse(x));
            schema.SetConvertExpression<XDocument, string>(x => x.ToString(SaveOptions.DisableFormatting));
            schema.SetConvertExpression<XDocument, DataParameter>(x => new DataParameter { DataType = DataType.NVarChar, Value = x.ToString(SaveOptions.DisableFormatting) });

            return schema;
        }
    }
}