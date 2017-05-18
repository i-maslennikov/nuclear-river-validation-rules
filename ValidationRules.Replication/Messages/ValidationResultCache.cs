using System;
using System.Collections.Generic;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Messages
{
    internal sealed class ValidationResultCache
    {
        private CacheRecord _record;
        private static readonly Lazy<ValidationResultCache> Singleton = new Lazy<ValidationResultCache>(() => new ValidationResultCache());

        private ValidationResultCache()
        {
        }

        public static ValidationResultCache Instance => Singleton.Value;

        public bool TryGet(long key, out IReadOnlyCollection<Version.ValidationResult> value)
        {
            var record = _record;
            value = record?.Key == key ? record.Value : null;
            return value != null;
        }

        public void Put(long key, IReadOnlyCollection<Version.ValidationResult> value)
        {
            _record = new CacheRecord(key, new HashSet<Version.ValidationResult>(value));
        }

        public void ApplyPatch(IReadOnlyCollection<Version.ValidationResult> added, IReadOnlyCollection<Version.ValidationResult> removed)
        {
            var record = _record;
            var patched = new HashSet<Version.ValidationResult>(record.Value);
            patched.UnionWith(added);
            patched.ExceptWith(removed);
            _record = new CacheRecord(record.Key + 1, patched);
        }

        private class CacheRecord
        {
            public CacheRecord(long key, IReadOnlyCollection<Version.ValidationResult> value)
            {
                Key = key;
                Value = value;
            }

            public long Key { get; }

            public IReadOnlyCollection<Version.ValidationResult> Value { get; }
        }
    }
}