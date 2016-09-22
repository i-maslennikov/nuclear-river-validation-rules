using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery
{
    public sealed class ResultDeliveryService
    {
        private readonly ITransportDecorator _transport;
        private readonly UserReadModel _userReadModel;
        private readonly ResultReadModel _resultReadModel;
        private readonly UnityLocalizedMessageFactory _unityLocalizedMessageFactory;
        private readonly ResultDeliverySettingsAspect _settings;
        private readonly IMessageRedirectionService _redirectionService;

        public ResultDeliveryService(ITransportDecorator transport, UserReadModel userReadModel, ResultReadModel resultReadModel, UnityLocalizedMessageFactory unityLocalizedMessageFactory, ResultDeliverySettingsAspect settings, IMessageRedirectionService redirectionService)
        {
            _transport = transport;
            _userReadModel = userReadModel;
            _resultReadModel = resultReadModel;
            _unityLocalizedMessageFactory = unityLocalizedMessageFactory;
            _settings = settings;
            _redirectionService = redirectionService;
        }

        public void Execute(DateTime iterationTime)
        {
            var currentIteration = _userReadModel.GetCurrentIterationRequest(GetSubscribedUsersFilter(), GetCurrentIterationTimeZonesFilter(iterationTime));

            foreach (var user in currentIteration.GroupBy(x => x.UserAccount, x => Tuple.Create(x.OrderId, x.PeriodStart)))
            {
                var messages = _resultReadModel.GetResults(user)
                                               .Select(_unityLocalizedMessageFactory.Localize)
                                               .Where(x => x != null && x.Result >= Result.Info)
                                               .Distinct(new MessageComparer())
                                               .OrderByDescending(x => x.Result)
                                               .ToArray();

                _transport.SendMessage(_redirectionService.RedirectTo(user.Key), messages);
            }
        }

        private Expression<Func<Storage.Model.Erm.User, bool>> GetSubscribedUsersFilter()
        {
            var users = _transport.GetSubscribedUsers().SelectMany(_redirectionService.RedirectFrom).ToArray();
            return x => users.Contains(x.Account);
        }

        private Expression<Func<Storage.Model.Erm.TimeZone, bool>> GetCurrentIterationTimeZonesFilter(DateTime iterationTime)
        {
            var timeZones = GetCurrentIterationTimeZones(iterationTime).ToArray();
            return x => timeZones.Contains(x.TimeZoneId);
        }

        private IReadOnlyCollection<string> GetCurrentIterationTimeZones(DateTime iterationTime)
        {
            var timeZones = from timeZone in TimeZoneInfo.GetSystemTimeZones()
                            where TimeZoneInfo.ConvertTimeFromUtc(iterationTime, timeZone).Hour == _settings.ResultDeliveryHour
                            select timeZone.Id;

            return timeZones.ToArray();
        }

        /// <summary>
        /// Служит для того, чтобы не выводить пользоветелю два идентичных сообщения.
        /// Идентичные сообщения возможны, например, в случае проверки запрещённых позиций (совпадают с точностью до порядка наименований позиций, поскольку симметричны)
        /// </summary>
        private sealed class MessageComparer : IEqualityComparer<LocalizedMessage>
        {
            public bool Equals(LocalizedMessage x, LocalizedMessage y)
                => x.Result == y.Result && string.Equals(x.Message, y.Message) && string.Equals(x.Header, y.Header);

            public int GetHashCode(LocalizedMessage x)
                => x.Result.GetHashCode() ^ x.Message.GetHashCode() ^ x.Header.GetHashCode();
        }
    }
}