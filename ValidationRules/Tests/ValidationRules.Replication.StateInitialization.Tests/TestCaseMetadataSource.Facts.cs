using System;

using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    using Erm = Domain.Model.Erm;
    using Facts = Domain.Model.Facts;

    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredAssociatedPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredAssociatedPosition))
            .Erm(
                new Erm::AssociatedPosition { Id = 1, IsActive = false, IsDeleted = false },
                new Erm::AssociatedPosition { Id = 2, IsActive = true, IsDeleted = true })
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedAssociatedPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedAssociatedPosition))
            .Erm(
                new Erm::AssociatedPosition { Id = 1, IsActive = true, AssociatedPositionsGroupId = 1, ObjectBindingType = 2, PositionId = 3 }
                )
            .Fact(
                new Facts::AssociatedPosition { Id = 1, AssociatedPositionsGroupId = 1, ObjectBindingType = 2, PositionId = 3 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredAssociatedPositionsGroup
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredAssociatedPositionsGroup))
            .Erm(
                new Erm::AssociatedPositionsGroup { Id = 1, IsActive = false, IsDeleted = false },
                new Erm::AssociatedPositionsGroup { Id = 2, IsActive = true, IsDeleted = true }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedAssociatedPositionsGroup
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedAssociatedPositionsGroup))
            .Erm(
                new Erm::AssociatedPositionsGroup { Id = 1, IsActive = true, PricePositionId = 1 }
                )
            .Fact(
                new Facts::AssociatedPositionsGroup { Id = 1, PricePositionId = 1 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredCategory
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredCategory))
            .Erm(
                new Erm::Category { Id = 1, IsActive = false, IsDeleted = false },
                new Erm::Category { Id = 2, IsActive = true, IsDeleted = true }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedCategory
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedCategory))
            .Erm(
                new Erm::Category { Id = 1, IsActive = true, IsDeleted = false, ParentId = 1 }
                )
            .Fact(
                new Facts::Category { Id = 1, ParentId = 1 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredDeniedPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredDeniedPosition))
            .Erm(
                new Erm::DeniedPosition { Id = 1, IsActive = false, IsDeleted = false },
                new Erm::DeniedPosition { Id = 2, IsActive = true, IsDeleted = true }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedDeniedPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedDeniedPosition))
            .Erm(
                new Erm::DeniedPosition { Id = 1, IsActive = true, ObjectBindingType = 1, PositionDeniedId = 2, PositionId = 3, PriceId = 4 }
                )
            .Fact(
                new Facts::DeniedPosition { Id = 1, ObjectBindingType = 1, PositionDeniedId = 2, PositionId = 3, PriceId = 4 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredGlobalAssociatedPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredGlobalAssociatedPosition))
            .Erm(
                new Erm::GlobalAssociatedPosition { Id = 1, IsDeleted = true, RulesetId = 1},
                new Erm::GlobalAssociatedPosition { Id = 2, IsDeleted = false, RulesetId = 0 }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedGlobalAssociatedPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedGlobalAssociatedPosition))
            .Erm(
                new Erm::GlobalAssociatedPosition { Id = 1, RulesetId = 1, AssociatedPositionId = 2, ObjectBindingType = 3, PrincipalPositionId = 4 }
                )
            .Fact(
                new Facts::GlobalAssociatedPosition { Id = 1, RulesetId = 1, AssociatedPositionId = 2, ObjectBindingType = 3, PrincipalPositionId = 4 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredGlobalDeniedPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredGlobalDeniedPosition))
            .Erm(
                new Erm::GlobalDeniedPosition { Id = 1, IsDeleted = true, RulesetId = 1 },
                new Erm::GlobalDeniedPosition { Id = 2, IsDeleted = false, RulesetId = 0 }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedGlobalDeniedPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedGlobalDeniedPosition))
            .Erm(
                new Erm::GlobalDeniedPosition { Id = 1, RulesetId = 1, DeniedPositionId = 2, ObjectBindingType = 3, PrincipalPositionId = 4 }
                )
            .Fact(
                new Facts::GlobalDeniedPosition { Id = 1, RulesetId = 1, DeniedPositionId = 2, ObjectBindingType = 3, PrincipalPositionId = 4 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredOrder
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredOrder))
            .Erm(
                new Erm::Order { Id = 1, IsActive = false, IsDeleted = false },
                new Erm::Order { Id = 2, IsActive = true, IsDeleted = true }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedOrder
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedOrder))
            .Erm(
                new Erm::Order { Id = 1, IsActive = true, BeginDistributionDate = DateTime.Parse("2012-01-01"), BeginReleaseNumber = 1, DestOrganizationUnitId = 2, EndDistributionDateFact = DateTime.Parse("2012-01-31T23:59:59"), EndReleaseNumberFact = 3, EndReleaseNumberPlan = 4, FirmId = 5, Number = "Number", OwnerCode = 6, SourceOrganizationUnitId = 7, WorkflowStepId = 8 }
                )
            .Fact(
                new Facts::Order { Id = 1, BeginDistributionDate = DateTime.Parse("2012-01-01"), BeginReleaseNumber = 1, DestOrganizationUnitId = 2, EndDistributionDateFact = DateTime.Parse("2012-02-01"), EndReleaseNumberFact = 3, EndReleaseNumberPlan = 4, FirmId = 5, Number = "Number", OwnerId = 6, SourceOrganizationUnitId = 7, WorkflowStepId = 8 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredOrderPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredOrderPosition))
            .Erm(
                new Erm::OrderPosition { Id = 1, IsActive = false, IsDeleted = false },
                new Erm::OrderPosition { Id = 2, IsActive = true, IsDeleted = true }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedOrderPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedOrderPosition))
            .Erm(
                new Erm::OrderPosition { Id = 1, IsActive = true, OrderId = 1, PricePositionId = 2 }
                )
            .Fact(
                new Facts::OrderPosition { Id = 1, OrderId = 1, PricePositionId = 2 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedOrderPositionAdvertisement
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedOrderPositionAdvertisement))
            .Erm(
                new Erm::OrderPositionAdvertisement { Id = 1, CategoryId = 1, FirmAddressId = 2, OrderPositionId = 3, PositionId = 4 }
                )
            .Fact(
                new Facts::OrderPositionAdvertisement { Id = 1, CategoryId = 1, FirmAddressId = 2, OrderPositionId = 3, PositionId = 4 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredOrganizationUnit
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredOrganizationUnit))
            .Erm(
                new Erm::OrganizationUnit { Id = 1, IsActive = false, IsDeleted = false },
                new Erm::OrganizationUnit { Id = 2, IsActive = true, IsDeleted = true }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedOrganizationUnit
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedOrganizationUnit))
            .Erm(
                new Erm::OrganizationUnit { Id = 1, IsActive = true }
                )
            .Fact(
                new Facts::OrganizationUnit { Id = 1 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredPosition))
            .Erm(
                new Erm::Position { Id = 2, IsDeleted = true }
                )
            .Fact(
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedPosition
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedPosition))
            .Erm(
                new Erm::Position { Id = 33, BindingObjectTypeEnum = 33 },
                new Erm::Position { Id = 34, BindingObjectTypeEnum = 34 },
                new Erm::Position { Id = 1, BindingObjectTypeEnum = 1 },

                new Erm::Position { Id = 6, BindingObjectTypeEnum = 6 },
                new Erm::Position { Id = 35, BindingObjectTypeEnum = 35 },

                new Erm::Position { Id = 7, BindingObjectTypeEnum = 7 },
                new Erm::Position { Id = 8, BindingObjectTypeEnum = 8 },

                new Erm::Position { Id = 36, BindingObjectTypeEnum = 36 },
                new Erm::Position { Id = 37, BindingObjectTypeEnum = 37 },

                new Erm::Position { Id = 999, BindingObjectTypeEnum = 999, CategoryCode = 1, IsComposite = true, IsControlledByAmount = true, Name = "Name" }
                )
            .Fact(
                new Facts::Position { Id = 33, CompareMode = 1 },
                new Facts::Position { Id = 34, CompareMode = 1 },
                new Facts::Position { Id = 1, CompareMode = 1 },

                new Facts::Position { Id = 6, CompareMode = 2 },
                new Facts::Position { Id = 35, CompareMode = 2 },

                new Facts::Position { Id = 7, CompareMode = 3 },
                new Facts::Position { Id = 8, CompareMode = 3 },

                new Facts::Position { Id = 36, CompareMode = 4 },
                new Facts::Position { Id = 37, CompareMode = 4 },

                new Facts::Position { Id = 999, CompareMode = 0, CategoryCode = 1, IsComposite = true, IsControlledByAmount = true, Name = "Name" }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredPricePosition
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredPricePosition))
            .Erm(
                new Erm::PricePosition { Id = 1, IsActive = false, IsDeleted = false },
                new Erm::PricePosition { Id = 2, IsActive = false, IsDeleted = true },
                new Erm::PricePosition { Id = 3, IsActive = true, IsDeleted = true }
                )
            .Fact();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedPricePosition
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedPricePosition))
            .Erm(
                new Erm::PricePosition { Id = 1, IsActive = true, IsDeleted = false }
                )
            .Fact(
                new Facts::PricePosition { Id = 1 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement IgnoredProject
        => ArrangeMetadataElement.Config
            .Name(nameof(IgnoredProject))
            .Erm(
                new Erm::Project { Id = 1, IsActive = false, OrganizationUnitId = 1 },
                new Erm::Project { Id = 2, IsActive = true, OrganizationUnitId = null }
                )
            .Fact(
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReplicatedProject
        => ArrangeMetadataElement.Config
            .Name(nameof(ReplicatedProject))
            .Erm(
                new Erm::Project { Id = 1, IsActive = true, OrganizationUnitId = 1 }
                )
            .Fact(
                new Facts::Project { Id = 1, OrganizationUnitId = 1 }
                );

    }
}
