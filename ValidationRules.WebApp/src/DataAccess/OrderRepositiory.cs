using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using LinqToDB.Data;

using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.DataAccess
{
    public sealed class OrderDto
    {
        public DateTime Begin { get; set; }
        public DateTime EndFact { get; set; }
    }

    public class OrderRepositiory
    {
        private readonly DataConnectionFactory _factory;

        public OrderRepositiory(DataConnectionFactory factory)
        {
            _factory = factory;
        }

        public IDictionary<long, OrderDto> GetPublicOrders(long? account, long? project, DateTime distributionDate)
        {
            using (var connection = _factory.CreateDataConnection("Erm"))
            {
                var ids = connection.GetTable<Order>()
                                    .Where(x => x.IsActive && !x.IsDeleted)
                                    .Where(x => new[] { 4, 5 }.Contains(x.WorkflowStepId))
                                    .Where(x => x.BeginDistributionDate <= distributionDate && x.EndDistributionDateFact >= distributionDate)
                                    .Where(GetProjectFilter(connection, project))
                                    .Where(GetAccountFilter(account))
                                    .Select(x => new { x.Id, x.BeginDistributionDate, x.EndDistributionDateFact });

                return ids.ToDictionary(x => x.Id, x => new OrderDto { Begin = x.BeginDistributionDate, EndFact = x.EndDistributionDateFact });
            }
        }

        public IDictionary<long, OrderDto> GetDraftOrders(long? account, long? project, DateTime distributionDate)
        {
            using (var connection = _factory.CreateDataConnection("Erm"))
            {
                var ids = connection.GetTable<Order>()
                                    .Where(x => x.IsActive && !x.IsDeleted)
                                    .Where(x => x.WorkflowStepId == 2)
                                    .Where(x => x.BeginDistributionDate <= distributionDate && x.EndDistributionDateFact >= distributionDate)
                                    .Where(GetProjectFilter(connection, project))
                                    .Where(GetAccountFilter(account))
                                    .Select(x => new { x.Id, x.BeginDistributionDate, x.EndDistributionDateFact });

                return ids.ToDictionary(x => x.Id, x => new OrderDto { Begin = x.BeginDistributionDate, EndFact = x.EndDistributionDateFact });
            }
        }

        private Expression<Func<Order, bool>> GetProjectFilter(DataConnection connection, long? projectId)
        {
            if (projectId.HasValue)
            {
                var project = connection.GetTable<Project>()
                                                 .SingleOrDefault(x => x.Id == projectId && x.OrganizationUnitId.HasValue);

                if (project == null)
                {
                    throw new ArgumentException($"Город не найден {projectId.Value}", nameof(project));
                }

                return x => x.DestOrganizationUnitId == project.OrganizationUnitId.Value || x.SourceOrganizationUnitId == project.OrganizationUnitId.Value;
            }

            return x => true;
        }

        private Expression<Func<Order, bool>> GetAccountFilter(long? account)
        {
            if (account.HasValue)
            {
                return x => x.OwnerCode == account.Value;
            }

            return x => true;
        }
    }
}