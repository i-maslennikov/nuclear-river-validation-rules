using NuClear.Model.Common.Entities;

namespace NuClear.Replication.OperationsProcessing.Transports
{
    public interface IEntityTypeParser
    {
        IEntityType Parse(int id);
    }
}