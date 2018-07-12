
using System.Linq;

using NuClear.Storage.API.Specifications;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.Specifications
{
    public static partial class Specs
    {
        public static partial class Find
        {
            public static class Erm
            {
                public static FindSpecification<Erm::Category> Category { get; }
                    = new FindSpecification<Erm::Category>(x => true);

                public static FindSpecification<Erm::Firm> Firm { get; }
                    = new FindSpecification<Erm::Firm>(x => true);

                public static FindSpecification<Erm::FirmAddress> FirmAddress { get; }
                    = new FindSpecification<Erm::FirmAddress>(x => true);

                public static FindSpecification<Erm::LegalPersonProfile> LegalPersonProfile { get; }
                    = new FindSpecification<Erm::LegalPersonProfile>(x => x.IsActive && !x.IsDeleted);

                public static FindSpecification<Erm::Order> Order { get; }
                    = new FindSpecification<Erm::Order>(x => x.IsActive && !x.IsDeleted && !Erm::Order.FilteredStates.Contains(x.WorkflowStepId));

                public static FindSpecification<Erm::OrderPosition> OrderPosition { get; }
                    = new FindSpecification<Erm::OrderPosition>(x => x.IsActive && !x.IsDeleted);

                public static FindSpecification<Erm::Position> Position { get; }
                    = new FindSpecification<Erm::Position>(x => true);

                public static FindSpecification<Erm::Project> Project { get; }
                    = new FindSpecification<Erm::Project>(x => x.IsActive && x.OrganizationUnitId != null);

                public static FindSpecification<Erm::Theme> Theme { get; }
                    = new FindSpecification<Erm::Theme>(x => x.IsActive && !x.IsDeleted);
            }

            public static class Facts
            {
                public static FindSpecification<Facts::Ruleset> Ruleset { get; }
                    = new FindSpecification<Facts::Ruleset>(x => !x.IsDeleted);
            }
        }
    }
}
