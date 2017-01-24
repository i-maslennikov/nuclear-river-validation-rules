using System.Collections.Generic;

using NuClear.Replication.Core;

namespace NuClear.ValidationRules.OperationsProcessing
{
    internal interface ICommandFactory
    {
        IEnumerable<ICommand> CreateCommands(IEvent @event);
    }
}