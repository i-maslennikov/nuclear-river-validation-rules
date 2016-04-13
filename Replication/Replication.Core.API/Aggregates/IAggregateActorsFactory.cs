using System.Collections.Generic;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IAggregateActorsFactory
    {
        ICreateDataObjectsActor CreateRootInitializationActor();
        ISyncDataObjectsActor CreateRootSyncActor();
        IDeleteDataObjectsActor CreateRootDestructionActor();
        IReadOnlyCollection<IReplaceDataObjectsActor> CreateValueObjectsActors();
    }
}