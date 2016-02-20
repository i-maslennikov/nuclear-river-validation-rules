using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Bulk.API
{
    public static class Specs
    {
        public static class Find
        {
            public static FindSpecification<T> All<T>()
            {
                return new FindSpecification<T>(x => true);
            }
        }
    }
}