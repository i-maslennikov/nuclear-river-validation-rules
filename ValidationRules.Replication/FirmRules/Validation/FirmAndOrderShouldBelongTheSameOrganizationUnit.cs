using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.FirmRules.Validation
{
    /// <summary>
    /// Для заказов, фирма которых находится не в городе назначения заказа, должна выводиться ошибка.
    /// "Отделение организации назначения заказа не соответствует отделению организации выбранной фирмы"
    /// 
    /// Source: FirmBelongsToOrdersDestOrgUnitOrderValidationRule
    /// </summary>
    public sealed class FirmAndOrderShouldBelongTheSameOrganizationUnit : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public FirmAndOrderShouldBelongTheSameOrganizationUnit(IQuery query) : base(query, MessageTypeCode.FirmAndOrderShouldBelongTheSameOrganizationUnit)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var messages =
                from fail in query.For<Order.FirmOrganiationUnitMismatch>()
                from order in query.For<Order>().Where(x => x.Id == fail.OrderId)
                from firm in query.For<Firm>().Where(x => x.Id == order.FirmId)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeFirm>(firm.Id),
                                    new Reference<EntityTypeOrder>(order.Id))
                                .ToXDocument(),

                        PeriodStart = order.Begin,
                        PeriodEnd = order.End,
                        OrderId = order.Id,

                        Result = RuleResult,
                    };


            return messages;
        }
    }
}
