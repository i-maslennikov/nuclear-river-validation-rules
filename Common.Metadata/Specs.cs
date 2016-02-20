using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.Storage.API.Specifications;

namespace NuClear.AdvancedSearch.Common.Metadata
{
    public static class Specs
    {
        public static class Find
        {
            public static FindSpecification<T> ByIds<T>(IReadOnlyCollection<long> ids)
                where T : IIdentifiable<long>
            {
                return new FindSpecification<T>(DefaultIdentityProvider.Instance.Create<T>(ids));
            }
        }

        public static class Map
        {
            // Какое-то Г. зачем T[] => int[], когда есть T => int?
            // Тем более, что используется Func, а не Expression, а значит, все вычисления на стороне приложения
            public static MapSpecification<IEnumerable<T>, IEnumerable<long>> ToIds<T>()
                where T : IIdentifiable<long>
            {
                Func<T, long> identityProjector = DefaultIdentityProvider.Instance.ExtractIdentity<T>().Compile();
                Func<IEnumerable<T>, IEnumerable<long>> projector = item => item.Select(identityProjector);
                return new MapSpecification<IEnumerable<T>, IEnumerable<long>>(projector);
            }

            public static MapSpecification<IEnumerable<T>, IEnumerable<T>> ZeroMapping<T>()
            {
                return new MapSpecification<IEnumerable<T>, IEnumerable<T>>(x => x);
            }
        }
    }
}