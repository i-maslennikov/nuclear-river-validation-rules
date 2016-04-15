using System.Collections.Generic;
using System.Linq;

using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.API
{
    public interface IStorageBasedDataObjectAccessor<TDataObject>
    {
        IEqualityComparer<TDataObject> EqualityComparer { get; }

        IQueryable<TDataObject> GetSource(IQuery query);

        FindSpecification<TDataObject> GetFindSpecification(IReadOnlyCollection<ICommand> commands);
    }
}