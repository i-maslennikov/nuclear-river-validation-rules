using System;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IAggregateCommandActorFactory
    {
        IActor Create(Type commandType, Type aggregateRootType);
    }
}