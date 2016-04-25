using System;
using System.Collections.Generic;

namespace NuClear.Replication.Core.DataObjects
{
    public interface IDataObjectTypesProvider
    {
        IReadOnlyCollection<Type> Get<TCommand>() where TCommand : ICommand;
    }
}