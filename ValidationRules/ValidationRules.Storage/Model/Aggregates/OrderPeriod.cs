using System;

namespace NuClear.ValidationRules.Storage.Model.Aggregates
{
    /// <summary>
    /// Период заказа.
    /// Сумма всех периодов заказа должна быть неразрывна и совпадать с периодом размещения заказа.
    /// </summary>
    public sealed class OrderPeriod
    {
        public long OrderId { get; set; }
        public long OrganizationUnitId { get; set; }
        public DateTime Start { get; set; }
    }
}