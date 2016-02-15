using System.Collections.Generic;
using System.Linq;

using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Specifications;

namespace NuClear.River.Common.Metadata
{
    public static class Specs
    {
        public static class Find
        {
            public static FindSpecification<T> ByIds<T>(IReadOnlyCollection<long> ids) where T : IIdentifiable
            {
                return new FindSpecification<T>(x => ids.Contains(x.Id));
            }
        }

        public static class Map
        {
            public static MapSpecification<IEnumerable<T>, IEnumerable<long>> ToIds<T>()
                where T : IIdentifiable
            {
                return new MapSpecification<IEnumerable<T>, IEnumerable<long>>(x => x.Select(y => y.Id));
            }

            public static MapSpecification<IEnumerable<T>, IEnumerable<T>> ZeroMapping<T>()
            {
                return new MapSpecification<IEnumerable<T>, IEnumerable<T>>(x => x);
            }
        }
    }
}