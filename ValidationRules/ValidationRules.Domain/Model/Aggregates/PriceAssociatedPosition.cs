using NuClear.River.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// Описание позиции, основной к данной.
    /// </summary>
    public sealed class PriceAssociatedPosition : IAggregateValueObject
    {
        public long PriceId { get; set; }

        public long AssociatedPositionId { get; set; }
        public long PrincipalPositionId { get; set; }

        public int ObjectBindingType { get; set; }

        /// <summary>
        /// Признак группы. В каждой группе должна быть в наличии хотя бы одна PrincipalPositionId.
        /// </summary>
        public long GroupId { get; set; }
    }
}