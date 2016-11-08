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
        private static ArrangeMetadataElement ThemePeriodMustContainOrderPeriodPositive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(ThemePeriodMustContainOrderPeriodPositive))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDateFact = FirstDayFeb},
                    new Facts::Project {Id = 3, OrganizationUnitId = 2},

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, ThemeId = 5 },

                    new Facts::Theme { Id = 5, Name = "Theme5", BeginDistribution = FirstDayFeb, EndDistribution = FirstDayFeb }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDateFact = FirstDayFeb },
                    new Aggregates::Order.OrderTheme { OrderId = 1, ThemeId = 5 },

                    new Aggregates::Theme { Id = 5, Name = "Theme5", BeginDistribution = FirstDayFeb, EndDistribution = FirstDayFeb }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><order id = \"1\" number=\"Order1\" /><theme id = \"5\" name=\"Theme5\" /></root>"),
                        MessageType = (int)MessageTypeCode.ThemePeriodMustContainOrderPeriod,
                        Result = 255,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        OrderId = 1,
                    }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ThemePeriodMustContainOrderPeriodNegative
            => ArrangeMetadataElement
                .Config
                .Name(nameof(ThemePeriodMustContainOrderPeriodNegative))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDateFact = FirstDayFeb },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, ThemeId = 5 },

                    new Facts::Theme { Id = 5, Name = "Theme5", BeginDistribution = FirstDayJan, EndDistribution = FirstDayFeb }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDateFact = FirstDayFeb },
                    new Aggregates::Order.OrderTheme { OrderId = 1, ThemeId = 5 },

                    new Aggregates::Theme { Id = 5, Name = "Theme5", BeginDistribution = FirstDayJan, EndDistribution = FirstDayFeb }
                )
                .Message(
                );
    }
}
