using System;
using System.Collections.Generic;

using NuClear.Model.Common;
using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

using AccountAggregates = NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;
using AdvertisementAggregates = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using PriceAggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using ProjectAggregates = NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;
using ConsistencyAggregates = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using FirmAggregates = NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using ThemeAggregates = NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates;

namespace NuClear.ValidationRules.OperationsProcessing
{
    internal static class EntityTypeMap
    {
        private static readonly Dictionary<long, IReadOnlyCollection<Type>> FactsTypeMap = new Dictionary<long, IReadOnlyCollection<Type>>()
            .AddMapping<EntityTypeAccount>(typeof(Facts::Account))
            .AddMapping<EntityTypeAdvertisement>(typeof(Facts::Advertisement))
            .AddMapping<EntityTypeAdvertisementElement>(typeof(Facts::AdvertisementElement))
            .AddMapping<EntityTypeAdvertisementElementStatus>(typeof(Facts::AdvertisementElement))
            .AddMapping<EntityTypeAdvertisementElementTemplate>(typeof(Facts::AdvertisementElementTemplate))
            .AddMapping<EntityTypeAdvertisementTemplate>(typeof(Facts::AdvertisementTemplate))
            .AddMapping<EntityTypeAssociatedPosition>(typeof(Facts::AssociatedPosition))
            .AddMapping<EntityTypeAssociatedPositionsGroup>(typeof(Facts::AssociatedPositionsGroup))
            .AddMapping<EntityTypeBargain>(typeof(Facts::Bargain))
            .AddMapping<EntityTypeBargainFile>(typeof(Facts::BargainScanFile))
            .AddMapping<EntityTypeBill>(typeof(Facts::Bill))
            .AddMapping<EntityTypeBranchOffice>(typeof(Facts::BranchOffice))
            .AddMapping<EntityTypeBranchOfficeOrganizationUnit>(typeof(Facts::BranchOfficeOrganizationUnit))
            .AddMapping<EntityTypeCategory>(typeof(Facts::Category))
            .AddMapping<EntityTypeCategoryOrganizationUnit>(typeof(Facts::CategoryOrganizationUnit))
            .AddMapping<EntityTypeDeal>(typeof(Facts::Deal))
            .AddMapping<EntityTypeDeniedPosition>(typeof(Facts::DeniedPosition))
            .AddMapping<EntityTypeFirm>(typeof(Facts::Firm))
            .AddMapping<EntityTypeFirmAddress>(typeof(Facts::FirmAddress))
            .AddMapping<EntityTypeCategoryFirmAddress>(typeof(Facts::FirmAddressCategory))
            .AddMapping<EntityTypeFirmContact>(typeof(Facts::FirmAddressWebsite))
            .AddMapping<EntityTypeLegalPerson>(typeof(Facts::LegalPerson))
            .AddMapping<EntityTypeLegalPersonProfile>(typeof(Facts::LegalPersonProfile))
            .AddMapping<EntityTypeLock>(typeof(Facts::Lock))
            .AddMapping<EntityTypeOrder>(typeof(Facts::Order),
                                         typeof(Facts::UnlimitedOrder))
            .AddMapping<EntityTypeOrderPosition>(typeof(Facts::OrderPosition),
                                                 typeof(Facts::OrderPositionCostPerClick))
            .AddMapping<EntityTypeOrderPositionAdvertisement>(typeof(Facts::OrderPositionAdvertisement))
            .AddMapping<EntityTypeOrderFile>(typeof(Facts::OrderScanFile))
            .AddMapping<EntityTypePosition>(typeof(Facts::Position),
                                            typeof(Facts::PositionChild))
            .AddMapping<EntityTypePrice>(typeof(Facts::Price))
            .AddMapping<EntityTypePricePosition>(typeof(Facts::PricePosition),
                                                 typeof(Facts::NomenclatureCategory))
            .AddMapping<EntityTypeProject>(typeof(Facts::Project),
                                           typeof(Facts::CostPerClickCategoryRestriction),
                                           typeof(Facts::SalesModelCategoryRestriction))
            .AddMapping<EntityTypeReleaseInfo>(typeof(Facts::ReleaseInfo))
            .AddMapping<EntityTypeReleaseWithdrawal>(typeof(Facts::ReleaseWithdrawal))
            .AddMapping<EntityTypeRuleset>(typeof(Facts::RulesetRule))
            .AddMapping<EntityTypeTheme>(typeof(Facts::Theme))
            .AddMapping<EntityTypeThemeCategory>(typeof(Facts::ThemeCategory))
            .AddMapping<EntityTypeThemeOrganizationUnit>(typeof(Facts::ThemeOrganizationUnit));


