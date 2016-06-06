using System;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredAssociatedPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredAssociatedPosition))
            .Erm(
                new AssociatedPosition { Id = 1, IsActive = false, IsDeleted = false },
                new AssociatedPosition { Id = 2, IsActive = true, IsDeleted = true })
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedAssociatedPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedAssociatedPosition))
            .Erm(
                new AssociatedPosition { Id = 1, IsActive = true, AssociatedPositionsGroupId = 1, ObjectBindingType = 2, PositionId = 3 }
                )
            .Fact(
                new Storage.Model.Facts.AssociatedPosition { Id = 1, AssociatedPositionsGroupId = 1, ObjectBindingType = 2, PositionId = 3 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredAssociatedPositionsGroup
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredAssociatedPositionsGroup))
            .Erm(
                new AssociatedPositionsGroup { Id = 1, IsActive = false, IsDeleted = false },
                new AssociatedPositionsGroup { Id = 2, IsActive = true, IsDeleted = true }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedAssociatedPositionsGroup
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedAssociatedPositionsGroup))
            .Erm(
                new AssociatedPositionsGroup { Id = 1, IsActive = true, PricePositionId = 1 }
                )
            .Fact(
                new Storage.Model.Facts.AssociatedPositionsGroup { Id = 1, PricePositionId = 1 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredCategory
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredCategory))
            .Erm(
                new Category { Id = 1, IsActive = false, IsDeleted = false },
                new Category { Id = 2, IsActive = true, IsDeleted = true }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedCategory
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedCategory))
            .Erm(
                new Category { Id = 1, IsActive = true, IsDeleted = false, ParentId = 1 }
                )
            .Fact(
                new Storage.Model.Facts.Category { Id = 1, ParentId = 1 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredDeniedPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredDeniedPosition))
            .Erm(
                new DeniedPosition { Id = 1, IsActive = false, IsDeleted = false },
                new DeniedPosition { Id = 2, IsActive = true, IsDeleted = true }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedDeniedPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedDeniedPosition))
            .Erm(
                new DeniedPosition { Id = 1, IsActive = true, ObjectBindingType = 1, PositionDeniedId = 2, PositionId = 3, PriceId = 4 }
                )
            .Fact(
                new Storage.Model.Facts.DeniedPosition { Id = 1, ObjectBindingType = 1, PositionDeniedId = 2, PositionId = 3, PriceId = 4 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredRuleset
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredRuleset))
            .Erm(
                new Ruleset { Id = 1, IsDeleted = true },
                new RulesetRule { RulesetId = 1},
                new RulesetRule { RulesetId = 0 }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement Ruleset
        => ArrangeMetadataElement.Config
            .Name(nameof(Ruleset))
            .Erm(
                new Ruleset { Id = 1, Priority = 1 },
                new RulesetRule { RulesetId = 1, DependentPositionId = 2, ObjectBindingType = 3, PrincipalPositionId = 4 },
                new Ruleset { Id = 2, Priority = 2 },
                new RulesetRule { RulesetId = 2, DependentPositionId = 2, ObjectBindingType = 3, PrincipalPositionId = 4 }
                )
            .Fact(
                new Storage.Model.Facts.RulesetRule { Id = 1, Priority = 1, DependentPositionId = 2, ObjectBindingType = 3, PrincipalPositionId = 4 },
                new Storage.Model.Facts.RulesetRule { Id = 2, Priority = 2, DependentPositionId = 2, ObjectBindingType = 3, PrincipalPositionId = 4 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredOrder
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredOrder))
            .Erm(
                new Order { Id = 1, IsActive = false, IsDeleted = false },
                new Order { Id = 2, IsActive = true, IsDeleted = true }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedOrder
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedOrder))
            .Erm(
                new Order { Id = 1, IsActive = true, BeginDistributionDate = DateTime.Parse("2012-01-01"), BeginReleaseNumber = 1, DestOrganizationUnitId = 2, EndDistributionDateFact = DateTime.Parse("2012-01-31T23:59:59"), EndReleaseNumberFact = 3, EndReleaseNumberPlan = 4, FirmId = 5, Number = "Number", OwnerCode = 6, SourceOrganizationUnitId = 7, WorkflowStepId = 8 },
                new Project { Id = 3, OrganizationUnitId = 2, IsActive = true },
                new Project { Id = 8, OrganizationUnitId = 7, IsActive = true }
                )
            .Fact(
                new Storage.Model.Facts.Order { Id = 1, BeginDistributionDate = DateTime.Parse("2012-01-01"), BeginReleaseNumber = 1, DestProjectId = 3, EndDistributionDateFact = DateTime.Parse("2012-02-01"), EndReleaseNumberFact = 3, EndReleaseNumberPlan = 4, FirmId = 5, Number = "Number", OwnerId = 6, SourceProjectId = 8, WorkflowStepId = 8 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredOrderPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredOrderPosition))
            .Erm(
                new OrderPosition { Id = 1, IsActive = false, IsDeleted = false },
                new OrderPosition { Id = 2, IsActive = true, IsDeleted = true }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedOrderPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedOrderPosition))
            .Erm(
                new OrderPosition { Id = 1, IsActive = true, OrderId = 1, PricePositionId = 2 }
                )
            .Fact(
                new Storage.Model.Facts.OrderPosition { Id = 1, OrderId = 1, PricePositionId = 2 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedOrderPositionAdvertisement
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedOrderPositionAdvertisement))
            .Erm(
                new OrderPositionAdvertisement { Id = 1, CategoryId = 1, FirmAddressId = 2, OrderPositionId = 3, PositionId = 4 }
                )
            .Fact(
                new Storage.Model.Facts.OrderPositionAdvertisement { Id = 1, CategoryId = 1, FirmAddressId = 2, OrderPositionId = 3, PositionId = 4 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredOrganizationUnit
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredOrganizationUnit))
            .Erm(
                new OrganizationUnit { Id = 1, IsActive = false, IsDeleted = false },
                new OrganizationUnit { Id = 2, IsActive = true, IsDeleted = true }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedOrganizationUnit
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedOrganizationUnit))
            .Erm(
                new OrganizationUnit { Id = 1, IsActive = true }
                )
            .Fact(
                new Storage.Model.Facts.OrganizationUnit { Id = 1 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredPosition))
            .Erm(
                new Position { Id = 2, IsDeleted = true }
                )
            .Fact(
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedPosition))
            .Erm(
                new Position { Id = 33, BindingObjectTypeEnum = 33 },
                new Position { Id = 34, BindingObjectTypeEnum = 34 },
                new Position { Id = 1, BindingObjectTypeEnum = 1 },

                new Position { Id = 6, BindingObjectTypeEnum = 6 },
                new Position { Id = 35, BindingObjectTypeEnum = 35 },

                new Position { Id = 7, BindingObjectTypeEnum = 7 },
                new Position { Id = 8, BindingObjectTypeEnum = 8 },

                new Position { Id = 36, BindingObjectTypeEnum = 36 },
                new Position { Id = 37, BindingObjectTypeEnum = 37 },

                new Position { Id = 999, BindingObjectTypeEnum = 999, CategoryCode = 1, IsComposite = true, IsControlledByAmount = true, Name = "Name" }
                )
            .Fact(
                new Storage.Model.Facts.Position { Id = 33, CompareMode = 1 },
                new Storage.Model.Facts.Position { Id = 34, CompareMode = 1 },
                new Storage.Model.Facts.Position { Id = 1, CompareMode = 1 },

                new Storage.Model.Facts.Position { Id = 6, CompareMode = 2 },
                new Storage.Model.Facts.Position { Id = 35, CompareMode = 2 },

                new Storage.Model.Facts.Position { Id = 7, CompareMode = 3 },
                new Storage.Model.Facts.Position { Id = 8, CompareMode = 3 },

                new Storage.Model.Facts.Position { Id = 36, CompareMode = 4 },
                new Storage.Model.Facts.Position { Id = 37, CompareMode = 4 },

                new Storage.Model.Facts.Position { Id = 999, CompareMode = 0, CategoryCode = 1, IsComposite = true, IsControlledByAmount = true, Name = "Name" }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredPricePosition
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredPricePosition))
            .Erm(
                new PricePosition { Id = 1, IsActive = false, IsDeleted = false },
                new PricePosition { Id = 2, IsActive = false, IsDeleted = true },
                new PricePosition { Id = 3, IsActive = true, IsDeleted = true }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedPricePosition
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedPricePosition))
            .Erm(
                new PricePosition { Id = 1, IsActive = true, IsDeleted = false }
                )
            .Fact(
                new Storage.Model.Facts.PricePosition { Id = 1 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredProject
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredProject))
            .Erm(
                new Project { Id = 1, IsActive = false, OrganizationUnitId = 1 },
                new Project { Id = 2, IsActive = true, OrganizationUnitId = null }
                )
            .Fact(
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedProject
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedProject))
            .Erm(
                new Project { Id = 1, IsActive = true, OrganizationUnitId = 1 }
                )
            .Fact(
                new Storage.Model.Facts.Project { Id = 1, OrganizationUnitId = 1 }
                );

    }
}
