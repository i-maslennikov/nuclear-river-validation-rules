using System.Collections.Generic;

namespace NuClear.Replication.Core.API.Actors
{
    public interface IEntityActor : IActor
    {
        IReadOnlyCollection<IActor> GetValueObjectActors();
    }
}