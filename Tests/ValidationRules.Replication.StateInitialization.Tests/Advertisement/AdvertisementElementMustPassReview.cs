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
        private const int StatusValid = 1;
        private const int StatusInvalid = 2;

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AdvertisementElementMustPassReviewPositive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(AdvertisementElementMustPassReviewPositive))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionPlan = FirstDayFeb },
                    new Facts::Project {Id = 3, OrganizationUnitId = 2},

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Advertisement { Id = 6, FirmId = 0, AdvertisementTemplateId = 9, IsDeleted = false },
                    new Facts::AdvertisementTemplate { Id = 9, DummyAdvertisementId = -6 },

                    new Facts::AdvertisementElement { Id = 7, AdvertisementId = 6, AdvertisementElementTemplateId = 8, IsEmpty = true, Status = StatusInvalid }, // ЭРМ не выверен
                    new Facts::AdvertisementElementTemplate { Id = 8, NeedsValidation = true } // ЭРМ должен быть выверен
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },
                    new Aggregates::Order.OrderPositionAdvertisement { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Aggregates::Advertisement { Id = 6, },
                    new Aggregates::Advertisement.ElementNotPassedReview { AdvertisementId = 6, AdvertisementElementId = 7, AdvertisementElementTemplateId = 8, Status = Aggregates::Advertisement.ReviewStatus.Invalid }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(
                                new Dictionary<string, object> { { "advertisementElementStatus", "1"} },
                                new Reference<EntityTypeOrder>(1),
                                new Reference<EntityTypeAdvertisement>(6),
                                new Reference<EntityTypeAdvertisementElement>(7,
                                    new Reference<EntityTypeAdvertisementElementTemplate>(8))).ToXDocument(),
                        MessageType = (int)MessageTypeCode.AdvertisementElementMustPassReview,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        OrderId = 1,
                    }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AdvertisementElementMustPassReviewNegative
            => ArrangeMetadataElement
                .Config
                .Name(nameof(AdvertisementElementMustPassReviewNegative))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionPlan = FirstDayFeb },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Advertisement { Id = 6, FirmId = 0, AdvertisementTemplateId = 9, IsDeleted = false },
                    new Facts::AdvertisementTemplate { Id = 9, DummyAdvertisementId = -6 },

                    new Facts::AdvertisementElement { Id = 7, AdvertisementId = 6, AdvertisementElementTemplateId = 8, IsEmpty = true, Status = StatusValid }, // ЭРМ выверен
                    new Facts::AdvertisementElementTemplate { Id = 8, NeedsValidation = true } // ЭРМ должен быть выверен
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },
                    new Aggregates::Order.OrderPositionAdvertisement { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Aggregates::Advertisement { Id = 6 }
                )
                .Message(
                );
    }
}
