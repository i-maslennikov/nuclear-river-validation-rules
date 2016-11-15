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
        {
            get
            {
                var schema = new MappingSchema(nameof(Messages), new SqlServerMappingSchema());

                // XDocument mapping to nvarchar
                schema.SetDataType(typeof(XDocument), new SqlDataType(DataType.NVarChar, 4000));
                schema.SetConvertExpression<string, XDocument>(x => XDocument.Parse(x));
                schema.SetConvertExpression<XDocument, string>(x => x.ToString(SaveOptions.DisableFormatting));
                schema.SetConvertExpression<XDocument, DataParameter>(x => new DataParameter { DataType = DataType.NVarChar, Value = x.ToString(SaveOptions.DisableFormatting)});

                var config = schema.GetFluentMappingBuilder();

                config.Entity<Version>()
                      .HasSchemaName(MessagesSchema)
                      .HasPrimaryKey(x => x.Id);

                config.Entity<Version.ErmState>()
                      .HasSchemaName(MessagesSchema);

                config.Entity<Version.ErmStateBulkDelete>()
                      .HasTableName(nameof(Version.ErmState))
                      .HasSchemaName(MessagesSchema);

                config.Entity<Version.ValidationResult>()
                      .HasSchemaName(MessagesSchema);

                config.Entity<Version.ValidationResultBulkDelete>()
                      .HasTableName(nameof(Version.ValidationResult))
                      .HasSchemaName(MessagesSchema);

                return schema;
            }
        }
    }
}