using System;
using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;

using Aggregates = NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.FirmRules.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmWithSpecialCategoryShouldHaveSpecialPurchasesAggregate
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmWithSpecialCategoryShouldHaveSpecialPurchasesAggregate))
                .Fact(
                    // Для фирмы с рубрикой "Выгодные покупки" создаются периоды размещения позиций "Самореклама только для ПК", "Выгодные покупки с 2ГИС (для ПК)"
                    new Facts::Firm { Id = 1 },
                    new Facts::FirmAddress { Id = 2, FirmId = 1 },
                    new Facts::FirmAddressCategory { Id = 3, FirmAddressId = 2, CategoryId = 18599 },

                    new Facts::Order { Id = 1, FirmId = 1, BeginDistribution = MonthStart(1), EndDistributionFact = MonthStart(4), WorkflowStep = 5 },
                    new Facts::OrderPosition { Id = 2, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 3, OrderPositionId = 2, PositionId = 4 },
                    new Facts::SpecialPosition { Id = 4, IsAdvantageousPurchaseOnPc = true },

                    new Facts::Order { Id = 2, FirmId = 1, BeginDistribution = MonthStart(2), EndDistributionFact = MonthStart(5), WorkflowStep = 1 },
                    new Facts::OrderPosition { Id = 3, OrderId = 2 },
                    new Facts::OrderPositionAdvertisement { Id = 4, OrderPositionId = 3, PositionId = 5 },
                    new Facts::SpecialPosition { Id = 5, IsSelfAdvertisementOnPc = true },

                    new Facts::Order { Id = 3, FirmId = 1, BeginDistribution = MonthStart(3), EndDistributionFact = MonthStart(6), WorkflowStep = 4 },

                    // Для фирмы без рубрики "Выгодные покупки" периоды не создаются
                    new Facts::Firm { Id = 2 },
                    new Facts::FirmAddress { Id = 3, FirmId = 2 })
                .Aggregate(
                    new Aggregates::Firm.AdvantageousPurchasePositionDistributionPeriod { FirmId = 1, Begin = DateTime.MinValue, End = DateTime.MaxValue, HasPosition = false, Scope = 0 },
                    new Aggregates::Firm.AdvantageousPurchasePositionDistributionPeriod { FirmId = 1, Begin = MonthStart(1), End = MonthStart(4), HasPosition = true, Scope = 0 },
                    new Aggregates::Firm.AdvantageousPurchasePositionDistributionPeriod { FirmId = 1, Begin = MonthStart(2), End = MonthStart(5), HasPosition = true, Scope = 2 },
                    new Aggregates::Firm.AdvantageousPurchasePositionDistributionPeriod { FirmId = 1, Begin = MonthStart(3), End = MonthStart(6), HasPosition = false, Scope = 0 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmWithSpecialCategoryShouldHaveSpecialPurchasesMessage
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmWithSpecialCategoryShouldHaveSpecialPurchasesMessage))
                .Aggregate(
                    new Aggregates::Firm.AdvantageousPurchasePositionDistributionPeriod { FirmId = 1, Begin = DateTime.MinValue, End = DateTime.MaxValue, HasPosition = false, Scope = 0 },
                    new Aggregates::Firm.AdvantageousPurchasePositionDistributionPeriod { FirmId = 1, Begin = MonthStart(1), End = MonthStart(2), HasPosition = true, Scope = 0 },
                    new Aggregates::Firm.AdvantageousPurchasePositionDistributionPeriod { FirmId = 1, Begin = MonthStart(1), End = MonthStart(3), HasPosition = true, Scope = 2 },

                    new Aggregates::Firm { Id = 1, Name = "Firm" },
                    new Aggregates::Order { Id = 1, FirmId = 1, Number = "Order", Begin = MonthStart(1), End = MonthStart(3), Scope = 0 },
                    new Aggregates::Order { Id = 2, FirmId = 1, Number = "Order", Begin = MonthStart(1), End = MonthStart(3), Scope = 2 })
                .Message(
                    // Фирма №1
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root><firm id=\"1\" name=\"Firm\" /></root>"),
                            MessageType = (int)MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchases,
                            Result = 252,
                            PeriodStart = DateTime.MinValue,
                            PeriodEnd = MonthStart(1),
                            ProjectId = 0,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root><firm id=\"1\" name=\"Firm\" /></root>"),
                            MessageType = (int)MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchases,
                            Result = 252,
                            PeriodStart = MonthStart(2),
                            PeriodEnd = MonthStart(3),
                            ProjectId = 0,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root><firm id=\"1\" name=\"Firm\" /></root>"),
                            MessageType = (int)MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchases,
                            Result = 252,
                            PeriodStart = MonthStart(3),
                            PeriodEnd = DateTime.MaxValue,
                            ProjectId = 0,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root><firm id=\"1\" name=\"Firm\" /><order id=\"1\" number=\"Order\" /></root>"),
                            MessageType = (int)MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchasesOrder,
                            Result = 2,
                            PeriodStart = MonthStart(2),
                            PeriodEnd = MonthStart(3),
                            OrderId = 1,
                        });
    }
}
