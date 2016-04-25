using System.Collections.Generic;

namespace NuClear.Replication.Core.API.Actors
{
    public interface IActor
    {
        IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands);
    }
}