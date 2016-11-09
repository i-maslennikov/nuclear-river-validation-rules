using System;
using System.Xml.Linq;

namespace NuClear.ValidationRules.WebApp.Entity
{
    public class ValidationResult
    {
        public long VersionId { get; set; }

        public int MessageType { get; set; }
        public XDocument MessageParams { get; set; }

        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public long? OrderId { get; set; }
        public long? ProjectId { get; set; }

        public int Result { get; set; }
        public bool Resolved { get; set; }
    }
}
