using System;

using NuClear.Model.Common.Entities;

namespace NuClear.Replication.OperationsProcessing.Transports
{
    [Obsolete]
    public interface IEntityTypeParser
    {
        IEntityType Parse(int id);
    }
}