        private static readonly Dictionary<Type, IReadOnlyCollection<Type>> AggregatesTypeMap = new Dictionary<Type, IReadOnlyCollection<Type>>()
            .AddMapping<Facts::Account>(typeof(AccountAggregates::Account))
            .AddMapping<Facts::Advertisement>(typeof(AdvertisementAggregates::Advertisement))
            .AddMapping<Facts::AdvertisementElementTemplate>(typeof(AdvertisementAggregates::AdvertisementElementTemplate))
            .AddMapping<Facts::Category>(typeof(PriceAggregates::Category),
                                         typeof(ProjectAggregates::Category),
                                         typeof(ThemeAggregates::Category))
            .AddMapping<Facts::Firm>(typeof(AdvertisementAggregates::Firm),
                                     typeof(FirmAggregates::Firm))
            .AddMapping<Facts::FirmAddress>(typeof(ProjectAggregates::FirmAddress))
            .AddMapping<Facts::Order>(typeof(AccountAggregates::Order),
                                      typeof(AdvertisementAggregates::Order),
                                      typeof(ConsistencyAggregates::Order),
                                      typeof(FirmAggregates::Order),
                                      typeof(PriceAggregates::Order),
                                      typeof(ProjectAggregates::Order),
                                      typeof(ThemeAggregates::Order))
            .AddMapping<Facts::Position>(typeof(PriceAggregates::Position),
                                         typeof(ProjectAggregates::Position),
                                         typeof(AdvertisementAggregates::Position))
            .AddMapping<Facts::Price>(typeof(PriceAggregates::Price))
            .AddMapping<Facts::Project>(typeof(PriceAggregates::Project),
                                        typeof(ProjectAggregates::Project),
                                        typeof(ThemeAggregates::Project))
            .AddMapping<Facts::Theme>(typeof(PriceAggregates::Theme),
                                      typeof(ThemeAggregates::Theme));

