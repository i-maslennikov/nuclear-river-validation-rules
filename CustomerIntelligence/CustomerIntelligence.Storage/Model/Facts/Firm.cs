using System;

namespace NuClear.CustomerIntelligence.Storage.Model.Facts
{
    public sealed class Firm
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset? LastDisqualifiedOn { get; set; }

        public long? ClientId { get; set; }

        public long OrganizationUnitId { get; set; }

        public long OwnerId { get; set; }
    }
}