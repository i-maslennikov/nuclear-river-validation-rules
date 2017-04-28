using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement InvalidCategory
            => ArrangeMetadataElement
                .Config
                .Name(nameof(InvalidCategory))
                .Fact(
                    new Facts::Order { Id = 1, FirmId = 1 },
                    new Facts::OrderPosition { Id = 1, OrderId = 1 },
                    new Facts::Position { Id = 1 },
                    new Facts::Position { Id = 2, BindingObjectType = Facts::Position.BindingObjectTypeCategoryMultipleAsterix },
                    new Facts::FirmAddress { Id = 1, FirmId = 1, IsActive = true },

                    // Активная рубрика принадлежит фирме
                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 1, CategoryId = 1, PositionId = 1 },
                    new Facts::FirmAddressCategory { Id = 1, FirmAddressId = 1, CategoryId = 1 },
                    new Facts::Category { Id = 1, IsActiveNotDeleted = true },

                    // Неактивная рубрика принадлежит фирме
                    new Facts::OrderPositionAdvertisement { Id = 2, OrderPositionId = 1, CategoryId = 2, PositionId = 1 },
                    new Facts::FirmAddressCategory { Id = 2, FirmAddressId = 2, CategoryId = 2 },
                    new Facts::Category { Id = 2, IsActiveNotDeleted = false },

                    // Активная рубрика не принадлежит фирме
                    new Facts::OrderPositionAdvertisement { Id = 3, OrderPositionId = 1, CategoryId = 3, PositionId = 1 },
                    new Facts::Category { Id = 3, IsActiveNotDeleted = true },

                    // Неактивная рубрика не принадлежит фирме
                    new Facts::OrderPositionAdvertisement { Id = 4, OrderPositionId = 1, CategoryId = 4, PositionId = 1 },
                    new Facts::Category { Id = 4, IsActiveNotDeleted = false },

                    // Неактивная рубрика не принадлежит фирме, но номенклатурная позиция с особым типом объекта привязки
                    new Facts::OrderPositionAdvertisement { Id = 5, OrderPositionId = 1, CategoryId = 5, PositionId = 2 },
                    new Facts::Category { Id = 5, IsActiveNotDeleted = false })
                .Aggregate(
                    new Aggregates::Order.InvalidCategory { OrderId = 1, CategoryId = 2, OrderPositionId = 1, PositionId = 1, State = InvalidCategoryState.Inactive, MayNotBelongToFirm = false },
                    new Aggregates::Order.InvalidCategory { OrderId = 1, CategoryId = 3, OrderPositionId = 1, PositionId = 1, State = InvalidCategoryState.NotBelongToFirm, MayNotBelongToFirm = false },
                    new Aggregates::Order.InvalidCategory { OrderId = 1, CategoryId = 4, OrderPositionId = 1, PositionId = 1, State = InvalidCategoryState.Inactive, MayNotBelongToFirm = false },
                    new Aggregates::Order.InvalidCategory { OrderId = 1, CategoryId = 5, OrderPositionId = 1, PositionId = 2, State = InvalidCategoryState.Inactive, MayNotBelongToFirm = true });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement LinkedCategoryAsterixMayBelongToFirm
            => ArrangeMetadataElement
                .Config
                .Name(nameof(LinkedCategoryAsterixMayBelongToFirm))
                .Aggregate(
                    new Aggregates::Order { Id = 3, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Aggregates::Order.InvalidCategory { OrderId = 3, CategoryId = 3, OrderPositionId = 1, PositionId = 2, State = InvalidCategoryState.NotBelongToFirm, MayNotBelongToFirm = true })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                    new Reference<EntityTypeCategory>(3),
                                    new Reference<EntityTypeOrder>(3),
                                    new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                        new Reference<EntityTypeOrderPosition>(1),
                                        new Reference<EntityTypePosition>(2)))
                                .ToXDocument(),
                        MessageType = (int)MessageTypeCode.LinkedCategoryAsterixMayBelongToFirm,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 3,
                        });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement LinkedCategoryShouldBelongToFirm
            => ArrangeMetadataElement
                .Config
                .Name(nameof(LinkedCategoryShouldBelongToFirm))
                .Aggregate(
                    new Aggregates::Order { Id = 3, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Aggregates::Order.InvalidCategory { OrderId = 3, CategoryId = 3, OrderPositionId = 1, PositionId = 2, State = InvalidCategoryState.NotBelongToFirm, MayNotBelongToFirm = false })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                    new Reference<EntityTypeCategory>(3),
                                    new Reference<EntityTypeOrder>(3),
                                    new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                        new Reference<EntityTypeOrderPosition>(1),
                                        new Reference<EntityTypePosition>(2)))
                                .ToXDocument(),
                            MessageType = (int)MessageTypeCode.LinkedCategoryShouldBelongToFirm,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 3,
                        });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement LinkedCategoryShouldBeActive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(LinkedCategoryShouldBeActive))
                .Aggregate(
                    new Aggregates::Order { Id = 3, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Aggregates::Order.InvalidCategory { OrderId = 3, CategoryId = 3, OrderPositionId = 1, PositionId = 2, State = InvalidCategoryState.Inactive })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                    new Reference<EntityTypeCategory>(3),
                                    new Reference<EntityTypeOrder>(3),
                                    new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                        new Reference<EntityTypeOrderPosition>(1),
                                        new Reference<EntityTypePosition>(2)))
                                .ToXDocument(),
                        MessageType = (int)MessageTypeCode.LinkedCategoryShouldBeActive,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 3,
                        });
    }
}
