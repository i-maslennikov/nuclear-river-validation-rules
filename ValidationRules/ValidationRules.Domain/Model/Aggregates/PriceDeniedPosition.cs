using NuClear.River.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// Описание пары запрещённых друг к другу позиций.
    /// </summary>
    public sealed class PriceDeniedPosition : IAggregateValueObject
    {
        public long PriceId { get; set; }

        public long DeniedPositionId { get; set; }
        public long PrincipalPositionId { get; set; }

        public int ObjectBindingType { get; set; }
    }
}