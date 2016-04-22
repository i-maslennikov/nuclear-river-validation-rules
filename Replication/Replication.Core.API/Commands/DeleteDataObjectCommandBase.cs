using System;

using NuClear.River.Common.Metadata;

namespace NuClear.Replication.Core.API.Commands
{
    public abstract class DeleteDataObjectCommandBase : ICommand
    {
        public abstract Type DataObjectType { get; }
    }
}