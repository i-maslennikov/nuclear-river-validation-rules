using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.Specifications
{
    public static partial class Specs
    {
        public static partial class Find
        {
            public static class Facts
            {
                public static FindSpecification<Ruleset> Ruleset { get; }
                    = new FindSpecification<Ruleset>(x => !x.IsDeleted);
            }
        }
    }
}