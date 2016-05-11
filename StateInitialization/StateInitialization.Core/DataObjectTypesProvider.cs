using System;
using System.Collections.Generic;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;

namespace NuClear.StateInitialization.Core
{
    public class DataObjectTypesProvider : IDataObjectTypesProvider
    {
        private readonly IReadOnlyCollection<Type> _dataObjectTypes;

        public DataObjectTypesProvider(IReadOnlyCollection<Type> dataObjectTypes)
        {
            _dataObjectTypes = dataObjectTypes;
        }

        public IReadOnlyCollection<Type> Get<TCommand>() where TCommand : ICommand
        {
            return _dataObjectTypes;
        }
    }
}