using System;

namespace NuClear.CustomerIntelligence.Storage.Model.Erm
{
    public sealed class Firm
    {
        public Firm()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset? LastDisqualifyTime { get; set; }

        public long? ClientId { get; set; }

        public long OrganizationUnitId { get; set; }

        public long OwnerId { get; set; }

        public bool ClosedForAscertainment { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}