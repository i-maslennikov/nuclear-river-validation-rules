using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

using Microsoft.Extensions.Options;

using NuClear.ValidationRules.WebApp.Configuration;
using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.DataAccess
{
    public class DataConnectionFactory
    {
        private readonly MappingSchema _schema;

        public DataConnectionFactory(IOptions<ConnectionStringSettings> settings)
        {
            DataConnection.AddConfiguration("Erm", settings.Value.Erm, SqlServerTools.GetDataProvider(SqlServerVersion.v2012));
            DataConnection.AddConfiguration("Messages", settings.Value.Messages, SqlServerTools.GetDataProvider(SqlServerVersion.v2012));
            _schema = CreateSchema();
        }

        public DataConnection CreateDataConnection(string context)
        {
            var connection = new DataConnection(context);
            connection.AddMappingSchema(_schema);
            connection.BeginTransaction(System.Data.IsolationLevel.Snapshot);
            return connection;
        }

        private MappingSchema CreateSchema()
        {
            var builder = new MappingSchema(new SqlServer2012MappingSchema())
                .GetFluentMappingBuilder();

            builder.Entity<Order>()
                .HasSchemaName("Billing")
                .HasTableName("Orders");

            builder.Entity<UserOrganizationUnit>()
                .HasSchemaName("Security")
                .HasTableName("UserOrganizationUnits");

            builder.Entity<User>()
                .HasSchemaName("Security")
                .HasTableName("Users");

            builder.Entity<Project>()
                .HasSchemaName("Billing")
                .HasTableName("Projects");

            builder.Entity<ReleaseInfo>()
                .HasSchemaName("Billing")
                .HasTableName("ReleaseInfos");

            builder.Entity<ValidationResult>()
                .HasSchemaName("Messages")
                .HasTableName("ValidationResultByOrder");

            return builder.MappingSchema;
        }
    }
}