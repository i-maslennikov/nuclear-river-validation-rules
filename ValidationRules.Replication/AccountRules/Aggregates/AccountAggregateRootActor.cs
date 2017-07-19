using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.AccountRules.Aggregates
{
    public sealed class AccountAggregateRootActor : AggregateRootActor<Account>
    {
        public AccountAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Account> accountBulkRepository,
            IBulkRepository<Account.AccountPeriod> accountPeriodBulkRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new AccountAccessor(query), accountBulkRepository,
                HasValueObject(new AccountPeriodAccessor(query), accountPeriodBulkRepository));
        }

        public sealed class AccountAccessor : DataChangesHandler<Account>, IStorageBasedDataObjectAccessor<Account>
        {
            private readonly IQuery _query;

            public AccountAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator();

            public IQueryable<Account> GetSource()
                => _query.For<Facts::Account>().Select(x => new Account { Id = x.Id });

            public FindSpecification<Account> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Account>(x => aggregateIds.Contains(x.Id));
            }
        }

        public sealed class AccountPeriodAccessor : DataChangesHandler<Account.AccountPeriod>, IStorageBasedDataObjectAccessor<Account.AccountPeriod>
        {
            private readonly IQuery _query;

            public AccountPeriodAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.AccountBalanceShouldBePositive
                    };

            public IQueryable<Account.AccountPeriod> GetSource()
            {
                var releaseWithdrawalPeriods =
                    from order in _query.For<Facts::Order>().Where(x => !x.IsFreeOfCharge && Facts::Order.State.Payable.Contains(x.WorkflowStep))
                    from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                    from releaseWithdrawal in _query.For<Facts::ReleaseWithdrawal>()
                                                    .Where(x => x.OrderPositionId == orderPosition.Id)
                                                    .Where(x => x.Start < order.EndDistributionFact)
                    from account in _query.For<Facts::Account>().Where(x => x.LegalPersonId == order.LegalPersonId && x.BranchOfficeOrganizationUnitId == order.BranchOfficeOrganizationUnitId)
                    select new { AccountId = account.Id, releaseWithdrawal.Start, releaseWithdrawal.Amount, Type = 1 };

                var locks = _query.For<Facts::Lock>().Where(x => !x.IsOrderFreeOfCharge);

                var lockPeriods =
                    from @lock in locks
                    from account in _query.For<Facts::Account>().Where(x => x.Id == @lock.AccountId)
                    select new { AccountId = account.Id, @lock.Start, @lock.Amount, Type = 3 };

                var lockSums =
                    from @lock in locks.GroupBy(x => x.AccountId)
                    select new { AccountId = @lock.Key, Sum = @lock.Select(x => x.Amount).Sum() };

                var result =
                    from item in releaseWithdrawalPeriods.Concat(lockPeriods).GroupBy(a => new { a.AccountId, a.Start })
                    from account in _query.For<Facts::Account>().Where(x => x.Id == item.Key.AccountId)
                    from sum in lockSums.Where(x => x.AccountId == item.Key.AccountId).DefaultIfEmpty()
                    select new Account.AccountPeriod
                        {
                            AccountId = item.Key.AccountId,
                            Start = item.Key.Start,
                            Balance = account.Balance,
                            End = item.Key.Start.AddMonths(1),
                            ReleaseAmount = item.Where(x => x.Type == 1).Select(x => x.Amount).Sum(),
                            LockedAmount = item.Where(x => x.Type == 3).Select(x => x.Amount).Sum(),
                            OwerallLockedAmount = sum != null ? sum.Sum : 0,
                        };

                return result;
            }

            public FindSpecification<Account.AccountPeriod> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Account.AccountPeriod>(x => aggregateIds.Contains(x.AccountId));
            }
        }
    }
}