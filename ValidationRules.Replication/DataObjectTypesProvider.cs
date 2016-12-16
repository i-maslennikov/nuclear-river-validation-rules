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
                return new List<Type>
                    {
                            typeof(Account),
                            typeof(Advertisement),
                            typeof(AdvertisementElement),
                            typeof(AdvertisementElementTemplate),
                            typeof(AdvertisementTemplate),
                            typeof(AssociatedPosition),
                            typeof(AssociatedPositionsGroup),
                            typeof(Bargain),
                            typeof(BargainScanFile),
                            typeof(Bill),
                            typeof(BranchOffice),
                            typeof(BranchOfficeOrganizationUnit),
                            typeof(Category),
                            typeof(CategoryOrganizationUnit),
                            typeof(CostPerClickCategoryRestriction),
                            typeof(Deal),
                            typeof(DeniedPosition),
                            typeof(Firm),
                            typeof(FirmAddress),
                            typeof(FirmAddressCategory),
                            typeof(FirmAddressWebsite),
                            typeof(LegalPerson),
                            typeof(LegalPersonProfile),
                            typeof(Lock),
                            typeof(NomenclatureCategory),
                            typeof(Order),
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
                            typeof(RulesetRule),
                            typeof(SalesModelCategoryRestriction),
                            typeof(Theme),
                            typeof(ThemeCategory),
                            typeof(ThemeOrganizationUnit),
                            typeof(UnlimitedOrder),
                    };
            }

            throw new ArgumentException($"Unkown command type {typeof(TCommand).FullName}");
        }
    }
}