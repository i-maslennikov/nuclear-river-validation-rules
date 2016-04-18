using System;

using NuClear.River.Common.Metadata;

namespace NuClear.CustomerIntelligence.Domain.Commands
{
    public interface IAggregateCommand : ICommand
    {
        Type AggregateRootType { get; }
        long AggregateRootId { get; }
    }
}