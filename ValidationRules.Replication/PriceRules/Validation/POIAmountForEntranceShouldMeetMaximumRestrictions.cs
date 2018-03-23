using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// При превышении допустимого количества POI на один вход должна выводиться ошибка:
    /// "Превышено допустимое количество POI на вход: {0}. Месяц: {1}. Адрес: {2}. Вход: {3}. Конфликтующие заказы: {4}"
    /// </summary>
    public class PoiAmountForEntranceShouldMeetMaximumRestrictions : ValidationResultAccessorBase
    {
        private const int MaxSalesOnEntrance = 1;

        public PoiAmountForEntranceShouldMeetMaximumRestrictions(IQuery query) : base(query, MessageTypeCode.PoiAmountForEntranceShouldMeetMaximumRestrictions)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var salePeriods = from period in query.For<Period>()
                              from orderPeriod in query.For<Order.OrderPeriod>().Where(x => x.Begin <= period.Start && period.End <= x.End)
                              from position in query.For<Order.EntranceControlledPosition>().Where(x => orderPeriod.OrderId == x.OrderId)
                              select new { period.Start, period.End, orderPeriod.Scope, position.OrderId, position.EntranceCode, position.FirmAddressId };

            var violations = from salePeriod in salePeriods
                             from conflictingSalePeriod in salePeriods.Where(x => x.EntranceCode == salePeriod.EntranceCode &&
                                                                                  x.Start <= salePeriod.Start && salePeriod.End <= x.End &&
                                                                                  x.OrderId != salePeriod.OrderId &&
                                                                                  Scope.CanSee(salePeriod.Scope, x.Scope))
                             select new
                                 {
                                     OrdersCountOnTheSameEntrance = salePeriods.Count(x => x.EntranceCode == salePeriod.EntranceCode &&
                                                                                           x.Start <= salePeriod.Start && salePeriod.End <= x.End &&
                                                                                           Scope.CanSee(salePeriod.Scope, x.Scope)),
                                     salePeriod.OrderId,
                                     ConflictingOrderId = conflictingSalePeriod.OrderId,
                                     salePeriod.EntranceCode,
                                     salePeriod.FirmAddressId,
                                     salePeriod.Start,
                                     salePeriod.End,
                                 };

            return from violation in violations
                   where violation.OrdersCountOnTheSameEntrance > MaxSalesOnEntrance
                   select new Version.ValidationResult
                       {
                           MessageParams =
                               new MessageParams(new Dictionary<string, object>
                                                     {
                                                         { "begin", violation.Start },
                                                         { "end", violation.End },
                                                         { "maxCount", MaxSalesOnEntrance },
                                                         { "entranceCode", violation.EntranceCode }
                                                     },
                                                 new Reference<EntityTypeOrder>(violation.ConflictingOrderId),
                                                 new Reference<EntityTypeFirmAddress>(violation.FirmAddressId))
                                   .ToXDocument(),

                           PeriodStart = violation.Start,
                           PeriodEnd = violation.End,
                           OrderId = violation.OrderId
                       };
        }
    }
}