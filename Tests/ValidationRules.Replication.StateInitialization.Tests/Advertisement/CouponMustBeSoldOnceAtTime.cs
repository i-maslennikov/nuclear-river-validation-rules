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
        private static ArrangeMetadataElement CouponMustBeSoldOnceAtTime
            => ArrangeMetadataElement
                .Config
                .Name(nameof(CouponMustBeSoldOnceAtTime))
                .Fact(
                    // Заказ на расторжении даёт два периода размещения купона.
                    new Facts::Order { Id = 1, BeginDistributionDate = MonthStart(1), EndDistributionDateFact = MonthStart(2), EndDistributionDatePlan = MonthStart(3), WorkflowStepId = 4 },
                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    // Заказ "на оформлении" даёт период, влияющий только на сам заказ
                    new Facts::Order { Id = 2, BeginDistributionDate = MonthStart(1), EndDistributionDateFact = MonthStart(3), EndDistributionDatePlan = MonthStart(3), WorkflowStepId = 1 },
                    new Facts::OrderPosition { Id = 5, OrderId = 2, },
                    new Facts::OrderPositionAdvertisement { Id = 2, OrderPositionId = 5, PositionId = 5, AdvertisementId = 6 },

                    // Заказ "на утверждении" даёт период, влияющий на сам заказ и все остальные
                    new Facts::Order { Id = 3, BeginDistributionDate = MonthStart(1), EndDistributionDateFact = MonthStart(3), EndDistributionDatePlan = MonthStart(3), WorkflowStepId = 2 },
                    new Facts::OrderPosition { Id = 6, OrderId = 3, },
                    new Facts::OrderPositionAdvertisement { Id = 3, OrderPositionId = 6, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Position { Id = 5, CategoryCode = 14 },
                    new Facts::Advertisement { Id = 6, AdvertisementTemplateId = 9 },
                    new Facts::AdvertisementTemplate { Id = 9, DummyAdvertisementId = -6 }
                )
                .Aggregate(
                    new Aggregates::Order.CouponDistributionPeriod { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6, Begin = MonthStart(1), End = MonthStart(2), Scope = 0 },
                    new Aggregates::Order.CouponDistributionPeriod { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6, Begin = MonthStart(2), End = MonthStart(3), Scope = 1 },

                    new Aggregates::Order.CouponDistributionPeriod { OrderId = 2, OrderPositionId = 5, PositionId = 5, AdvertisementId = 6, Begin = MonthStart(1), End = MonthStart(3), Scope = 2 },

                    new Aggregates::Order.CouponDistributionPeriod { OrderId = 3, OrderPositionId = 6, PositionId = 5, AdvertisementId = 6, Begin = MonthStart(1), End = MonthStart(3), Scope = -1 });

        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once InconsistentNaming
        private static ArrangeMetadataElement CouponMustBeSoldOnceAtTime_SingleOrder_NoError
            => ArrangeMetadataElement
                .Config
                .Name(nameof(CouponMustBeSoldOnceAtTime_SingleOrder_NoError))
                .Aggregate(
                    // Купон размещается в одном заказе в каждый момент времени - ошибок нет
                    new Aggregates::Order { Id = 1, Number = "Order1" },
                    new Aggregates::Order.CouponDistributionPeriod { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6, Begin = MonthStart(1), End = MonthStart(2) },

                    new Aggregates::Order { Id = 2, Number = "Order2" },
                    new Aggregates::Order.CouponDistributionPeriod { OrderId = 2, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6, Begin = MonthStart(2), End = MonthStart(3) },

                    new Aggregates::Order { Id = 3, Number = "Order3" },
                    new Aggregates::Order.CouponDistributionPeriod { OrderId = 3, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6, Begin = MonthStart(3), End = MonthStart(4) },

                    new Aggregates::Advertisement { Id = 6, Name = "Advertisement6", FirmId = 7 },
                    new Aggregates::Position { Id = 5, Name = "Position5" }
                )
                .Message();

        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once InconsistentNaming
        private static ArrangeMetadataElement CouponMustBeSoldOnceAtTime_SingleOrder
            => ArrangeMetadataElement
                .Config
                .Name(nameof(CouponMustBeSoldOnceAtTime_SingleOrder))
                .Aggregate(
                    new Aggregates::Order { Id = 1, Number = "Order1" },
                    new Aggregates::Order.CouponDistributionPeriod { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6, Begin = MonthStart(1), End = MonthStart(2) },
                    new Aggregates::Order.CouponDistributionPeriod { OrderId = 1, OrderPositionId = 5, PositionId = 5, AdvertisementId = 6, Begin = MonthStart(1), End = MonthStart(2) },

                    new Aggregates::Advertisement { Id = 6, Name = "Advertisement6", FirmId = 7 },
                    new Aggregates::Position { Id = 5, Name = "Position5" }
                )
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root>" +
                                                            "<advertisement id = \"6\" name=\"Advertisement6\" />" +
                                                            "<order id = \"1\" number=\"Order1\" />" +
                                                            "<orderPosition id = \"4\" name=\"Position5\" />" +
                                                            "<orderPosition id = \"5\" name=\"Position5\" />" +
                                                            "</root>"),
                            MessageType = (int)MessageTypeCode.CouponMustBeSoldOnceAtTime,
                            Result = 255,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                        });

        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once InconsistentNaming
        private static ArrangeMetadataElement CouponMustBeSoldOnceAtTime_ParallelOrdersOnRegistration
            => ArrangeMetadataElement
                .Config
                .Name(nameof(CouponMustBeSoldOnceAtTime_ParallelOrdersOnRegistration))
                .Aggregate(
                    // Заказ "на оформлении"
                    new Aggregates::Order { Id = 1, Number = "Order1" },
                    new Aggregates::Order.CouponDistributionPeriod { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6, Scope = 1, Begin = MonthStart(1), End = MonthStart(2) },

                    // Заказ "на оформлении"
                    new Aggregates::Order { Id = 2, Number = "Order2" },
                    new Aggregates::Order.CouponDistributionPeriod { OrderId = 2, OrderPositionId = 5, PositionId = 5, AdvertisementId = 6, Scope = 2, Begin = MonthStart(1), End = MonthStart(2) },

                    // Заказ "одобрен"
                    new Aggregates::Order { Id = 3, Number = "Order3" },
                    new Aggregates::Order.CouponDistributionPeriod { OrderId = 3, OrderPositionId = 6, PositionId = 5, AdvertisementId = 6, Scope = 0, Begin = MonthStart(1), End = MonthStart(2) },

                    new Aggregates::Advertisement { Id = 6, Name = "Advertisement6", FirmId = 7 },
                    new Aggregates::Position { Id = 5, Name = "Position5" }
                )
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root>" +
                                                            "<advertisement id = \"6\" name=\"Advertisement6\" />" +
                                                            "<order id = \"1\" number=\"Order1\" />" +
                                                            "<orderPosition id = \"4\" name=\"Position5\" />" +
                                                            "<orderPosition id = \"6\" name=\"Position5\" />" +
                                                            "</root>"),
                            MessageType = (int)MessageTypeCode.CouponMustBeSoldOnceAtTime,
                            Result = 255,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root>" +
                                                            "<advertisement id = \"6\" name=\"Advertisement6\" />" +
                                                            "<order id = \"2\" number=\"Order2\" />" +
                                                            "<orderPosition id = \"5\" name=\"Position5\" />" +
                                                            "<orderPosition id = \"6\" name=\"Position5\" />" +
                                                            "</root>"),
                            MessageType = (int)MessageTypeCode.CouponMustBeSoldOnceAtTime,
                            Result = 255,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                        });



        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once InconsistentNaming
        private static ArrangeMetadataElement CouponMustBeSoldOnceAtTime_ParallelOrdersOnTermination
            => ArrangeMetadataElement
                .Config
                .Name(nameof(CouponMustBeSoldOnceAtTime_ParallelOrdersOnTermination))
                .Aggregate(
                    // Заказ "на расторжении"
                    new Aggregates::Order { Id = 1, Number = "Order1" },
                    new Aggregates::Order.CouponDistributionPeriod { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6, Scope = 0, Begin = MonthStart(1), End = MonthStart(2) },
                    new Aggregates::Order.CouponDistributionPeriod { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6, Scope = 1, Begin = MonthStart(2), End = MonthStart(3) },

                    // Заказ "одобрен"
                    new Aggregates::Order { Id = 2, Number = "Order2" },
                    new Aggregates::Order.CouponDistributionPeriod { OrderId = 2, OrderPositionId = 5, PositionId = 5, AdvertisementId = 6, Scope = 0, Begin = MonthStart(2), End = MonthStart(3) },

                    new Aggregates::Advertisement { Id = 6, Name = "Advertisement6", FirmId = 7 },
                    new Aggregates::Position { Id = 5, Name = "Position5" }
                )
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root>" +
                                                            "<advertisement id = \"6\" name=\"Advertisement6\" />" +
                                                            "<order id = \"1\" number=\"Order1\" />" +
                                                            "<orderPosition id = \"4\" name=\"Position5\" />" +
                                                            "<orderPosition id = \"5\" name=\"Position5\" />" +
                                                            "</root>"),
                            MessageType = (int)MessageTypeCode.CouponMustBeSoldOnceAtTime,
                            Result = 255,
                            PeriodStart = MonthStart(2),
                            PeriodEnd = MonthStart(3),
                        });
    }
}
