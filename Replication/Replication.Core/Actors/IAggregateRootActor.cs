using System.Collections.Generic;

namespace NuClear.Replication.Core.Actors
{
    public interface IAggregateRootActor : IEntityActor
    {
        IReadOnlyCollection<IEntityActor> GetEntityActors();
    }
}