using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

using NuClear.ValidationRules.Storage.Model.WebApp;

namespace NuClear.ValidationRules.Storage
{
    public static partial class Schema
    {
        private const string WebAppSchema = "WebApp";

        public static MappingSchema WebApp
            => new MappingSchema(nameof(WebApp), new SqlServerMappingSchema())
                .GetFluentMappingBuilder()
                .RegisterWebApp()
                .MappingSchema;

        private static FluentMappingBuilder RegisterWebApp(this FluentMappingBuilder builder)
        {
            builder.Entity<Lock>()
                   .HasSchemaName(WebAppSchema)
                   .HasPrimaryKey(x => x.Id);

            return builder;
        }
    }
}