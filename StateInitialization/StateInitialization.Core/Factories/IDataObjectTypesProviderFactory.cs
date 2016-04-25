using NuClear.Replication.Core.DataObjects;
using NuClear.StateInitialization.Core.Commands;

namespace NuClear.StateInitialization.Core.Factories
{
    public interface IDataObjectTypesProviderFactory
    {
        IDataObjectTypesProvider Create(ReplaceDataObjectsInBulkCommand command);
    }
}