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
        private static ArrangeMetadataElement AdvertisementWebsiteShouldNotBeFirmWebsitePositive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(AdvertisementWebsiteShouldNotBeFirmWebsitePositive))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionPlan = FirstDayFeb, FirmId = 1},
                    new Facts::Project {Id = 3, OrganizationUnitId = 2},

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Position { Id = 5 },

                    new Facts::Advertisement { Id = 6, AdvertisementTemplateId =  9, FirmId = 1 },
                    new Facts::AdvertisementTemplate { Id = 9, DummyAdvertisementId = -6 },

                    new Facts::AdvertisementElement { Id = 7, AdvertisementId = 6, AdvertisementElementTemplateId = 8, Text = "http://localhost/path" },
                    new Facts::AdvertisementElementTemplate { Id = 8, IsAdvertisementLink = true },

                    new Facts::Firm { Id = 1, IsActive = true },
                    new Facts::FirmAddress { Id = 1, FirmId = 1, IsActive = true },
                    new Facts::FirmAddressWebsite { Id = 2, FirmAddressId = 1, Website = "http://localhost"}
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb, FirmId = 1},
                    new Aggregates::Order.OrderPositionAdvertisement { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Aggregates::Advertisement { Id = 6, FirmId = 1 },
                    new Aggregates::Advertisement.AdvertisementWebsite { AdvertisementId = 6, Website = "http://localhost/path" },

                    new Aggregates::Firm { Id = 1 },
                    new Aggregates::Firm.FirmWebsite { FirmId = 1, Website = "http://localhost" }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(
                                new Dictionary<string, object> { {"website", "http://localhost/path"} },
                                new Reference<EntityTypeOrder>(1),
                                new Reference<EntityTypeFirm>(1),
                                new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                    new Reference<EntityTypeOrderPosition>(4),
                                    new Reference<EntityTypePosition>(5))).ToXDocument(),
                        MessageType = (int)MessageTypeCode.AdvertisementWebsiteShouldNotBeFirmWebsite,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        OrderId = 1,
                    }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AdvertisementWebsiteShouldNotBeFirmWebsiteFirmNotActive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(AdvertisementWebsiteShouldNotBeFirmWebsiteFirmNotActive))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionPlan = FirstDayFeb, FirmId = 1 },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Position { Id = 5 },

                    new Facts::Advertisement { Id = 6, AdvertisementTemplateId = 9, FirmId = 1 },
                    new Facts::AdvertisementTemplate { Id = 9, DummyAdvertisementId = -6 },

                    new Facts::AdvertisementElement { Id = 7, AdvertisementId = 6, AdvertisementElementTemplateId = 8, Text = "http://localhost/path" },
                    new Facts::AdvertisementElementTemplate { Id = 8, IsAdvertisementLink = true },

                    // firm not active
                    new Facts::Firm { Id = 1, IsActive = false },
                    new Facts::FirmAddress { Id = 1, FirmId = 1, IsActive = false },
                    new Facts::FirmAddressWebsite { Id = 2, FirmAddressId = 1, Website = "http://localhost" }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb, FirmId = 1 },
                    new Aggregates::Order.OrderPositionAdvertisement { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Aggregates::Advertisement { Id = 6, FirmId = 1 },
                    new Aggregates::Advertisement.AdvertisementWebsite { AdvertisementId = 6, Website = "http://localhost/path" },

                    new Aggregates::Firm { Id = 1 }
                )
                .Message();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AdvertisementWebsiteShouldNotBeFirmWebsiteNegative
            => ArrangeMetadataElement
                .Config
                .Name(nameof(AdvertisementWebsiteShouldNotBeFirmWebsiteNegative))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionPlan = FirstDayFeb, FirmId = 1 },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Position { Id = 5 },

                    new Facts::Advertisement { Id = 6, AdvertisementTemplateId = 9, FirmId = 1 },
                    new Facts::AdvertisementTemplate { Id = 9, DummyAdvertisementId = -6 },

                    new Facts::AdvertisementElement { Id = 7, AdvertisementId = 6, AdvertisementElementTemplateId = 8, Text = "http://localhost/path" },
                    new Facts::AdvertisementElementTemplate { Id = 8, IsAdvertisementLink = true },

                    new Facts::Firm { Id = 1, IsActive = true },
                    new Facts::FirmAddress { Id = 1, FirmId = 1, IsActive = true },
                    // localhost1 != localhost
                    new Facts::FirmAddressWebsite { Id = 2, FirmAddressId = 1, Website = "http://localhost1" }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb, FirmId = 1 },
                    new Aggregates::Order.OrderPositionAdvertisement { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Aggregates::Advertisement { Id = 6, FirmId = 1 },
                    new Aggregates::Advertisement.AdvertisementWebsite { AdvertisementId = 6, Website = "http://localhost/path" },

                    new Aggregates::Firm { Id = 1 },
                    new Aggregates::Firm.FirmWebsite { FirmId = 1, Website = "http://localhost1" }
                )
                .Message(
                );
    }
}
