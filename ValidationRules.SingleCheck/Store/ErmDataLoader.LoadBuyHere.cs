using LinqToDB.Data;
using NuClear.ValidationRules.Storage.Model.Erm;
using System.Linq;

namespace NuClear.ValidationRules.SingleCheck.Store
{
    public static partial class ErmDataLoader
    {
        private static void LoadBuyHere(DataConnection query, Order order, IStore store)
        {
            const long CategoryCodePremiumAdvertising = 809065011136692321; // Реклама в профилях партнеров (приоритетное размещение)
            const long CategoryCodeAdvertisingAddress = 809065011136692326; // Реклама в профилях партнеров (адреса)
         const long CategoryCodeBasicPackage = 303; // пакет "Базовый"
         const long CategoryCodeMediaContextBanner = 395122163464046280; // МКБ
         const long CategoryCodeContextBanner = 809065011136692318; // КБ
            var categoryCodes = new[] { CategoryCodePremiumAdvertising, CategoryCodeAdvertisingAddress, CategoryCodeBasicPackage, CategoryCodeMediaContextBanner, CategoryCodeContextBanner };

            var positions =
                query.GetTable<Position>()
                     .Where(x => categoryCodes.Contains(x.CategoryCode))
                     .Execute();
            store.AddRange(positions);

            var firmAddresses = (
                from op in query.GetTable<OrderPosition>().Where(x => x.IsActive && !x.IsDeleted).Where(x => x.OrderId == order.Id)
                from opa in query.GetTable<OrderPositionAdvertisement>().Where(x => x.OrderPositionId == op.Id)
                from position in query.GetTable<Position>().Where(x => x.CategoryCode == CategoryCodeAdvertisingAddress).Where(x => x.Id == opa.PositionId)
                from address in query.GetTable<FirmAddress>().Where(x => x.Id == opa.FirmAddressId)
                select address
            ).Execute();
            store.AddRange(firmAddresses);
            var firmAddressIds = firmAddresses.Select(x => x.Id).ToList();

            if (!firmAddresses.Any())
            {
                return;
            }

            var firmIds = firmAddresses.Select(x => x.FirmId).Distinct().ToList();

            // Нужны заказы этих фирм - вдруг фирма является рекламодателем.
            var firmOrders = query.GetTable<Order>()
                .Where(x => firmIds.Contains(x.FirmId))
                .Where(x => new[] { 2, 4, 5 }.Contains(x.WorkflowStepId))
                .Where(x => x.BeginDistributionDate < order.EndDistributionDatePlan && order.BeginDistributionDate < x.EndDistributionDatePlan)
                .Execute();
            store.AddRange(firmOrders);

            var positionIds = positions.Select(x => x.Id).ToList();

            // Нужны другие ЗМК заказы на те же самые адреса
            var xxxOrders =
                from opa in query.GetTable<OrderPositionAdvertisement>()
                    .Where(x => positionIds.Contains(x.PositionId))
                    .Where(x => !x.FirmAddressId.HasValue || firmAddressIds.Contains(x.FirmAddressId.Value))
                from op in query.GetTable<OrderPosition>()
                    .Where(x => x.IsActive && !x.IsDeleted)
                    .Where(x => x.Id == opa.OrderPositionId)
                from o in query.GetTable<Order>()
                    .Where(x => new[] { 2, 4, 5 }.Contains(x.WorkflowStepId)) // заказы "на оформлении" не нужны - проверяемый их в любом лучае не видит
                    .Where(x => x.IsActive && !x.IsDeleted)
                    .Where(x => x.BeginDistributionDate < order.EndDistributionDatePlan && order.BeginDistributionDate < x.EndDistributionDatePlan && x.DestOrganizationUnitId == order.DestOrganizationUnitId)
                    .Where(x => x.Id == op.OrderId)
                select new { Order = o, OrderPosition = op, OrderPositionAdvertisement = opa };

            store.AddRange(xxxOrders.Select(x => x.Order).Distinct().Execute());
            store.AddRange(xxxOrders.Select(x => x.OrderPosition).Distinct().Execute());
            store.AddRange(xxxOrders.Select(x => x.OrderPositionAdvertisement).Distinct().Execute());
        }
    }
}
