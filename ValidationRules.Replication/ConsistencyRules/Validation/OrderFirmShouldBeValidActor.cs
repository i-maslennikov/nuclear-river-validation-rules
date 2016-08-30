using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    public sealed class OrderFirmShouldBeValidActor : IActor
    {
        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            throw new NotImplementedException();
        }
    }
}
