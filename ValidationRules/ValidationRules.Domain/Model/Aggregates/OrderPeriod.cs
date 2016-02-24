using NuClear.River.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// Период заказа.
    /// Сумма всех периодов заказа должна быть неразрывна и совпадать с периодом размещения заказа.
    /// </summary>
    public sealed class OrderPeriod : IAggregateValueObject
    {
        public long OrderId { get; set; }
        public long PeriodId { get; set; }
    }
}