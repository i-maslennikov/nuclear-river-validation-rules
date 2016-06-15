using System;
using System.Xml.Linq;

namespace NuClear.ValidationRules.Storage.Model.Messages
{
    public sealed class Version
    {
        public long Id { get; set; }

        public sealed class ErmState
        {
            public long VersionId { get; set; }
            public Guid Token { get; set; }
        }

        public sealed class ValidationResult
        {
            public long VersionId { get; set; }

            public int MessageType { get; set; }
            public XDocument MessageParams { get; set; }

            public DateTime PeriodStart { get; set; }
            public DateTime PeriodEnd { get; set; }
            public long ProjectId { get; set; }

            public int ReferenceType { get; set; }
            public long ReferenceId { get; set; }

            public int Result { get; set; }
        }

        public sealed class ValidationResultForBulkDelete
        {
            public int MessageType { get; set; }
            public long VersionId { get; set; }
        }
    }
}
