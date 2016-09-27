using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Mapping;

using PriceFacts = NuClear.ValidationRules.Storage.Model.PriceRules.Facts;
using ProjectFacts = NuClear.ValidationRules.Storage.Model.ProjectRules.Facts;
using AccountFacts = NuClear.ValidationRules.Storage.Model.AccountRules.Facts;
using AdvertisementFacts = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;
using ConsistencyFacts = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts;
using FirmFacts = NuClear.ValidationRules.Storage.Model.FirmRules.Facts;

namespace NuClear.ValidationRules.Storage
{
    public static partial class Schema
    {
        private const string PriceFactsSchema = "PriceFacts";
        private const string ProjectFactsSchema = "ProjectFacts";
        private const string AccountFactsSchema = "AccountFacts";
        private const string AdvertisementFactsSchema = "AdvertisementFacts";
        private const string ConsistencyFactsSchema = "ConsistencyFacts";
        private const string FirmFactsSchema = "FirmFacts";

        public static MappingSchema Facts
        {
            get
            {
                var schema = new MappingSchema(nameof(Facts), new SqlServerMappingSchema());
                schema.GetFluentMappingBuilder()
                      .RegisterPriceFacts()
                      .RegisterProjectFacts()
                      .RegisterAccountFacts()
                      .RegisterConsistencyFacts()
                      .RegisterFirmFacts()
                      .RegisterAdvertisementFacts();

                return schema;
            }
        }

