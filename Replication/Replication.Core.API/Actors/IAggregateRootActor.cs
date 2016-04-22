using System.Collections.Generic;

namespace NuClear.Replication.Core.API.Actors
{
    public interface IAggregateRootActor : IEntityActor
    {
        IReadOnlyCollection<IEntityActor> GetEntityActors();
    }
}