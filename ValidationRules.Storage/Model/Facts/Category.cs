namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class Category
    {
        public const long AdvantageousPurchaseWith2Gis = 18599; // Выгодные покупки с 2ГИС.

        public long Id { get; set; }

        public long? L1Id { get; set; }
        public long? L2Id { get; set; }
        public long? L3Id { get; set; }

        public bool IsActiveNotDeleted { get; set; }
    }
}