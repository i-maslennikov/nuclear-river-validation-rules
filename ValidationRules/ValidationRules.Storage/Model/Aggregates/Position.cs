namespace NuClear.ValidationRules.Storage.Model.Aggregates
{
    /// <summary>
    /// Импортированная из ERM сущность номенклатурной позиции
    /// </summary>
    public sealed class Position
    {
        public long Id { get; set; }
        public long CategoryCode { get; set; }
        public bool IsControlledByAmount { get; set; }
        public string Name { get; set; }
    }
}