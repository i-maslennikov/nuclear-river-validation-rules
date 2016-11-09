using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;

using Aggregates = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;
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
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb, FirmId = 1},
                    new Facts::Project {Id = 3, OrganizationUnitId = 2},

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Position { Id = 5, Name = "Position5" },

                    new Facts::Advertisement { Id = 6, Name = "Advertisement6", AdvertisementTemplateId =  9, FirmId = 1 },
                    new Facts::AdvertisementTemplate { Id = 9, DummyAdvertisementId = -6 },

                    new Facts::AdvertisementElement { Id = 7, AdvertisementId = 6, AdvertisementElementTemplateId = 8, Text = "http://localhost" },
                    new Facts::AdvertisementElementTemplate { Id = 8, Name = "AdvertisementElementTemplate8", IsAdvertisementLink = true },

                    new Facts::Firm { Id = 1, Name = "Firm1" },
                    new Facts::FirmAddress { Id = 1, FirmId = 1 },
                    new Facts::FirmAddressWebsite { Id = 2, FirmAddressId = 1, Website = "http://localhost"}
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb, FirmId = 1},
                    new Aggregates::Order.OrderPositionAdvertisement { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Aggregates::Advertisement { Id = 6, Name = "Advertisement6", FirmId = 1 },
                    new Aggregates::Advertisement.AdvertisementWebsite { AdvertisementId = 6, Website = "http://localhost" },

                    new Aggregates::Position { Id = 5, Name = "Position5" },
                    new Aggregates::AdvertisementElementTemplate { Id = 8, Name = "AdvertisementElementTemplate8" },

                    new Aggregates::Firm { Id = 1, Name = "Firm1" },
                    new Aggregates::Firm.FirmWebsite { FirmId = 1, Website = "http://localhost" }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><order id = \"1\" name=\"Order1\" /><firm id=\"1\" name=\"Firm1\" /><orderPosition id=\"4\" name=\"Position5\" /><message website=\"http://localhost\" /></root>"),
                        MessageType = (int)MessageTypeCode.AdvertisementWebsiteShouldNotBeFirmWebsite,
                        Result = 2,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        OrderId = 1,
                    }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AdvertisementWebsiteShouldNotBeFirmWebsiteNegative
            => ArrangeMetadataElement
                .Config
                .Name(nameof(AdvertisementWebsiteShouldNotBeFirmWebsiteNegative))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb, FirmId = 1 },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Position { Id = 5, Name = "Position5" },

                    new Facts::Advertisement { Id = 6, Name = "Advertisement6", AdvertisementTemplateId = 9, FirmId = 1 },
                    new Facts::AdvertisementTemplate { Id = 9, DummyAdvertisementId = -6 },

                    new Facts::AdvertisementElement { Id = 7, AdvertisementId = 6, AdvertisementElementTemplateId = 8, Text = "http://localhost" },
                    new Facts::AdvertisementElementTemplate { Id = 8, Name = "AdvertisementElementTemplate8", IsAdvertisementLink = true },

                    new Facts::Firm { Id = 1, Name = "Firm1" },
                    new Facts::FirmAddress { Id = 1, FirmId = 1 },
                    new Facts::FirmAddressWebsite { Id = 2, FirmAddressId = 1, Website = "http://localhost1" }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb, FirmId = 1 },
                    new Aggregates::Order.OrderPositionAdvertisement { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Aggregates::Advertisement { Id = 6, Name = "Advertisement6", FirmId = 1 },
                    new Aggregates::Advertisement.AdvertisementWebsite { AdvertisementId = 6, Website = "http://localhost" },

                    new Aggregates::Position { Id = 5, Name = "Position5" },
                    new Aggregates::AdvertisementElementTemplate { Id = 8, Name = "AdvertisementElementTemplate8" },

                    new Aggregates::Firm { Id = 1, Name = "Firm1" },
                    new Aggregates::Firm.FirmWebsite { FirmId = 1, Website = "http://localhost1" }
                )
                .Message(
                );
    }
}
