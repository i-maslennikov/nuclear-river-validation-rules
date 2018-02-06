using System.Collections.Generic;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

// ReSharper disable once CheckNamespace
namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OptionalAdvertisementMustPassReview
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OptionalAdvertisementMustPassReview))
                .Fact(
                    new Facts::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Facts::Position { Id = 100, ContentSales = 2 },

                    new Facts::OrderPosition { Id = 6, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 7, OrderPositionId = 6, AdvertisementId = 8, PositionId = 100  },
                    new Facts::Advertisement { Id = 8, StateCode = 1 }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, BeginDistributionDate = MonthStart(1), EndDistributionDatePlan = MonthStart(2) },
                    new Aggregates::Order.AdvertisementFailedReview { OrderId = 1, AdvertisementId = 8, ReviewState = 1, AdvertisementIsOptional = true }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object> { { "reviewState", 1 } },
                                    new Reference<EntityTypeOrder>(1),
                                    new Reference<EntityTypeAdvertisement>(8))
                                .ToXDocument(),

                        MessageType = (int)MessageTypeCode.OptionalAdvertisementMustPassReview,
                        PeriodStart = MonthStart(1),
                        PeriodEnd = MonthStart(2),
                        OrderId = 1
                    }
                );
    }
}