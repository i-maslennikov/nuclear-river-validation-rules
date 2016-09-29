using System;
using System.Collections.Generic;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Commands;
using NuClear.Replication.Core.DataObjects;

using AccountFacts = NuClear.ValidationRules.Storage.Model.AccountRules.Facts;
using PriceFacts = NuClear.ValidationRules.Storage.Model.PriceRules.Facts;
using ConsistencyFacts = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts;
using FirmFacts = NuClear.ValidationRules.Storage.Model.FirmRules.Facts;

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
                    };
            }

            throw new ArgumentException($"Unkown command type {typeof(TCommand).FullName}");
        }
    }
}