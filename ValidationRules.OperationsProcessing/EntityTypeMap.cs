using System;

using NuClear.ValidationRules.OperationsProcessing.Contexts;
using NuClear.Model.Common;
using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes;

using AccountFacts = NuClear.ValidationRules.Storage.Model.AccountRules.Facts;
using PriceFacts = NuClear.ValidationRules.Storage.Model.PriceRules.Facts;
using ConsistencyFacts = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts;
using AdvertisementFacts = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;

namespace NuClear.ValidationRules.OperationsProcessing
{
    public static class EntityTypeMap
    {
        private static readonly Action<EntityTypeMappingRegistryBuilder> ErmTypeMap
            = builder => builder
                .AddMapping<EntityTypeAssociatedPosition, Storage.Model.Erm.AssociatedPosition>()
                .AddMapping<EntityTypeAssociatedPositionsGroup, Storage.Model.Erm.AssociatedPositionsGroup>()
                .AddMapping<EntityTypeCategory, Storage.Model.Erm.Category>()
                .AddMapping<EntityTypeDeniedPosition, Storage.Model.Erm.DeniedPosition>()
                .AddMapping<EntityTypeRuleset, Storage.Model.Erm.Ruleset>()
                .AddMapping<EntityTypeOrder, Storage.Model.Erm.Order>()
                .AddMapping<EntityTypeOrderPosition, Storage.Model.Erm.OrderPosition>()
                .AddMapping<EntityTypeOrderPositionAdvertisement, Storage.Model.Erm.OrderPositionAdvertisement>()
                .AddMapping<EntityTypeOrganizationUnit, Storage.Model.Erm.OrganizationUnit>()
                .AddMapping<EntityTypePosition, Storage.Model.Erm.Position>()
                .AddMapping<EntityTypePrice, Storage.Model.Erm.Price>()
                .AddMapping<EntityTypePricePosition, Storage.Model.Erm.PricePosition>()
                .AddMapping<EntityTypeProject, Storage.Model.Erm.Project>();

        private static readonly Action<EntityTypeMappingRegistryBuilder> AccountFactsTypeMap
            = builder => builder
                             .AddMapping<EntityTypeAccount, AccountFacts::Account>()
                             .AddMapping<EntityTypeOrder, AccountFacts::Order>()
                             .AddMapping<EntityTypeProject, AccountFacts::Project>()
                             .AddMapping<EntityTypeLock, AccountFacts::Lock>()
                             .AddMapping<EntityTypeLimit, AccountFacts::Limit>()
                             .AddMapping<EntityTypeOrderPosition, AccountFacts::OrderPosition>()
                             .AddMapping<EntityTypeReleaseWithdrawal, AccountFacts::ReleaseWithdrawal>();

        private static readonly Action<EntityTypeMappingRegistryBuilder> ConsistencyFactsTypeMap
            = builder => builder
                             .AddMapping<EntityTypeBargain, ConsistencyFacts::Bargain>()
                             .AddMapping<EntityTypeBargainFile, ConsistencyFacts::BargainScanFile>()
                             .AddMapping<EntityTypeBill, ConsistencyFacts::Bill>()
                             .AddMapping<EntityTypeCategory, ConsistencyFacts::Category>()
                             .AddMapping<EntityTypeCategoryFirmAddress, ConsistencyFacts::CategoryFirmAddress>()
                             .AddMapping<EntityTypeFirm, ConsistencyFacts::Firm>()
                             .AddMapping<EntityTypeFirmAddress, ConsistencyFacts::FirmAddress>()
                             .AddMapping<EntityTypeLegalPersonProfile, ConsistencyFacts::LegalPersonProfile>()
                             .AddMapping<EntityTypeOrder, ConsistencyFacts::Order>()
                             .AddMapping<EntityTypeOrderPosition, ConsistencyFacts::OrderPosition>()
                             .AddMapping<EntityTypeOrderPositionAdvertisement, ConsistencyFacts::OrderPositionAdvertisement>()
                             .AddMapping<EntityTypeOrderFile, ConsistencyFacts::OrderScanFile>()
                             .AddMapping<EntityTypePosition, ConsistencyFacts::Position>()
                             .AddMapping<EntityTypeProject, ConsistencyFacts::Project>()
                             .AddMapping<EntityTypeReleaseWithdrawal, ConsistencyFacts::ReleaseWithdrawal>();

        private static readonly Action<EntityTypeMappingRegistryBuilder> PriceFactsTypeMap
            = builder => builder
                             .AddMapping<EntityTypeAssociatedPosition, PriceFacts::AssociatedPosition>()
                             .AddMapping<EntityTypeAssociatedPositionsGroup, PriceFacts::AssociatedPositionsGroup>()
                             .AddMapping<EntityTypeCategory, PriceFacts::Category>()
                             .AddMapping<EntityTypeDeniedPosition, PriceFacts::DeniedPosition>()
                             .AddMapping<EntityTypeOrder, PriceFacts::Order>()
                             .AddMapping<EntityTypeOrderPosition, PriceFacts::OrderPosition>()
                             .AddMapping<EntityTypeOrderPositionAdvertisement, PriceFacts::OrderPositionAdvertisement>()
                             .AddMapping<EntityTypePosition, PriceFacts::Position>()
                             .AddMapping<EntityTypePrice, PriceFacts::Price>()
                             .AddMapping<EntityTypePricePosition, PriceFacts::PricePosition>()
                             //.AddMapping<EntityTypePricePositionNotActive, PriceFacts::PricePositionNotActive>()
                             .AddMapping<EntityTypeProject, PriceFacts::Project>()
                             .AddMapping<EntityTypeRuleset, PriceFacts::RulesetRule>()
                             .AddMapping<EntityTypeTheme, PriceFacts::Theme>();

