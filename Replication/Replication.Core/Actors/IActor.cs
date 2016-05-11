using System.Collections.Generic;

namespace NuClear.Replication.Core.Actors
{
    public interface IActor
    {
        IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands);
    }
}