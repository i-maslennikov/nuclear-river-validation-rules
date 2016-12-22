using NMemory.Modularity;
using NMemory.Tables;

namespace NuClear.ValidationRules.SingleCheck.Store
{
    public interface INMemoryTableRegistrar
    {
        ITable<T> RegisterTable<T>(IDatabase database) where T : class;
    }
}