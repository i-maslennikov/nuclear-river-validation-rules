using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.ValidationRules.WebApp.DataAccess.Entity;

namespace NuClear.ValidationRules.WebApp.DataAccess
{
    public class UserRepositiory
    {
        private readonly DataConnectionFactory _factory;

        public UserRepositiory(DataConnectionFactory factory)
        {
            _factory = factory;
        }

        public IReadOnlyCollection<User> Search(string query)
        {
            using (var connection = _factory.CreateDataConnection("Erm"))
            {
                var items = query.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                Expression<Func<User, bool>> filter;
                if (items.Length == 1)
                    filter = x => x.Account.StartsWith(items[0]) || x.FirstName.StartsWith(items[0]) || x.LastName.StartsWith(items[0]);
                else if (items.Length == 2)
                    filter = x => x.LastName.StartsWith(items[0]) && x.FirstName.StartsWith(items[1]) || x.LastName.StartsWith(items[1]) && x.FirstName.StartsWith(items[0]);
                else
                    return new User[0];

                var users = connection
                    .GetTable<User>()
                    .Where(x => x.IsActive && !x.IsDeleted)
                    .Where(filter)
                    .OrderBy(x => x.LastName)
                    .ThenBy(x => x.FirstName)
                    .Take(5);

                return users.ToArray();
            }
        }

        public long GetDefaultProject(long account)
        {
            using (var connection = _factory.CreateDataConnection("Erm"))
            {
                var project = connection.GetTable<UserOrganizationUnit>()
                    .Join(connection.GetTable<Project>(), x => x.OrganizationUnitId, x => x.OrganizationUnitId, (x, p) => p)
                    .OrderBy(x => x.DisplayName)
                    .FirstOrDefault();

                if (project == null)
                {
                    throw new ArgumentException($"Project for user '{account}' not found", nameof(account));
                }

                return project.Id;
            }
        }

        public long GetUserId(string domainName)
        {
            if (string.IsNullOrWhiteSpace(domainName))
            {
                throw new ArgumentException("User is not specified", nameof(domainName));
            }

            var account = domainName.Split('\\').Last();

            using (var connection = _factory.CreateDataConnection("Erm"))
            {
                var user = connection
                    .GetTable<User>()
                    .SingleOrDefault(x => x.Account == account);

                if (user == null)
                {
                    throw new ArgumentException($"User '{domainName}' not found", nameof(domainName));
                }

                return user.Id;
            }
        }

        public string GetAccountName(long id)
        {
            using (var connection = _factory.CreateDataConnection("Erm"))
            {
                var user = connection
                    .GetTable<User>()
                    .SingleOrDefault(x => x.Id == id);

                if (user == null)
                {
                    throw new ArgumentException($"User '{id}' not found", nameof(id));
                }

                return user.Account;
            }
        }
    }
}