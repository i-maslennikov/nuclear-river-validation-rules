using System.Collections.Generic;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AdvertisementMustPassReview
            => ArrangeMetadataElement
                .Config
                .Name(nameof(AdvertisementMustPassReview))
                .Fact(
                    new Facts::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },

                    new Facts::OrderPosition { Id = 3, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 4, OrderPositionId = 3, AdvertisementId = 5 },
                    new Facts::Advertisement { Id = 5, StateCode = 0 },

                    new Facts::OrderPosition { Id = 6, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 7, OrderPositionId = 6, AdvertisementId = 8 },
                    new Facts::Advertisement { Id = 8, StateCode = 1 },

                    new Facts::OrderPosition { Id = 9, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 10, OrderPositionId = 9, AdvertisementId = 11 },
                    new Facts::Advertisement { Id = 11, StateCode = 3 }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, BeginDistributionDate = MonthStart(1), EndDistributionDatePlan = MonthStart(2) },
                    new Aggregates::Order.AdvertisementFailedReview { OrderId = 1, AdvertisementId = 8, ReviewState = 1 },
                    new Aggregates::Order.AdvertisementFailedReview { OrderId = 1, AdvertisementId = 11, ReviewState = 3 }
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

                        MessageType = (int)MessageTypeCode.AdvertisementMustPassReview,
                        PeriodStart = MonthStart(1),
                        PeriodEnd = MonthStart(2),
                        OrderId = 1,
                    },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                                  new Dictionary<string, object> { { "reviewState", 3 } },
                                                  new Reference<EntityTypeOrder>(1),
                                                  new Reference<EntityTypeAdvertisement>(11))
                                    .ToXDocument(),

                            MessageType = (int)MessageTypeCode.AdvertisementShouldNotHaveComments,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 1,
                        }

                );
    }
}
