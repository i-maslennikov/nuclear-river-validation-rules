using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// Импортированная из ERM сущность прайс-листа
    /// </summary>
    public sealed class Price : IAggregateRoot
    {
        public long Id { get; set; }
    }
}