        private static FluentMappingBuilder RegisterFirmFacts(this FluentMappingBuilder builder)
        {
            builder.Entity<FirmFacts::Firm>()
                  .HasSchemaName(FirmFactsSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<FirmFacts::FirmAddress>()
                  .HasSchemaName(FirmFactsSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<FirmFacts::FirmAddressCategory>()
                  .HasSchemaName(FirmFactsSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<FirmFacts::Order>()
                  .HasSchemaName(FirmFactsSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<FirmFacts::OrderPosition>()
                  .HasSchemaName(FirmFactsSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<FirmFacts::OrderPositionAdvertisement>()
                  .HasSchemaName(FirmFactsSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<FirmFacts::SpecialPosition>()
                  .HasSchemaName(FirmFactsSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<FirmFacts::Project>()
                  .HasSchemaName(FirmFactsSchema)
                  .HasPrimaryKey(x => x.Id);

            return builder;
        }

        private static FluentMappingBuilder RegisterAdvertisementFacts(this FluentMappingBuilder builder)
        {
            builder.Entity<AdvertisementFacts::AdvertisementElementTemplate>()
                  .HasSchemaName(AdvertisementFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<AdvertisementFacts::AdvertisementElement>()
                  .HasSchemaName(AdvertisementFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<AdvertisementFacts::AdvertisementTemplate>()
                  .HasSchemaName(AdvertisementFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<AdvertisementFacts::Position>()
                  .HasSchemaName(AdvertisementFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<AdvertisementFacts::PricePosition>()
                  .HasSchemaName(AdvertisementFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<AdvertisementFacts::OrderPositionAdvertisement>()
                  .HasSchemaName(AdvertisementFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<AdvertisementFacts::OrderPosition>()
                  .HasSchemaName(AdvertisementFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<AdvertisementFacts::Order>()
                  .HasSchemaName(AdvertisementFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<AdvertisementFacts::Project>()
                  .HasSchemaName(AdvertisementFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<AdvertisementFacts::Advertisement>()
                  .HasSchemaName(AdvertisementFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<AdvertisementFacts::Firm>()
                  .HasSchemaName(AdvertisementFactsSchema)
                  .HasPrimaryKey(x => x.Id);

            return builder;
        }

        private static FluentMappingBuilder RegisterPriceFacts(this FluentMappingBuilder builder)
        {
            builder.Entity<PriceFacts::AssociatedPositionsGroup>()
                  .HasSchemaName(PriceFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::AssociatedPosition>()
                  .HasSchemaName(PriceFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::DeniedPosition>()
                  .HasSchemaName(PriceFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::Order>()
                  .HasSchemaName(PriceFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::OrderPosition>()
                  .HasSchemaName(PriceFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::OrderPositionAdvertisement>()
                  .HasSchemaName(PriceFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::Price>()
                  .HasSchemaName(PriceFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::PricePosition>()
                  .HasSchemaName(PriceFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::PricePositionNotActive>()
                  .HasSchemaName(PriceFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::Project>()
                  .HasSchemaName(PriceFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::Position>()
                  .HasSchemaName(PriceFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::Category>()
                  .HasSchemaName(PriceFactsSchema)
                  .HasPrimaryKey(x => x.Id);
            builder.Entity<PriceFacts::Theme>()
                  .HasSchemaName(PriceFactsSchema)
                  .HasPrimaryKey(x => x.Id);

            // TODO: хак чтобы не делать факты со сложным ключом
            builder.Entity<PriceFacts::RulesetRule>()
                  .HasSchemaName(PriceFactsSchema)
                  .HasPrimaryKey(x => x.Id)
                  .HasPrimaryKey(x => x.RuleType)
                  .HasPrimaryKey(x => x.DependentPositionId)
                  .HasPrimaryKey(x => x.PrincipalPositionId);

            return builder;
        }

        private static FluentMappingBuilder RegisterProjectFacts(this FluentMappingBuilder builder)
        {
            builder.Entity<ProjectFacts::Firm>()
                  .HasSchemaName(ProjectFactsSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<ProjectFacts::Order>()
                  .HasSchemaName(ProjectFactsSchema)
                  .HasPrimaryKey(x => x.Id);

            return builder;
        }

        private static FluentMappingBuilder RegisterAccountFacts(this FluentMappingBuilder builder)
        {
            builder.Entity<AccountFacts::Order>()
                  .HasSchemaName(AccountFactsSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<AccountFacts::Account>()
                  .HasSchemaName(AccountFactsSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<AccountFacts::Project>()
                  .HasSchemaName(AccountFactsSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<AccountFacts::Lock>()
                  .HasSchemaName(AccountFactsSchema)
                  .HasPrimaryKey(x => x.Id);

            builder.Entity<AccountFacts::Limit>()
              .HasSchemaName(AccountFactsSchema)
              .HasPrimaryKey(x => x.Id);

            builder.Entity<AccountFacts::ReleaseWithdrawal>()
              .HasSchemaName(AccountFactsSchema)
              .HasPrimaryKey(x => x.Id);

            builder.Entity<AccountFacts::OrderPosition>()
              .HasSchemaName(AccountFactsSchema)
              .HasPrimaryKey(x => x.Id);

            return builder;
        }

        private static FluentMappingBuilder RegisterConsistencyFacts(this FluentMappingBuilder builder)
        {
            builder.Entity<ConsistencyFacts::Bargain>()
                   .HasSchemaName(ConsistencyFactsSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity<ConsistencyFacts::BargainScanFile>()
                   .HasSchemaName(ConsistencyFactsSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity<ConsistencyFacts::Bill>()
                   .HasSchemaName(ConsistencyFactsSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity<ConsistencyFacts::Category>()
                   .HasSchemaName(ConsistencyFactsSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity<ConsistencyFacts::CategoryFirmAddress>()
                   .HasSchemaName(ConsistencyFactsSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity<ConsistencyFacts::Firm>()
                   .HasSchemaName(ConsistencyFactsSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity<ConsistencyFacts::FirmAddress>()
                   .HasSchemaName(ConsistencyFactsSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity<ConsistencyFacts::LegalPersonProfile>()
                   .HasSchemaName(ConsistencyFactsSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity<ConsistencyFacts::Order>()
                   .HasSchemaName(ConsistencyFactsSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity < ConsistencyFacts::OrderPosition> ()
                   .HasSchemaName(ConsistencyFactsSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity<ConsistencyFacts::OrderPositionAdvertisement>()
                   .HasSchemaName(ConsistencyFactsSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity<ConsistencyFacts::OrderScanFile>()
                   .HasSchemaName(ConsistencyFactsSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity<ConsistencyFacts::Position>()
                   .HasSchemaName(ConsistencyFactsSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity<ConsistencyFacts::Project>()
                   .HasSchemaName(ConsistencyFactsSchema)
                   .HasPrimaryKey(x => x.Id);

            builder.Entity<ConsistencyFacts::ReleaseWithdrawal>()
                   .HasSchemaName(ConsistencyFactsSchema)
                   .HasPrimaryKey(x => x.Id);

            return builder;
        }
    }
}
