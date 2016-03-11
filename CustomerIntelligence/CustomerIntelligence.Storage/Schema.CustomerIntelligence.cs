using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

using NuClear.CustomerIntelligence.Domain.Model.CI;
using NuClear.CustomerIntelligence.Domain.Model.Statistics;

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

                config.Entity<CategoryGroup>().HasSchemaName(CustomerIntelligenceSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Client>().HasSchemaName(CustomerIntelligenceSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<ClientContact>().HasSchemaName(CustomerIntelligenceSchema)
                    .Property(x => x.ContactId).IsPrimaryKey()
                    .Property(x => x.ClientId).IsPrimaryKey();
                config.Entity<Firm>().HasSchemaName(CustomerIntelligenceSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<FirmActivity>().HasSchemaName(CustomerIntelligenceSchema).Property(x => x.FirmId).IsPrimaryKey();
                config.Entity<FirmBalance>().HasSchemaName(CustomerIntelligenceSchema)
                    .Property(x => x.AccountId).IsPrimaryKey()
                    .Property(x => x.FirmId).IsPrimaryKey();
                config.Entity<FirmCategory1>().HasSchemaName(CustomerIntelligenceSchema)
                    .Property(x => x.CategoryId).IsPrimaryKey()
                    .Property(x => x.FirmId).IsPrimaryKey();
                config.Entity<FirmCategory2>().HasSchemaName(CustomerIntelligenceSchema)
                    .Property(x => x.CategoryId).IsPrimaryKey()
                    .Property(x => x.FirmId).IsPrimaryKey();
                config.Entity<FirmTerritory>().HasSchemaName(CustomerIntelligenceSchema)
                    .Property(x => x.FirmId).IsPrimaryKey()
                    .Property(x => x.FirmAddressId).IsPrimaryKey();
                config.Entity<Project>().HasSchemaName(CustomerIntelligenceSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<ProjectCategory>().HasSchemaName(CustomerIntelligenceSchema)
                    .Property(x => x.ProjectId).IsPrimaryKey()
                    .Property(x => x.CategoryId).IsPrimaryKey();
                config.Entity<Territory>().HasSchemaName(CustomerIntelligenceSchema).Property(x => x.Id).IsPrimaryKey();

                config.Entity<FirmCategory3>().HasSchemaName(CustomerIntelligenceSchema)
                    .Property(x => x.FirmId).IsPrimaryKey()
                    .Property(x => x.CategoryId).IsPrimaryKey();

                config.Entity<FirmForecast>().HasSchemaName(CustomerIntelligenceSchema)
                    .Property(x => x.FirmId).IsPrimaryKey();

                return schema;
            }
        }
    }
}