using System;
using System.Linq;
using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;
using Aggregates = NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.FirmRules.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmWithSpecialCategoryShouldHaveSpecialPurchases
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmWithSpecialCategoryShouldHaveSpecialPurchases))
                .Fact(
                    new Facts::Firm { Id = 1 },
                    new Facts::FirmAddress { Id = 2, FirmId = 1 },
                    new Facts::FirmAddressCategory { Id = 3, FirmAddressId = 2, CategoryId = 18599 },

                    new Facts::Order { Id = 1 },
                    new Facts::OrderPosition { Id = 2, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 3, OrderPositionId = 2, PositionId = 4 },
                    new Facts::Position { Id = 4, CategoryCode = 287 },

                    new Facts::Order { Id = 5 },
                    new Facts::OrderPosition { Id = 6, OrderId = 5 },
                    new Facts::OrderPositionAdvertisement { Id = 7, OrderPositionId = 6, PositionId = 8 },
                    new Facts::Position { Id = 8, CategoryCode = 14, Platform = 1 })
                .Aggregate(
                    new Aggregates::Firm { Id = 1, NeedsSpecialPosition = true },

                    new Aggregates::Order.SpecialPosition { OrderId = 1 },
                    new Aggregates::Order.SpecialPosition { OrderId = 5 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmWithSpecialCategoryShouldHaveSpecialPurchasesFooBar
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmWithSpecialCategoryShouldHaveSpecialPurchasesFooBar))
                .Aggregate(
                    // Ошибка есть в фирме с рубрикой, но без позиции.
                    new Aggregates::Firm { Id = 1, Name = "Firm", NeedsSpecialPosition = true },
                    new Aggregates::Order { Id = 2, Number = "Order", FirmId = 1, Begin = FirstDayJan, End = LastSecondJan },

                    // Если добавили позицию - ошибки нет
                    new Aggregates::Firm { Id = 3, Name = "Firm", NeedsSpecialPosition = true },
                    new Aggregates::Order { Id = 4, Number = "Order", FirmId = 3, Begin = FirstDayJan, End = LastSecondMar },
                    new Aggregates::Order.SpecialPosition { OrderId = 4 },

                    // Но в те месяцы, когда позиция уже не размещается - ошибка есть
                    new Aggregates::Order { Id = 5, Number = "Order", FirmId = 3, Begin = FirstDayFeb, End = LastSecondApr, Scope = 5 },

                    // Даже если в заказе "на утверждении" позиция добавлена
                    new Aggregates::Order { Id = 6, Number = "Order", FirmId = 3, Begin = FirstDayFeb, End = LastSecondApr, Scope = 5 },
                    new Aggregates::Order.SpecialPosition { OrderId = 6 })
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><firm id=\"1\" name=\"Firm\" /><order id=\"2\" number=\"Order\" /></root>"),
                        MessageType = (int)MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchases,
                        Result = 254,
                        PeriodStart = DateTime.MinValue,
                        PeriodEnd = DateTime.MaxValue,
                    },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><firm id=\"3\" name=\"Firm\" /><order id=\"5\" number=\"Order\" /></root>"),
                        MessageType = (int)MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchases,
                        Result = 254,
                        PeriodStart = LastSecondMar,
                        PeriodEnd = DateTime.MaxValue,
                    });
    }
}
