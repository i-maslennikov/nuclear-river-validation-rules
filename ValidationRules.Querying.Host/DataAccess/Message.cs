using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.DataAccess
{
    public sealed class Message
    {
        public MessageTypeCode MessageType { get; set; }
        public XDocument Xml { get; set; }

        public Result Result { get; set; }
    }

    public static class MessageExtensions
    {
        public static IEnumerable<Message> ToMessages(this IEnumerable<Version.ValidationResult> query, ResultType resultType)
        {
            return query.Select(x => new Message
            {
                MessageType = (MessageTypeCode)x.MessageType,
                Xml = x.MessageParams,
                Result = x.Result.ToResult(resultType)
            });
        }
    }
}