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

                    new Facts::Order { Id = 1, BranchOfficeOrganizationUnitId = 1, LegalPersonId = 2, BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayMar, WorkflowStep = 4 },
                    new Facts::OrderPosition { Id = 2, OrderId = 1 },
                    new Facts::ReleaseWithdrawal { Id = 3, OrderPositionId = 2, Amount = 10, Start = FirstDayJan },
                    new Facts::ReleaseWithdrawal { Id = 4, OrderPositionId = 2, Amount = 10, Start = FirstDayFeb },
                    new Facts::Lock { Id = 1, OrderId = 1, AccountId = 1, Start = FirstDayJan, Amount = 10 },

                    new Facts::Order { Id = 2, BranchOfficeOrganizationUnitId = 1, LegalPersonId = 2, BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayMar, WorkflowStep = 4 },
                    new Facts::OrderPosition { Id = 3, OrderId = 2 },
                    new Facts::ReleaseWithdrawal { Id = 5, OrderPositionId = 2, Amount = 1, Start = FirstDayJan },
                    new Facts::ReleaseWithdrawal { Id = 6, OrderPositionId = 2, Amount = 2, Start = FirstDayFeb },
                    new Facts::Lock { Id = 2, OrderId = 2, AccountId = 1, Start = FirstDayJan, Amount = 1 },
                    new Facts::UnlimitedOrder { OrderId = 2, PeriodStart = FirstDayFeb, PeriodEnd = FirstDayMar },

                    new Facts::Order { Id = 3, BranchOfficeOrganizationUnitId = 1, LegalPersonId = 2, BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayMar, WorkflowStep = 4, IsFreeOfCharge = true },
                    new Facts::OrderPosition { Id = 4, OrderId = 3 },
                    new Facts::ReleaseWithdrawal { Id = 7, OrderPositionId = 4, Amount = 0, Start = FirstDayJan },
                    new Facts::ReleaseWithdrawal { Id = 8, OrderPositionId = 4, Amount = 0, Start = FirstDayFeb },
                    new Facts::Lock { Id = 3, OrderId = 3, AccountId = 1, Start = FirstDayJan, Amount = 0 },

                    new Facts::Project())
                .Aggregate(
                    new Aggregates::Order { Id = 1, AccountId = 1, BeginDistributionDate = FirstDayJan, EndDistributionDate = FirstDayMar },
                    new Aggregates::Order { Id = 2, AccountId = 1, BeginDistributionDate = FirstDayJan, EndDistributionDate = FirstDayMar },
                    new Aggregates::Order.DebtPermission { OrderId = 2, Start = FirstDayFeb, End = FirstDayMar },
                    new Aggregates::Order { Id = 3, AccountId = 1, BeginDistributionDate = FirstDayJan, EndDistributionDate = FirstDayMar, IsFreeOfCharge = true},
                    new Aggregates::Account.AccountPeriod { AccountId = 1, Balance = 11, LockedAmount = 11, OwerallLockedAmount = 11, ReleaseAmount = 11, Start = FirstDayJan, End = FirstDayFeb },
                    new Aggregates::Account.AccountPeriod { AccountId = 1, Balance = 11, LockedAmount = 0, OwerallLockedAmount = 11, ReleaseAmount = 12, Start = FirstDayFeb, End = FirstDayMar })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                new Dictionary<string, object> { { "available", 0.0000m }, { "planned", 12.0000m } },
                                new Reference<EntityTypeAccount>(1),
                                new Reference<EntityTypeOrder>(1)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.AccountBalanceShouldBePositive,
                            Result = 960,
                            PeriodStart = FirstDayFeb,
                            PeriodEnd = FirstDayMar,
                            OrderId = 1,
                        });
    }
}
