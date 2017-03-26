using System;

using LinqToDB.Mapping;

using NuClear.Replication.Core;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.SingleCheck.FieldComparer;
using NuClear.ValidationRules.Storage;

namespace NuClear.ValidationRules.SingleCheck.Store
{
    public sealed class HashSetStoreFactory : IStoreFactory
    {
        public static readonly Lazy<MappingSchema> MappingSchema =
            new Lazy<MappingSchema>(() => new MappingSchema(Schema.Erm, Schema.Facts, Schema.Aggregates, Schema.Messages));

        public static readonly Lazy<EqualityComparerFactory> EqualityComparerFactory =
            new Lazy<EqualityComparerFactory>(() => new EqualityComparerFactory(new LinqToDbPropertyProvider(MappingSchema.Value), new XDocumentComparer(), new DateTimeComparer()));

        private readonly HashSetStore _store;

        public HashSetStoreFactory()
        {
            _store = new HashSetStore(EqualityComparerFactory.Value);
        }

        public IStore CreateStore()
        {
            return _store;
        }

        public IQuery CreateQuery()
        {
            return _store;
        }

        public void Dispose()
        {
        }
    }
}