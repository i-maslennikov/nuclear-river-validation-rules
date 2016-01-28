using System.Collections.Generic;

namespace NuClear.AdvancedSearch.Common.Metadata.Context
{
    public interface ISlicer<out TSlice>
    {
        IEnumerable<TSlice> Slice(IEnumerable<Predicate> predicates);
    }
}
