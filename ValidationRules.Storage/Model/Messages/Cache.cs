using System;
using System.Xml.Linq;

namespace NuClear.ValidationRules.Storage.Model.Messages
{
    public sealed class Cache
    {
        public sealed class ValidationResult
        {
            public MessageTypeCode MessageType { get; set; }
            public byte[] Data { get; set; }
        }
    }
}