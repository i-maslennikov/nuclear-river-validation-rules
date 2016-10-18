using System;
using System.Collections.Generic;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Commands;
using NuClear.Replication.Core.DataObjects;

using AccountFacts = NuClear.ValidationRules.Storage.Model.AccountRules.Facts;
using PriceFacts = NuClear.ValidationRules.Storage.Model.PriceRules.Facts;
using ConsistencyFacts = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts;
using FirmFacts = NuClear.ValidationRules.Storage.Model.FirmRules.Facts;
using AdvertisementFacts = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;
using ProjectFacts = NuClear.ValidationRules.Storage.Model.ProjectRules.Facts;
using ThemeFacts = NuClear.ValidationRules.Storage.Model.ThemeRules.Facts;

namespace NuClear.ValidationRules.Replication
{
    public class DataObjectTypesProvider : IDataObjectTypesProvider
    {
        public IReadOnlyCollection<Type> Get<TCommand>() where TCommand : ICommand
        {
            if (typeof(ISyncDataObjectCommand).IsAssignableFrom(typeof(TCommand)))
            {
                return new List<Type>
                    {
                            typeof(PriceFacts::AssociatedPosition),
                            typeof(PriceFacts::AssociatedPositionsGroup),
                            typeof(PriceFacts::Category),
                            typeof(PriceFacts::DeniedPosition),
                            typeof(PriceFacts::Order),
                            typeof(PriceFacts::OrderPosition),
                            typeof(PriceFacts::OrderPositionAdvertisement),
                            typeof(PriceFacts::Position),
                            typeof(PriceFacts::Price),
                            typeof(PriceFacts::PricePosition),
                            typeof(PriceFacts::PricePositionNotActive),
                            typeof(PriceFacts::Project),
                            typeof(PriceFacts::RulesetRule),
                            typeof(PriceFacts::Theme),

                            typeof(AccountFacts::Account),
                            typeof(AccountFacts::Order),
                            typeof(AccountFacts::Project),
                            typeof(AccountFacts::Lock),
                            typeof(AccountFacts::Limit),
                            typeof(AccountFacts::OrderPosition),
                            typeof(AccountFacts::ReleaseWithdrawal),

                            typeof(ConsistencyFacts::Bargain),
                            typeof(ConsistencyFacts::BargainScanFile),
                            typeof(ConsistencyFacts::Bill),
                            typeof(ConsistencyFacts::Category),
                            typeof(ConsistencyFacts::CategoryFirmAddress),
                            typeof(ConsistencyFacts::Firm),
                            typeof(ConsistencyFacts::FirmAddress),
                            typeof(ConsistencyFacts::LegalPersonProfile),
                            typeof(ConsistencyFacts::Order),
                            typeof(ConsistencyFacts::OrderPosition),
                            typeof(ConsistencyFacts::OrderPositionAdvertisement),
                            typeof(ConsistencyFacts::OrderScanFile),
                            typeof(ConsistencyFacts::Position),
                            typeof(ConsistencyFacts::Project),
                            typeof(ConsistencyFacts::ReleaseWithdrawal),

                            typeof(FirmFacts::Firm),
                            typeof(FirmFacts::FirmAddress),
                            typeof(FirmFacts::FirmAddressCategory),
                            typeof(FirmFacts::Order),
                            typeof(FirmFacts::OrderPosition),
                            typeof(FirmFacts::OrderPositionAdvertisement),
                            typeof(FirmFacts::SpecialPosition),
                            typeof(FirmFacts::Project),

                            typeof(AdvertisementFacts::Order),
                            typeof(AdvertisementFacts::Project),
                            typeof(AdvertisementFacts::OrderPosition),
                            typeof(AdvertisementFacts::OrderPositionAdvertisement),
                            typeof(AdvertisementFacts::PricePosition),
                            typeof(AdvertisementFacts::Position),
                            typeof(AdvertisementFacts::AdvertisementTemplate),
                            typeof(AdvertisementFacts::Advertisement),
                            typeof(AdvertisementFacts::Firm),
                            typeof(AdvertisementFacts::AdvertisementElement),
                            typeof(AdvertisementFacts::AdvertisementElementTemplate),

                            typeof(ProjectFacts::Category),
                            typeof(ProjectFacts::CategoryOrganizationUnit),
                            typeof(ProjectFacts::CostPerClickCategoryRestriction),
                            typeof(ProjectFacts::FirmAddress),
                            typeof(ProjectFacts::Order),
                            typeof(ProjectFacts::OrderPosition),
                            typeof(ProjectFacts::OrderPositionAdvertisement),
                            typeof(ProjectFacts::OrderPositionCostPerClick),
                            typeof(ProjectFacts::Position),
                            typeof(ProjectFacts::PricePosition),
                            typeof(ProjectFacts::Project),
                            typeof(ProjectFacts::ReleaseInfo),
                            typeof(ProjectFacts::SalesModelCategoryRestriction),

                            typeof(ThemeFacts::Theme),
                            typeof(ThemeFacts::ThemeCategory),
                            typeof(ThemeFacts::ThemeOrganizationUnit),
                            typeof(ThemeFacts::Category),
                            typeof(ThemeFacts::Order),
                            typeof(ThemeFacts::OrderPosition),
                            typeof(ThemeFacts::OrderPositionAdvertisement),
                            typeof(ThemeFacts::Project),
                    };
            }

            throw new ArgumentException($"Unkown command type {typeof(TCommand).FullName}");
        }
    }
}