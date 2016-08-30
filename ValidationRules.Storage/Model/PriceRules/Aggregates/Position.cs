using System;

namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    /// <summary>
    /// »мпортированна€ из ERM сущность номенклатурной позиции
    /// </summary>
    public sealed class Position
    {
        public long Id { get; set; }
        public long CategoryCode { get; set; }
        [Obsolete("“еперь, когда ограничени€ создаютс€ только дл€ нужных позиций, нет смысла т€нуть это поле в агрегаты")]
        public bool IsControlledByAmount { get; set; }
        public string Name { get; set; }
    }
}