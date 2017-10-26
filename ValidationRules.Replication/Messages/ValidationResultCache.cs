using System;
using System.Collections.Generic;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Messages
{
    internal sealed class ValidationResultCache
    {
        private static readonly Lazy<ValidationResultCache> Singleton = new Lazy<ValidationResultCache>(() => new ValidationResultCache());

        private ValidationResultCache()
        {
        }

        public static ValidationResultCache Instance => Singleton.Value;

        public bool TryGet(long key, out IReadOnlyCollection<Version.ValidationResult> value)
        {
            value = null;
            return false;
        }

        public void Put(long key, IReadOnlyCollection<Version.ValidationResult> value)
        {
        }

        public void ApplyPatch(IReadOnlyCollection<Version.ValidationResult> added, IReadOnlyCollection<Version.ValidationResult> removed)
        {
        }
    }
}