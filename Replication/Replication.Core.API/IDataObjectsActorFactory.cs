using System.Collections.Generic;

namespace NuClear.Replication.Core.API
{
    public interface IDataObjectsActorFactory
    {
        IReadOnlyCollection<IActor> Create();
    }
}