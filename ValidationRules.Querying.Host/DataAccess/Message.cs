using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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

    public static class MessageExtensions
    {
        public static IEnumerable<Message> ToMessages(this IEnumerable<Version.ValidationResult> query, ResultType resultType)
            => query.Select(x => ToMessage(x, resultType));

        public static Message ToMessage(Version.ValidationResult x, ResultType resultType)
            => new Message
                {
                    MessageType = (MessageTypeCode)x.MessageType,
                    References = ParseReferences(x.MessageParams),
                    Extra = ParseExtra(x.MessageParams),
                    Result = x.Result.ToResult(resultType),

                    OrderId = x.OrderId,
                    ProjectId = x.ProjectId,
                };

        private static IReadOnlyCollection<Reference> ParseReferences(XDocument messageParams)
            => messageParams.Root.Elements().Select(Parse).ToList();

        private static IReadOnlyDictionary<string, string> ParseExtra(XDocument messageParams)
            => messageParams.Root.Attributes().ToDictionary(x => x.Name.LocalName, x => x.Value);

        private static Reference Parse(XElement element)
            => new Reference(
                (int)element.Attribute("type"),
                (long)element.Attribute("id"),
                element.Elements().Select(Parse).ToArray());

    }
}