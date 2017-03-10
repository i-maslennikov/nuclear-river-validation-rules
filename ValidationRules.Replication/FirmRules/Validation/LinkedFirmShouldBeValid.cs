using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.FirmRules.Validation
{
    /// <summary>
    /// Для заказов, к которым привязана неактуальная фирма, должна выводиться ошибка.
    /// "Фирма {0} удалена"
    /// "Фирма {0} скрыта навсегда"
    /// "Фирма {0} скрыта до выяснения"
    /// 
    /// Source: FirmsOrderValidationRule
    /// </summary>
    public sealed class LinkedFirmShouldBeValid : ValidationResultAccessorBase
    {
        public LinkedFirmShouldBeValid(IQuery query) : base(query, MessageTypeCode.LinkedFirmShouldBeValid)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from firm in query.For<Order.InvalidFirm>().Where(x => x.OrderId == order.Id)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object> { { "invalidFirmState", (int)firm.State } },
                                    new Reference<EntityTypeFirm>(firm.FirmId),
                                    new Reference<EntityTypeOrder>(order.Id))
                                .ToXDocument(),

                        PeriodStart = order.Begin,
                        PeriodEnd = order.End,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}
