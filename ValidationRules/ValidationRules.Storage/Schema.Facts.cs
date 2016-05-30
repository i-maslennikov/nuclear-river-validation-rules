using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Storage
{
    public static partial class Schema
    {
        private const string PriceContextSchema = "PriceContext";

        public static MappingSchema Facts
        {
            get
            {
                var schema = new MappingSchema(nameof(Facts), new SqlServerMappingSchema());
                var config = schema.GetFluentMappingBuilder();

                config.Entity<AssociatedPositionsGroup>()
                      .HasSchemaName(PriceContextSchema)
                      .HasPrimaryKey(x => x.Id);
                config.Entity<AssociatedPosition>()
                      .HasSchemaName(PriceContextSchema)
                      .HasPrimaryKey(x => x.Id);
                config.Entity<DeniedPosition>()
                      .HasSchemaName(PriceContextSchema)
                      .HasPrimaryKey(x => x.Id);
                config.Entity<Order>()
                      .HasSchemaName(PriceContextSchema)
                      .HasPrimaryKey(x => x.Id);
                config.Entity<OrderPosition>()
                      .HasSchemaName(PriceContextSchema)
                      .HasPrimaryKey(x => x.Id);
                config.Entity<OrderPositionAdvertisement>()
                      .HasSchemaName(PriceContextSchema)
                      .HasPrimaryKey(x => x.Id);
                config.Entity<OrganizationUnit>()
                      .HasSchemaName(PriceContextSchema)
                      .HasPrimaryKey(x => x.Id);
                config.Entity<Price>()
                      .HasSchemaName(PriceContextSchema)
                      .HasPrimaryKey(x => x.Id);
                config.Entity<PricePosition>()
                      .HasSchemaName(PriceContextSchema)
                      .HasPrimaryKey(x => x.Id);
                config.Entity<Project>()
                      .HasSchemaName(PriceContextSchema)
                      .HasPrimaryKey(x => x.Id);
                config.Entity<Position>()
                      .HasSchemaName(PriceContextSchema)
                      .HasPrimaryKey(x => x.Id);
                config.Entity<Category>()
                      .HasSchemaName(PriceContextSchema)
                      .HasPrimaryKey(x => x.Id);

                // TODO: хак чтобы не делать факты со сложным ключом
                config.Entity<RulesetRule>()
                      .HasSchemaName(PriceContextSchema)
                      .HasPrimaryKey(x => x.Id)
                      .HasPrimaryKey(x => x.RuleType)
                      .HasPrimaryKey(x => x.DependentPositionId)
                      .HasPrimaryKey(x => x.PrincipalPositionId);

                return schema;
            }
        }
    }
}
