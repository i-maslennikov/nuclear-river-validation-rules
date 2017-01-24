using NuClear.Storage.API.Readings;

namespace NuClear.ValidationRules.SingleCheck.Store
{
    public interface IStoreFactory
    {
        IStore CreateStore();
        IQuery CreateQuery();
    }
}
