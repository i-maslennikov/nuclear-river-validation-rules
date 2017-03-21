using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.DataAccess
{
    internal static class ValidationResultExtensions
    {
        internal static IQueryable<Version.ValidationResult> ForPeriod(this IQueryable<Version.ValidationResult> query, DateTime start, DateTime end)
        {
            return query.Where(x => x.PeriodStart < end && start < x.PeriodEnd);
        }

        internal static IQueryable<Version.ValidationResult> ForOrdersOrProject(this IQueryable<Version.ValidationResult> query, IReadOnlyCollection<long> orderIds, long? projectId)
        {
            return query.Where(x => x.OrderId.HasValue && orderIds.Contains(x.OrderId.Value) || x.ProjectId.HasValue && x.ProjectId == projectId);
        }

        internal static IReadOnlyCollection<Message> ToMessages(this IEnumerable<Version.ValidationResult> query, ResultType resultType)
        {
            var resultMap = ResultTypeMap.Map[resultType];

            return query.Aggregate(new List<Message>(), (list, validationResult) =>
            {
                Result result;
                if (resultMap.TryGetValue((MessageTypeCode)validationResult.MessageType, out result))
                {
                    list.Add(validationResult.ToMessage(result));
                }

                return list;
            });
        }

        internal static Message ToMessage(this Version.ValidationResult x, Result result)
            => new Message
                {
                    MessageType = (MessageTypeCode)x.MessageType,
                    References = ParseReferences(x.MessageParams),
                    Extra = ParseExtra(x.MessageParams),
                    OrderId = x.OrderId,
                    ProjectId = x.ProjectId,

                    Result = result,
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