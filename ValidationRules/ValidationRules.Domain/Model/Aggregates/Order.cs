using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    /// <summary>
    /// ��������������� �� ERM �������� ������
    /// </summary>
    public class Order : IAggregateRoot
    {
        public long Id { get; set; }
        public long FirmId { get; set; }
    }
}