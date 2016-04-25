using System;

namespace NuClear.CustomerIntelligence.Storage.Model.Erm
{
    public sealed class Client
    {
        public Client()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset? LastDisqualifyTime { get; set; }

        public string MainPhoneNumber { get; set; }

        public string AdditionalPhoneNumber1 { get; set; }

        public string AdditionalPhoneNumber2 { get; set; }

        public string Website { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}