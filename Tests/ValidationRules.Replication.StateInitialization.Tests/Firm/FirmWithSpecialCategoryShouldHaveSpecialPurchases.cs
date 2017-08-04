using System;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

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
                    new Facts::Firm { Id = 1, IsActive = true },
                    new Facts::FirmAddress { Id = 2, FirmId = 1, IsActive = true },
                    new Facts::FirmAddressCategory { Id = 3, FirmAddressId = 2, CategoryId = 18599 },

                    new Facts::Order { Id = 1, FirmId = 1, BeginDistribution = MonthStart(1), EndDistributionFact = MonthStart(4), WorkflowStep = 5 },
                    new Facts::OrderPosition { Id = 2, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 3, OrderPositionId = 2, PositionId = 4 },
                    new Facts::Position { Id = 4, CategoryCode = 14, Platform = 1 },

                    new Facts::Order { Id = 2, FirmId = 1, BeginDistribution = MonthStart(2), EndDistributionFact = MonthStart(5), WorkflowStep = 1 },
                    new Facts::OrderPosition { Id = 3, OrderId = 2 },
                    new Facts::OrderPositionAdvertisement { Id = 4, OrderPositionId = 3, PositionId = 5 },
                    new Facts::Position { Id = 5, CategoryCode = 14, Platform = 1 },

                    new Facts::Order { Id = 3, FirmId = 1, BeginDistribution = MonthStart(3), EndDistributionFact = MonthStart(6), WorkflowStep = 4 },

                    // Для фирмы без рубрики "Выгодные покупки" периоды не создаются
                    new Facts::Firm { Id = 2, IsActive = true },
                    new Facts::FirmAddress { Id = 3, FirmId = 2, IsActive = true },

                    // для неактивных фирм периоды не создаются
                    new Facts::Firm { Id = 3, IsActive = false },
                    new Facts::FirmAddress { Id = 4, FirmId = 3, IsActive = false },
                    new Facts::FirmAddressCategory { Id = 4, FirmAddressId = 4, CategoryId = 18599 },

                    new Facts::Order { Id = 4, FirmId = 3, BeginDistribution = MonthStart(1), EndDistributionFact = MonthStart(4), WorkflowStep = 5 },
                    new Facts::OrderPosition { Id = 4, OrderId = 4 },
                    new Facts::OrderPositionAdvertisement { Id = 5, OrderPositionId = 4, PositionId = 6 },
                    new Facts::Position { Id = 6, CategoryCode = 14, Platform = 1 })

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

                    new Aggregates::Firm { Id = 1 },
                    new Aggregates::Order { Id = 1, FirmId = 1, Begin = MonthStart(1), End = MonthStart(3), Scope = 0 },
                    new Aggregates::Order { Id = 2, FirmId = 1, Begin = MonthStart(1), End = MonthStart(3), Scope = 2 })
                .Message(
                    // Фирма №1
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(new Reference<EntityTypeFirm>(1)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchases,
                            PeriodStart = DateTime.MinValue,
                            PeriodEnd = MonthStart(1),
                            ProjectId = 0,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(new Reference<EntityTypeFirm>(1)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchases,
                            PeriodStart = MonthStart(2),
                            PeriodEnd = MonthStart(3),
                            ProjectId = 0,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(new Reference<EntityTypeFirm>(1)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchases,
                            PeriodStart = MonthStart(3),
                            PeriodEnd = DateTime.MaxValue,
                            ProjectId = 0,
                        });
    }
}
