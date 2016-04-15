using System.Collections.Generic;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IAggregateRootActor : IEntityActor
    {
        IReadOnlyCollection<IEntityActor> GetEntityActors();
    }
}