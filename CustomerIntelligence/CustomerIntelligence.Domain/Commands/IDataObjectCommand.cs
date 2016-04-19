using System;

using NuClear.River.Common.Metadata;

namespace NuClear.CustomerIntelligence.Domain.Commands
{
    public interface IDataObjectCommand : ICommand
    {
        Type DataObjectType { get; }
    }
}