        private static readonly Action<EntityTypeMappingRegistryBuilder> AdvertisementFactsTypeMap
            = builder => builder
                             .AddMapping<EntityTypeOrder, AdvertisementFacts::Order>()
                             .AddMapping<EntityTypeProject, AdvertisementFacts::Project>()
                             .AddMapping<EntityTypeOrderPosition, AdvertisementFacts::OrderPosition>()
                             .AddMapping<EntityTypeOrderPositionAdvertisement, AdvertisementFacts::OrderPositionAdvertisement>()
                             .AddMapping<EntityTypePricePosition, AdvertisementFacts::PricePosition>()
                             .AddMapping<EntityTypePosition, AdvertisementFacts::Position>()
                             .AddMapping<EntityTypeAdvertisementTemplate, AdvertisementFacts::AdvertisementTemplate>()
                             .AddMapping<EntityTypeAdvertisement, AdvertisementFacts::Advertisement>()
                             .AddMapping<EntityTypeFirm, AdvertisementFacts::Firm>()
                             .AddMapping<EntityTypeAdvertisementElement, AdvertisementFacts::AdvertisementElement>()
                             .AddMapping<EntityTypeAdvertisementElementTemplate, AdvertisementFacts::AdvertisementElementTemplate>();


        private static readonly Action<EntityTypeMappingRegistryBuilder> AggregateTypeMap
            = builder => builder
                .AddMapping<EntityTypeOrder, Storage.Model.PriceRules.Aggregates.Order>()
                .AddMapping<EntityTypePrice, Storage.Model.PriceRules.Aggregates.Price>()
                .AddMapping<EntityTypePosition, Storage.Model.PriceRules.Aggregates.Position>()
                .AddMapping<EntityTypePeriod, Storage.Model.PriceRules.Aggregates.Period>()
                .AddAsPersistenceOnly(typeof(Storage.Model.PriceRules.Aggregates.AdvertisementAmountRestriction))
                .AddAsPersistenceOnly(typeof(Storage.Model.PriceRules.Aggregates.OrderPeriod))
                .AddAsPersistenceOnly(typeof(Storage.Model.PriceRules.Aggregates.OrderPosition))
                .AddAsPersistenceOnly(typeof(Storage.Model.PriceRules.Aggregates.OrderPricePosition))
                .AddAsPersistenceOnly(typeof(Storage.Model.PriceRules.Aggregates.PricePeriod))
                .AddAsPersistenceOnly(typeof(Storage.Model.PriceRules.Aggregates.Project));

        public static IEntityTypeMappingRegistry<ErmSubDomain> CreateErmContext()
        {
            var builder = new EntityTypeMappingRegistryBuilder();
            ErmTypeMap.Invoke(builder);
            return builder.Create<ErmSubDomain>();
        }

        public static IEntityTypeMappingRegistry<AccountFactsSubDomain> CreateAccountFactsContext()
        {
            var builder = new EntityTypeMappingRegistryBuilder();
            AccountFactsTypeMap.Invoke(builder);
            return builder.Create<AccountFactsSubDomain>();
        }

        public static IEntityTypeMappingRegistry<ConsistencyFactsSubDomain> CreateConsistencyFactsContext()
        {
            var builder = new EntityTypeMappingRegistryBuilder();
            ConsistencyFactsTypeMap.Invoke(builder);
            return builder.Create<ConsistencyFactsSubDomain>();
        }

        public static IEntityTypeMappingRegistry<PriceFactsSubDomain> CreatePriceFactsContext()
        {
            var builder = new EntityTypeMappingRegistryBuilder();
            PriceFactsTypeMap.Invoke(builder);
            return builder.Create<PriceFactsSubDomain>();
        }

        public static IEntityTypeMappingRegistry<AdvertisementFactsSubDomain> CreateAdvertisementFactsContext()
        {
            var builder = new EntityTypeMappingRegistryBuilder();
            AdvertisementFactsTypeMap.Invoke(builder);
            return builder.Create<AdvertisementFactsSubDomain>();
        }

        public static IEntityTypeMappingRegistry<AggregateSubDomain> CreateAggregateContext()
        {
            var builder = new EntityTypeMappingRegistryBuilder();
            AggregateTypeMap.Invoke(builder);
            return builder.Create<AggregateSubDomain>();
        }
    }

    public static class EntityTypeMappingRegistryBuilderExtensions
    {
        public static EntityTypeMappingRegistryBuilder AddMapping<TEntityType, TAggregateType>(this EntityTypeMappingRegistryBuilder registryBuilder)
            where TEntityType : IdentityBase<TEntityType>, IEntityType, new()
        {
            return registryBuilder.AddMapping(IdentityBase<TEntityType>.Instance, typeof(TAggregateType));
        }
    }
}