namespace NuClear.ValidationRules.Storage.Model.Aggregates
{
    /// <summary>
    /// ����� ������ � ��� �������� � �������� �����-�����
    /// </summary>
    public sealed class OrderPricePosition
    {
        public long OrderId { get; set; }
        public long OrderPositionId { get; set; }
        public string PositionName { get; set; }
        public long? PriceId { get; set; }
    }
}