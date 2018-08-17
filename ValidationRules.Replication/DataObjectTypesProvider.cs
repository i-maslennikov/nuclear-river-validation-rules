using System;
using System.Collections.Generic;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Commands;
using NuClear.Replication.Core.DataObjects;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication
{
    public class DataObjectTypesProvider : IDataObjectTypesProvider
    {
        public IReadOnlyCollection<Type> Get<TCommand>() where TCommand : ICommand
        {
            if (typeof(ISyncDataObjectCommand).IsAssignableFrom(typeof(TCommand)))
            {
                return new []
                    {
                            typeof(Account),
                            typeof(AccountDetail),
                            typeof(Bargain),
                            typeof(BargainScanFile),
                            typeof(Bill),
                            typeof(BranchOffice),
                            typeof(BranchOfficeOrganizationUnit),
                            typeof(Category),
                            typeof(CategoryOrganizationUnit),
                            typeof(CostPerClickCategoryRestriction),
                            typeof(Deal),
                            typeof(Firm),
                            typeof(FirmAddress),
                            typeof(FirmAddressCategory),
                            typeof(LegalPerson),
                            typeof(LegalPersonProfile),
                            typeof(NomenclatureCategory),
                            typeof(Order),
                            typeof(OrderItem),
                            typeof(OrderPosition),
                            typeof(OrderPositionAdvertisement),
                            typeof(OrderPositionCostPerClick),
                            typeof(OrderScanFile),
                            typeof(Position),
                            typeof(PositionChild),
                            typeof(Price),
                            typeof(PricePosition),
                            typeof(Project),
                            typeof(ReleaseInfo),
                            typeof(ReleaseWithdrawal),
                            typeof(SalesModelCategoryRestriction),
                            typeof(Theme),
                            typeof(ThemeCategory),
                            typeof(ThemeOrganizationUnit),
                            typeof(UnlimitedOrder),
                    };
            }

            if (typeof(IReplaceDataObjectCommand).IsAssignableFrom(typeof(TCommand)))
            {
                return new[]
                    {
                        typeof(Advertisement),
                        typeof(Ruleset),
                        typeof(Ruleset.AssociatedRule),
                        typeof(Ruleset.DeniedRule),
                        typeof(Ruleset.QuantitativeRule),
                        typeof(Ruleset.RulesetProject)
                    };
            }

            throw new ArgumentException($"Unkown command type {typeof(TCommand).FullName}");
        }
    }
}