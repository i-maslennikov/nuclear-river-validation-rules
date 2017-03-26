using System;
using System.Transactions;

using LinqToDB.Data;
using LinqToDB.Mapping;

using NuClear.Replication.Core;
using NuClear.Storage.API.Readings;
using NuClear.Telemetry.Probing;
using NuClear.ValidationRules.Storage;

namespace NuClear.ValidationRules.SingleCheck.Store
{
    public sealed class ErmStoreFactory : IStoreFactory
    {
        public static readonly Lazy<MappingSchema> Erm =
            new Lazy<MappingSchema>(() => Schema.Erm);

        public static readonly Lazy<EqualityComparerFactory> EqualityComparerFactory =
            new Lazy<EqualityComparerFactory>(() => new EqualityComparerFactory(new LinqToDbPropertyProvider(Erm.Value)));

        private readonly long _orderId;

        public ErmStoreFactory(long orderId)
        {
            _orderId = orderId;
        }

        public IStore CreateStore()
        {
            throw new NotSupportedException("Erm is read only");
        }

        public IQuery CreateQuery()
        {
            // Есть принципиально другой путь - не создать данные в памяти, а только обвещать "родной" IQuery фильтрами, чтобы он не видел лишних сущностей.
            // Возможно, хоть и не факт, что это позволит сэкономить одну секунду. Вряд ли больше.
            using(Probe.Create("Read erm"))
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
            using (var connection = new DataConnection("Erm").AddMappingSchema(Erm.Value))
            {
                var store = new HashSetStore(EqualityComparerFactory.Value);
                ErmDataLoader.Load(_orderId, connection, store);
                return store;
            }
        }

        public void Dispose()
        {
        }
    }
}