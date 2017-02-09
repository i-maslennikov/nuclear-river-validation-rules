namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    public interface IBindingObject
    {
        bool HasNoBinding { get; }
        long? Category1Id { get; }
        long? Category3Id { get; }
        long? FirmAddressId { get; }
    }
}