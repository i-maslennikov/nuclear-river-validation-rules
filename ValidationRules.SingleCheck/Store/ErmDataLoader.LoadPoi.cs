using System.Linq;

using LinqToDB.Data;

using NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.SingleCheck.Store
{
    public static partial class ErmDataLoader
    {
        private static void LoadPoi(DataConnection query, Order order, IStore store)
        {
            var positions = query.GetTable<Position>()
                                 .Where(x => Storage.Model.Facts.Position.CategoryCodesPoiAddressCheck.Contains(x.CategoryCode))
                                 .Execute();

            var positionIds = positions.Select(x => x.Id).ToList();

            var entranceCodes = (from op in query.GetTable<OrderPosition>().Where(x => x.IsActive && !x.IsDeleted).Where(x => x.OrderId == order.Id)
                                 from opa in query.GetTable<OrderPositionAdvertisement>().Where(x => x.OrderPositionId == op.Id && positionIds.Contains(x.PositionId))
                                 from address in query.GetTable<FirmAddress>().Where(x => x.Id == opa.FirmAddressId && x.EntranceCode != null)
                                 select address.EntranceCode).Execute();

            if (!entranceCodes.Any())
            {
                return;
            }

            var correlatingOrders =
                from opa in query.GetTable<OrderPositionAdvertisement>()
                                 .Where(x => positionIds.Contains(x.PositionId))
                from opaAddress in query.GetTable<FirmAddress>()
                                        .Where(x => x.Id == opa.FirmAddressId)
                                        .Where(x => entranceCodes.Contains(x.EntranceCode))
                from op in query.GetTable<OrderPosition>()
                                .Where(x => x.IsActive && !x.IsDeleted)
                                .Where(x => x.Id == opa.OrderPositionId)
                from o in query.GetTable<Order>()
                               .Where(x => new[] { 2, 4, 5 }.Contains(x.WorkflowStepId))
                               .Where(x => x.IsActive && !x.IsDeleted)
                               .Where(x => x.BeginDistributionDate < order.EndDistributionDatePlan &&
                                           order.BeginDistributionDate < x.EndDistributionDatePlan &&
                                           x.DestOrganizationUnitId == order.DestOrganizationUnitId)
                               .Where(x => x.Id == op.OrderId)
                select new { Order = o, OrderPosition = op, OrderPositionAdvertisement = opa, FirmAddress = opaAddress };

            store.AddRange(positions);
            store.AddRange(correlatingOrders.Select(x => x.Order).Distinct().Execute());
            store.AddRange(correlatingOrders.Select(x => x.OrderPosition).Distinct().Execute());
            store.AddRange(correlatingOrders.Select(x => x.OrderPositionAdvertisement).Distinct().Execute());
            store.AddRange(correlatingOrders.Select(x => x.FirmAddress).Distinct().Execute());
        }
    }
}