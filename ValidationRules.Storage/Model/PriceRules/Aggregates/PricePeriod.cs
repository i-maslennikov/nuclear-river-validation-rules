using System;

namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    /// <summary>
    /// Период действия прайс-листа.
    /// Сумма всех периоднов одного прайс-листа должна быть неразрывной и совпадать периодом действия прайс-листа
    /// </summary>
    public sealed class PricePeriod
    {
        public long PriceId { get; set; }
        public long OrganizationUnitId { get; set; }
        public DateTime Start { get; set; }
    }
}