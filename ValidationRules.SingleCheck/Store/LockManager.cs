using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using LinqToDB;
using LinqToDB.Data;

using NuClear.ValidationRules.Storage;
using NuClear.ValidationRules.Storage.Model.WebApp;

namespace NuClear.ValidationRules.SingleCheck.Store
{
    public sealed class LockManager
    {
        private static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(1);
        private int RetryCount = 10;

        public Lock GetLock()
        {
            return Retry(GetLockInternal, RetryCount);
        }

        public IReadOnlyCollection<Lock> GetAllLocks()
        {
            return Retry(GetAllLocksInternal, RetryCount);
        }

        public void Release(Lock @lock)
        {
            using (var scope = CreateTransaction())
            using (var db = CreateConnection())
            {
                @lock.InUse = false;
                db.Update(@lock);
                scope.Complete();
            }
        }

        private Lock GetLockInternal()
        {
            using (var scope = CreateTransaction())
            using (var db = CreateConnection())
            {
                var result = db.GetTable<Lock>().OrderBy(x => x.Expires).FirstOrDefault(x => !x.InUse || x.Expires <= DateTime.UtcNow);
                if (result == null)
                {
                    var id = (db.GetTable<Lock>().Max(x => (long?)x.Id) ?? 0) + 1;
                    result = new Lock { Id = id, InUse = true, Expires = DateTime.UtcNow + LockDuration, IsNew = true };
                    db.Insert(result);
                }
                else
                {
                    result.InUse = true;
                    result.Expires = DateTime.UtcNow + LockDuration;
                    db.Update(result);
                }

                scope.Complete();
                return result;
            }
        }

        private IReadOnlyCollection<Lock> GetAllLocksInternal()
        {
            using (var scope = CreateTransaction())
            using (var db = CreateConnection())
            {
                var result = db.GetTable<Lock>().ToArray();
                var activeLocks = result.Where(x => x.InUse && x.Expires > DateTime.UtcNow).ToArray();

                if (activeLocks.Any())
                {
                    var activeLock = activeLocks.OrderByDescending(x => x.Expires).First();
                    var releaseTime = activeLock.Expires - DateTime.UtcNow;
                    throw new InvalidOperationException($"Lock {activeLock.Id} is busy till {activeLock.Expires} and will be available after {releaseTime}. Try later.");
                }

                foreach (var @lock in result)
                {
                    @lock.InUse = true;
                    @lock.Expires = DateTime.UtcNow + LockDuration;
                    db.Update(@lock);
                }

                scope.Complete();
                return result;
            }
        }

        private T Retry<T>(Func<T> action, int times)
        {
            var exceptions = new List<Exception>();

            for (var i = 0; i < times; i++)
            {
                try
                {
                    var result = action.Invoke();
                    return result;
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            throw new AggregateException("Can not acquire lock for pooled table set", exceptions);
        }

        private TransactionScope CreateTransaction()
            => new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot, Timeout = TimeSpan.Zero });

        private DataConnection CreateConnection()
            => new DataConnection("Messages").AddMappingSchema(Schema.WebApp);
    }
}