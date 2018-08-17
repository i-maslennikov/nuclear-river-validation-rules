using System;
using System.Collections.Generic;

using NuClear.Model.Common;
using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.OperationsProcessing
{
    internal static partial class EntityTypeMap
    {
        private static readonly Dictionary<int, IReadOnlyCollection<Type>> ErmFactsTypeMap = new Dictionary<int, IReadOnlyCollection<Type>>()
            .AddMapping<EntityTypeAccount>(typeof(Facts::Account))
            .AddMapping<EntityTypeAccountDetail>(typeof(Facts::AccountDetail))
            .AddMapping<EntityTypeBargain>(typeof(Facts::Bargain))
            .AddMapping<EntityTypeBargainFile>(typeof(Facts::BargainScanFile))
            .AddMapping<EntityTypeBill>(typeof(Facts::Bill))
            .AddMapping<EntityTypeBranchOffice>(typeof(Facts::BranchOffice))
            .AddMapping<EntityTypeBranchOfficeOrganizationUnit>(typeof(Facts::BranchOfficeOrganizationUnit))
            .AddMapping<EntityTypeCategory>(typeof(Facts::Category))
            .AddMapping<EntityTypeCategoryOrganizationUnit>(typeof(Facts::CategoryOrganizationUnit))
            .AddMapping<EntityTypeDeal>(typeof(Facts::Deal))
            .AddMapping<EntityTypeFirm>(typeof(Facts::Firm))
            .AddMapping<EntityTypeFirmAddress>(typeof(Facts::FirmAddress))
            .AddMapping<EntityTypeCategoryFirmAddress>(typeof(Facts::FirmAddressCategory))
            .AddMapping<EntityTypeLegalPerson>(typeof(Facts::LegalPerson))
            .AddMapping<EntityTypeLegalPersonProfile>(typeof(Facts::LegalPersonProfile))
            .AddMapping<EntityTypeOrder>(typeof(Facts::Order),
                                         typeof(Facts::UnlimitedOrder))
            .AddMapping<EntityTypeOrderPosition>(typeof(Facts::OrderPosition),
                                                 typeof(Facts::OrderPositionCostPerClick),
                                                 typeof(Facts::ReleaseWithdrawal),
                                                 typeof(Facts::OrderItem))
            .AddMapping<EntityTypeOrderPositionAdvertisement>(typeof(Facts::OrderPositionAdvertisement))
            .AddMapping<EntityTypeOrderFile>(typeof(Facts::OrderScanFile))
            .AddMapping<EntityTypePosition>(typeof(Facts::Position),
                                            typeof(Facts::PositionChild))
            .AddMapping<EntityTypePrice>(typeof(Facts::Price),
                                         typeof(Facts::NomenclatureCategory))
            .AddMapping<EntityTypePricePosition>(typeof(Facts::PricePosition))
            .AddMapping<EntityTypeProject>(typeof(Facts::Project),
                                           typeof(Facts::CostPerClickCategoryRestriction),
                                           typeof(Facts::SalesModelCategoryRestriction))
            .AddMapping<EntityTypeReleaseInfo>(typeof(Facts::ReleaseInfo))
            .AddMapping<EntityTypeTheme>(typeof(Facts::Theme))
            .AddMapping<EntityTypeThemeCategory>(typeof(Facts::ThemeCategory))
            .AddMapping<EntityTypeThemeOrganizationUnit>(typeof(Facts::ThemeOrganizationUnit));

        public static bool TryGetErmFactTypes(int entityTypeId, out IReadOnlyCollection<Type> factTypes)
        {
            return ErmFactsTypeMap.TryGetValue(entityTypeId, out factTypes);
        }

        private static Dictionary<int, IReadOnlyCollection<Type>> AddMapping<TEntityType>(this Dictionary<int, IReadOnlyCollection<Type>> dictionary, params Type[] types)
            where TEntityType : IdentityBase<TEntityType>, IEntityType, new()
        {
            dictionary.Add(IdentityBase<TEntityType>.Instance.Id, types);
            return dictionary;
        }
    }
}