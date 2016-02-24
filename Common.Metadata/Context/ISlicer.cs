using System.Collections.Generic;

namespace NuClear.River.Common.Metadata.Context
{
    public interface ISlicer<out TSlice>
    {
        IEnumerable<TSlice> Slice(IEnumerable<Predicate> predicates);
    }
}
