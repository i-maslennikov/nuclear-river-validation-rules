using System.Collections.Generic;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IEntityActor : IActor
    {
        IReadOnlyCollection<IActor> GetValueObjectActors();
    }
}