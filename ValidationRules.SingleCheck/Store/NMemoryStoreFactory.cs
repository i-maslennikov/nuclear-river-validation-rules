using System;

using LinqToDB.Mapping;

using NMemory;

using NuClear.Replication.Core;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.SingleCheck.FieldComparer;
using NuClear.ValidationRules.Storage;

namespace NuClear.ValidationRules.SingleCheck.Store
{
    public sealed class NMemoryStoreFactory : IStoreFactory
    {
        public static readonly Lazy<MappingSchema> MappingSchema =
            new Lazy<MappingSchema>(() => new MappingSchema(Schema.Facts, Schema.Aggregates));

        public static readonly Lazy<EqualityComparerFactory> EqualityComparerFactory =
            new Lazy<EqualityComparerFactory>(() => new EqualityComparerFactory(new LinqToDbPropertyProvider(MappingSchema.Value), new DateTimeComparer()));

        private readonly NMemoryQuery _store;

        public NMemoryStoreFactory()
        {
            _store = new NMemoryQuery(new Database(), new NMemoryTableRegistrar(MappingSchema.Value), EqualityComparerFactory.Value);
        }

        public IStore CreateStore()
            => _store;

        public IQuery CreateQuery()
            => _store;
    }
}