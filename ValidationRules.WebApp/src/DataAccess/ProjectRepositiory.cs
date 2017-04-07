using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.WebApp.DataAccess.Entity;

namespace NuClear.ValidationRules.WebApp.DataAccess
{
    public class ProjectRepositiory
    {
        private readonly DataConnectionFactory _factory;

        public ProjectRepositiory(DataConnectionFactory factory)
        {
            _factory = factory;
        }

        public IReadOnlyCollection<Project> Search(string query)
        {
            using (var connection = _factory.CreateDataConnection("Erm"))
            {
                var projects = connection
                    .GetTable<Project>()
                    .Where(x => x.IsActive && x.OrganizationUnitId.HasValue)
                    .Where(x => x.DisplayName.StartsWith(query))
                    .OrderBy(x => x.DisplayName)
                    .Take(5);

                return projects.ToArray();
            }
        }

        public string GetProjectName(long id)
        {
            using (var connection = _factory.CreateDataConnection("Erm"))
            {
                var user = connection
                    .GetTable<Project>()
                    .SingleOrDefault(x => x.Id == id);

                if (user == null)
                {
                    throw new ArgumentException($"Project '{id}' not found", nameof(id));
                }

                return user.DisplayName;
            }
        }

        public DateTime GetNextRelease(long projectId)
        {
            using (var connection = _factory.CreateDataConnection("Erm"))
            {
                var project = connection
                    .GetTable<Project>()
                    .SingleOrDefault(x => x.Id == projectId);

                if (project == null)
                    throw new ArgumentException($"Проект {projectId}", nameof(project));

                var lastRelease = connection
                    .GetTable<ReleaseInfo>()
                    .Where(x => x.IsActive && !x.IsDeleted && !x.IsBeta && x.Status == 2)
                    .OrderByDescending(x => x.PeriodEndDate)
                    .FirstOrDefault(x => x.OrganizationUnitId == project.OrganizationUnitId);

                if (lastRelease == null)
                    throw new ArgumentException($"Не найден предыдущий релиз для проекта {projectId}", nameof(project));

                return lastRelease.PeriodEndDate.AddSeconds(1);
            }
        }
    }
}