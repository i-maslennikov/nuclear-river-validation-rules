using System;
using System.Collections.Generic;

using NuClear.Replication.Core.Commands;

namespace NuClear.ValidationRules.Replication.Commands
{
    public sealed class ReplaceDataObjectCommand : IReplaceDataObjectCommand
    {
        public Type DataObjectType { get; }
        public IEnumerable<object> Dtos { get; }

        public ReplaceDataObjectCommand(Type dataObjectType, IEnumerable<object> dtos)
        {
            DataObjectType = dataObjectType;
            Dtos = dtos;
        }
    }
}