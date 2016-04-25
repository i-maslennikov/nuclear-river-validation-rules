using System;

namespace NuClear.CustomerIntelligence.Storage.Model.CI
{
    public sealed class Firm
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset? LastDisqualifiedOn { get; set; }

        public DateTimeOffset? LastDistributedOn { get; set; }

        public bool HasPhone { get; set; }

        public bool HasWebsite { get; set; }

        public int AddressCount { get; set; }

        public long CategoryGroupId { get; set; }

        public long? ClientId { get; set; }

        public long ProjectId { get; set; }

        public long OwnerId { get; set; }
    }
}