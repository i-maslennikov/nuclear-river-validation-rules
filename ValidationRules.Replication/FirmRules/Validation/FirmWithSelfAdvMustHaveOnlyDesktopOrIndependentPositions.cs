using System;
using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.Specifications;

using Order = NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates.Order;
using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.FirmRules.Validation
{
    /// <summary>
    /// Для фирм, с позициями "Самореклама только для ПК" и хотя бы одной позицией с платформой размещения, отличной от "Desktop" или "Independent", должна выводиться ошибка
    /// "Позиция "Самореклама только для ПК" продана одновременно с рекламой в другую платформу"
    /// 
    /// Source: SelfAdvertisementOrderValidationRule
    /// 
    /// Q: Получается, если размещён заказ на саморекламу, а потом создаётся на мобилку, то ошибка возникнет только в одобренном на саморекламу?
    /// A: Нет, в режиме одиночной проверки ERM покажет её в обоих заказах, а вот в массовой - только одну, для заказа с саморекламой.
    ///    А вот мы так не умеем.
    /// </summary>
    public sealed class FirmWithSelfAdvMustHaveOnlyDesktopOrIndependentPositions : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public FirmWithSelfAdvMustHaveOnlyDesktopOrIndependentPositions(IQuery query) : base(query, MessageTypeCode.FirmWithSelfAdvMustHaveOnlyDesktopOrIndependentPositions)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var selfAdvOrders =
                from order in query.For<Order>()
                where query.For<Order.SelfAdvertisementPosition>().Any(x => x.OrderId == order.Id)
                select order;

            var nonDesktopOrders =
                from order in query.For<Order>()
                where query.For<Order.NotApplicapleForDesktopPosition>().Any(x => x.OrderId == order.Id)
                select order;

            var errorsInSelfAdvOrders =
                from order in selfAdvOrders
                where nonDesktopOrders.Where(x => Scope.CanSee(order.Scope, x.Scope)).Any(x => x.FirmId == order.FirmId && x.Begin < order.End && order.Begin < x.End)
                select order;

            var errorsInNonDesktopOrders =
                from order in nonDesktopOrders
                where selfAdvOrders.Where(x => Scope.CanSee(order.Scope, x.Scope)).Any(x => x.FirmId == order.FirmId && x.Begin < order.End && order.Begin < x.End)
                select order;

            var result =
                from order in errorsInSelfAdvOrders.Union(errorsInNonDesktopOrders)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new XDocument(new XElement("root",
                                new XElement("order",
                                    new XAttribute("id", order.Id),
                                    new XAttribute("number", order.Number)))),

                        PeriodStart = order.Begin,
                        PeriodEnd = order.End,
                        OrderId = order.Id,

                        Result = RuleResult,
                    };

            return result;
        }
    }
}
