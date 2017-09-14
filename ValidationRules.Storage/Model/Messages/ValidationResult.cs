using System;
using System.Xml.Linq;

namespace NuClear.ValidationRules.Storage.Model.Messages
{
    public sealed class Version
    {
        public long Id { get; set; }
        public DateTime UtcDateTime { get; set; }

        public sealed class ErmState
        {
            public long VersionId { get; set; }
            public Guid Token { get; set; }
            public DateTime UtcDateTime { get; set; }
        }

        public sealed class ErmStateBulkDelete
        {
            public long VersionId { get; set; }
        }

        public sealed class AmsState
        {
            public long VersionId { get; set; }
            public long Offset { get; set; }
            public DateTime UtcDateTime { get; set; }
        }

        public sealed class AmsStateBulkDelete
        {
            public long VersionId { get; set; }
        }

        public sealed class ValidationResult
        {
            public long VersionId { get; set; }

            public int MessageType { get; set; }
            public XDocument MessageParams { get; set; }

            public DateTime PeriodStart { get; set; }
            public DateTime PeriodEnd { get; set; }

            public long? ProjectId { get; set; }
            public long? OrderId { get; set; }

            public bool Resolved { get; set; }
        }

        public sealed class ValidationResultBulkDelete
        {
            public long VersionId { get; set; }
        }
    }
}
