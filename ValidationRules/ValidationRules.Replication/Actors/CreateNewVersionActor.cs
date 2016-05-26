using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.ValidationRules.Replication.Commands;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Actors
{
    public class CreateNewVersionActor : IActor
    {
        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            //var versionStates = commands.OfType<CreateNewVersionCommand>().SelectMany(x => x.States);
            //var version = new Version { Id = 0 };
            throw new NotImplementedException();
        }
    }
}