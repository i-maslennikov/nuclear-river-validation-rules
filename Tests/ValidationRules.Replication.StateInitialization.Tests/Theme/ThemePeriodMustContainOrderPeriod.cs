using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

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
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayFeb},
                    new Facts::Project {Id = 3, OrganizationUnitId = 2},

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, ThemeId = 5 },

                    new Facts::Theme { Id = 5, BeginDistribution = FirstDayFeb, EndDistribution = FirstDayFeb }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDateFact = FirstDayFeb },
                    new Aggregates::Order.OrderTheme { OrderId = 1, ThemeId = 5 },

                    new Aggregates::Theme { Id = 5, BeginDistribution = FirstDayFeb, EndDistribution = FirstDayFeb }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(
                                new Reference<EntityTypeOrder>(1),
                                new Reference<EntityTypeTheme>(5)).ToXDocument(),
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
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayFeb },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, ThemeId = 5 },

                    new Facts::Theme { Id = 5, BeginDistribution = FirstDayJan, EndDistribution = FirstDayFeb }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDateFact = FirstDayFeb },
                    new Aggregates::Order.OrderTheme { OrderId = 1, ThemeId = 5 },

                    new Aggregates::Theme { Id = 5, BeginDistribution = FirstDayJan, EndDistribution = FirstDayFeb }
                )
                .Message(
                );
    }
}
