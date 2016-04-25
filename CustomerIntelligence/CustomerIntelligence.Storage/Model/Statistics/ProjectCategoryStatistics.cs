namespace NuClear.CustomerIntelligence.Storage.Model.Statistics
{
    public sealed class ProjectCategoryStatistics
    {
        public long CategoryId { get; set; } // Не является первичным ключом, но идентификцирует сущность внутри агрегата
        public long ProjectId { get; set; }
    }
}