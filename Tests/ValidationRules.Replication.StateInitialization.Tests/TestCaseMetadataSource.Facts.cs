using System;

using NuClear.DataTest.Metamodel.Dsl;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;
using PriceFacts = NuClear.ValidationRules.Storage.Model.PriceRules.Facts;
using ConsistencyFacts = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts;
using AccountFacts = NuClear.ValidationRules.Storage.Model.AccountRules.Facts;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        private static readonly DateTime FirstDayJan = DateTime.Parse("2012-01-01");
        private static readonly DateTime FirstDayFeb = DateTime.Parse("2012-02-01");
        private static readonly DateTime LastSecondJan = DateTime.Parse("2012-01-31T23:59:59");
        private static readonly DateTime LastSecondMar = DateTime.Parse("2012-03-31T23:59:59");
        private static readonly DateTime LastSecondApr = DateTime.Parse("2012-04-30T23:59:59");

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(FirmFacts))
            .Erm(
                new Erm::Firm { Id = 1, IsActive = true, IsDeleted = false, ClosedForAscertainment = false, Name = "1" },
                new Erm::Firm { Id = 2, IsActive = false, IsDeleted = true, ClosedForAscertainment = false, Name = "2" },
                new Erm::Firm { Id = 3, IsActive = false, IsDeleted = false, ClosedForAscertainment = true, Name = "3" })
            .Fact(
                new ConsistencyFacts::Firm { Id = 1, IsActive = true, IsDeleted = false, IsClosedForAscertainment = false, Name = "1" },
                new ConsistencyFacts::Firm { Id = 2, IsActive = false, IsDeleted = true, IsClosedForAscertainment = false, Name = "2" },
                new ConsistencyFacts::Firm { Id = 3, IsActive = false, IsDeleted = false, IsClosedForAscertainment = true, Name = "3" });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmAddressFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(FirmAddressFacts))
            .Erm(
                new Erm::FirmAddress { Id = 1, IsActive = true, IsDeleted = false, ClosedForAscertainment = false, Address = "1" },
                new Erm::FirmAddress { Id = 2, IsActive = false, IsDeleted = true, ClosedForAscertainment = false, Address = "2" },
                new Erm::FirmAddress { Id = 3, IsActive = false, IsDeleted = false, ClosedForAscertainment = true, Address = "3" })
            .Fact(
                new ConsistencyFacts::FirmAddress { Id = 1, IsActive = true, IsDeleted = false, IsClosedForAscertainment = false, Name = "1" },
                new ConsistencyFacts::FirmAddress { Id = 2, IsActive = false, IsDeleted = true, IsClosedForAscertainment = false, Name = "2" },
                new ConsistencyFacts::FirmAddress { Id = 3, IsActive = false, IsDeleted = false, IsClosedForAscertainment = true, Name = "3" });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement BillFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(BillFacts))
            .Erm(
                new Erm::Bill { Id = 1, IsActive = true, IsDeleted = false, BeginDistributionDate = FirstDayJan, EndDistributionDate = LastSecondJan, BillType = 1, OrderId = 2, PayablePlan = 123 })
            .Fact(
                new ConsistencyFacts::Bill { Id = 1, Begin = FirstDayJan, End = LastSecondJan, OrderId = 2, PayablePlan = 123 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement BargainFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(BargainFacts))
            .Erm(
                new Erm::Bargain { Id = 1, IsActive = true, IsDeleted = false })
            .Fact(
                new ConsistencyFacts::Bargain { Id = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement CategoryFirmAddressFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(CategoryFirmAddressFacts))
            .Erm(
                new Erm::CategoryFirmAddress { Id = 1, IsActive = true, IsDeleted = false })
            .Fact(
                new ConsistencyFacts::CategoryFirmAddress { Id = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderFileFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(OrderFileFacts))
            .Erm(
                new Erm::OrderFile { Id = 1, OrderId = 1, IsActive = true, IsDeleted = false, FileKind = 8 })
            .Fact(
                new ConsistencyFacts::OrderScanFile { Id = 1, OrderId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement BargainFileFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(BargainFileFacts))
            .Erm(
                new Erm::BargainFile { Id = 1, IsActive = true, IsDeleted = false, BargainId = 1, FileKind = 12 })
            .Fact(
                new ConsistencyFacts::BargainScanFile { Id = 1, BargainId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement LegalPersonProfileFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(LegalPersonProfileFacts))
            .Erm(
                new Erm::LegalPersonProfile { Id = 1, IsActive = true, IsDeleted = false, BargainEndDate = FirstDayJan, WarrantyEndDate = FirstDayFeb, LegalPersonId = 1, Name = "1" },
                new Erm::LegalPersonProfile { Id = 2, IsActive = false, IsDeleted = false },
                new Erm::LegalPersonProfile { Id = 3, IsActive = true, IsDeleted = true },
                new Erm::LegalPersonProfile { Id = 4, IsActive = false, IsDeleted = true })
            .Fact(
                new ConsistencyFacts::LegalPersonProfile { Id = 1, BargainEndDate = FirstDayJan, WarrantyEndDate = FirstDayFeb, LegalPersonId = 1, Name = "1" });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AssociatedPositionFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(AssociatedPositionFacts))
            .Erm(
                new Erm::AssociatedPosition { Id = 1, IsActive = true, AssociatedPositionsGroupId = 1, ObjectBindingType = 2, PositionId = 3 },
                new Erm::AssociatedPosition { Id = 2, IsActive = false, IsDeleted = false },
                new Erm::AssociatedPosition { Id = 3, IsActive = true, IsDeleted = true })
            .Fact(
                new PriceFacts::AssociatedPosition { Id = 1, AssociatedPositionsGroupId = 1, ObjectBindingType = 2, PositionId = 3 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AssociatedPositionsGroupFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(AssociatedPositionsGroupFacts))
            .Erm(
                new Erm::AssociatedPositionsGroup { Id = 1, IsActive = true, PricePositionId = 1 },
                new Erm::AssociatedPositionsGroup { Id = 2, IsActive = false, IsDeleted = false },
                new Erm::AssociatedPositionsGroup { Id = 3, IsActive = true, IsDeleted = true })
            .Fact(
                new PriceFacts::AssociatedPositionsGroup { Id = 1, PricePositionId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement CategoryFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(CategoryFacts))
            .Erm(
                new Erm::Category { Id = 1, IsActive = true, IsDeleted = false, Level = 1, Name = "1" },
                new Erm::Category { Id = 2, IsActive = true, IsDeleted = false, Level = 2, ParentId = 1, Name = "2" },
                new Erm::Category { Id = 3, IsActive = true, IsDeleted = false, Level = 3, ParentId = 2, Name = "3" },
                new Erm::Category { Id = 4, IsActive = false, IsDeleted = false },
                new Erm::Category { Id = 5, IsActive = true, IsDeleted = true })
            .Fact(
                new PriceFacts::Category { Id = 1, L1Id = 1, Name = "1" },
                new PriceFacts::Category { Id = 2, L1Id = 1, L2Id = 2, Name = "2" },
                new PriceFacts::Category { Id = 3, L1Id = 1, L2Id = 2, L3Id = 3, Name = "3" },

                new ConsistencyFacts::Category { Id = 1, IsActiveNotDeleted = true, Name = "1" },
                new ConsistencyFacts::Category { Id = 2, IsActiveNotDeleted = true, Name = "2" },
                new ConsistencyFacts::Category { Id = 3, IsActiveNotDeleted = true, Name = "3" },
                new ConsistencyFacts::Category { Id = 4, IsActiveNotDeleted = false },
                new ConsistencyFacts::Category { Id = 5, IsActiveNotDeleted = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement DeniedPositionFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(DeniedPositionFacts))
            .Erm(
                new Erm::DeniedPosition { Id = 1, IsActive = true, ObjectBindingType = 1, PositionDeniedId = 2, PositionId = 3, PriceId = 4 },
                new Erm::DeniedPosition { Id = 2, IsActive = false, IsDeleted = false },
                new Erm::DeniedPosition { Id = 3, IsActive = true, IsDeleted = true })
            .Fact(
                new PriceFacts::DeniedPosition { Id = 1, ObjectBindingType = 1, PositionDeniedId = 2, PositionId = 3, PriceId = 4 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement RulesetFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(RulesetFacts))
            .Erm(
                new Erm::Ruleset { Id = 1, Priority = 1 },
                new Erm::RulesetRule { RulesetId = 1, DependentPositionId = 2, ObjectBindingType = 3, PrincipalPositionId = 4 },
                new Erm::Ruleset { Id = 2, Priority = 2 },
                new Erm::RulesetRule { RulesetId = 2, DependentPositionId = 2, ObjectBindingType = 3, PrincipalPositionId = 4 },

                new Erm::Ruleset { Id = 3, IsDeleted = true },
                new Erm::RulesetRule { RulesetId = 3 },
                new Erm::RulesetRule { RulesetId = 0 }
                )
            .Fact(
                new PriceFacts::RulesetRule { Id = 1, Priority = 1, DependentPositionId = 2, ObjectBindingType = 3, PrincipalPositionId = 4 },
                new PriceFacts::RulesetRule { Id = 2, Priority = 2, DependentPositionId = 2, ObjectBindingType = 3, PrincipalPositionId = 4 }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(OrderFacts))
            .Erm(
                new Erm::Order { Id = 1, IsActive = true, BeginDistributionDate = FirstDayJan, EndDistributionDateFact = LastSecondJan, EndDistributionDatePlan = LastSecondMar, BeginReleaseNumber = 1, DestOrganizationUnitId = 2, EndReleaseNumberFact = 3, EndReleaseNumberPlan = 4, FirmId = 5, Number = "Number", OwnerCode = 6, SourceOrganizationUnitId = 7, WorkflowStepId = 8, CurrencyId = 9, ReleaseCountPlan = 3, OrderType = 2 },
                new Erm::Order { Id = 2, IsActive = false, IsDeleted = false },
                new Erm::Order { Id = 3, IsActive = true, IsDeleted = true })
            .Fact(
                new PriceFacts::Order { Id = 1, BeginDistributionDate = FirstDayJan, EndDistributionDateFact = LastSecondJan.AddSeconds(1), EndDistributionDatePlan = LastSecondMar.AddSeconds(1), BeginReleaseNumber = 1, DestOrganizationUnitId = 2, EndReleaseNumberFact = 3, EndReleaseNumberPlan = 4, FirmId = 5, Number = "Number", OwnerId = 6, SourceOrganizationUnitId = 7, WorkflowStepId = 8 },
                new ConsistencyFacts::Order { Id = 1, BeginDistribution = FirstDayJan, EndDistributionFact = LastSecondJan, EndDistributionPlan = LastSecondMar, DestOrganizationUnitId = 2, FirmId = 5, Number = "Number", CurrencyId = 9, ReleaseCountPlan = 3, WorkflowStep = 8, IsFreeOfCharge = true });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPositionFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(OrderPositionFacts))
                .Erm(
                    new Erm::OrderPosition { Id = 1, IsActive = true, OrderId = 1, PricePositionId = 2 },
                    new Erm::OrderPosition { Id = 2, IsActive = false, IsDeleted = false },
                    new Erm::OrderPosition { Id = 3, IsActive = true, IsDeleted = true })
                .Fact(
                    new PriceFacts::OrderPosition { Id = 1, OrderId = 1, PricePositionId = 2 },
                    new ConsistencyFacts::OrderPosition { Id = 1, OrderId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPositionAdvertisementFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(OrderPositionAdvertisementFacts))
            .Erm(
                new Erm::OrderPositionAdvertisement { Id = 1, CategoryId = 1, FirmAddressId = 2, OrderPositionId = 3, PositionId = 4 })
            .Fact(
                new PriceFacts::OrderPositionAdvertisement { Id = 1, CategoryId = 1, FirmAddressId = 2, OrderPositionId = 3, PositionId = 4 },
                new ConsistencyFacts::OrderPositionAdvertisement { Id = 1, CategoryId = 1, FirmAddressId = 2, OrderPositionId = 3, PositionId = 4 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PositionFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(PositionFacts))
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

                new Erm::Position { Id = 999, BindingObjectTypeEnum = 999, CategoryCode = 1, IsComposite = true, IsControlledByAmount = true, Name = "Name" },

                new Erm::Position { Id = 1000, IsDeleted = true })
            .Fact(
                new PriceFacts::Position { Id = 33 },
                new PriceFacts::Position { Id = 34 },
                new PriceFacts::Position { Id = 1 },

                new PriceFacts::Position { Id = 6 },
                new PriceFacts::Position { Id = 35 },

                new PriceFacts::Position { Id = 7 },
                new PriceFacts::Position { Id = 8 },

                new PriceFacts::Position { Id = 36 },
                new PriceFacts::Position { Id = 37 },

                new PriceFacts::Position { Id = 999, CategoryCode = 1, IsComposite = true, IsControlledByAmount = true, Name = "Name" },

                new ConsistencyFacts::Position { Id = 33, BindingObjectType = 33 },
                new ConsistencyFacts::Position { Id = 34, BindingObjectType = 34 },
                new ConsistencyFacts::Position { Id = 1, BindingObjectType = 1 },

                new ConsistencyFacts::Position { Id = 6, BindingObjectType = 6 },
                new ConsistencyFacts::Position { Id = 35, BindingObjectType = 35 },


                new ConsistencyFacts::Position { Id = 7, BindingObjectType = 7 },
                new ConsistencyFacts::Position { Id = 8, BindingObjectType = 8 },


                new ConsistencyFacts::Position { Id = 36, BindingObjectType = 36 },
                new ConsistencyFacts::Position { Id = 37, BindingObjectType = 37 },

                new ConsistencyFacts::Position { Id = 999, BindingObjectType = 999, Name = "Name" });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PricePositionFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(PricePositionFacts))
                .Erm(
                    new Erm::PricePosition { Id = 1, IsActive = false, IsDeleted = false },
                    new Erm::PricePosition { Id = 2, IsActive = false, IsDeleted = true },
                    new Erm::PricePosition { Id = 3, IsActive = true, IsDeleted = true },
                    new Erm::PricePosition { Id = 4, IsActive = true, IsDeleted = false })
                .Fact(
                    new PriceFacts::PricePositionNotActive { Id = 1 },
                    new PriceFacts::PricePositionNotActive { Id = 2 },
                    new PriceFacts::PricePositionNotActive { Id = 3 },
                    new PriceFacts::PricePosition { Id = 4 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ProjectFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(ProjectFacts))
                .Erm(
                    new Erm::Project { Id = 1, IsActive = true, OrganizationUnitId = 1 },
                    new Erm::Project { Id = 2, IsActive = false, OrganizationUnitId = 1 },
                    new Erm::Project { Id = 3, IsActive = true, OrganizationUnitId = null })
                .Fact(
                    new PriceFacts::Project { Id = 1, OrganizationUnitId = 1 },
                    new ConsistencyFacts::Project { Id = 1, OrganizationUnitId = 1 },
                    new AccountFacts::Project { Id = 1, OrganizationUnitId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PriceFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(PriceFacts))
                .Erm(
                    new Erm::Price { Id = 1, IsActive = true, IsDeleted = false, IsPublished = true, OrganizationUnitId = 1 },
                    new Erm::Price { Id = 2, IsActive = false, IsDeleted = false, IsPublished = true },
                    new Erm::Price { Id = 3, IsActive = true, IsDeleted = true, IsPublished = true },
                    new Erm::Price { Id = 4, IsActive = true, IsDeleted = false, IsPublished = false })
                .Fact(
                    new PriceFacts::Price { Id = 1, OrganizationUnitId = 1 });
    }
}
