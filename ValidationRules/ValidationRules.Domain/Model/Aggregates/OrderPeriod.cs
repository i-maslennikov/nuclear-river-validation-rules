namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// Период заказа.
    /// Сумма всех периодов заказа должна быть неразрывна и совпадать с периодом размещения заказа.
    /// </summary>
    public class OrderPeriod
    {
        public long OrderId { get; set; }
        public long PeriodId { get; set; }
    }
}