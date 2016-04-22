using System.Collections.Generic;

using NuClear.Replication.Core.API.Actors;

namespace NuClear.Replication.Core.API
{
    public interface IDataObjectsActorFactory
    {
        IReadOnlyCollection<IActor> Create();
    }
}