using System.Collections.Generic;

using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.DataAccess
{
    public sealed class Message
    {
        public MessageTypeCode MessageType { get; set; }
        public IReadOnlyCollection<Reference> References { get; set; }
        public IReadOnlyDictionary<string, string> Extra { get; set; }
        public Result Result { get; set; }

        public long? OrderId { get; set; }
        public long? ProjectId { get; set; }
    }
}