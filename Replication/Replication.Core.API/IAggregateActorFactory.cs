using System;

using NuClear.Replication.Core.API.Actors;

namespace NuClear.Replication.Core.API
{
    public interface IAggregateActorFactory
    {
        IActor Create(Type aggregateRootType);
    }
}