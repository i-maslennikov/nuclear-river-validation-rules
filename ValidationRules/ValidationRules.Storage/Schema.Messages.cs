using LinqToDB.DataProvider.SqlServer;
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
                var schema = new MappingSchema(new SqlServerMappingSchema());
                var config = schema.GetFluentMappingBuilder();

                config.Entity<ValidationResult>()
                      .HasSchemaName(MessagesSchema)
                      .HasPrimaryKey(x => x.OrderId)
                      .HasPrimaryKey(x => x.MessageType)
                      .HasPrimaryKey(x => x.PeriodStart)
                      .HasPrimaryKey(x => x.PeriodEnd);

                config.Entity<HistoryValidationResult>()
                      .HasSchemaName(MessagesSchema)
                      .HasPrimaryKey(x => x.OrderVersion)
                      .HasPrimaryKey(x => x.MessageType)
                      .HasPrimaryKey(x => x.PeriodStart)
                      .HasPrimaryKey(x => x.PeriodEnd)
                      .HasPrimaryKey(x => x.OrderVersion);

                return schema;
            }
        }
    }
}
