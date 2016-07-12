using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.User.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.User
{
    public sealed class UserProfileAccessor : IStorageBasedDataObjectAccessor<UserProfile>, IDataChangesHandler<UserProfile>
    {
        private readonly IQuery _query;

        public UserProfileAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<UserProfile> GetSource()
            => from profile in _query.For(Specs.Find.Erm.UserProfiles())
               // timeZone - предположительно неизменяемый в erm объект, поэтому не тащим в факты. Изменения всешда будет проходить через профиль.
               join timeZone in _query.For<Erm::TimeZone>() on profile.TimeZoneId equals timeZone.Id
               select new UserProfile
                   {
                       Id = profile.Id,
                       UserId = profile.UserId,
                       TimeZoneId = timeZone.TimeZoneId,
               };

        public FindSpecification<UserProfile> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<UserProfile>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<UserProfile> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<UserProfile> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<UserProfile> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<UserProfile> dataObjects)
            => Array.Empty<IEvent>();
    }
}