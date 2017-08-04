using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ValidationRules.Replication.Comparison.Tests.ErmService
{
    internal sealed class ErmToRiverResultAdapter
    {
        private readonly OrderValidationApplicationServiceClient _ermService;

        public ErmToRiverResultAdapter(string endpointConfigurationName)
        {
            _ermService = new OrderValidationApplicationServiceClient(endpointConfigurationName);
        }

        public IReadOnlyCollection<ErmOrderValidationMessage> ValidateSingle(long orderId)
        {
            var validationResult = _ermService.ValidateSingleOrder(new ValidateSingleOrderRequest(orderId)).ValidateSingleOrderResult;
            return Format(validationResult.Messages).ToArray();
        }

        public IReadOnlyCollection<ErmOrderValidationMessage> ValidateSingleForCancel(long orderId)
        {
            var validationResult = _ermService.ValidateSingleOrderStateChange(new ValidateSingleOrderStateChangeRequest(orderId, 4)).ValidateSingleOrderStateChangeResult;
            return Format(validationResult.Messages).ToArray();
        }

        public ErmValidationResult ValidateMassManualWithAccounts(long organizationUnitId, DateTime releaseDate)
        {
            var request = new ValidateOrdersRequest(ValidationType.ManualReportWithAccountsCheck,
                                                    organizationUnitId,
                                                    new TimePeriod { Start = releaseDate, End = releaseDate.AddMonths(1).AddSeconds(-1) },
                                                    null,
                                                    false);

            var validationResult = _ermService.ValidateOrders(request).ValidateOrdersResult;
            validationResult.Messages = Format(validationResult.Messages).ToArray();
            return validationResult;
        }

        public ErmValidationResult ValidateMassRelease(long organizationUnitId, DateTime releaseDate)
        {
            var request = new ValidateOrdersRequest(ValidationType.PreReleaseFinal,
                                                    organizationUnitId,
                                                    new TimePeriod { Start = releaseDate, End = releaseDate.AddMonths(1).AddSeconds(-1) },
                                                    null,
                                                    false);

            var validationResult = _ermService.ValidateOrders(request).ValidateOrdersResult;
            validationResult.Messages = Format(validationResult.Messages).ToArray();
            return validationResult;
        }

        private static IEnumerable<ErmOrderValidationMessage> Format(IReadOnlyCollection<ErmOrderValidationMessage> messages)
        {
            var filter1 = FormatLinkingObjectsOrderValidationRule(messages).ToList();
            var filter2 = FormatOrderHasActiveLegalDetailsOrderValidationRule(filter1).ToList();
            var finalFilter = FormatAdvertisementAmountOrderValidationRule(filter2);

            return finalFilter;
        }

        // ERM выдаёт несколько ошибок по адресам фирм, мы их схлапываем в одну (самую важную)
        // Согласовано с Антоном
        private static IEnumerable<ErmOrderValidationMessage> FormatLinkingObjectsOrderValidationRule(IReadOnlyCollection<ErmOrderValidationMessage> messages)
        {
            const int LinkingObjectsOrderValidationRule = 12;
            var parser = new AddressMessage();

            var competitors = messages.Where(x => x.RuleCode == LinkingObjectsOrderValidationRule && parser.IsSupported(x.MessageText)).ToList();
            var winners = competitors
                .GroupBy(x => new { x.TargetEntityTypeCode, x.TargetEntityId, Address = parser.GetParams(x.MessageText) }, x => x)
                .Select(x => x.OrderBy(y => parser.GetPriority(y.MessageText)).First());

            return messages
                .Except(competitors)
                .Concat(winners);
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
                .Select(x =>
                            {
                                if (x.RuleCode == OrderHasActiveLegalDetailsOrderValidationRule)
                                {
                                    if (searchRegex.IsMatch(x.MessageText))
                                    {
                                        x.MessageText = x.MessageText.Replace(ReplaceKey, ReplaceValue);
                                    }
                                }

                                return x;
                            });

            return processed;
        }

        private static IEnumerable<ErmOrderValidationMessage> FormatAdvertisementAmountOrderValidationRule(IReadOnlyCollection<ErmOrderValidationMessage> messages)
        {
            const int AdvertisementAmountOrderValidationRule = 26;
            var regex1 = new Regex(@"\(оформлено - \d*, содержит ошибки - \d*\)");
            var regex2 = new Regex(@"\sVIP");
            var regex3 = new Regex(@"\s\(с видео\)");

            var processed = messages
                .Select(x =>
                            {
                                if (x.RuleCode == AdvertisementAmountOrderValidationRule)
                                {
                                    // ERM выдаёт дополнительные сообщения об ошибках, которые VR не выдаёт
                                    if (regex1.IsMatch(x.MessageText))
                                    {
                                        return null;
                                    }

                                    // ERM не совсем правильно выбирает имя категории номенклатуры, корректировка на 'VIP'
                                    x.MessageText = regex2.Replace(x.MessageText, string.Empty);
                                    x.MessageText = regex3.Replace(x.MessageText, string.Empty);
                                    x.MessageText = x.MessageText.Replace("August", "Август");
                                    x.MessageText = x.MessageText.Replace("September", "Сентябрь");
                                    x.MessageText = x.MessageText.Replace("October", "Октябрь");
                                    x.MessageText = x.MessageText.Replace("November", "Ноябрь");
                                    x.MessageText = x.MessageText.Replace("December", "Декабрь");
                                }

                                return x;
                            })
                .Where(x => x != null);

            return processed;
        }

        private class AddressMessage
        {
            private static readonly Regex[] SupportedMessages = new[]
                {
                    "В позиции (.+) найден адрес (.+), не принадлежащий фирме заказа",
                    "В позиции (.+) адрес фирмы (.+) скрыт навсегда",
                    "В позиции (.+) найден неактивный адрес (.+)",
                    "В позиции (.+) адрес фирмы (.+) скрыт до выяснения"
                }.Select(x => new Regex(x)).ToArray();

            public bool IsSupported(string message)
                => SupportedMessages.Any(regex => regex.Match(message).Success);

            public Tuple<string, string> GetParams(string message)
            {
                var matched = SupportedMessages.Select(regex => regex.Match(message)).First(regex => regex.Success);
                return Tuple.Create(matched.Groups[1].Value, matched.Groups[2].Value);
            }

            public int GetPriority(string message)
            {
                var matched = SupportedMessages.Select((regex, i) => new { regex.Match(message).Success, i }).First(x => x.Success);
                return matched.i;
            }
        }
    }
}