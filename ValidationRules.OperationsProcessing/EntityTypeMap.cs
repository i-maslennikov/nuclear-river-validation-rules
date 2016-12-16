using System;

using NuClear.ValidationRules.OperationsProcessing.Contexts;
using NuClear.Model.Common;
using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.OperationsProcessing
{
    public static class EntityTypeMap
    {
        private static readonly Action<EntityTypeMappingRegistryBuilder> ErmTypeMap
            = builder => builder
                .AddMapping<EntityTypeAccount, Erm::Account>()
                .AddMapping<EntityTypeAdvertisement, Erm::Advertisement>()
                .AddMapping<EntityTypeAdvertisementElement, Erm::AdvertisementElement>()
                .AddMapping<EntityTypeAdvertisementElementStatus, Erm::AdvertisementElementStatus>()
                .AddMapping<EntityTypeAdvertisementElementTemplate, Erm::AdvertisementElementTemplate>()
                .AddMapping<EntityTypeAdvertisementTemplate, Erm::AdvertisementTemplate>()
                .AddMapping<EntityTypeAssociatedPosition, Erm::AssociatedPosition>()
                .AddMapping<EntityTypeAssociatedPositionsGroup, Erm::AssociatedPositionsGroup>()
                .AddMapping<EntityTypeBargain, Erm::Bargain>()
                .AddMapping<EntityTypeBargainFile, Erm::BargainFile>()
                .AddMapping<EntityTypeBill, Erm::Bill>()
                .AddMapping<EntityTypeBranchOffice, Erm::BranchOffice>()
                .AddMapping<EntityTypeBranchOfficeOrganizationUnit, Erm::BranchOfficeOrganizationUnit>()
                .AddMapping<EntityTypeCategory, Erm::Category>()
                .AddMapping<EntityTypeCategoryFirmAddress, Erm::CategoryFirmAddress>()
                .AddMapping<EntityTypeCategoryOrganizationUnit, Erm::CategoryOrganizationUnit>()
                .AddMapping<EntityTypeDeal, Erm::Deal>()
                .AddMapping<EntityTypeDeniedPosition, Erm::DeniedPosition>()
                .AddMapping<EntityTypeFirm, Erm::Firm>()
                .AddMapping<EntityTypeFirmAddress, Erm::FirmAddress>()
                .AddMapping<EntityTypeFirmContact, Erm::FirmContact>()
                .AddMapping<EntityTypeLegalPerson, Erm::LegalPerson>()
                .AddMapping<EntityTypeLegalPersonProfile, Erm::LegalPersonProfile>()
                .AddMapping<EntityTypeLock, Erm::Lock>()
                .AddMapping<EntityTypeOrder, Erm::Order>()
                .AddMapping<EntityTypeOrderFile, Erm::OrderFile>()
                .AddMapping<EntityTypeOrderPosition, Erm::OrderPosition>()
                .AddMapping<EntityTypeOrderPositionAdvertisement, Erm::OrderPositionAdvertisement>()
                .AddMapping<EntityTypePosition, Erm::Position>()
                .AddMapping<EntityTypePrice, Erm::Price>()
                .AddMapping<EntityTypePricePosition, Erm::PricePosition>()
                .AddMapping<EntityTypeProject, Erm::Project>()
                .AddMapping<EntityTypeReleaseInfo, Erm::ReleaseInfo>()
                .AddMapping<EntityTypeReleaseWithdrawal, Erm::ReleaseWithdrawal>()
                .AddMapping<EntityTypeRuleset, Erm::Ruleset>()
                .AddMapping<EntityTypeTheme, Erm::Theme>()
                .AddMapping<EntityTypeThemeCategory, Erm::ThemeCategory>()
                .AddMapping<EntityTypeThemeOrganizationUnit, Erm::ThemeOrganizationUnit>();

        private static readonly Action<EntityTypeMappingRegistryBuilder> FactsTypeMap
            = builder => builder
                .AddMapping<EntityTypeAccount, Facts::Account>()
                .AddMapping<EntityTypeAdvertisement, Facts::Advertisement>()
                .AddMapping<EntityTypeAdvertisementElement, Facts::AdvertisementElement>()
                .AddMapping<EntityTypeAdvertisementElementTemplate, Facts::AdvertisementElementTemplate>()
                .AddMapping<EntityTypeAdvertisementTemplate, Facts::AdvertisementTemplate>()
                .AddMapping<EntityTypeAssociatedPosition, Facts::AssociatedPosition>()
                .AddMapping<EntityTypeAssociatedPositionsGroup, Facts::AssociatedPositionsGroup>()
                .AddMapping<EntityTypeBargain, Facts::Bargain>()
                .AddMapping<EntityTypeBargainFile, Facts::BargainScanFile>()
                .AddMapping<EntityTypeBill, Facts::Bill>()
                .AddMapping<EntityTypeBranchOffice, Facts::BranchOffice>()
                .AddMapping<EntityTypeBranchOfficeOrganizationUnit, Facts::BranchOfficeOrganizationUnit>()
                .AddMapping<EntityTypeCategory, Facts::Category>()
                .AddMapping<EntityTypeCategoryOrganizationUnit, Facts::CategoryOrganizationUnit>()
                .AddAsPersistenceOnly(typeof(Facts::CostPerClickCategoryRestriction))
                .AddMapping<EntityTypeDeal, Facts::Deal>()
                .AddMapping<EntityTypeDeniedPosition, Facts::DeniedPosition>()
                .AddMapping<EntityTypeFirm, Facts::Firm>()
                .AddMapping<EntityTypeFirmAddress, Facts::FirmAddress>()
                .AddMapping<EntityTypeCategoryFirmAddress, Facts::FirmAddressCategory>()
                .AddMapping<EntityTypeFirmContact, Facts::FirmAddressWebsite>()
                .AddMapping<EntityTypeLegalPerson, Facts::LegalPerson>()
                .AddMapping<EntityTypeLegalPersonProfile, Facts::LegalPersonProfile>()
                .AddMapping<EntityTypeLock, Facts::Lock>()
                .AddAsPersistenceOnly(typeof(Facts::NomenclatureCategory))
                .AddMapping<EntityTypeOrder, Facts::Order>()
                .AddMapping<EntityTypeOrderPosition, Facts::OrderPosition>()
                .AddMapping<EntityTypeOrderPositionAdvertisement, Facts::OrderPositionAdvertisement>()
                .AddAsPersistenceOnly(typeof(Facts::OrderPositionCostPerClick))
                .AddMapping<EntityTypeOrderFile, Facts::OrderScanFile>()
                .AddMapping<EntityTypePosition, Facts::Position>()
                .AddAsPersistenceOnly(typeof(Facts::PositionChild))
                .AddMapping<EntityTypePrice, Facts::Price>()
                .AddMapping<EntityTypePricePosition, Facts::PricePosition>()
                .AddMapping<EntityTypeProject, Facts::Project>()
                .AddMapping<EntityTypeReleaseInfo, Facts::ReleaseInfo>()
                .AddMapping<EntityTypeReleaseWithdrawal, Facts::ReleaseWithdrawal>()
                .AddMapping<EntityTypeRuleset, Facts::RulesetRule>()
                .AddAsPersistenceOnly(typeof(Facts::SalesModelCategoryRestriction))
                .AddMapping<EntityTypeTheme, Facts::Theme>()
                .AddMapping<EntityTypeThemeCategory, Facts::ThemeCategory>()
                .AddMapping<EntityTypeThemeOrganizationUnit, Facts::ThemeOrganizationUnit>()
                .AddAsPersistenceOnly(typeof(Facts::UnlimitedOrder));

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

        public static IEntityTypeMappingRegistry<FactsSubDomain> CreateFactsContext()
        {
            var builder = new EntityTypeMappingRegistryBuilder();
            FactsTypeMap.Invoke(builder);
            return builder.Create<FactsSubDomain>();
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