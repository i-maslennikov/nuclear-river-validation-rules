using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;

using Aggregates = NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.ThemeRules.Facts;

using Messages = NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ThemeCategoryMustBeActiveAndNotDeleted_OneOrder
            => ArrangeMetadataElement
                .Config
                .Name(nameof(ThemeCategoryMustBeActiveAndNotDeleted_OneOrder))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDateFact = FirstDayFeb},
                    new Facts::Project {Id = 3, OrganizationUnitId = 2},

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, ThemeId = 5 },

                    new Facts::Theme { Id = 5, Name = "Theme5", BeginDistribution = FirstDayJan, EndDistribution = FirstDayFeb },
                    new Facts::Category { Id = 6, Name = "Category6", IsNotActiveOrDeleted = true },

                    new Facts::ThemeCategory { ThemeId = 5, CategoryId = 6 }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDateFact = FirstDayFeb },
                    new Aggregates::Order.OrderTheme { OrderId = 1, ThemeId = 5 },

                    new Aggregates::Theme { Id = 5, Name = "Theme5", BeginDistribution = FirstDayJan, EndDistribution = FirstDayFeb },
                    new Aggregates::Theme.InvalidCategory { ThemeId = 5, CategoryId = 6 },

                    new Aggregates::Category{ Id = 6, Name = "Category6" }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><theme id = \"5\" name=\"Theme5\" /><category id = \"6\" name=\"Category6\" /></root>"),
                        MessageType = (int)MessageTypeCode.ThemeCategoryMustBeActiveAndNotDeleted,
                        Result = 252,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        ProjectId = 3,
                    }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ThemeCategoryMustBeActiveAndNotDeleted_TwoOrders
            => ArrangeMetadataElement
                .Config
                .Name(nameof(ThemeCategoryMustBeActiveAndNotDeleted_TwoOrders))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDateFact = FirstDayMar },
                    new Facts::Order { Id = 2, DestOrganizationUnitId = 2, Number = "Order2", BeginDistributionDate = FirstDayFeb, EndDistributionDateFact = FirstDayApr },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPosition { Id = 5, OrderId = 2, },
                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 4, ThemeId = 5 },
                    new Facts::OrderPositionAdvertisement { Id = 2, OrderPositionId = 5, ThemeId = 5 },

                    new Facts::Theme { Id = 5, Name = "Theme5", BeginDistribution = FirstDayJan, EndDistribution = FirstDayApr },
                    new Facts::Category { Id = 6, Name = "Category6", IsNotActiveOrDeleted = true },

                    new Facts::ThemeCategory { ThemeId = 5, CategoryId = 6 }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDateFact = FirstDayMar },
                    new Aggregates::Order { Id = 2, ProjectId = 3, Number = "Order2", BeginDistributionDate = FirstDayFeb, EndDistributionDateFact = FirstDayApr },
                    new Aggregates::Order.OrderTheme { OrderId = 1, ThemeId = 5 },
                    new Aggregates::Order.OrderTheme { OrderId = 2, ThemeId = 5 },

                    new Aggregates::Theme { Id = 5, Name = "Theme5", BeginDistribution = FirstDayJan, EndDistribution = FirstDayApr },
                    new Aggregates::Theme.InvalidCategory { ThemeId = 5, CategoryId = 6 },

                    new Aggregates::Category { Id = 6, Name = "Category6" }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><theme id = \"5\" name=\"Theme5\" /><category id = \"6\" name=\"Category6\" /></root>"),
                        MessageType = (int)MessageTypeCode.ThemeCategoryMustBeActiveAndNotDeleted,
                        Result = 252,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayApr,
                        ProjectId = 3,
                    }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ThemeCategoryMustBeActiveAndNotDeletedNaegative
            => ArrangeMetadataElement
                .Config
                .Name(nameof(ThemeCategoryMustBeActiveAndNotDeletedNaegative))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDateFact = FirstDayFeb },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, ThemeId = 5 },

                    new Facts::Theme { Id = 5, Name = "Theme5", BeginDistribution = FirstDayJan, EndDistribution = FirstDayFeb },
                    new Facts::Category { Id = 6, Name = "Category6", IsNotActiveOrDeleted = false },

                    new Facts::ThemeCategory { ThemeId = 5, CategoryId = 6 }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDateFact = FirstDayFeb },
                    new Aggregates::Order.OrderTheme { OrderId = 1, ThemeId = 5 },

                    new Aggregates::Theme { Id = 5, Name = "Theme5", BeginDistribution = FirstDayJan, EndDistribution = FirstDayFeb },

                    new Aggregates::Category { Id = 6, Name = "Category6" }
                )
                .Message(
                );
    }
}
