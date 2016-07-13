using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.Specifications;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery
{
    public sealed class UserReadModel
    {
        private const int DeliveryHour = 9;

        private readonly IQuery _query;
        private readonly SlackDecorator _slackService;

        public UserReadModel(IQuery query, SlackDecorator slackService)
        {
            _query = query;
            _slackService = slackService;
        }

        public IReadOnlyCollection<ResultRequest> GetCurrentIteration(DateTime iterationTime)
        {
            var subscribedUsersFilter = GetSubscribedUsersFilter();
            var timeZonesFilter = GetCurrentIterationTimeZonesFilter(iterationTime);

            var query = from user in _query.For(Specs.Find.Erm.Users()).Where(subscribedUsersFilter)
                        join profile in _query.For(Specs.Find.Erm.UserProfiles()) on user.Id equals profile.UserId
                        join timeZone in _query.For(Specs.Find.Erm.TimeZones()).Where(timeZonesFilter) on profile.TimeZoneId equals timeZone.Id
                        join order in _query.For(Specs.Find.Erm.Orders()) on user.Id equals order.OwnerCode
                        let release = _query.For(Specs.Find.Erm.ReleaseInfos()).Where(x => x.OrganizationUnitId == order.DestOrganizationUnitId).OrderByDescending(x => x.PeriodStartDate).First()
                        select new ResultRequest
                            {
                                UserAccount = user.Account,
                                OrderId = order.Id,
                                PeriodStart = release.PeriodStartDate.AddMonths(1)
                            };

            return query.ToArray();
        }

        private Expression<Func<Storage.Model.Erm.User, bool>> GetSubscribedUsersFilter()
        {
            //return x => _slackService.GetUsers().Contains(x.Account);
            return x => true;
        }

        private Expression<Func<Storage.Model.Erm.TimeZone, bool>> GetCurrentIterationTimeZonesFilter(DateTime iterationTime)
        {
            //var timeZones = GetCurrentIterationTimeZones(iterationTime).ToArray();
            //return x => timeZones.Contains(x.TimeZoneId);
            return x => true;

        }
        private IEnumerable<string> GetCurrentIterationTimeZones(DateTime iterationTime)
        {
            var timeZones = from profile in _query.For(Specs.Find.Erm.UserProfiles())
                            join timeZone in _query.For(Specs.Find.Erm.TimeZones()) on profile.TimeZoneId equals timeZone.Id
                            select timeZone.TimeZoneId;

            foreach (var timeZone in timeZones.ToArray())
            {
                // Можно было бы в базе положить просто смещение относительно utc
                // и получать все профили для отправки в текущей итерации,
                // но ведь есть переводы стрелок, летнее/зименее время:
                // всё это обрабатывается TimeZoneInfo (нужны актуальные патчи :)
                var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                var userLocalHour = TimeZoneInfo.ConvertTimeFromUtc(iterationTime, tz).Hour;
                if (userLocalHour == DeliveryHour)
                {
                    yield return timeZone;
                }
            }
        }
    }
}
