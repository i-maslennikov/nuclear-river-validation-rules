using System;

using NuClear.ValidationRules.OperationsProcessing.Contexts;
using NuClear.Model.Common;
using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes;

using AccountFacts = NuClear.ValidationRules.Storage.Model.AccountRules.Facts;
using PriceFacts = NuClear.ValidationRules.Storage.Model.PriceRules.Facts;
using ConsistencyFacts = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts;
using FirmFacts = NuClear.ValidationRules.Storage.Model.FirmRules.Facts;
using AdvertisementFacts = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;
using ProjectFacts = NuClear.ValidationRules.Storage.Model.ProjectRules.Facts;
using ThemeFacts = NuClear.ValidationRules.Storage.Model.ThemeRules.Facts;

namespace NuClear.ValidationRules.OperationsProcessing
{
    public static class EntityTypeMap
    {
        private static readonly Action<EntityTypeMappingRegistryBuilder> ErmTypeMap
            = builder => builder
                .AddMapping<EntityTypeAccount, Storage.Model.Erm.Account>()
                .AddMapping<EntityTypeAdvertisement, Storage.Model.Erm.Advertisement>()
                .AddMapping<EntityTypeAdvertisementElement, Storage.Model.Erm.AdvertisementElement>()
                .AddMapping<EntityTypeAdvertisementElementStatus, Storage.Model.Erm.AdvertisementElementStatus>()
                .AddMapping<EntityTypeAdvertisementElementTemplate, Storage.Model.Erm.AdvertisementElementTemplate>()
                .AddMapping<EntityTypeAdvertisementTemplate, Storage.Model.Erm.AdvertisementTemplate>()
                .AddMapping<EntityTypeAssociatedPosition, Storage.Model.Erm.AssociatedPosition>()
                .AddMapping<EntityTypeAssociatedPositionsGroup, Storage.Model.Erm.AssociatedPositionsGroup>()
                .AddMapping<EntityTypeBargain, Storage.Model.Erm.Bargain>()
                .AddMapping<EntityTypeBargainFile, Storage.Model.Erm.BargainFile>()
                .AddMapping<EntityTypeBill, Storage.Model.Erm.Bill>()
                .AddMapping<EntityTypeBranchOffice, Storage.Model.Erm.BranchOffice>()
                .AddMapping<EntityTypeBranchOfficeOrganizationUnit, Storage.Model.Erm.BranchOfficeOrganizationUnit>()
                .AddMapping<EntityTypeCategory, Storage.Model.Erm.Category>()
                .AddMapping<EntityTypeCategoryFirmAddress, Storage.Model.Erm.CategoryFirmAddress>()
                .AddMapping<EntityTypeCategoryOrganizationUnit, Storage.Model.Erm.CategoryOrganizationUnit>()
                .AddMapping<EntityTypeDeal, Storage.Model.Erm.Deal>()
                .AddMapping<EntityTypeDeniedPosition, Storage.Model.Erm.DeniedPosition>()
                .AddMapping<EntityTypeFirm, Storage.Model.Erm.Firm>()
                .AddMapping<EntityTypeFirmAddress, Storage.Model.Erm.FirmAddress>()
                .AddMapping<EntityTypeFirmContact, Storage.Model.Erm.FirmContact>()
                .AddMapping<EntityTypeLegalPerson, Storage.Model.Erm.LegalPerson>()
                .AddMapping<EntityTypeLegalPersonProfile, Storage.Model.Erm.LegalPersonProfile>()
                .AddMapping<EntityTypeLock, Storage.Model.Erm.Lock>()
                .AddMapping<EntityTypeOrder, Storage.Model.Erm.Order>()
                .AddMapping<EntityTypeOrderFile, Storage.Model.Erm.OrderFile>()
                .AddMapping<EntityTypeOrderPosition, Storage.Model.Erm.OrderPosition>()
                .AddMapping<EntityTypeOrderPositionAdvertisement, Storage.Model.Erm.OrderPositionAdvertisement>()
                .AddMapping<EntityTypeOrganizationUnit, Storage.Model.Erm.OrganizationUnit>()
                .AddMapping<EntityTypePosition, Storage.Model.Erm.Position>()
                .AddMapping<EntityTypePosition, Storage.Model.Erm.Position>()
                .AddMapping<EntityTypePrice, Storage.Model.Erm.Price>()
                .AddMapping<EntityTypePricePosition, Storage.Model.Erm.PricePosition>()
                .AddMapping<EntityTypeProject, Storage.Model.Erm.Project>()
                .AddMapping<EntityTypeReleaseInfo, Storage.Model.Erm.ReleaseInfo>()
                .AddMapping<EntityTypeReleaseWithdrawal, Storage.Model.Erm.ReleaseWithdrawal>()
                .AddMapping<EntityTypeRuleset, Storage.Model.Erm.Ruleset>()
                .AddMapping<EntityTypeTheme, Storage.Model.Erm.Theme>()
                .AddMapping<EntityTypeThemeCategory, Storage.Model.Erm.ThemeCategory>()
                .AddMapping<EntityTypeThemeOrganizationUnit, Storage.Model.Erm.ThemeOrganizationUnit>();

