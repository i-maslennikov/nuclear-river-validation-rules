using System.Collections.Generic;

using NuClear.River.Common.Metadata;

namespace NuClear.Replication.Core.API
{
    public interface IActor
    {
        IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands);
    }
}