using System;

namespace NuClear.Replication.Core.API
{
    public interface IDataObjectsActorFactory
    {
        IActor Create(Type dataObjectType);
    }
}