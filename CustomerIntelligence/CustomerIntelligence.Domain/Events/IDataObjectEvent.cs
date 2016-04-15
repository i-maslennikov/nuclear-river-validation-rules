using System;

using NuClear.River.Common.Metadata;

namespace NuClear.CustomerIntelligence.Domain.Events
{
    public interface IDataObjectEvent<out TId> : IEvent
    {
        Type DataObjectType { get; }
        TId DataObjectId { get; }
    }
}