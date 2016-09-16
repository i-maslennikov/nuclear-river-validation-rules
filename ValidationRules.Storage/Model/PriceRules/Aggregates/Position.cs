using System;

namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    /// <summary>
    /// Импортированная из ERM сущность номенклатурной позиции
    /// </summary>
    public sealed class Position
    {
        public long Id { get; set; }
        public long CategoryCode { get; set; }
        [Obsolete("Теперь, когда ограничения создаются только для нужных позиций, нет смысла тянуть это поле в агрегаты")]
        public bool IsControlledByAmount { get; set; }
        public string Name { get; set; }
    }
}