using System;

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
            public string MessageParams { get; set; }
            public int Result { get; set; }

            public long OrderId { get; set; }
            public DateTime PeriodStart { get; set; }
            public DateTime PeriodEnd { get; set; }
            public long ProjectId { get; set; }
        }
    }
}