        private static readonly Action<EntityTypeMappingRegistryBuilder> AccountFactsTypeMap
            = builder => builder
                .AddMapping<EntityTypeAccount, AccountFacts::Account>()
                .AddMapping<EntityTypeOrder, AccountFacts::Order>()
                .AddMapping<EntityTypeProject, AccountFacts::Project>()
                .AddMapping<EntityTypeLock, AccountFacts::Lock>()
                .AddMapping<EntityTypeOrderPosition, AccountFacts::OrderPosition>()
                .AddMapping<EntityTypeReleaseWithdrawal, AccountFacts::ReleaseWithdrawal>();

        private static readonly Action<EntityTypeMappingRegistryBuilder> ConsistencyFactsTypeMap
            = builder => builder
                .AddMapping<EntityTypeBargain, ConsistencyFacts::Bargain>()
                .AddMapping<EntityTypeBargainFile, ConsistencyFacts::BargainScanFile>()
                .AddMapping<EntityTypeBranchOffice, ConsistencyFacts::BranchOffice>()
                .AddMapping<EntityTypeBranchOfficeOrganizationUnit, ConsistencyFacts::BranchOfficeOrganizationUnit>()
                .AddMapping<EntityTypeBill, ConsistencyFacts::Bill>()
                .AddMapping<EntityTypeCategory, ConsistencyFacts::Category>()
                .AddMapping<EntityTypeCategoryFirmAddress, ConsistencyFacts::CategoryFirmAddress>()
                .AddMapping<EntityTypeDeal, ConsistencyFacts::Deal>()
                .AddMapping<EntityTypeFirm, ConsistencyFacts::Firm>()
                .AddMapping<EntityTypeFirmAddress, ConsistencyFacts::FirmAddress>()
                .AddMapping<EntityTypeLegalPerson, ConsistencyFacts::LegalPerson>()
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

        private static readonly Action<EntityTypeMappingRegistryBuilder> FirmFactsTypeMap
            = builder => builder
                .AddMapping<EntityTypeFirm, FirmFacts::Firm>()
                .AddMapping<EntityTypeFirmAddress, FirmFacts::FirmAddress>()
                .AddMapping<EntityTypeCategoryFirmAddress, FirmFacts::FirmAddressCategory>()
                .AddMapping<EntityTypeOrder, FirmFacts::Order>()
                .AddMapping<EntityTypeOrderPosition, FirmFacts::OrderPosition>()
                .AddMapping<EntityTypeOrderPositionAdvertisement, FirmFacts::OrderPositionAdvertisement>()
                .AddMapping<EntityTypePosition, FirmFacts::SpecialPosition>()
                .AddMapping<EntityTypeProject, FirmFacts::Project>();

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
                .AddMapping<EntityTypeFirmAddress, AdvertisementFacts::FirmAddress>()
                .AddMapping<EntityTypeFirmContact, AdvertisementFacts::FirmAddressWebsite>()
                .AddMapping<EntityTypeAdvertisementElement, AdvertisementFacts::AdvertisementElement>()
                .AddMapping<EntityTypeAdvertisementElementTemplate, AdvertisementFacts::AdvertisementElementTemplate>();

