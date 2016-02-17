using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// Связь заказа с номенклатурной позицией, импортируется из ERM.OrderPosition + ERM.OrderPositionAdv
    /// </summary>
    public sealed class OrderPosition : IAggregateValueObject
    {
        public long OrderId { get; set; }
        public long ItemPositionId { get; set; }
        public int CompareMode { get; set; }
        public long? Category3Id { get; set; }
        public long? FirmAddressId { get; set; }

        public long PackagePositionId { get; set; }
        public long? Category1Id { get; set; }
    }
}