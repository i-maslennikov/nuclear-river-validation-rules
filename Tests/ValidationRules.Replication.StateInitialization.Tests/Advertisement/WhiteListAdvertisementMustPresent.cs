﻿using System;
using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;

using Aggregates = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;

using Messages = NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement WhiteListAdvertisementMustPresent
            => ArrangeMetadataElement
                .Config
                .Name(nameof(WhiteListAdvertisementMustPresent))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDateFact = FirstDayFeb, EndDistributionDatePlan = FirstDayFeb, FirmId = 7 },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Advertisement { Id = 6, Name = "Advertisement6", FirmId = 7, AdvertisementTemplateId = 8, IsSelectedToWhiteList = false }, // РМ не выбран в белый список
                    new Facts::Firm { Id = 7, Name = "Firm7" },
                    new Facts::AdvertisementTemplate { Id = 8, IsAllowedToWhiteList = true } // должен быть РМ в белом списке
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb, EndDistributionDateFact = FirstDayFeb, FirmId = 7, RequireWhiteListAdvertisement = true },
                    new Aggregates::Order.LinkedProject { OrderId = 1, ProjectId = 3 },
                    new Aggregates::Order.OrderPositionAdvertisement { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Aggregates::Firm.WhiteListDistributionPeriod { FirmId = 7, Start = FirstDayJan, End = FirstDayFeb, AdvertisementId = null, ProvidedByOrderId = null },

                    new Aggregates::Advertisement { Id = 6, Name = "Advertisement6", FirmId = 7, IsSelectedToWhiteList = false },
                    new Aggregates::Firm { Id = 7, Name = "Firm7" }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><order id = \"1\" number=\"Order1\" /><firm id = \"7\" name=\"Firm7\" /></root>"),
                        MessageType = (int)MessageTypeCode.WhiteListAdvertisementMustPresent,
                        Result = 250,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        ProjectId = 3,
                    });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement WhiteListAdvertisementMayPresent
            => ArrangeMetadataElement
                .Config
                .Name(nameof(WhiteListAdvertisementMayPresent))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDateFact = FirstDayFeb, EndDistributionDatePlan = FirstDayFeb, FirmId = 7 },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Advertisement { Id = 6, Name = "Advertisement6", FirmId = 7, AdvertisementTemplateId = 8, IsSelectedToWhiteList = true }, // РМ выбран в белый список
                    new Facts::Firm { Id = 7, Name = "Firm7" },
                    new Facts::AdvertisementTemplate { Id = 8, IsAllowedToWhiteList = true } // должен быть РМ в белом списке
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb, EndDistributionDateFact = FirstDayFeb, FirmId = 7, RequireWhiteListAdvertisement = true, ProvideWhiteListAdvertisement = true },
                    new Aggregates::Order.LinkedProject { OrderId = 1, ProjectId = 3 },
                    new Aggregates::Order.OrderPositionAdvertisement { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Aggregates::Firm.WhiteListDistributionPeriod { FirmId = 7, Start = FirstDayJan, End = FirstDayFeb, AdvertisementId = 6, ProvidedByOrderId = 1 },

                    new Aggregates::Advertisement { Id = 6, Name = "Advertisement6", FirmId = 7, IsSelectedToWhiteList = true },
                    new Aggregates::Firm { Id = 7, Name = "Firm7" }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><order id = \"1\" number=\"Order1\" /><firm id = \"7\" name=\"Firm7\" /><advertisement id=\"6\" name=\"Advertisement6\" /></root>"),
                        MessageType = (int)MessageTypeCode.WhiteListAdvertisementMayPresent,
                        Result = 85,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        ProjectId = 3,
                    }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement WhiteListAdvertisementMayAndMustPresent
            => ArrangeMetadataElement
                .Config
                .Name(nameof(WhiteListAdvertisementMayAndMustPresent))
                .Aggregate(
                    new Aggregates::Order { Id = 1, Number = "Order", BeginDistributionDate = MonthStart(1), EndDistributionDatePlan = MonthStart(3), RequireWhiteListAdvertisement = true },
                    new Aggregates::Order.LinkedProject { OrderId = 1 },

                    new Aggregates::Firm.WhiteListDistributionPeriod { Start = DateTime.MinValue, End = MonthStart(1), ProvidedByOrderId = null, AdvertisementId = null },
                    new Aggregates::Firm.WhiteListDistributionPeriod { Start = MonthStart(1), End = MonthStart(2), ProvidedByOrderId = 111, AdvertisementId = 111 },
                    new Aggregates::Firm.WhiteListDistributionPeriod { Start = MonthStart(2), End = DateTime.MaxValue, ProvidedByOrderId = null, AdvertisementId = null },

                    new Aggregates::Advertisement { Name = "Advertisement", IsSelectedToWhiteList = true },
                    new Aggregates::Firm { Name = "Firm" }
                )
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root><order id = \"1\" number=\"Order\" /><firm id = \"0\" name=\"Firm\" /><advertisement id=\"0\" name=\"Advertisement\" /></root>"),
                            MessageType = (int)MessageTypeCode.WhiteListAdvertisementMayPresent,
                            Result = 85,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root><order id = \"1\" number=\"Order\" /><firm id = \"0\" name=\"Firm\" /></root>"),
                            MessageType = (int)MessageTypeCode.WhiteListAdvertisementMustPresent,
                            Result = 250,
                            PeriodStart = MonthStart(2),
                            PeriodEnd = MonthStart(3),
                        });
    }
}