        private static readonly Dictionary<Tuple<Type, Type>, IReadOnlyCollection<Type>> RelatedAggregatesTypeMap = new Dictionary<Tuple<Type, Type>, IReadOnlyCollection<Type>>()
            .AddMapping<Facts::Account, Facts::Order>(typeof(AccountAggregates::Order))
            .AddMapping<Facts::Advertisement, Facts::Order>(typeof(AdvertisementAggregates::Order))
            .AddMapping<Facts::Advertisement, Facts::Firm>(typeof(AdvertisementAggregates::Firm))
            .AddMapping<Facts::AdvertisementElement, Facts::Advertisement>(typeof(AdvertisementAggregates::Advertisement))
            .AddMapping<Facts::AdvertisementElementTemplate, Facts::Advertisement>(typeof(AdvertisementAggregates::Advertisement))
            .AddMapping<Facts::AdvertisementTemplate, Facts::Advertisement>(typeof(AdvertisementAggregates::Advertisement))
            .AddMapping<Facts::AdvertisementTemplate, Facts::Order>(typeof(AdvertisementAggregates::Order))
            .AddMapping<Facts::AssociatedPosition, Facts::Order>(typeof(PriceAggregates::Order))
            .AddMapping<Facts::AssociatedPositionsGroup, Facts::Order>(typeof(PriceAggregates::Order))
            .AddMapping<Facts::AssociatedPositionsGroup, Facts::Price>(typeof(PriceAggregates::Price))
            .AddMapping<Facts::Bargain, Facts::Order>(typeof(ConsistencyAggregates::Order))
            .AddMapping<Facts::BargainScanFile, Facts::Order>(typeof(ConsistencyAggregates::Order))
            .AddMapping<Facts::Bill, Facts::Order>(typeof(ConsistencyAggregates::Order))
            .AddMapping<Facts::BranchOffice, Facts::Order>(typeof(ConsistencyAggregates::Order))
            .AddMapping<Facts::BranchOfficeOrganizationUnit, Facts::Order>(typeof(ConsistencyAggregates::Order))
            .AddMapping<Facts::Category, Facts::Theme>(typeof(ThemeAggregates::Theme))
            .AddMapping<Facts::Category, Facts::Order>(typeof(ConsistencyAggregates::Order),
                                                       typeof(PriceAggregates::Order),
                                                       typeof(ProjectAggregates::Order))
            .AddMapping<Facts::CategoryOrganizationUnit, Facts::Project>(typeof(ProjectAggregates::Project))
            .AddMapping<Facts::Deal, Facts::Order>(typeof(ConsistencyAggregates::Order))
            .AddMapping<Facts::DeniedPosition, Facts::Order>(typeof(PriceAggregates::Order))
            .AddMapping<Facts::Firm, Facts::Order>(typeof(AdvertisementAggregates::Order),
                                                   typeof(ConsistencyAggregates::Order),
                                                   typeof(FirmAggregates::Order))
            .AddMapping<Facts::FirmAddress, Facts::Firm>(typeof(AdvertisementAggregates::Firm),
                                                         typeof(FirmAggregates::Firm))
            .AddMapping<Facts::FirmAddress, Facts::Order>(typeof(ConsistencyAggregates::Order),
                                                         typeof(ProjectAggregates::Order))
            .AddMapping<Facts::FirmAddressCategory, Facts::Order>(typeof(ConsistencyAggregates::Order))
            .AddMapping<Facts::FirmAddressCategory, Facts::Firm>(typeof(FirmAggregates::Firm))
            .AddMapping<Facts::FirmAddressWebsite, Facts::Firm>(typeof(AdvertisementAggregates::Firm))
            .AddMapping<Facts::LegalPerson, Facts::Order>(typeof(ConsistencyAggregates::Order))
            .AddMapping<Facts::LegalPersonProfile, Facts::Order>(typeof(ConsistencyAggregates::Order))
            .AddMapping<Facts::Lock, Facts::Account>(typeof(AccountAggregates::Account))
            .AddMapping<Facts::Lock, Facts::Order>(typeof(AccountAggregates::Order))
            .AddMapping<Facts::Order, Facts::Account>(typeof(AccountAggregates::Account))
            .AddMapping<Facts::Order, Facts::Firm>(typeof(AdvertisementAggregates::Firm),
                                                   typeof(FirmAggregates::Firm))
            .AddMapping<Facts::OrderPosition, Facts::Order>(typeof(AdvertisementAggregates::Order),
                                                            typeof(ConsistencyAggregates::Order),
                                                            typeof(FirmAggregates::Order),
                                                            typeof(PriceAggregates::Order),
                                                            typeof(ProjectAggregates::Order),
                                                            typeof(ThemeAggregates::Order))
            .AddMapping<Facts::OrderPosition, Facts::Account>(typeof(AccountAggregates::Account))
            .AddMapping<Facts::OrderPosition, Facts::Firm>(typeof(AdvertisementAggregates::Firm),
                                                           typeof(FirmAggregates::Firm))
            .AddMapping<Facts::OrderPositionAdvertisement, Facts::Order>(typeof(AdvertisementAggregates::Order),
                                                                         typeof(ConsistencyAggregates::Order),
                                                                         typeof(FirmAggregates::Order),
                                                                         typeof(PriceAggregates::Order),
                                                                         typeof(ProjectAggregates::Order),
                                                                         typeof(ThemeAggregates::Order))
            .AddMapping<Facts::OrderPositionAdvertisement, Facts::Firm>(typeof(AdvertisementAggregates::Firm),
                                                                        typeof(FirmAggregates::Firm))
            .AddMapping<Facts::OrderScanFile, Facts::Order>(typeof(ConsistencyAggregates::Order))
            .AddMapping<Facts::Position, Facts::Order>(typeof(AdvertisementAggregates::Order),
                                                       typeof(ConsistencyAggregates::Order),
                                                       typeof(PriceAggregates::Order),
                                                       typeof(ProjectAggregates::Order),
                                                       typeof(FirmAggregates::Order))
            .AddMapping<Facts::Position, Facts::Firm>(typeof(FirmAggregates::Firm))
            .AddMapping<Facts::Position, Facts::Price>(typeof(PriceAggregates::Price))
            .AddMapping<Facts::PricePosition, Facts::Order>(typeof(AdvertisementAggregates::Order),
                                                            typeof(PriceAggregates::Order),
                                                            typeof(ProjectAggregates::Order))
            .AddMapping<Facts::PricePosition, Facts::Price>(typeof(PriceAggregates::Price))
            .AddMapping<Facts::Project, Facts::Order>(typeof(AccountAggregates::Order),
                                                      typeof(AdvertisementAggregates::Order),
                                                      typeof(ConsistencyAggregates::Order),
                                                      typeof(FirmAggregates::Order),
                                                      typeof(ProjectAggregates::Order),
                                                      typeof(ThemeAggregates::Order))
            .AddMapping<Facts::Project, Facts::Firm>(typeof(FirmAggregates::Firm))
            .AddMapping<Facts::ReleaseInfo, Facts::Project>(typeof(ProjectAggregates::Project))
            .AddMapping<Facts::ReleaseWithdrawal, Facts::Account>(typeof(AccountAggregates::Account))
            .AddMapping<Facts::ReleaseWithdrawal, Facts::Order>(typeof(ConsistencyAggregates::Order))
            .AddMapping<Facts::RulesetRule, Facts::Order>(typeof(PriceAggregates::Order))
            .AddMapping<Facts::Theme, Facts::Project>(typeof(ThemeAggregates::Project))
            .AddMapping<Facts::ThemeCategory, Facts::Theme>(typeof(ThemeAggregates::Theme))
            .AddMapping<Facts::ThemeOrganizationUnit, Facts::Project>(typeof(ThemeAggregates.Project))
            ;

