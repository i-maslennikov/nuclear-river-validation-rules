namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// Описание пары запрещённых друг к другу позиций.
    /// </summary>
    public sealed class DeniedPosition
    {
        public long PositionId { get; set; }
        public long DeniedPositionId { get; set; }

        /// <summary>
        /// Опциональное поле, если null, то правило действует для всех прайс-листов.
        /// </summary>
        public long? PriceId { get; set; }

        public int ObjectBindingType { get; set; }
    }
}