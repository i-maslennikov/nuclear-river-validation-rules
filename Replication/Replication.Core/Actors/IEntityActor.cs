using System.Collections.Generic;

namespace NuClear.Replication.Core.Actors
{
    public interface IEntityActor : IActor
    {
        IReadOnlyCollection<IActor> GetValueObjectActors();
    }
}