using LinqToDB;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;
using LinqToDB.SqlQuery;

using NuClear.CustomerIntelligence.Storage.Model.CI;
using NuClear.CustomerIntelligence.Storage.Model.Statistics;

namespace NuClear.CustomerIntelligence.Storage
{
    public static partial class Schema
    {
        private const string CustomerIntelligenceSchema = "CustomerIntelligence";

        public static MappingSchema CustomerIntelligence
        {
            get
            {
                var schema = new MappingSchema(nameof(CustomerIntelligence), new SqlServerMappingSchema());
                var config = schema.GetFluentMappingBuilder();

                config.Entity<CategoryGroup>()
                    .HasSchemaName(CustomerIntelligenceSchema)
                    .HasPrimaryKey(x => x.Id);

                config.Entity<Client>()
                    .HasSchemaName(CustomerIntelligenceSchema)
                    .HasPrimaryKey(x => x.Id);

                config.Entity<ClientContact>()
                    .HasSchemaName(CustomerIntelligenceSchema);

                config.Entity<Firm>()
                    .HasSchemaName(CustomerIntelligenceSchema)
                    .HasPrimaryKey(x => x.Id);

                config.Entity<FirmLead>()
                      .HasSchemaName(CustomerIntelligenceSchema);

                config.Entity<FirmActivity>()
                    .HasSchemaName(CustomerIntelligenceSchema);

                config.Entity<FirmBalance>()
                    .HasSchemaName(CustomerIntelligenceSchema);

                config.Entity<FirmCategory1>()
                    .HasSchemaName(CustomerIntelligenceSchema);

                config.Entity<FirmCategory2>()
                    .HasSchemaName(CustomerIntelligenceSchema);

                config.Entity<FirmTerritory>()
                    .HasSchemaName(CustomerIntelligenceSchema);

                config.Entity<Project>()
                    .HasSchemaName(CustomerIntelligenceSchema)
                    .HasPrimaryKey(x => x.Id);

                config.Entity<ProjectCategory>()
                    .HasSchemaName(CustomerIntelligenceSchema);

                config.Entity<Territory>()
                    .HasSchemaName(CustomerIntelligenceSchema)
                    .HasPrimaryKey(x => x.Id);

                config.Entity<ProjectStatistics>()
                    .HasSchemaName(CustomerIntelligenceSchema)
                    .HasPrimaryKey(x => x.Id);

                config.Entity<ProjectCategoryStatistics>()
                    .HasSchemaName(CustomerIntelligenceSchema);

                config.Entity<FirmForecast>()
                    .HasSchemaName(CustomerIntelligenceSchema)
                    .HasPrimaryKey(x => x.FirmId);

                config.Entity<FirmCategory3>()
                    .HasSchemaName(CustomerIntelligenceSchema);

                schema.SetDataType(typeof(decimal), new SqlDataType(DataType.Decimal, 19, 4));
                schema.SetDataType(typeof(decimal?), new SqlDataType(DataType.Decimal, 19, 4));
                schema.SetDataType(typeof(string), new SqlDataType(DataType.NVarChar, int.MaxValue));

                return schema;
            }
        }
    }
}