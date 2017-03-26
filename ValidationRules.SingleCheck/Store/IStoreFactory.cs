using System;

using NuClear.Storage.API.Readings;

namespace NuClear.ValidationRules.SingleCheck.Store
{
    public interface IStoreFactory : IDisposable
    {
        IStore CreateStore();
        IQuery CreateQuery();
    }
}
