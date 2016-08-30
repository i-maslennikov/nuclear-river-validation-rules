using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.ValidationRules.Replication.Host.ResultDelivery.Slack;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery
{
    public sealed class ResultDeliveryService
    {
        private readonly ITransportDecorator _transport;
        private readonly UserReadModel _userReadModel;
        private readonly ResultReadModel _resultReadModel;
        private readonly UnityLocalizedMessageFactory _unityLocalizedMessageFactory;
        private readonly ResultDeliverySettingsAspect _settings;

        public ResultDeliveryService(ITransportDecorator transport, UserReadModel userReadModel, ResultReadModel resultReadModel, UnityLocalizedMessageFactory unityLocalizedMessageFactory, ResultDeliverySettingsAspect settings)
        {
            _transport = transport;
            _userReadModel = userReadModel;
            _resultReadModel = resultReadModel;
            _unityLocalizedMessageFactory = unityLocalizedMessageFactory;
            _settings = settings;
        }

        public void Execute(DateTime iterationTime)
        {
            var currentIteration = _userReadModel.GetCurrentIterationRequest(GetSubscribedUsersFilter(), GetCurrentIterationTimeZonesFilter(iterationTime));

            foreach (var user in currentIteration.GroupBy(x => x.UserAccount, x => Tuple.Create(x.OrderId, x.PeriodStart)))
            {
                var messages = _resultReadModel.GetResults(user)
                                               .Select(_unityLocalizedMessageFactory.Localize)
                                               .Where(x => x.Result >= Result.Info)
                                               .OrderByDescending(x => x.Result)
                                               .ToArray();

                _transport.SendMessage(user.Key, messages);
            }
        }

        private Expression<Func<Storage.Model.Erm.User, bool>> GetSubscribedUsersFilter()
        {
            var users = GetSubscribedUsers();
            return x => users.Contains(x.Account);
        }

        private IReadOnlyCollection<string> GetSubscribedUsers()
        {
            //return _transport.GetSubscribedUsers();
            return SlackTransportDecorator.UserMap.Keys.ToArray();
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
    }
}