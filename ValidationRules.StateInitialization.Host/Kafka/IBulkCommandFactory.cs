using System.Collections.Generic;

using NuClear.Messaging.API.Flows;
using NuClear.Replication.Core;

namespace NuClear.ValidationRules.StateInitialization.Host.Kafka
{
    public interface IBulkCommandFactory<in TMessage>
        where TMessage : class
    {
        IReadOnlyCollection<IMessageFlow> AppropriateFlows { get; }
        IReadOnlyCollection<ICommand> CreateCommands(IReadOnlyCollection<TMessage> messages);
    }
}
