using System;

using NuClear.Replication.Core.Actors;

namespace NuClear.Replication.Core
{
    public interface IAggregateActorFactory
    {
        IActor Create(Type aggregateRootType);
    }
}