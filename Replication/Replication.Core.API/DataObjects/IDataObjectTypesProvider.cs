using System;
using System.Collections.Generic;

namespace NuClear.Replication.Core.API.DataObjects
{
    public interface IDataObjectTypesProvider
    {
        IReadOnlyCollection<Type> Get<TCommand>() where TCommand : ICommand;
    }
}