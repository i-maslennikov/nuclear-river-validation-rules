using System.Collections.Generic;

using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Specifications;

namespace NuClear.River.Common.Metadata
{
    public static class Specs
    {
        public static class Find
        {
            public static FindSpecification<T> ByIds<T>(IReadOnlyCollection<long> ids)
                where T : IIdentifiable<long>
            {
                return new FindSpecification<T>(DefaultIdentityProvider.Instance.Create<T, long>(ids));
            }
        }
    }
}