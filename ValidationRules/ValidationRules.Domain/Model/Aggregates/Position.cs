using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// Импортированная из ERM сущность номенклатурной позиции
    /// </summary>
    public sealed class Position : IAggregateRoot
    {
        public long Id { get; set; }
        public long PositionCategoryId { get; set; }
    }
}