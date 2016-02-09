namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// Описание позиции, основной к данной.
    /// </summary>
    public class MasterPosition
    {
        public long PositionId { get; set; }
        public long MasterPositionId { get; set; }

        /// <summary>
        /// Опциональное поле, если null, то правило действует для всех прайс-листов.
        /// </summary>
        public long? PriceId { get; set; }

        /// <summary>
        /// Признак группы. В каждой группе должна быть в наличии хотя бы одна MasterPositionId.
        /// </summary>
        public long? GroupId { get; set; }

        public int ObjectBindingType { get; set; }
    }
}