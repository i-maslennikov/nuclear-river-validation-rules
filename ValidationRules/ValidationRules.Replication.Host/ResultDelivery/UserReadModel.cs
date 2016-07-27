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
        private readonly IQuery _query;

        public UserReadModel(IQuery query)
        {
            _query = query;
        }

        public IReadOnlyCollection<ValidationMessageRequest> GetCurrentIterationRequest(
            Expression<Func<Storage.Model.Erm.User, bool>> subscriptionFilter,
            Expression<Func<Storage.Model.Erm.TimeZone, bool>> timeZoneFilter)
        {
            var query = from user in _query.For(Specs.Find.Erm.Users()).Where(subscriptionFilter)
                        join profile in _query.For(Specs.Find.Erm.UserProfiles()) on user.Id equals profile.UserId
                        join timeZone in _query.For(Specs.Find.Erm.TimeZones()).Where(timeZoneFilter) on profile.TimeZoneId equals timeZone.Id
                        join order in _query.For(Specs.Find.Erm.Orders()) on user.Id equals order.OwnerCode
                        let release = _query.For(Specs.Find.Erm.ReleaseInfos()).Where(x => x.OrganizationUnitId == order.DestOrganizationUnitId).OrderByDescending(x => x.PeriodStartDate).First()
                        select new ValidationMessageRequest
                            {
                                UserAccount = user.Account,
                                OrderId = order.Id,
                                PeriodStart = release.PeriodStartDate.AddMonths(1)
                            };

            return query.ToArray();
        }
    }
}
