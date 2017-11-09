using System;
using System.Collections.Generic;

using NuClear.Replication.Core.Commands;

namespace NuClear.ValidationRules.Replication.Commands
{
    public sealed class ReplaceDataObjectCommand<T> : IReplaceDataObjectCommand
    {
        public ReplaceDataObjectCommand(Type dataObjectType, IReadOnlyCollection<T> dataObjects)
        {
            DataObjectType = dataObjectType;
            DataObjects = dataObjects;
        }

        public Type DataObjectType { get; }
        public IReadOnlyCollection<T> DataObjects { get; }
    }
}