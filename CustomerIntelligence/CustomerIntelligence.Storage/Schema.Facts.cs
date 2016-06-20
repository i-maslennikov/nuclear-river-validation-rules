using LinqToDB;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;
using LinqToDB.SqlQuery;

using NuClear.CustomerIntelligence.Storage.Model.Bit;
using NuClear.CustomerIntelligence.Storage.Model.Facts;

namespace NuClear.CustomerIntelligence.Storage
{
    public static partial class Schema
    {
        private const string ErmSchema = "ERM";
        private const string BitSchema = "BIT";

        public static MappingSchema Facts
        {
            get
            {
                var schema = new MappingSchema(nameof(Facts), new SqlServerMappingSchema());
                var config = schema.GetFluentMappingBuilder();

                config.Entity<Account>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Activity>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Category>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<CategoryGroup>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<BranchOfficeOrganizationUnit>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<CategoryFirmAddress>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<CategoryOrganizationUnit>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Client>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Contact>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Firm>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<FirmAddress>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<FirmContact>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Lead>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<LegalPerson>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Order>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Project>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<Territory>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();
                config.Entity<SalesModelCategoryRestriction>().HasSchemaName(ErmSchema).Property(x => x.Id).IsPrimaryKey();

                config.Entity<FirmCategoryStatistics>().HasSchemaName(BitSchema)
                      .Property(x => x.FirmId).IsPrimaryKey()
                      .Property(x => x.CategoryId).IsPrimaryKey();

                config.Entity<ProjectCategoryStatistics>().HasSchemaName(BitSchema)
                      .Property(x => x.ProjectId).IsPrimaryKey()
                      .Property(x => x.CategoryId).IsPrimaryKey();

                config.Entity<FirmCategoryForecast>().HasSchemaName(BitSchema)
                      .Property(x => x.ProjectId).IsPrimaryKey()
                      .Property(x => x.FirmId).IsPrimaryKey()
                      .Property(x => x.CategoryId).IsPrimaryKey();

                config.Entity<FirmForecast>().HasSchemaName(BitSchema)
                      .Property(x => x.ProjectId).IsPrimaryKey()
                      .Property(x => x.FirmId).IsPrimaryKey();

                schema.SetDataType(typeof(decimal), new SqlDataType(DataType.Decimal, 19, 4));
                schema.SetDataType(typeof(decimal?), new SqlDataType(DataType.Decimal, 19, 4));
                schema.SetDataType(typeof(string), new SqlDataType(DataType.NVarChar, int.MaxValue));

                return schema;
            }
        }
    }
}