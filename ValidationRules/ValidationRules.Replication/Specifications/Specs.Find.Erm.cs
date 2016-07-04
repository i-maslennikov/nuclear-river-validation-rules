using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Specifications
{
    public static partial class Specs
    {
        public static partial class Find
        {
            public static class Erm
            {
                private const int RulesetDraftPriority = 0;
                private const int OrderStateArchive = 6;
                private const int OrderStateRejected = 3;

                public static FindSpecification<AssociatedPosition> AssociatedPositions()
                {
                    return new FindSpecification<AssociatedPosition>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<AssociatedPositionsGroup> AssociatedPositionsGroups()
                {
                    return new FindSpecification<AssociatedPositionsGroup>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<DeniedPosition> DeniedPositions()
                {
                    return new FindSpecification<DeniedPosition>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<Ruleset> Rulesets()
                {
                    return new FindSpecification<Ruleset>(x => x.Priority != RulesetDraftPriority && !x.IsDeleted);
                }

                public static FindSpecification<Category> Categories()
                {
                    return new FindSpecification<Category>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<Order> Orders()
                {
                    return new FindSpecification<Order>(x => x.IsActive && !x.IsDeleted && x.WorkflowStepId != OrderStateArchive && x.WorkflowStepId != OrderStateRejected);
                }

                public static FindSpecification<OrderPosition> OrderPositions()
                {
                    return new FindSpecification<OrderPosition>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<OrderPositionAdvertisement> OrderPositionAdvertisements()
                {
                    return new FindSpecification<OrderPositionAdvertisement>(x => true);
                }

                public static FindSpecification<OrganizationUnit> OrganizationUnits()
                {
                    return new FindSpecification<OrganizationUnit>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<Position> Positions()
                {
                    return new FindSpecification<Position>(x => !x.IsDeleted);
                }

                public static FindSpecification<Price> Prices()
                {
                    return new FindSpecification<Price>(x => x.IsActive && !x.IsDeleted && x.IsPublished);
                }

                public static FindSpecification<PricePosition> PricePositions()
                {
                    return new FindSpecification<PricePosition>(x => x.IsActive && !x.IsDeleted);
                }

                public static FindSpecification<PricePosition> PricePositionsNotActive()
                {
                    return new FindSpecification<PricePosition>(x => !x.IsActive || x.IsDeleted);
                }

                public static FindSpecification<Project> Projects()
                {
                    return new FindSpecification<Project>(x => x.IsActive && x.OrganizationUnitId != null);
                }

                public static FindSpecification<Account> Accounts()
                {
                    return new FindSpecification<Account>(x => !x.IsArchived);
                }
            }
        }
    }
}