        private static Dictionary<long, IReadOnlyCollection<Type>> AddMapping<TEntityType>(this Dictionary<long, IReadOnlyCollection<Type>> dictionary, params Type[] types)
            where TEntityType : IdentityBase<TEntityType>, IEntityType, new()
        {
            dictionary.Add(IdentityBase<TEntityType>.Instance.Id, types);
            return dictionary;
        }

        private static Dictionary<Type, IReadOnlyCollection<Type>> AddMapping<TFact>(this Dictionary<Type, IReadOnlyCollection<Type>> dictionary, params Type[] types)
        {
            dictionary.Add(typeof(TFact), types);
            return dictionary;
        }

        private static Dictionary<Tuple<Type, Type>, IReadOnlyCollection<Type>> AddMapping<TFact, TRelatedFact>(this Dictionary<Tuple<Type, Type>, IReadOnlyCollection<Type>> dictionary, params Type[] types)
        {
            dictionary.Add(Tuple.Create(typeof(TFact), typeof(TRelatedFact)), types);
            return dictionary;
        }

        public static bool TryGetFactTypes(long entityTypeId, out IReadOnlyCollection<Type> factTypes)
        {
            return FactsTypeMap.TryGetValue(entityTypeId, out factTypes);
        }

        public static bool TryGetAggregateTypes(Type factType, out IReadOnlyCollection<Type> aggregateTypes)
        {
            return AggregatesTypeMap.TryGetValue(factType, out aggregateTypes);
        }

        public static bool TryGetRelatedAggregateTypes(Type factType, Type relatedFactType, out IReadOnlyCollection<Type> aggregateTypes)
        {
            var key = Tuple.Create(factType, relatedFactType);
            return RelatedAggregatesTypeMap.TryGetValue(key, out aggregateTypes);
        }
    }
}