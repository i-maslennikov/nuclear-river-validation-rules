using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery
{
    public interface IMessageRedirectionService
    {
        IEnumerable<string> RedirectFrom(string user);
        string RedirectTo(string user);
    }

    internal sealed class NoMessageRedirection : IMessageRedirectionService
    {
        public IEnumerable<string> RedirectFrom(string user)
            => new[] { user };

        public string RedirectTo(string user)
            => user;
    }

    internal sealed class FirstWeekMessageRedirection : IMessageRedirectionService
    {
        private static readonly IDictionary<long, string> UserMap = new Dictionary<long, string>
            {
                { 861889905804968041, "a.rechkalov" },
                { 558960500179208392, "a.rechkalov" },
                { 727228710813434015, "a.rechkalov" },

                { 620594536648802517, "m.pashuk" },
                { 750212591384526955, "m.pashuk" },
                { 654743609286852755, "m.pashuk" },

                { 846720541111877747, "d.ivanov" },
                { 421347940589554120, "d.ivanov" },
                { 654710592271024275, "d.ivanov" },
            };

        private readonly IQuery _query;

        public FirstWeekMessageRedirection(IQuery query)
        {
            _query = query;
        }

        public IEnumerable<string> RedirectFrom(string user)
        {
            var ids = UserMap.Where(x => string.Equals(x.Value, user)).Select(x => x.Key);
            var accounts = _query.For<User>().Where(x => ids.Contains(x.Id)).Select(x => x.Account).ToArray();
            return accounts;
        }

        public string RedirectTo(string user)
        {
            var id = _query.For<User>().Where(x => x.Account == user).Select(x => x.Id).SingleOrDefault();
            return UserMap[id];
        }
    }
}