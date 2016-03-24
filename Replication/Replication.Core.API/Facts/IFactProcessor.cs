using System.Collections.Generic;

using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.Replication.Core.API.Facts
{
    public interface IFactProcessor
    {
        IReadOnlyCollection<IOperation> Execute(IReadOnlyCollection<FactOperation> commands);
    }
}