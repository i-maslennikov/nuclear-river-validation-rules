using System;
using System.Collections.Generic;

using NuClear.River.Common.Metadata;

namespace NuClear.Replication.Core.API
{
    public interface IDataObjectTypesProvider
    {
        IReadOnlyCollection<Type> Get<TCommand>() where TCommand : ICommand;
    }
}