using System;
using System.Collections.Generic;

using NuClear.Replication.Core.API.DataObjects;
using NuClear.River.Common.Metadata;

namespace NuClear.Replication.Bulk.API
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