using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Storage
{
    public static partial class Schema
    {
        private const string FactsSchema = "Facts";

        public static MappingSchema Facts
            => new MappingSchema(nameof(Facts), new SqlServerMappingSchema())
                .RegisterDataTypes()
                .GetFluentMappingBuilder()
                .RegisterFacts()
                .MappingSchema;

        private static FluentMappingBuilder RegisterFacts(this FluentMappingBuilder builder)
        {
            builder.Entity<Account>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<AccountDetail>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id)
                   .HasIndex(x => new { x.OrderId, x.PeriodStartDate });
            builder.Entity<Advertisement>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<Bargain>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<BargainScanFile>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<Bill>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<BranchOffice>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<BranchOfficeOrganizationUnit>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<Category>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<CategoryOrganizationUnit>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<CostPerClickCategoryRestriction>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.ProjectId)
                   .HasPrimaryKey(x => x.CategoryId)
                   .HasPrimaryKey(x => x.Begin);
            builder.Entity<Deal>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<EntityName>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id)
                   .HasPrimaryKey(x => x.EntityType);
            builder.Entity<Firm>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id)
                   .HasIndex(x => new { x.Id }, x => new { x.IsClosedForAscertainment, x.IsActive, x.IsDeleted });
            builder.Entity<FirmAddress>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id)
                   .HasIndex(x => new { x.FirmId, x.IsActive, x.IsDeleted, x.IsClosedForAscertainment }, x => new { x.Id });
            builder.Entity<FirmAddressCategory>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id)
                   .HasPrimaryKey(x => x.CategoryId)
                   .HasIndex(x => new { x.CategoryId }, x => new { x.FirmAddressId })
                   .HasIndex(x => new { x.FirmAddressId, x.CategoryId });
            builder.Entity<LegalPerson>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<LegalPersonProfile>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id)
                   .HasIndex(x => new { x.LegalPersonId }, x => new { x.Id });
            builder.Entity<NomenclatureCategory>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<Order>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id)
                   .HasIndex(x => new { x.DestOrganizationUnitId }, x => new { x.Id, x.FirmId, x.BeginDistribution, x.EndDistributionFact, x.EndDistributionPlan, x.WorkflowStep })
                   .HasIndex(x => new { x.LegalPersonId, x.SignupDate }, x => new { x.Id })
                   .HasIndex(x => new { x.BargainId }, x => new { x.Id })
                   .HasIndex(x => new { x.BargainId, x.SignupDate }, x => new { x.Id })
                   .HasIndex(x => new { x.BeginDistribution })
                   .HasIndex(x => new { x.EndDistributionFact })
                   .HasIndex(x => new { x.EndDistributionPlan });
            builder.Entity<OrderItem>()
                   .HasSchemaName(FactsSchema)
                   .HasIndex(x => new { x.OrderId }, x => new { x.ItemPositionId });
            builder.Entity<OrderPosition>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id)
                   .HasIndex(x => new { x.OrderId }, x => new { x.Id, x.PricePositionId })
                   .HasIndex(x => new { x.PricePositionId }, x => new { x.Id, x.OrderId });
            builder.Entity<OrderPositionAdvertisement>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id)
                   .HasIndex(x => new { x.AdvertisementId }, x => new { x.OrderPositionId, x.PositionId })
                   .HasIndex(x => new { x.OrderPositionId }, x => new { x.FirmAddressId, x.PositionId })
                   .HasIndex(x => new { x.PositionId })
                   .HasIndex(x => new { x.FirmAddressId, x.CategoryId }, x => new { x.OrderPositionId, x.PositionId })
                   .HasIndex(x => new { x.CategoryId }, x => new { x.OrderPositionId });
            builder.Entity<OrderPositionCostPerClick>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.OrderPositionId)
                   .HasPrimaryKey(x => x.CategoryId);
            builder.Entity<OrderScanFile>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id)
                   .HasIndex(x => new { x.OrderId }, x => new { x.Id });
            builder.Entity<Position>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<PositionChild>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.MasterPositionId)
                   .HasPrimaryKey(x => x.ChildPositionId);
            builder.Entity<Price>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<PricePosition>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id)
                   .HasIndex(x => new { x.PriceId })
                   .HasIndex(x => new { x.PositionId });
            builder.Entity<Project>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<ReleaseInfo>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<ReleaseWithdrawal>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.OrderPositionId)
                   .HasPrimaryKey(x => x.Start);
            builder.Entity<SalesModelCategoryRestriction>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.ProjectId)
                   .HasPrimaryKey(x => x.CategoryId)
                   .HasPrimaryKey(x => x.Begin);
            builder.Entity<SystemStatus>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<Theme>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<ThemeCategory>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<ThemeOrganizationUnit>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<UnlimitedOrder>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.OrderId)
                   .HasPrimaryKey(x => x.PeriodStart);

            builder.Entity<Ruleset>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.Id);
            builder.Entity<Ruleset.AssociatedRule>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.RulesetId)
                   .HasPrimaryKey(x => x.PrincipalNomenclatureId)
                   .HasPrimaryKey(x => x.AssociatedNomenclatureId)
                   .HasIndex(x => new {x.RulesetId, x.AssociatedNomenclatureId}, x => new {x.PrincipalNomenclatureId, x.ConsideringBindingObject });
            builder.Entity<Ruleset.DeniedRule>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.RulesetId)
                   .HasPrimaryKey(x => x.NomenclatureId)
                   .HasPrimaryKey(x => x.DeniedNomenclatureId)
                   .HasIndex(x => new { x.RulesetId, x.NomenclatureId }, x => new { x.DeniedNomenclatureId, x.BindingObjectStrategy });
            builder.Entity<Ruleset.QuantitativeRule>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.RulesetId)
                   .HasPrimaryKey(x => x.NomenclatureCategoryCode)
                   .HasIndex(x => new { x.RulesetId, x.NomenclatureCategoryCode });
            builder.Entity<Ruleset.RulesetProject>()
                   .HasSchemaName(FactsSchema)
                   .HasPrimaryKey(x => x.RulesetId)
                   .HasPrimaryKey(x => x.ProjectId)
                   .HasIndex(x => new { x.ProjectId, x.RulesetId });

            return builder;
        }
    }
}
