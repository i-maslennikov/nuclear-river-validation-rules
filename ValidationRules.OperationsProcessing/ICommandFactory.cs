using System.Collections.Generic;

using NuClear.Messaging.API;
using NuClear.Replication.Core;

namespace NuClear.ValidationRules.OperationsProcessing
{
    public interface ICommandFactory<in TMessage>
        where TMessage : class, IMessage
    {
        IReadOnlyCollection<ICommand> CreateCommands(TMessage message);
    }
}
