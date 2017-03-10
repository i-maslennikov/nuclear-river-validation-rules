using System.Collections.Generic;
using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
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
        private static ArrangeMetadataElement OrderMustHaveActiveDealAggregate
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderMustHaveActiveDealAggregate))
                .Fact(
                    new Facts::Order { Id = 1, DealId = null, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Facts::Order { Id = 2, DealId = 2, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Facts::Order { Id = 3, DealId = 3, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },

                    new Facts::Deal { Id = 3 },
                    new Facts::Project())
                .Aggregate(
                    new Aggregates::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    CreateOrderMissingRequiredField(orderId: 1, deal: true),
                    new Aggregates::Order.InactiveReference { OrderId = 1, Deal = false },

                    new Aggregates::Order { Id = 2, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    CreateOrderMissingRequiredField(orderId: 2, deal: false),
                    new Aggregates::Order.InactiveReference { OrderId = 2, Deal = true },

                    new Aggregates::Order { Id = 3, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    CreateOrderMissingRequiredField(orderId: 3, deal: false),
                    new Aggregates::Order.InactiveReference { OrderId = 3, Deal = false });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderMustHaveActiveDealMessage
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderMustHaveActiveDealMessage))
                .Aggregate(
                    new Aggregates::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Aggregates::Order.MissingRequiredField { OrderId = 1, Deal = true },
                    new Aggregates::Order.InactiveReference { OrderId = 1, Deal = false },

                    new Aggregates::Order { Id = 2, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Aggregates::Order.MissingRequiredField { OrderId = 2, Deal = false },
                    new Aggregates::Order.InactiveReference { OrderId = 2, Deal = true },

                    new Aggregates::Order { Id = 3, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Aggregates::Order.MissingRequiredField { OrderId = 3, Deal = false },
                    new Aggregates::Order.InactiveReference { OrderId = 3, Deal = false })
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(
                                new Dictionary<string, object> { { "state", (int)Aggregates::DealState.Missing } },
                                new Reference<EntityTypeOrder>(1)).ToXDocument(),
                        MessageType = (int)MessageTypeCode.OrderMustHaveActiveDeal,
                        PeriodStart = MonthStart(1),
                        PeriodEnd = MonthStart(2),
                        OrderId = 1,
                    },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(
                                new Dictionary<string, object> { { "state", (int)Aggregates::DealState.Inactive } },
                                new Reference<EntityTypeOrder>(2)).ToXDocument(),
                        MessageType = (int)MessageTypeCode.OrderMustHaveActiveDeal,
                        PeriodStart = MonthStart(1),
                        PeriodEnd = MonthStart(2),
                        OrderId = 2,
                    });

        private static Aggregates::Order.MissingRequiredField CreateOrderMissingRequiredField(long orderId,
                                                                                      bool branchOfficeOrganizationUnit = true,
                                                                                      bool currency = true,
                                                                                      bool deal = true,
                                                                                      bool inspector = true,
                                                                                      bool legalPerson = true,
                                                                                      bool legalPersonProfile = true,
                                                                                      bool releaseCountPlan = true)
        {
            return new Aggregates::Order.MissingRequiredField
                {
                    OrderId = orderId,
                    BranchOfficeOrganizationUnit = branchOfficeOrganizationUnit,
                    Currency = currency,
                    Deal = deal,
                    Inspector = inspector,
                    LegalPerson = legalPerson,
                    LegalPersonProfile = legalPersonProfile,
                    ReleaseCountPlan = releaseCountPlan,
                };
        }
    }
}
