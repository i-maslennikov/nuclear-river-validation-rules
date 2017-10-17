using System.Collections.Generic;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AccountBalanceShouldBePositive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(AccountBalanceShouldBePositive))
                .Fact(
                    new Facts::Account { Id = 1, Balance = 11, BranchOfficeOrganizationUnitId = 1, LegalPersonId = 2 },
                    new Facts::Account { Id = 2, Balance = 0, BranchOfficeOrganizationUnitId = 3, LegalPersonId = 4 },

                    new Facts::Order { Id = 1, BranchOfficeOrganizationUnitId = 1, LegalPersonId = 2, BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayMar, WorkflowStep = 4 },
                    new Facts::OrderPosition { Id = 2, OrderId = 1 },
                    new Facts::ReleaseWithdrawal { OrderPositionId = 2, Amount = 10, Start = FirstDayJan, End = FirstDayFeb },
                    new Facts::ReleaseWithdrawal { OrderPositionId = 2, Amount = 10, Start = FirstDayFeb, End = FirstDayMar },
                    new Facts::AccountDetail { Id = 1, AccountId = 1, PeriodStartDate = FirstDayJan, OrderId = 1 },

                    new Facts::Order { Id = 2, BranchOfficeOrganizationUnitId = 1, LegalPersonId = 2, BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayMar, WorkflowStep = 4 },
                    new Facts::OrderPosition { Id = 3, OrderId = 2 },
                    new Facts::ReleaseWithdrawal { OrderPositionId = 3, Amount = 1, Start = FirstDayJan, End = FirstDayFeb },
                    new Facts::ReleaseWithdrawal { OrderPositionId = 3, Amount = 2, Start = FirstDayFeb, End = FirstDayMar },
                    new Facts::AccountDetail { Id = 2, AccountId = 1, PeriodStartDate = FirstDayJan, OrderId = 2 },
                    new Facts::UnlimitedOrder { OrderId = 2, PeriodStart = FirstDayFeb, PeriodEnd = FirstDayMar },

                    new Facts::Order { Id = 3, BranchOfficeOrganizationUnitId = 1, LegalPersonId = 2, BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayMar, WorkflowStep = 4, IsFreeOfCharge = true },
                    new Facts::OrderPosition { Id = 4, OrderId = 3 },
                    new Facts::ReleaseWithdrawal { OrderPositionId = 4, Amount = 0, Start = FirstDayJan, End = FirstDayFeb },
                    new Facts::ReleaseWithdrawal { OrderPositionId = 4, Amount = 0, Start = FirstDayFeb, End = FirstDayMar },
                    new Facts::AccountDetail { Id = 3, AccountId = 1, PeriodStartDate = FirstDayJan, OrderId = 3 },

                    new Facts::Order { Id = 4, BranchOfficeOrganizationUnitId = 3, LegalPersonId = 4, BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayMar, WorkflowStep = 4 },
                    new Facts::OrderPosition { Id = 5, OrderId = 4 },
                    new Facts::ReleaseWithdrawal { OrderPositionId = 5, Amount = 1000, Start = FirstDayApr, End = FirstDayMay },

                    new Facts::Project())
                .Aggregate(
                    new Aggregates::Account { Id = 1 },
                    new Aggregates::Account { Id = 2 },
                    new Aggregates::Order { Id = 1, AccountId = 1, BeginDistributionDate = FirstDayJan, EndDistributionDate = FirstDayMar },
                    new Aggregates::Order { Id = 2, AccountId = 1, BeginDistributionDate = FirstDayJan, EndDistributionDate = FirstDayMar },
                    new Aggregates::Order.DebtPermission { OrderId = 2, Start = FirstDayFeb, End = FirstDayMar },
                    new Aggregates::Order { Id = 3, AccountId = 1, BeginDistributionDate = FirstDayJan, EndDistributionDate = FirstDayMar, IsFreeOfCharge = true },
                    new Aggregates::Order { Id = 4, AccountId = 2, BeginDistributionDate = FirstDayJan, EndDistributionDate = FirstDayMar },
                    new Aggregates::Account.AccountPeriod { AccountId = 1, Balance = 11, ReleaseAmount = 12, Start = FirstDayFeb, End = FirstDayMar }
                    )
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                new Dictionary<string, object> { { "available", 11.0000m }, { "planned", 12.0000m } },
                                new Reference<EntityTypeAccount>(1),
                                new Reference<EntityTypeOrder>(1)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.AccountBalanceShouldBePositive,
                            PeriodStart = FirstDayFeb,
                            PeriodEnd = FirstDayMar,
                            OrderId = 1,
                        });
    }
}
