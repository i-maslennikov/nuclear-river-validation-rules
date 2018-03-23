using System.Collections.Generic;
using System.Linq;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Aggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;


namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PoiAmountForEntranceShouldMeetMaximumRestrictionsF2A
            => ArrangeMetadataElement
               .Config
               .Name(nameof(PoiAmountForEntranceShouldMeetMaximumRestrictionsF2A))
               .Fact(
                     new Facts::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                     new Facts::Order { Id = 2, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                     new Facts::OrderPosition { Id = 1, OrderId = 1 },
                     new Facts::OrderPosition { Id = 2, OrderId = 2 },
                     new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 1, FirmAddressId = 1, PositionId = 1 },
                     new Facts::OrderPositionAdvertisement { Id = 2, OrderPositionId = 2, FirmAddressId = 2, PositionId = 2 },
                     new Facts::FirmAddress { Id = 1, EntranceCode = 1 },
                     new Facts::FirmAddress { Id = 2, EntranceCode = 2 },
                     new Facts::Position { Id = 1, CategoryCode = Facts::Position.CategoryCodesPoiAddressCheck.First() },
                     new Facts::Position { Id = 2 })
               .Aggregate(new Aggregates::Order.EntranceControlledPosition { OrderId = 1, EntranceCode = 1, FirmAddressId = 1});


        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PoiAmountForEntranceShouldMeetMaximumRestrictionsA2M
            => ArrangeMetadataElement
               .Config
               .Name(nameof(PoiAmountForEntranceShouldMeetMaximumRestrictionsA2M))
               .Aggregate(
                          new Aggregates::Order { Id = 1 },
                          new Aggregates::Order.EntranceControlledPosition { OrderId = 1, EntranceCode = 1, FirmAddressId = 1 },
                          new Aggregates::Order.OrderPeriod { OrderId = 1, Begin = MonthStart(1), End = MonthStart(3), Scope = 0 },

                          new Aggregates::Order { Id = 2 },
                          new Aggregates::Order.EntranceControlledPosition { OrderId = 2, EntranceCode = 1, FirmAddressId = 1 },
                          new Aggregates::Order.OrderPeriod { OrderId = 2, Begin = MonthStart(1), End = MonthStart(3), Scope = 0 },

                          new Aggregates::Order { Id = 3 },
                          new Aggregates::Order.EntranceControlledPosition { OrderId = 3, EntranceCode = 1, FirmAddressId = 1 },
                          new Aggregates::Order.OrderPeriod { OrderId = 3, Begin = MonthStart(1), End = MonthStart(2), Scope = -1 },

                          new Aggregates::Order { Id = 4 },
                          new Aggregates::Order.EntranceControlledPosition { OrderId = 4, EntranceCode = 2, FirmAddressId = 2 },
                          new Aggregates::Order.OrderPeriod { OrderId = 4, Begin = MonthStart(1), End = MonthStart(2), Scope = 4 },

                          new Aggregates::Order { Id = 5 },
                          new Aggregates::Order.EntranceControlledPosition { OrderId = 5, EntranceCode = 1, FirmAddressId = 1 },
                          new Aggregates::Order.OrderPeriod { OrderId = 5, Begin = MonthStart(2), End = MonthStart(3), Scope = 5 },

                          new Aggregates::Period { Start = MonthStart(1), End = MonthStart(2) },
                          new Aggregates::Period { Start = MonthStart(2), End = MonthStart(3) })
               .Message(
                        new Messages::Version.ValidationResult
                            {
                                MessageParams =
                                    new MessageParams(
                                                      new Dictionary<string, object> { { "begin", MonthStart(1) }, { "end", MonthStart(2) }, { "maxCount", 1 }, { "entranceCode", 1 } },
                                                      new Reference<EntityTypeOrder>(2),
                                                      new Reference<EntityTypeFirmAddress>(1)).ToXDocument(),
                                MessageType = (int)MessageTypeCode.PoiAmountForEntranceShouldMeetMaximumRestrictions,
                                PeriodStart = MonthStart(1),
                                PeriodEnd = MonthStart(2),
                                OrderId = 1,
                            },
                        new Messages::Version.ValidationResult
                            {
                                MessageParams =
                                    new MessageParams(
                                                      new Dictionary<string, object> { { "begin", MonthStart(2) }, { "end", MonthStart(3) }, { "maxCount", 1 }, { "entranceCode", 1 } },
                                                      new Reference<EntityTypeOrder>(2),
                                                      new Reference<EntityTypeFirmAddress>(1)).ToXDocument(),
                                MessageType = (int)MessageTypeCode.PoiAmountForEntranceShouldMeetMaximumRestrictions,
                                PeriodStart = MonthStart(2),
                                PeriodEnd = MonthStart(3),
                                OrderId = 1,
                            },
                        new Messages::Version.ValidationResult
                            {
                                MessageParams =
                                    new MessageParams(
                                                      new Dictionary<string, object> { { "begin", MonthStart(1) }, { "end", MonthStart(2) }, { "maxCount", 1 }, { "entranceCode", 1 } },
                                                      new Reference<EntityTypeOrder>(1),
                                                      new Reference<EntityTypeFirmAddress>(1)).ToXDocument(),
                                MessageType = (int)MessageTypeCode.PoiAmountForEntranceShouldMeetMaximumRestrictions,
                                PeriodStart = MonthStart(1),
                                PeriodEnd = MonthStart(2),
                                OrderId = 2,
                            },
                        new Messages::Version.ValidationResult
                            {
                                MessageParams =
                                    new MessageParams(
                                                      new Dictionary<string, object> { { "begin", MonthStart(2) }, { "end", MonthStart(3) }, { "maxCount", 1 }, { "entranceCode", 1 } },
                                                      new Reference<EntityTypeOrder>(1),
                                                      new Reference<EntityTypeFirmAddress>(1)).ToXDocument(),
                                MessageType = (int)MessageTypeCode.PoiAmountForEntranceShouldMeetMaximumRestrictions,
                                PeriodStart = MonthStart(2),
                                PeriodEnd = MonthStart(3),
                                OrderId = 2,
                            },
                        new Messages::Version.ValidationResult
                            {
                                MessageParams =
                                    new MessageParams(
                                                      new Dictionary<string, object> { { "begin", MonthStart(1) }, { "end", MonthStart(2) }, { "maxCount", 1 }, { "entranceCode", 1 } },
                                                      new Reference<EntityTypeOrder>(1),
                                                      new Reference<EntityTypeFirmAddress>(1)).ToXDocument(),
                                MessageType = (int)MessageTypeCode.PoiAmountForEntranceShouldMeetMaximumRestrictions,
                                PeriodStart = MonthStart(1),
                                PeriodEnd = MonthStart(2),
                                OrderId = 3,
                            },
                        new Messages::Version.ValidationResult
                            {
                                MessageParams =
                                    new MessageParams(
                                                      new Dictionary<string, object> { { "begin", MonthStart(1) }, { "end", MonthStart(2) }, { "maxCount", 1 }, { "entranceCode", 1 } },
                                                      new Reference<EntityTypeOrder>(2),
                                                      new Reference<EntityTypeFirmAddress>(1)).ToXDocument(),
                                MessageType = (int)MessageTypeCode.PoiAmountForEntranceShouldMeetMaximumRestrictions,
                                PeriodStart = MonthStart(1),
                                PeriodEnd = MonthStart(2),
                                OrderId = 3,
                            },
                        new Messages::Version.ValidationResult
                            {
                                MessageParams =
                                    new MessageParams(
                                                      new Dictionary<string, object> { { "begin", MonthStart(2) }, { "end", MonthStart(3) }, { "maxCount", 1 }, { "entranceCode", 1 } },
                                                      new Reference<EntityTypeOrder>(1),
                                                      new Reference<EntityTypeFirmAddress>(1)).ToXDocument(),
                                MessageType = (int)MessageTypeCode.PoiAmountForEntranceShouldMeetMaximumRestrictions,
                                PeriodStart = MonthStart(2),
                                PeriodEnd = MonthStart(3),
                                OrderId = 5,
                            },
                        new Messages::Version.ValidationResult
                            {
                                MessageParams =
                                    new MessageParams(
                                                      new Dictionary<string, object> { { "begin", MonthStart(2) }, { "end", MonthStart(3) }, { "maxCount", 1 }, { "entranceCode", 1 } },
                                                      new Reference<EntityTypeOrder>(2),
                                                      new Reference<EntityTypeFirmAddress>(1)).ToXDocument(),
                                MessageType = (int)MessageTypeCode.PoiAmountForEntranceShouldMeetMaximumRestrictions,
                                PeriodStart = MonthStart(2),
                                PeriodEnd = MonthStart(3),
                                OrderId = 5,
                            });
    }
}