        private static readonly Action<EntityTypeMappingRegistryBuilder> ProjectFactsTypeMap
            = builder => builder
                .AddMapping<EntityTypeProject, ProjectFacts::Project>()
                .AddMapping<EntityTypeCategory, ProjectFacts::Category>()
                .AddMapping<EntityTypeCategoryOrganizationUnit, ProjectFacts::CategoryOrganizationUnit>()
                .AddMapping<EntityTypeFirmAddress, ProjectFacts::FirmAddress>()
                .AddMapping<EntityTypeOrder, ProjectFacts::Order>()
                .AddMapping<EntityTypeOrderPosition, ProjectFacts::OrderPosition>()
                .AddMapping<EntityTypeOrderPositionAdvertisement, ProjectFacts::OrderPositionAdvertisement>()
                .AddMapping<EntityTypePosition, ProjectFacts::Position>()
                .AddMapping<EntityTypePricePosition, ProjectFacts::PricePosition>()
                .AddMapping<EntityTypeReleaseInfo, ProjectFacts::ReleaseInfo>()
                .AddAsPersistenceOnly(typeof(ProjectFacts::OrderPositionCostPerClick))
                .AddAsPersistenceOnly(typeof(ProjectFacts::CostPerClickCategoryRestriction))
                .AddAsPersistenceOnly(typeof(ProjectFacts::SalesModelCategoryRestriction));

        private static readonly Action<EntityTypeMappingRegistryBuilder> ThemeFactsTypeMap
            = builder => builder
                .AddMapping<EntityTypeTheme, ThemeFacts::Theme>()
                .AddMapping<EntityTypeThemeCategory, ThemeFacts::ThemeCategory>()
                .AddMapping<EntityTypeThemeOrganizationUnit, ThemeFacts::ThemeOrganizationUnit>()
                .AddMapping<EntityTypeCategory, ThemeFacts::Category>()
                .AddMapping<EntityTypeOrder, ThemeFacts::Order>()
                .AddMapping<EntityTypeOrderPosition, ThemeFacts::OrderPosition>()
                .AddMapping<EntityTypeOrderPositionAdvertisement, ThemeFacts::OrderPositionAdvertisement>()
                .AddMapping<EntityTypeProject, ThemeFacts::Project>();

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

        public static IEntityTypeMappingRegistry<FirmFactsSubDomain> CreateFirmFactsContext()
        {
            var builder = new EntityTypeMappingRegistryBuilder();
            FirmFactsTypeMap.Invoke(builder);
            return builder.Create<FirmFactsSubDomain>();
        }

        public static IEntityTypeMappingRegistry<AdvertisementFactsSubDomain> CreateAdvertisementFactsContext()
        {
            var builder = new EntityTypeMappingRegistryBuilder();
            AdvertisementFactsTypeMap.Invoke(builder);
            return builder.Create<AdvertisementFactsSubDomain>();
        }

        public static IEntityTypeMappingRegistry<ProjectFactsSubDomain> CreateProjectFactsContext()
        {
            var builder = new EntityTypeMappingRegistryBuilder();
            ProjectFactsTypeMap.Invoke(builder);
            return builder.Create<ProjectFactsSubDomain>();
        }

        public static IEntityTypeMappingRegistry<AdvertisementFactsSubDomain> CreateThemeFactsContext()
        {
            var builder = new EntityTypeMappingRegistryBuilder();
            ThemeFactsTypeMap.Invoke(builder);
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