using System.Collections.Generic;

using NuClear.River.Common.Metadata;

namespace NuClear.Replication.Core.API.Actors
{
    public interface IActor
    {
        IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands);
    }
}