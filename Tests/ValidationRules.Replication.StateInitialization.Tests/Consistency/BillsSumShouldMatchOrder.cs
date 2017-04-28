using NuClear.DataTest.Metamodel.Dsl;
using NuClear.Model.Common.Entities;
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
        private static ArrangeMetadataElement BillsSumShouldMatchOrder
            => ArrangeMetadataElement
                .Config
                .Name(nameof(BillsSumShouldMatchOrder))
                .Fact(
                    // Заказ с корректными счетами
                    new Facts::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2), LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1, WorkflowStep = Facts::Order.State.OnRegistration },
                    new Facts::Bill { Id = 1, OrderId = 1, PayablePlan = 123 },
                    new Facts::OrderPosition { Id = 1, OrderId = 1 },
                    new Facts::ReleaseWithdrawal { OrderPositionId = 1, Amount = 123 },

                    // Pаказ с некорректными счетами
                    new Facts::Order { Id = 2, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2), LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1, WorkflowStep = Facts::Order.State.OnRegistration },
                    new Facts::Bill { Id = 2, OrderId = 2, PayablePlan = 123.1M },
                    new Facts::OrderPosition { Id = 2, OrderId = 2 },
                    new Facts::ReleaseWithdrawal { OrderPositionId = 2, Amount = 123 },

                    // Заказ без счетов
                    new Facts::Order { Id = 3, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2), LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1, WorkflowStep = Facts::Order.State.OnRegistration },
                    new Facts::OrderPosition { Id = 3, OrderId = 3 },
                    new Facts::ReleaseWithdrawal { OrderPositionId = 3, Amount = 123 })
                .Aggregate(
                    new Aggregates::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Aggregates::Order { Id = 2, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Aggregates::Order { Id = 3, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },

                    new Aggregates::Order.InvalidBillsTotal { OrderId = 2 })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(new Reference<EntityTypeOrder>(2)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.BillsSumShouldMatchOrder,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 2,
                        });
    }
}
