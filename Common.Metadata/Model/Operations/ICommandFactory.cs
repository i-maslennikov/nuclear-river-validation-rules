using NuClear.Model.Common.Entities;

namespace NuClear.River.Common.Metadata.Model.Operations
{
    public interface ICommandFactory<TKey>
    {
        IOperation Create(IEntityType entityType, TKey key);
    }
}