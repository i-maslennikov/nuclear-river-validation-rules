using System;

using NuClear.River.Common.Metadata;

namespace NuClear.Replication.Core.API.Commands
{
    public abstract class CreateDataObjectCommandBase : ICommand
    {
        public abstract Type DataObjectType { get; }
    }
}