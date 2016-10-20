using System;

using NuClear.DataTest.Metamodel.Dsl;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;
using PriceFacts = NuClear.ValidationRules.Storage.Model.PriceRules.Facts;
using ConsistencyFacts = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts;
using AccountFacts = NuClear.ValidationRules.Storage.Model.AccountRules.Facts;
using AdvertisementFacts = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;
using ProjectFacts = NuClear.ValidationRules.Storage.Model.ProjectRules.Facts;
using ThemeFacts = NuClear.ValidationRules.Storage.Model.ThemeRules.Facts;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        private static readonly DateTime FirstDayJan = DateTime.Parse("2012-01-01");
        private static readonly DateTime FirstDayFeb = DateTime.Parse("2012-02-01");
        private static readonly DateTime FirstDayMar = DateTime.Parse("2012-03-01");
        private static readonly DateTime FirstDayApr = DateTime.Parse("2012-04-01");
        private static readonly DateTime LastSecondJan = DateTime.Parse("2012-01-31T23:59:59");
        private static readonly DateTime LastSecondMar = DateTime.Parse("2012-03-31T23:59:59");
        private static readonly DateTime LastSecondApr = DateTime.Parse("2012-04-30T23:59:59");

        private static DateTime MonthStart(int i) => DateTime.Parse("2012-01-01").AddMonths(i - 1);

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmContactsFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(FirmContactsFacts))
            .Erm(
                new Erm::FirmContact { Id = 1, FirmAddressId = 1, ContactType = 4, Contact = "http://localhost"},
                new Erm::FirmContact { Id = 2, FirmAddressId = null, ContactType = 4, Contact = "http://localhost" },
                new Erm::FirmContact { Id = 3, FirmAddressId = 1, ContactType = 3, Contact = "http://localhost" })
            .Fact(
                new AdvertisementFacts::FirmAddressWebsite { Id = 1, FirmAddressId = 1, Website = "http://localhost" });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ThemeOrganizationUnitFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(ThemeOrganizationUnitFacts))
            .Erm(
                new Erm::ThemeOrganizationUnit { Id = 1, ThemeId = 2, OrganizationUnitId = 3, IsActive = true, IsDeleted = false },
                new Erm::ThemeOrganizationUnit { Id = 2, ThemeId = 2, OrganizationUnitId = 3, IsActive = false, IsDeleted = false },
                new Erm::ThemeOrganizationUnit { Id = 3, ThemeId = 2, OrganizationUnitId = 3, IsActive = true, IsDeleted = true })
            .Fact(
                new ThemeFacts::ThemeOrganizationUnit { Id = 1, ThemeId = 2, OrganizationUnitId = 3 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ThemeCategoryFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(ThemeCategoryFacts))
            .Erm(
                new Erm::ThemeCategory { Id = 1, ThemeId = 2, CategoryId = 3, IsDeleted = false },
                new Erm::ThemeCategory { Id = 2, ThemeId = 2, CategoryId = 3, IsDeleted = true })
            .Fact(
                new ThemeFacts::ThemeCategory { Id = 1, ThemeId = 2, CategoryId = 3 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement SalesModelCategoryRestrictions
        => ArrangeMetadataElement.Config
            .Name(nameof(SalesModelCategoryRestrictions))
            .Erm(
                new Erm::SalesModelCategoryRestriction { ProjectId = 1, BeginningDate = MonthStart(1), CategoryId = 1, SalesModel = 2 })
            .Fact(
                new ProjectFacts::SalesModelCategoryRestriction { ProjectId = 1, Begin = MonthStart(1), CategoryId = 1, SalesModel = 2 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AdvertisementFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(AdvertisementFacts))
            .Erm(
                new Erm::Advertisement { Id = 1, IsDeleted = false, Name = "Advertisement1" },
                new Erm::Advertisement { Id = 2, IsDeleted = true, Name = "Advertisement2" })
            .Fact(
                new AdvertisementFacts::Advertisement { Id = 1, IsDeleted = false, Name = "Advertisement1" },
                new AdvertisementFacts::Advertisement { Id = 2, IsDeleted = true, Name = "Advertisement2" });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AdvertisementTemplateFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(AdvertisementTemplateFacts))
            .Erm(
                new Erm::AdvertisementTemplate { Id = 1, IsDeleted = false, IsPublished = true, DummyAdvertisementId = 1 },
                new Erm::AdvertisementTemplate { Id = 2, IsDeleted = true },
                new Erm::AdvertisementTemplate { Id = 3, IsPublished = false },
                new Erm::AdvertisementTemplate { Id = 4, DummyAdvertisementId = null})
            .Fact(
                new AdvertisementFacts::AdvertisementTemplate { Id = 1, DummyAdvertisementId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AdvertisementElementFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(AdvertisementElementFacts))
            .Erm(
                new Erm::AdvertisementElement { Id = 1, IsDeleted = false },
                new Erm::AdvertisementElementStatus { Id = 1, Status = 1 },

                new Erm::AdvertisementElement { Id = 2, IsDeleted = false },

                new Erm::AdvertisementElement { Id = 3, IsDeleted = true },
                new Erm::AdvertisementElementStatus { Id = 3, Status = 3 })
            .Fact(
                new AdvertisementFacts::AdvertisementElement { Id = 1, IsEmpty = true, Status = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AdvertisementElementTemplateFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(AdvertisementElementTemplateFacts))
            .Erm(
                new Erm::AdvertisementElementTemplate { Id = 1, Name = "AdvertisementElementTemplate1", IsDeleted = false },
                new Erm::AdvertisementElementTemplate { Id = 2, IsDeleted = true })
            .Fact(
                new AdvertisementFacts::AdvertisementElementTemplate { Id = 1, Name = "AdvertisementElementTemplate1" });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PositionChildFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(PositionChildFacts))
            .Erm(
                new Erm::Position { Id = 1, Name = "Position", CategoryCode = 11 },
                new Erm::PositionChild {MasterPositionId = 1, ChildPositionId = 1 },

                new Erm::Position { Id = 2 })
            .Fact(
                new AdvertisementFacts::Position { Id = 1, Name = "Position", ChildPositionId = 1, CategoryCode = 11 },
                new AdvertisementFacts::Position { Id = 2, ChildPositionId = null },
                new ProjectFacts::Position { Id = 1, Name = "Position", CategoryCode = 11 },
                new ProjectFacts::Position { Id = 2 });

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
                    new Erm::FirmAddress { Id = 1, IsActive = true, IsDeleted = false, ClosedForAscertainment = false, Address = "1", IsLocatedOnTheMap = true },
                    new Erm::FirmAddress { Id = 2, IsActive = false, IsDeleted = true, ClosedForAscertainment = false, Address = "2" },
                    new Erm::FirmAddress { Id = 3, IsActive = false, IsDeleted = false, ClosedForAscertainment = true, Address = "3" })
                .Fact(
                    new ConsistencyFacts::FirmAddress { Id = 1, IsActive = true, IsDeleted = false, IsClosedForAscertainment = false, Name = "1" },
                    new ConsistencyFacts::FirmAddress { Id = 2, IsActive = false, IsDeleted = true, IsClosedForAscertainment = false, Name = "2" },
                    new ConsistencyFacts::FirmAddress { Id = 3, IsActive = false, IsDeleted = false, IsClosedForAscertainment = true, Name = "3" },

                    new ProjectFacts::FirmAddress { Id = 1, Name = "1", IsLocatedOnTheMap = true });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement BillFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(BillFacts))
            .Erm(
                new Erm::Bill { Id = 1, IsActive = true, IsDeleted = false, BeginDistributionDate = FirstDayJan, EndDistributionDate = LastSecondJan, BillType = 1, OrderId = 2, PayablePlan = 123 })
            .Fact(
                new ConsistencyFacts::Bill { Id = 1, Begin = FirstDayJan, End = LastSecondJan.AddSeconds(1), OrderId = 2, PayablePlan = 123 });

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
                new ConsistencyFacts::Category { Id = 5, IsActiveNotDeleted = false },

                new ProjectFacts::Category { Id = 1, Name = "1", Level = 1 },
                new ProjectFacts::Category { Id = 2, Name = "2", Level = 2 },
                new ProjectFacts::Category { Id = 3, Name = "3", Level = 3 });

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
                new ConsistencyFacts::Order { Id = 1, BeginDistribution = FirstDayJan, EndDistributionFact = LastSecondJan.AddSeconds(1), EndDistributionPlan = LastSecondMar.AddSeconds(1), DestOrganizationUnitId = 2, FirmId = 5, Number = "Number", CurrencyId = 9, ReleaseCountPlan = 3, WorkflowStep = 8, IsFreeOfCharge = true },
                new ProjectFacts::Order {Id = 1, Number = "Number", BeginDistribution = FirstDayJan, EndDistributionPlan = LastSecondMar.AddSeconds(1), DestOrganizationUnitId = 2, WorkflowStep = 8 });

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
                    new ConsistencyFacts::OrderPosition { Id = 1, OrderId = 1 },
                    new ProjectFacts::OrderPosition { Id = 1, OrderId = 1, PricePositionId = 2 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPositionAdvertisementFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(OrderPositionAdvertisementFacts))
                .Erm(
                    new Erm::OrderPositionAdvertisement { Id = 1, CategoryId = 1, FirmAddressId = 2, OrderPositionId = 3, PositionId = 4 })
                .Fact(
                    new PriceFacts::OrderPositionAdvertisement { Id = 1, CategoryId = 1, FirmAddressId = 2, OrderPositionId = 3, PositionId = 4 },
                    new ConsistencyFacts::OrderPositionAdvertisement { Id = 1, CategoryId = 1, FirmAddressId = 2, OrderPositionId = 3, PositionId = 4 },
                    new ProjectFacts::OrderPositionAdvertisement { Id = 1, CategoryId = 1, FirmAddressId = 2, OrderPositionId = 3, PositionId = 4 });

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
                    new Erm::PricePosition { Id = 4, IsActive = true, IsDeleted = false, PositionId = 1 })
                .Fact(
                    new PriceFacts::PricePositionNotActive { Id = 1 },
                    new PriceFacts::PricePositionNotActive { Id = 2 },
                    new PriceFacts::PricePositionNotActive { Id = 3 },
                    new PriceFacts::PricePosition { Id = 4, PositionId = 1 },
                    new ProjectFacts:: PricePosition {Id = 4, PositionId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ProjectFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(ProjectFacts))
                .Erm(
                    new Erm::Project { Id = 1, IsActive = true, OrganizationUnitId = 1, DisplayName = "Project" },
                    new Erm::Project { Id = 2, IsActive = false, OrganizationUnitId = 1 },
                    new Erm::Project { Id = 3, IsActive = true, OrganizationUnitId = null })
                .Fact(
                    new PriceFacts::Project { Id = 1, OrganizationUnitId = 1, Name = "Project" },
                    new ConsistencyFacts::Project { Id = 1, OrganizationUnitId = 1 },
                    new AccountFacts::Project { Id = 1, OrganizationUnitId = 1 },
                    new ProjectFacts::Project { Id = 1, Name = "Project", OrganizationUnitId = 1 });

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

        // ReSharper disable once UnusedMember.LocalFirmAddress
        private static ArrangeMetadataElement CategoryOrganizationUnitFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(CategoryOrganizationUnitFacts))
                .Erm(
                    new Erm::CategoryOrganizationUnit { Id = 1, IsActive = true, IsDeleted = true, CategoryId = 11, OrganizationUnitId = 3 },
                    new Erm::CategoryOrganizationUnit { Id = 2, IsActive = true, IsDeleted = false, CategoryId = 12, OrganizationUnitId = 3 },
                    new Erm::CategoryOrganizationUnit { Id = 3, IsActive = false, IsDeleted = true, CategoryId = 13, OrganizationUnitId = 3 },
                    new Erm::CategoryOrganizationUnit { Id = 4, IsActive = false, IsDeleted = false, CategoryId = 14, OrganizationUnitId = 3 })
                .Fact(
                    new ProjectFacts::CategoryOrganizationUnit { Id = 2, CategoryId = 12, OrganizationUnitId = 3 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement CostPerClickCategoryRestrictionFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(CostPerClickCategoryRestrictionFacts))
                .Erm(
                    new Erm::CostPerClickCategoryRestriction { ProjectId = 1, CategoryId = 2, BeginningDate = MonthStart(1), MinCostPerClick = 3 })
                .Fact(
                    new ProjectFacts::CostPerClickCategoryRestriction { ProjectId = 1, CategoryId = 2, Begin = MonthStart(1), MinCostPerClick = 3 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPositionCostPerClickFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(OrderPositionCostPerClickFacts))
                .Erm(
                    new Erm::OrderPositionCostPerClick { CategoryId = 1, OrderPositionId = 2, Amount = 1, BidIndex = 1 },
                    new Erm::OrderPositionCostPerClick { CategoryId = 1, OrderPositionId = 2, Amount = 2, BidIndex = 2 })
                .Fact(
                    new ProjectFacts::OrderPositionCostPerClick { CategoryId = 1, OrderPositionId = 2, Amount = 2 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReleaseInfoFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(ReleaseInfoFacts))
                .Erm(
                    new Erm::ReleaseInfo { Id = 1, OrganizationUnitId = 2, PeriodEndDate = LastSecondApr })
                .Fact(
                    new ProjectFacts::ReleaseInfo { Id = 1, OrganizationUnitId = 2, PeriodEndDate = LastSecondApr.AddSeconds(1) });
    }
}
