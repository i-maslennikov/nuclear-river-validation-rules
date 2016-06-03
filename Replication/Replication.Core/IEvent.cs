using System;

namespace NuClear.Replication.Core
{
    public interface IEvent
    {
        DateTime HappenedTime { get; }
    }
}