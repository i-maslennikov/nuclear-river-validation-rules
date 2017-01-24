namespace NuClear.ValidationRules.Storage.Model.Facts
{
    /// <summary>
    /// PriceId появился в этой сущности из-за стремления поддержать соместимость с erm.
    /// Реально нужно читать поток flowNomenclatures.NomenclatureCategory.
    /// </summary>
    public sealed class NomenclatureCategory
    {
        public long Id { get; set; }
        public long PriceId { get; set; }
        public string Name { get; set; }
    }
}