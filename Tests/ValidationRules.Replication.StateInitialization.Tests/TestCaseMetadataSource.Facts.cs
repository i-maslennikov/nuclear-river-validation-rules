using System;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        private static readonly DateTime FirstDayDec = DateTime.Parse("2011-12-01");
        private static readonly DateTime FirstDayJan = DateTime.Parse("2012-01-01");
        private static readonly DateTime FirstDayFeb = DateTime.Parse("2012-02-01");
        private static readonly DateTime FirstDayMar = DateTime.Parse("2012-03-01");
        private static readonly DateTime FirstDayApr = DateTime.Parse("2012-04-01");
        private static readonly DateTime FirstDayMay = DateTime.Parse("2012-05-01");
        private static readonly DateTime LastSecondJan = DateTime.Parse("2012-01-31T23:59:59");
        private static readonly DateTime LastSecondMar = DateTime.Parse("2012-03-31T23:59:59");
        private static readonly DateTime LastSecondApr = DateTime.Parse("2012-04-30T23:59:59");

        private static DateTime MonthStart(int i) => DateTime.Parse("2012-01-01").AddMonths(i - 1);

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement UnlimitedOrder
        => ArrangeMetadataElement.Config
            .Name(nameof(UnlimitedOrder))
            .Erm(
                new Erm::UnlimitedOrder { OrderId = 1, IsActive = true, PeriodStart = MonthStart(1), PeriodEnd = MonthStart(2).AddSeconds(-1) },
                new Erm::UnlimitedOrder { OrderId = 2, IsActive = false, PeriodStart = MonthStart(1), PeriodEnd = MonthStart(2).AddSeconds(-1) })
            .Fact(
                new UnlimitedOrder { OrderId = 1, PeriodStart = MonthStart(1), PeriodEnd = MonthStart(2) });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmContactsFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(FirmContactsFacts))
            .Erm(
                new Erm::FirmContact { Id = 1, FirmAddressId = 1, ContactType = 4, Contact = "http://localhost"},
                new Erm::FirmContact { Id = 2, FirmAddressId = null, ContactType = 4, Contact = "http://localhost" },
                new Erm::FirmContact { Id = 3, FirmAddressId = 1, ContactType = 3, Contact = "http://localhost" })
            .Fact(
                new FirmAddressWebsite { Id = 1, FirmAddressId = 1, Website = "http://localhost" });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ThemeOrganizationUnitFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(ThemeOrganizationUnitFacts))
            .Erm(
                new Erm::ThemeOrganizationUnit { Id = 1, ThemeId = 2, OrganizationUnitId = 3, IsActive = true, IsDeleted = false },
                new Erm::ThemeOrganizationUnit { Id = 2, ThemeId = 2, OrganizationUnitId = 3, IsActive = false, IsDeleted = false },
                new Erm::ThemeOrganizationUnit { Id = 3, ThemeId = 2, OrganizationUnitId = 3, IsActive = true, IsDeleted = true })
            .Fact(
                new ThemeOrganizationUnit { Id = 1, ThemeId = 2, OrganizationUnitId = 3 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ThemeCategoryFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(ThemeCategoryFacts))
            .Erm(
                new Erm::ThemeCategory { Id = 1, ThemeId = 2, CategoryId = 3, IsDeleted = false },
                new Erm::ThemeCategory { Id = 2, ThemeId = 2, CategoryId = 3, IsDeleted = true })
            .Fact(
                new ThemeCategory { Id = 1, ThemeId = 2, CategoryId = 3 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement NomenclatureCategoryFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(NomenclatureCategoryFacts))
            .Erm(
                new Erm::Position { Id = 1, CategoryCode = 1, Name = "one", IsControlledByAmount = true },
                new Erm::Position { Id = 2, CategoryCode = 1, Name = "two", IsControlledByAmount = true },
                new Erm::Position { Id = 3, CategoryCode = 1, Name = "three", IsControlledByAmount = true },
                new Erm::Position { Id = 4, CategoryCode = 1, Name = "four", IsControlledByAmount = true },

                new Erm::PricePosition { Id = 1, PriceId = 1, IsActive = true, PositionId = 1 },
                new Erm::PricePosition { Id = 2, PriceId = 1, IsActive = true, PositionId = 2 },
                new Erm::PricePosition { Id = 3, PriceId = 1, IsActive = true, PositionId = 3 },

                new Erm::PricePosition { Id = 4, PriceId = 2, IsActive = true, PositionId = 2 },
                new Erm::PricePosition { Id = 5, PriceId = 2, IsActive = true, PositionId = 3 },
                new Erm::PricePosition { Id = 6, PriceId = 2, IsActive = true, PositionId = 4 })
            .Fact(
                new NomenclatureCategory { Id = 1, Name = "one", PriceId = 1 },
                new NomenclatureCategory { Id = 1, Name = "two", PriceId = 2 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement SalesModelCategoryRestrictions
        => ArrangeMetadataElement.Config
            .Name(nameof(SalesModelCategoryRestrictions))
            .Erm(
                new Erm::SalesModelCategoryRestriction { ProjectId = 1, BeginningDate = MonthStart(1), CategoryId = 1, SalesModel = 2 })
            .Fact(
                new SalesModelCategoryRestriction { ProjectId = 1, Begin = MonthStart(1), CategoryId = 1, SalesModel = 2 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AdvertisementFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(AdvertisementFacts))
            .Erm(
                new Erm::Advertisement { Id = 1, IsDeleted = false },
                new Erm::Advertisement { Id = 2, IsDeleted = true })
            .Fact(
                new Advertisement { Id = 1, IsDeleted = false },
                new Advertisement { Id = 2, IsDeleted = true });

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
                new AdvertisementTemplate { Id = 1, DummyAdvertisementId = 1 });

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
                new AdvertisementElement { Id = 1, IsEmpty = true, Status = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AdvertisementElementTemplateFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(AdvertisementElementTemplateFacts))
            .Erm(
                new Erm::AdvertisementElementTemplate { Id = 1, IsDeleted = false },
                new Erm::AdvertisementElementTemplate { Id = 2, IsDeleted = true })
            .Fact(
                new AdvertisementElementTemplate { Id = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PositionChildFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(PositionChildFacts))
            .Erm(
                new Erm::PositionChild {MasterPositionId = 1, ChildPositionId = 1 })
            .Fact(
                new PositionChild { MasterPositionId = 1, ChildPositionId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(FirmFacts))
            .Erm(
                new Erm::Firm { Id = 1, IsActive = true, IsDeleted = false, ClosedForAscertainment = false },
                new Erm::Firm { Id = 2, IsActive = false, IsDeleted = true, ClosedForAscertainment = false },
                new Erm::Firm { Id = 3, IsActive = false, IsDeleted = false, ClosedForAscertainment = true })
            .Fact(
                new Firm { Id = 1, IsActive = true, IsDeleted = false, IsClosedForAscertainment = false },
                new Firm { Id = 2, IsActive = false, IsDeleted = true, IsClosedForAscertainment = false },
                new Firm { Id = 3, IsActive = false, IsDeleted = false, IsClosedForAscertainment = true });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmAddressFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(FirmAddressFacts))
                .Erm(
                    new Erm::FirmAddress { Id = 1, IsActive = true, IsDeleted = false, ClosedForAscertainment = false, IsLocatedOnTheMap = true },
                    new Erm::FirmAddress { Id = 2, IsActive = false, IsDeleted = true, ClosedForAscertainment = false },
                    new Erm::FirmAddress { Id = 3, IsActive = false, IsDeleted = false, ClosedForAscertainment = true })
                .Fact(
                    new FirmAddress { Id = 1, IsActive = true, IsDeleted = false, IsClosedForAscertainment = false, IsLocatedOnTheMap = true },
                    new FirmAddress { Id = 2, IsActive = false, IsDeleted = true, IsClosedForAscertainment = false },
                    new FirmAddress { Id = 3, IsActive = false, IsDeleted = false, IsClosedForAscertainment = true });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement BillFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(BillFacts))
            .Erm(
                new Erm::Bill { Id = 1, IsActive = true, IsDeleted = false, BeginDistributionDate = FirstDayJan, EndDistributionDate = LastSecondJan, BillType = 1, OrderId = 2, PayablePlan = 123 })
            .Fact(
                new Bill { Id = 1, Begin = FirstDayJan, End = LastSecondJan.AddSeconds(1), OrderId = 2, PayablePlan = 123 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement BargainFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(BargainFacts))
            .Erm(
                new Erm::Bargain { Id = 1, IsActive = true, IsDeleted = false })
            .Fact(
                new Bargain { Id = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement CategoryFirmAddressFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(CategoryFirmAddressFacts))
            .Erm(
                new Erm::CategoryFirmAddress { Id = 1, IsActive = true, IsDeleted = false })
            .Fact(
                new FirmAddressCategory { Id = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderFileFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(OrderFileFacts))
            .Erm(
                new Erm::OrderFile { Id = 1, OrderId = 1, IsActive = true, IsDeleted = false, FileKind = 8 })
            .Fact(
                new OrderScanFile { Id = 1, OrderId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement BargainFileFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(BargainFileFacts))
            .Erm(
                new Erm::BargainFile { Id = 1, IsActive = true, IsDeleted = false, BargainId = 1, FileKind = 12 })
            .Fact(
                new BargainScanFile { Id = 1, BargainId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement LegalPersonProfileFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(LegalPersonProfileFacts))
            .Erm(
                new Erm::LegalPersonProfile { Id = 1, IsActive = true, IsDeleted = false, BargainEndDate = FirstDayJan, WarrantyEndDate = FirstDayFeb, LegalPersonId = 1 },
                new Erm::LegalPersonProfile { Id = 2, IsActive = false, IsDeleted = false },
                new Erm::LegalPersonProfile { Id = 3, IsActive = true, IsDeleted = true },
                new Erm::LegalPersonProfile { Id = 4, IsActive = false, IsDeleted = true })
            .Fact(
                new LegalPersonProfile { Id = 1, BargainEndDate = FirstDayJan, WarrantyEndDate = FirstDayFeb, LegalPersonId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AssociatedPositionFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(AssociatedPositionFacts))
            .Erm(
                new Erm::AssociatedPosition { Id = 1, IsActive = true, AssociatedPositionsGroupId = 1, ObjectBindingType = 2, PositionId = 3 },
                new Erm::AssociatedPosition { Id = 2, IsActive = false, IsDeleted = false },
                new Erm::AssociatedPosition { Id = 3, IsActive = true, IsDeleted = true })
            .Fact(
                new AssociatedPosition { Id = 1, AssociatedPositionsGroupId = 1, ObjectBindingType = 2, PositionId = 3 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AssociatedPositionsGroupFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(AssociatedPositionsGroupFacts))
            .Erm(
                new Erm::AssociatedPositionsGroup { Id = 1, IsActive = true, PricePositionId = 1 },
                new Erm::AssociatedPositionsGroup { Id = 2, IsActive = false, IsDeleted = false },
                new Erm::AssociatedPositionsGroup { Id = 3, IsActive = true, IsDeleted = true })
            .Fact(
                new AssociatedPositionsGroup { Id = 1, PricePositionId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement CategoryFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(CategoryFacts))
            .Erm(
                new Erm::Category { Id = 1, IsActive = true, IsDeleted = false, Level = 1 },
                new Erm::Category { Id = 2, IsActive = true, IsDeleted = false, Level = 2, ParentId = 1 },
                new Erm::Category { Id = 3, IsActive = true, IsDeleted = false, Level = 3, ParentId = 2 },
                new Erm::Category { Id = 4, IsActive = false, IsDeleted = false },
                new Erm::Category { Id = 5, IsActive = true, IsDeleted = true })
            .Fact(

                new Category { Id = 1, IsActiveNotDeleted = true, L1Id = 1 },
                new Category { Id = 2, IsActiveNotDeleted = true, L1Id = 1, L2Id = 2 },
                new Category { Id = 3, IsActiveNotDeleted = true, L1Id = 1, L2Id = 2, L3Id = 3 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement DeniedPositionFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(DeniedPositionFacts))
            .Erm(
                new Erm::DeniedPosition { Id = 1, IsActive = true, ObjectBindingType = 1, PositionDeniedId = 2, PositionId = 3, PriceId = 4 },
                new Erm::DeniedPosition { Id = 2, IsActive = false, IsDeleted = false },
                new Erm::DeniedPosition { Id = 3, IsActive = true, IsDeleted = true })
            .Fact(
                new DeniedPosition { Id = 1, ObjectBindingType = 1, PositionDeniedId = 2, PositionId = 3, PriceId = 4 });

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

                new Erm::Ruleset { Id = 4, Priority = 0 },
                new Erm::RulesetRule { RulesetId = 4 })
            .Fact(
                new RulesetRule { DependentPositionId = 2, ObjectBindingType = 3, PrincipalPositionId = 4 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderFacts
        => ArrangeMetadataElement.Config
            .Name(nameof(OrderFacts))
            .Erm(
                new Erm::Order { Id = 1, IsActive = true, BeginDistributionDate = FirstDayJan, EndDistributionDateFact = LastSecondJan, EndDistributionDatePlan = LastSecondMar, DestOrganizationUnitId = 2, FirmId = 5, OwnerCode = 6, WorkflowStepId = 8, CurrencyId = 9, ReleaseCountPlan = 3, OrderType = 2 },
                new Erm::Order { Id = 2, IsActive = false, IsDeleted = false },
                new Erm::Order { Id = 3, IsActive = true, IsDeleted = true })
            .Fact(
                new Order { Id = 1, BeginDistribution = FirstDayJan, EndDistributionFact = LastSecondJan.AddSeconds(1), EndDistributionPlan = LastSecondMar.AddSeconds(1), DestOrganizationUnitId = 2, FirmId = 5, WorkflowStep = 8, CurrencyId = 9, ReleaseCountPlan = 3, IsFreeOfCharge = true, IsSelfAds = true });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPositionFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(OrderPositionFacts))
                .Erm(
                    new Erm::OrderPosition { Id = 1, IsActive = true, OrderId = 1, PricePositionId = 2 },
                    new Erm::OrderPosition { Id = 2, IsActive = false, IsDeleted = false },
                    new Erm::OrderPosition { Id = 3, IsActive = true, IsDeleted = true })
                .Fact(
                    new OrderPosition { Id = 1, OrderId = 1, PricePositionId = 2 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPositionAdvertisementFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(OrderPositionAdvertisementFacts))
                .Erm(
                    new Erm::OrderPositionAdvertisement { Id = 1, CategoryId = 1, FirmAddressId = 2, OrderPositionId = 3, PositionId = 4 })
                .Fact(
                    new OrderPositionAdvertisement { Id = 1, CategoryId = 1, FirmAddressId = 2, OrderPositionId = 3, PositionId = 4 });

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

                new Erm::Position { Id = 999, BindingObjectTypeEnum = 999, CategoryCode = 1, IsComposite = true, IsControlledByAmount = true },

                new Erm::Position { Id = 1000, IsDeleted = true })
            .Fact(
                new Position { Id = 33, BindingObjectType = 33 },
                new Position { Id = 34, BindingObjectType = 34 },
                new Position { Id = 1, BindingObjectType = 1 },

                new Position { Id = 6, BindingObjectType = 6 },
                new Position { Id = 35, BindingObjectType = 35 },

                new Position { Id = 7, BindingObjectType = 7 },
                new Position { Id = 8, BindingObjectType = 8 },

                new Position { Id = 36, BindingObjectType = 36 },
                new Position { Id = 37, BindingObjectType = 37 },

                new Position { Id = 999, BindingObjectType = 999, CategoryCode = 1, IsComposite = true, IsControlledByAmount = true },
                new Position { Id = 1000, IsDeleted = true });

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
                    new PricePosition { Id = 1, IsActiveNotDeleted = false },
                    new PricePosition { Id = 2, IsActiveNotDeleted = false },
                    new PricePosition { Id = 3, IsActiveNotDeleted = false },
                    new PricePosition { Id = 4, PositionId = 1, IsActiveNotDeleted = true });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ProjectFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(ProjectFacts))
                .Erm(
                    new Erm::Project { Id = 1, IsActive = true, OrganizationUnitId = 1 },
                    new Erm::Project { Id = 2, IsActive = false, OrganizationUnitId = 1 },
                    new Erm::Project { Id = 3, IsActive = true, OrganizationUnitId = null })
                .Fact(
                    new Project { Id = 1, OrganizationUnitId = 1 });

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
                    new Price { Id = 1, OrganizationUnitId = 1 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement CategoryOrganizationUnitFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(CategoryOrganizationUnitFacts))
                .Erm(
                    new Erm::CategoryOrganizationUnit { Id = 1, IsActive = true, IsDeleted = true, CategoryId = 11, OrganizationUnitId = 3 },
                    new Erm::CategoryOrganizationUnit { Id = 2, IsActive = true, IsDeleted = false, CategoryId = 12, OrganizationUnitId = 3 },
                    new Erm::CategoryOrganizationUnit { Id = 3, IsActive = false, IsDeleted = true, CategoryId = 13, OrganizationUnitId = 3 },
                    new Erm::CategoryOrganizationUnit { Id = 4, IsActive = false, IsDeleted = false, CategoryId = 14, OrganizationUnitId = 3 })
                .Fact(
                    new CategoryOrganizationUnit { Id = 2, CategoryId = 12, OrganizationUnitId = 3 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement CostPerClickCategoryRestrictionFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(CostPerClickCategoryRestrictionFacts))
                .Erm(
                    new Erm::CostPerClickCategoryRestriction { ProjectId = 1, CategoryId = 2, BeginningDate = MonthStart(1), MinCostPerClick = 3 })
                .Fact(
                    new CostPerClickCategoryRestriction { ProjectId = 1, CategoryId = 2, Begin = MonthStart(1), MinCostPerClick = 3 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPositionCostPerClickFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(OrderPositionCostPerClickFacts))
                .Erm(
                    new Erm::OrderPositionCostPerClick { CategoryId = 1, OrderPositionId = 2, Amount = 1, BidIndex = 1 },
                    new Erm::OrderPositionCostPerClick { CategoryId = 1, OrderPositionId = 2, Amount = 2, BidIndex = 2 })
                .Fact(
                    new OrderPositionCostPerClick { CategoryId = 1, OrderPositionId = 2, Amount = 2 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ReleaseInfoFacts
            => ArrangeMetadataElement.Config
                .Name(nameof(ReleaseInfoFacts))
                .Erm(
                    new Erm::ReleaseInfo { Id = 1, OrganizationUnitId = 2, PeriodEndDate = LastSecondApr, IsActive = true, IsDeleted = false, IsBeta = false, Status = 1 },
                    new Erm::ReleaseInfo { Id = 2, OrganizationUnitId = 2, PeriodEndDate = LastSecondApr, IsActive = true, IsDeleted = false, IsBeta = false, Status = 2 })
                .Fact(
                    new ReleaseInfo { Id = 2, OrganizationUnitId = 2, PeriodEndDate = FirstDayMay });
    }
}
