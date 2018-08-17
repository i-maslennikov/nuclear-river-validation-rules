using System;
using System.Collections.Generic;

using NuClear.Replication.Core;

namespace NuClear.ValidationRules.StateInitialization.Host.Kafka
{
    public sealed class BulkInsertInMemoryDataObjectsCommand : ICommand
    {
        public BulkInsertInMemoryDataObjectsCommand(Type dataObjectType, IEnumerable<object> dtos)
        {
            DataObjectType = dataObjectType;
            Dtos = dtos;
        }

        public Type DataObjectType { get; }
        public IEnumerable<object> Dtos { get; }
    }
}