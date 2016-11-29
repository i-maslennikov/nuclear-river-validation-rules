using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.AccountRules.Aggregates
{
    public sealed class AccountAggregateRootActor : EntityActorBase<Account>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IEqualityComparerFactory _equalityComparerFactory;
        private readonly IBulkRepository<AccountPeriod> _accountPeriodBulkRepository;

        public AccountAggregateRootActor(
            IQuery query,
            IBulkRepository<Account> accountBulkRepository,
            IBulkRepository<AccountPeriod> accountPeriodBulkRepository,
            IEqualityComparerFactory equalityComparerFactory)
            : base(query, accountBulkRepository, equalityComparerFactory, new AccountAccessor(query))
        {
            _query = query;
            _equalityComparerFactory = equalityComparerFactory;
            _accountPeriodBulkRepository = accountPeriodBulkRepository;
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<AccountPeriod>(_query, _accountPeriodBulkRepository, _equalityComparerFactory, new AccountPeriodAccessor(_query))
                };

        public sealed class AccountAccessor : IStorageBasedDataObjectAccessor<Account>
        {
            private readonly IQuery _query;

            public AccountAccessor(IQuery query)
            {
                _query = query;
            }

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

        public sealed class AccountPeriodAccessor : IStorageBasedDataObjectAccessor<AccountPeriod>
        {
            private readonly IQuery _query;

            public AccountPeriodAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<AccountPeriod> GetSource()
            {
                var releaseWithdrawalPeriods =
                    from releaseWithdrawal in _query.For<Facts::ReleaseWithdrawal>()
                    join orderPosition in _query.For<Facts::OrderPosition>() on releaseWithdrawal.OrderPositionId equals orderPosition.Id
                    join order in _query.For<Facts::Order>() on orderPosition.OrderId equals order.Id
                    from account in _query.For<Facts::Account>().Where(x => x.LegalPersonId == order.LegalPersonId && x.BranchOfficeOrganizationUnitId == order.BranchOfficeOrganizationUnitId)
                    select new { AccountId = account.Id, releaseWithdrawal.Start, releaseWithdrawal.Amount, Type = 1 };

                var lockPeriods =
                    from @lock in _query.For<Facts::Lock>()
                    join account in _query.For<Facts::Account>() on @lock.AccountId equals account.Id
                    select new { AccountId = account.Id, @lock.Start, @lock.Amount, Type = 3 };

                var lockSums =
                    from @lock in _query.For<Facts::Lock>().GroupBy(x => x.AccountId)
                    select new { AccountId = @lock.Key, Sum = @lock.Select(x => x.Amount).Sum() };

                var result =
                    from item in releaseWithdrawalPeriods.Concat(lockPeriods).GroupBy(a => new { a.AccountId, a.Start })
                    join account in _query.For<Facts::Account>() on item.Key.AccountId equals account.Id
                    from sum in lockSums.Where(x => x.AccountId == item.Key.AccountId).DefaultIfEmpty()
                    select new AccountPeriod
                        {
                            AccountId = item.Key.AccountId,
                            Start = item.Key.Start,
                            Balance = account.Balance,
                            End = item.Key.Start.AddMonths(1),
                            ReleaseAmount = item.Where(x => x.Type == 1).Select(x => x.Amount).Sum(),
                            LockedAmount = item.Where(x => x.Type == 3).Select(x => x.Amount).Sum(),
                            OwerallLockedAmount = sum.Sum,
                        };

                return result;
            }

            /// <summary>
            /// Хороший способ. Проще, понятнее и более производительный. Не работает из-за бага в linq2db
            /// https://github.com/linq2db/linq2db/issues/395
            /// </summary>
            /// <returns></returns>
            public IQueryable<AccountPeriod> GetSourceDisabled()
            {
                var releaseWithdrawals =
                    from releaseWithdrawal in _query.For<Facts::ReleaseWithdrawal>()
                    join orderPosition in _query.For<Facts::OrderPosition>() on releaseWithdrawal.OrderPositionId equals orderPosition.Id
                    join order in _query.For<Facts::Order>() on orderPosition.OrderId equals order.Id
                    from account in _query.For<Facts::Account>().Where(x => x.LegalPersonId == order.LegalPersonId && x.BranchOfficeOrganizationUnitId == order.BranchOfficeOrganizationUnitId)
                    select new { AccountId = account.Id, Start = releaseWithdrawal.Start, ReleaseWithdrawal = releaseWithdrawal.Amount, Lock = 0M };

                var locks =
                    from @lock in _query.For<Facts::Lock>()
                    join account in _query.For<Facts::Account>() on @lock.AccountId equals account.Id
                    select new { AccountId = account.Id, Start = @lock.Start, ReleaseWithdrawal = 0M, Lock = @lock.Amount };

                var result =
                    from item in releaseWithdrawals.Union(locks).Select(x => new { x.AccountId, x.Start, x.Lock, x.ReleaseWithdrawal }).GroupBy(a => new { a.AccountId, a.Start })
                    select new AccountPeriod
                        {
                            AccountId = item.Key.AccountId,
                            Start = item.Key.Start,
                            End = item.Key.Start.AddMonths(1),
                            ReleaseAmount = item.Select(x => x.ReleaseWithdrawal).Sum(),
                            LockedAmount = item.Select(x => x.Lock).Sum(),
                        };

                return result;
            }

            public FindSpecification<AccountPeriod> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<AccountPeriod>(x => aggregateIds.Contains(x.AccountId));
            }
        }
    }
}