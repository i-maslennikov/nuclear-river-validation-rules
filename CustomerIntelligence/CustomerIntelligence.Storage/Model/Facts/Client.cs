using System;

namespace NuClear.CustomerIntelligence.Storage.Model.Facts
{
    public sealed class Client
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset? LastDisqualifiedOn { get; set; }

        public bool HasPhone { get; set; }

        public bool HasWebsite { get; set; }
    }
}