using System;
using System.Collections.Generic;

using NuClear.River.Common.Metadata;

namespace NuClear.Replication.Core.API.DataObjects
{
    public interface IDataObjectTypesProvider
    {
        IReadOnlyCollection<Type> Get<TCommand>() where TCommand : ICommand;
    }
}