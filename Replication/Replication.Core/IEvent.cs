using System;

namespace NuClear.Replication.Core
{
    public interface IEvent
    {
        DateTime Time { get; }
    }
}