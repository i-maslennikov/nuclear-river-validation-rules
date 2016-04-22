using System;

using NuClear.River.Common.Metadata;

namespace NuClear.Replication.Core.API.Commands
{
    public interface IDataObjectCommand : ICommand
    {
        Type DataObjectType { get; }
    }
}