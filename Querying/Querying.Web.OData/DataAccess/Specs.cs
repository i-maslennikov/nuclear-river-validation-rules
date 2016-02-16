using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Specifications;

namespace NuClear.Querying.Web.OData.DataAccess
{
    public static class Specs
    {
        public static class Find
        {
            public static FindSpecification<T> ById<T>(long id)
            {
                return new FindSpecification<T>(x => ((IIdentifiable)x).Id == id);
            }
        }
    }
}