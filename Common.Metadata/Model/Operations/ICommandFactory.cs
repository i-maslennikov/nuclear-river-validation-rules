using System;

namespace NuClear.River.Common.Metadata.Model.Operations
{
    public interface ICommandFactory<TKey>
    {
        IOperation Create(Type entityType, TKey key);
    }
}