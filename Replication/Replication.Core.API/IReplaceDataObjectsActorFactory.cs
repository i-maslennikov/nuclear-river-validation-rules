using System;

namespace NuClear.Replication.Core.API
{
    public interface IReplaceDataObjectsActorFactory
    {
        IReplaceDataObjectsActor Create(Type commandType);
        IReplaceDataObjectsActor Create<TDataObject>();
    }
}