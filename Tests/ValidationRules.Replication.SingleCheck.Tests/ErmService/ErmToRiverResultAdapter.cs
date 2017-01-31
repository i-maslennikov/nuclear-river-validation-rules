using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ValidationRules.Replication.SingleCheck.Tests.ErmService
{
    internal sealed class ErmToRiverResultAdapter
    {
        private readonly OrderValidationApplicationServiceClient _ermService;

        public ErmToRiverResultAdapter(string endpointConfigurationName)
        {
            _ermService = new OrderValidationApplicationServiceClient(endpointConfigurationName);
        }

        public ErmValidationResult ValidateSingle(long orderId)
        {
            var validationResult = _ermService.ValidateSingleOrder(new ValidateSingleOrderRequest(orderId)).ValidateSingleOrderResult;
            validationResult.Messages = Format(validationResult.Messages).ToArray();
            return validationResult;
        }

        private static IEnumerable<ErmOrderValidationMessage> Format(IReadOnlyCollection<ErmOrderValidationMessage> messages)
        {
            return FormatLinkingObjectsOrderValidationRule(messages).Concat(
                   FormatOrderHasActiveLegalDetailsOrderValidationRule(messages));
        }

        // ERM выдаёт несколько ошибок по адресам фирм, мы их схлапываем в одну (самую важную)
        // Согласовано с Антоном
        private static IEnumerable<ErmOrderValidationMessage> FormatLinkingObjectsOrderValidationRule(IReadOnlyCollection<ErmOrderValidationMessage> messages)
        {
            const int LinkingObjectsOrderValidationRule = 12;
            var messagestoAggregate = new []
            {
                "В позиции {0} найден адрес {1}, не принадлежащий фирме заказа",
                "В позиции {0} адрес фирмы {1} скрыт навсегда",
                "В позиции {0} найден неактивный адрес {1}",
                "В позиции {0} адрес фирмы {1} скрыт до выяснения"
            };

            var groups = messages
                .Where(x => x.RuleCode == LinkingObjectsOrderValidationRule)
                .GroupBy(x => new { x.TargetEntityTypeCode, x.TargetEntityId });

            var toRemove = groups.Aggregate(new List<ErmOrderValidationMessage>(), (list, @group) =>
            {
                var messagesToRemove = @group.Select(message => new
                {
                    Index = message.MessageText.IndexOfMatched(messagestoAggregate),
                    Message = message,
                })
                .Where(x => x.Index != -1)
                .OrderBy(x => x.Index)
                .Skip(1)
                .Select(x => x.Message);

                list.AddRange(messagesToRemove);

                return list;
            });

            return messages.Except(toRemove);
        }

        // ERM выдаёт ошибку неактивности и для юр.лица исполнителя и для юр. лица организации
        // River выдаёт ошибку только для юр.лица исполнителя и это нормально
        private static IEnumerable<ErmOrderValidationMessage> FormatOrderHasActiveLegalDetailsOrderValidationRule(IReadOnlyCollection<ErmOrderValidationMessage> messages)
        {
            const int OrderHasActiveLegalDetailsOrderValidationRule = 52;
            var searchRegex = new Regex("Заказ ссылается на неактивные объекты: ");
            const string ReplaceKey = " Юридическое лицо исполнителя, Юр. лицо организации";
            const string ReplaceValue = " Юр. лицо исполнителя";

            var processed = messages
                .Where(x => x.RuleCode == OrderHasActiveLegalDetailsOrderValidationRule)
                .Select(x =>
                            {
                                if (searchRegex.IsMatch(x.MessageText))
                                {
                                    x.MessageText = x.MessageText.Replace(ReplaceKey, ReplaceValue);
                                }

                                return x;
                            });

            return processed;
        }
    }

    internal static class StringExtensions
    {
        public static int IndexOfMatched(this string input, IEnumerable<string> resourcePatterns)
        {
            var patterns = resourcePatterns.Select(x => Regex.Replace(x, @"({)*\d*}(})*", "(.*)")).ToList();

            for (var i = 0; i < patterns.Count; i++)
            {
                if (Regex.IsMatch(input, patterns[i]))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
