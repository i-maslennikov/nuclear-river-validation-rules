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
        private static ArrangeMetadataElement ThemeCategoryMustBeActiveAndNotDeleted_OneOrder
            => ArrangeMetadataElement
                .Config
                .Name(nameof(ThemeCategoryMustBeActiveAndNotDeleted_OneOrder))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayFeb},
                    new Facts::Project {Id = 3, OrganizationUnitId = 2},

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, ThemeId = 5 },

                    new Facts::Theme { Id = 5, BeginDistribution = FirstDayJan, EndDistribution = FirstDayFeb },
                    new Facts::Category { Id = 6, IsActiveNotDeleted = false },

                    new Facts::ThemeCategory { ThemeId = 5, CategoryId = 6 }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDateFact = FirstDayFeb },
                    new Aggregates::Order.OrderTheme { OrderId = 1, ThemeId = 5 },

                    new Aggregates::Theme { Id = 5, BeginDistribution = FirstDayJan, EndDistribution = FirstDayFeb },
                    new Aggregates::Theme.InvalidCategory { ThemeId = 5, CategoryId = 6 }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(
                                new Reference<EntityTypeTheme>(5),
                                new Reference<EntityTypeCategory>(6)).ToXDocument(),
                        MessageType = (int)MessageTypeCode.ThemeCategoryMustBeActiveAndNotDeleted,
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
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayMar },
                    new Facts::Order { Id = 2, DestOrganizationUnitId = 2, BeginDistribution = FirstDayFeb, EndDistributionFact = FirstDayApr },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPosition { Id = 5, OrderId = 2, },
                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 4, ThemeId = 5 },
                    new Facts::OrderPositionAdvertisement { Id = 2, OrderPositionId = 5, ThemeId = 5 },

                    new Facts::Theme { Id = 5, BeginDistribution = FirstDayJan, EndDistribution = FirstDayApr },
                    new Facts::Category { Id = 6, IsActiveNotDeleted = false },

                    new Facts::ThemeCategory { ThemeId = 5, CategoryId = 6 }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDateFact = FirstDayMar },
                    new Aggregates::Order { Id = 2, ProjectId = 3, BeginDistributionDate = FirstDayFeb, EndDistributionDateFact = FirstDayApr },
                    new Aggregates::Order.OrderTheme { OrderId = 1, ThemeId = 5 },
                    new Aggregates::Order.OrderTheme { OrderId = 2, ThemeId = 5 },

                    new Aggregates::Theme { Id = 5, BeginDistribution = FirstDayJan, EndDistribution = FirstDayApr },
                    new Aggregates::Theme.InvalidCategory { ThemeId = 5, CategoryId = 6 }
                )
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                new Reference<EntityTypeTheme>(5),
                                new Reference<EntityTypeCategory>(6)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.ThemeCategoryMustBeActiveAndNotDeleted,
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
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayFeb },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, ThemeId = 5 },

                    new Facts::Theme { Id = 5, BeginDistribution = FirstDayJan, EndDistribution = FirstDayFeb },
                    new Facts::Category { Id = 6, IsActiveNotDeleted = true },

                    new Facts::ThemeCategory { ThemeId = 5, CategoryId = 6 }
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
